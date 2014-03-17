using CliKit;
using CliKit.Cil;
using CliKit.IO;
using McCli.Compiler.IR;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace McCli.Compiler.CodeGen
{
	partial class FunctionEmitter : Visitor
	{
		#region Fields
		private static readonly FunctionTable pseudoBuiltins;

		private Label continueTargetLabel;
		private Label breakTargetLabel;
		#endregion

		#region Constructors
		static FunctionEmitter()
		{
			pseudoBuiltins = new FunctionTable();
			pseudoBuiltins.AddMethodsFromType(typeof(PseudoBuiltins));
		}
		#endregion

		#region Methods
		public override void VisitLiteral(Literal node)
		{
			using (BeginEmitStore(node.Target))
			{
				MRepr sourceRepr;
				if (node.Value is double)
				{
					cil.LoadFloat64((double)node.Value);
					sourceRepr = MClass.Double.ScalarRepr;
				}
				else if (node.Value is string)
				{
					var str = (string)node.Value;
					if (str.Length == 1)
					{
						cil.LoadInt32((int)str[0]);
						sourceRepr = MClass.Char.ScalarRepr;
					}
					else
					{
						cil.LoadString(str);
						cil.Invoke(typeof(MStrings).GetMethod("FromString"));
						sourceRepr = MClass.Char.FullArrayRepr;
					}
				}
				else
				{
					throw new NotImplementedException("Literals of type " + node.Value.GetType());
				}

				EmitConversion(sourceRepr, node.Target.StaticRepr);
			}
		}

		public override void VisitCopy(Copy node)
		{
			if (IsLiteral(node.Target)) return;

			using (BeginEmitStore(node.Target))
			{
				EmitLoad(node.Value);
				EmitCloneIfNeeded(node.Value.StaticRepr);
				EmitConversion(node.Value.StaticRepr, node.Target.StaticRepr);
			}
		}

		public override void VisitStaticCall(StaticCall node)
		{
			// Resolve the function
			var argumentTypes = node.Arguments.Select(a => a.StaticRepr);
			var function = functionLookup(node.FunctionName, argumentTypes);
			EmitCall(node.Targets, function, node.Arguments);
		}

		private void EmitCall(ImmutableArray<Variable> targets, FunctionMethod function, ImmutableArray<Variable> arguments)
		{
			var signature = function.Signature;
			Contract.Assert(arguments.Length == signature.Inputs.Count);
			Contract.Assert(targets.Length <= signature.Outputs.Count);

			// Prepare the eventual return value store
			if (signature.HasReturnValue && targets.Length == 1)
				BeginEmitStore(targets[0]);

			// Push the input arguments
			for (int i = 0; i < arguments.Length; ++i)
			{
				var argument = arguments[i];
				EmitLoad(argument);
				EmitConversion(argument.StaticRepr, signature.Inputs[i]);
			}

			if (signature.OutParameterCount > 0)
			{
				// Push the pointers to the output arguments
				for (int i = 0; i < signature.Outputs.Count; ++i)
				{
					if (i < targets.Length)
					{
						var target = targets[i];
						var location = GetLocation(target);
						if (declaration.Outputs.Contains(target) && declaration.Outputs.Length >= 2)
							cil.Load(location); // Target is a ByRef parameter, so already a (managed) pointer
						else
							cil.LoadAddress(location);
					}
					else
					{
						throw new NotImplementedException("Ignored outputs of a multi-output function.");
					}
				}
			}

			// Call the function
			var method = function.Method;
			if (method is System.Reflection.Emit.MethodBuilder)
			{
				// We have to supply the parameter types ourselves since MethodBodyWriter
				// can't call MethodBuilder.GetParameters() without receiving an exception.
				cil.Call(Opcode.GetDefaultCall(method), method,
					function.Signature.GetParameterCliTypes(), function.Signature.ReturnCliType);
			}
			else
			{
				// We're calling a builtin
				var builtinCilOpcodeAttribute = method.GetCustomAttribute<BuiltinCilOpcodeAttribute>();
				if (builtinCilOpcodeAttribute == null)
				{
					cil.Invoke(method);
				}
				else
				{
					// Calling the builtin is equivalent to emitting a sequence of CIL opcodes, so do that instead
					foreach (var opcodeValue in builtinCilOpcodeAttribute.Opcodes)
					{
						var opcode = Opcode.FromValue((OpcodeValue)opcodeValue);
						Contract.Assert(opcode != null);
						cil.Instruction(opcode);
					}
				}
			}

			// Handle the return value, if any
			if (signature.HasReturnValue)
			{
				if (targets.Length == 1)
				{
					EmitConversion(signature.Outputs[0], targets[0].StaticRepr);
					EndEmitStore(targets[0]);
				}
				else
				{
					// Return value ignored.
					Contract.Assert(targets.Length == 0);
					cil.Pop();
				}
			}
		}

		public override void VisitLoadParenthesized(LoadParenthesized node)
		{
			Contract.Assert(node.Targets.Length == 1);

			if (node.Arguments.Length == 1 && node.Arguments[0].IsColon)
			{
				// foo(:), linearization
				EmitCall(node.Targets, pseudoBuiltins.Lookup("Linearize", node.Subject.StaticRepr), node.Subject);
				return;
			}

			// TODO: Handle ':' indices
			// TODO: Handle the zero-argument case specially
			var argumentsBuilder = new ImmutableArray<Variable>.Builder(node.Arguments.Length + 1);
			argumentsBuilder[0] = node.Subject;
			for (int i = 0; i < node.Arguments.Length; ++i)
			{
				var argument = node.Arguments[i];
				if (argument.IsColon) throw new NotImplementedException("':' indices.");
				argumentsBuilder[1 + i] = argument.Variable;
			}
			var arguments = argumentsBuilder.Complete();

			var function = pseudoBuiltins.Lookup("ArrayGet", arguments.Select(v => v.StaticRepr));
			EmitCall(node.Targets, function, arguments);
		}

		public override void VisitStoreParenthesized(StoreParenthesized node)
		{
			if (!locals.ContainsKey(node.Array) && node.Array.StaticRepr.IsArray)
			{
				// Array declared on the fly, we should new it up
				if (depth > 0) throw new NotImplementedException("Store to undeclared array in a nested statement.");
				var newEmptyArrayMethod = typeof(MFullArray<>)
					.MakeGenericType(node.Array.StaticRepr.Type.CliType)
					.GetMethod("CreateEmpty");
				using (BeginEmitStore(node.Array))
					cil.Invoke(newEmptyArrayMethod);
			}

			// TODO: Handle ':' indices
			var argumentsBuilder = new ImmutableArray<Variable>.Builder(node.Indices.Length + 2);
			argumentsBuilder[0] = node.Array;
			for (int i = 0; i < node.Indices.Length; ++i)
			{
				var index = node.Indices[i];
				if (index.IsColon) throw new NotImplementedException("':' indices.");
				argumentsBuilder[1 + i] = index.Variable;
			}
			argumentsBuilder[argumentsBuilder.Length - 1] = node.Value;
			var arguments = argumentsBuilder.Complete();

			var function = pseudoBuiltins.Lookup("ArraySet", arguments.Select(v => v.StaticRepr));
			EmitCall(ImmutableArray.Empty, function, arguments);
		}

		public override void VisitIf(If node)
		{
			if (node.Then.Length == 0 && node.Else.Length == 0) return;

			EmitLoad(node.Condition);
			EmitIsTrue(node.Condition.StaticRepr);

			var endLabel = cil.CreateLabel("if_end");
			if (node.Then.Length == 0)
			{
				cil.Branch(true, endLabel);
				EmitStatements(node.Else);
			}
			else if (node.Else.Length == 0)
			{
				cil.Branch(false, endLabel);
				EmitStatements(node.Then);
			}
			else
			{
				var elseLabel = cil.CreateLabel("else");
				cil.Branch(false, elseLabel);
				EmitStatements(node.Then);
				cil.Branch(endLabel);
				cil.MarkLabel(elseLabel);
				EmitStatements(node.Else);
			}

			cil.MarkLabel(endLabel);
		}

		private void EmitIsTrue(MRepr conditionRepr)
		{
			if (conditionRepr.Type == MClass.Logical && conditionRepr.StructuralClass == MStructuralClass.Scalar)
				return;

			var isTrueFunction = pseudoBuiltins.Lookup("IsTrue", conditionRepr);
			EmitConversion(conditionRepr, isTrueFunction.Signature.Inputs[0]);
			cil.Invoke(isTrueFunction.Method);
		}

		public override void VisitWhile(While node)
		{
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("while_continue");
			breakTargetLabel = cil.CreateLabel("while_break");

			// Condition
			cil.MarkLabel(continueTargetLabel);
			EmitLoad(node.Condition);
			EmitIsTrue(node.Condition.StaticRepr);
			cil.Branch(false, breakTargetLabel);

			// Body
			EmitStatements(node.Body);
			cil.Branch(continueTargetLabel);

			// End
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitRangeFor(RangeFor node)
		{
			// Declare loop labels, preserving parent ones
			var previousContinueTargetLabel = continueTargetLabel;
			var previousBreakTargetLabel = breakTargetLabel;
			continueTargetLabel = cil.CreateLabel("for_continue");
			breakTargetLabel = cil.CreateLabel("for_break");
			var conditionLabel = cil.CreateLabel("for_condition");

			var repr = new MRepr(node.Iterator.StaticRepr.Type, MStructuralClass.Scalar);
			Contract.Assert(repr.Class.IsNumeric && !repr.IsComplex);

			// Allocate the the temporaries we need
			var currentLocal = temporaryPool.Alloc(repr.CliType);
			TemporaryLocalPool.AllocationScope? stepLocal = null, toLocal = null;

			try
			{
				// Setup the iterator variable, "current"
				EmitLoad(node.From);
				EmitConversion(node.From.StaticRepr, repr);
				cil.Store(currentLocal.Location);

				// Save the increment to a local variable if it's not a literal or initonly
				if (node.Step != null && !IsLiteral(node.Step) && !node.Step.IsInitOnly)
				{
					stepLocal = temporaryPool.Alloc(repr.CliType);
					EmitLoad(node.Step);
					EmitConversion(node.Step.StaticRepr, repr);
					EmitCloneIfNeeded(repr);
					cil.Store(stepLocal.Value.Location);
				}

				// Save the "to" variable to a local variable if it's not a literal or initonly
				if (!IsLiteral(node.To) && !node.To.IsInitOnly)
				{
					toLocal = temporaryPool.Alloc(repr.CliType);
					EmitLoad(node.To);
					EmitConversion(node.To.StaticRepr, repr);
					EmitCloneIfNeeded(repr);
					cil.Store(toLocal.Value.Location);
				}

				// Test the loop condition
				// TODO: support down-going loops
				// condition: if (current > to) goto break;
				cil.MarkLabel(conditionLabel);
				cil.Load(currentLocal.Location);
				
				// Load the "to" loop bound variable (or our copy of it)
				if (toLocal.HasValue)
					cil.Load(toLocal.Value.Location);
				else
					EmitLoad(node.To);
				
				cil.Branch(Comparison.GreaterThan, breakTargetLabel);

				// Setup the iterator variable (which can be modified inside the loop)
				cil.Load(currentLocal.Location);
				EmitConversion(repr, node.Iterator.StaticRepr);
				cil.Store(GetLocation(node.Iterator));

				// body
				EmitStatements(node.Body);

				// Loop increment
				// continue: current += increment; goto condition;
				cil.MarkLabel(continueTargetLabel);
				cil.Load(currentLocal.Location);

				// Load the "step" loop increment variable, our copy of it or the default value of 1
				if (stepLocal.HasValue) cil.Load(stepLocal.Value.Location);
				else if (node.Step == null) cil.LoadFloat64(1.0);
				else EmitLoad(node.Step);

				cil.Instruction(Opcode.Add);
				cil.Store(currentLocal.Location);
				cil.Branch(conditionLabel);
			}
			finally
			{
				currentLocal.Dispose();
				if (stepLocal.HasValue) stepLocal.Value.Dispose();
				if (toLocal.HasValue) toLocal.Value.Dispose();
			}

			// break:
			cil.MarkLabel(breakTargetLabel);

			// Restore parent loop labels
			continueTargetLabel = previousContinueTargetLabel;
			breakTargetLabel = previousBreakTargetLabel;
		}

		public override void VisitJump(Jump node)
		{
			switch(node.Kind)
			{
				case JumpKind.Continue: cil.Branch(continueTargetLabel); break;
				case JumpKind.Break: cil.Branch(breakTargetLabel); break;
				case JumpKind.Return: cil.Branch(returnTargetLabel); break;
				default: throw new NotImplementedException();
			}
		}

		public override void VisitNode(IR.Node node)
		{
			throw new NotImplementedException(string.Format("Visiting {0} IR nodes.", node.GetType().Name));
		}

		private void EmitCloneIfNeeded(MRepr type)
		{
			var sourceType = type.CliType;
			if (typeof(MValue).IsAssignableFrom(sourceType))
			{
				var deepCloneMethod = sourceType.GetMethod("DeepClone");
				cil.Invoke(deepCloneMethod);
				if (!sourceType.IsAssignableFrom(deepCloneMethod.ReturnType))
					cil.Instruction(Opcode.Castclass, sourceType);
			}
		}
		#endregion
	}
}
