# McCli #
**McCli** is a backend for the McLab compiler to compile MATLAB to the Commmon Language Infrastructure (CLI), most commonly referred to as .NET. The compiler focuses on a numerical subset of the MATLAB language for which types can be fully determined statically by the McLab Tamer. The backend works from an XML-serialized form of the McLab Tamer IR and generates CLI portable class libraries that can be integrated to existing .NET apps. It has been shown to work on Windows, Windows Store Apps, Windows Phone and Linux.

**This is a proof of concept** and is not meant to be used in any kind of production environment. There are numerous known issues and the code base is not mature. Moreover, the project was implemented focusing on a small set of benchmark programs and probably does not trivially generalize to arbitrary programs. 

This project was realized by Tristan Labelle as his Bachelor final project at McGill University, Montreal.

## Components ##
The McCli project is subdivided in the following components, corresponding to C# projects.

### McCli ###
The core library which provides the classes to representing MATLAB values and types. Important classes:

 - **MComplex**: Wraps a primitive type with a real and an imaginary component.
 - **MFullArray**: Represents MATLAB full arrays of any dimensions and size.
 - **MType/MClass**: Represents MATLAB types and their mapping to CLI types. A class is one of the primitive MATLAB classes (double, single, int32, uint8, char, logical, etc.) whereas a type is a more generalized notion that also includes the complex attribute.
 - **MStructuralClass**: Represents a kind of container for values of a given type and its mapping to CLI types. This includes scalar, integral ranges and full arrays.
 - **MRepr**: Combines a type and a structural class to fully describe the static representation of a MATLAB value.
 - **PseudoBuiltins**: Provides various core MATLAB support functions which do not directly map to built-in functions defined by MathWorks. The most important of these being ArrayGet and ArraySet. This is the only set of functions which the compiler is hard-coded to use.

### McCli.Builtins ###
Provides C#-based implementations of standard MATLAB built-in functions.

### McCli.Compiler ###
Implements the compilation logic. Important classes:

 - **IR.Node and derived classes**: Represents Tamer IR nodes using an immutable model.
 - **IR.Variable**: Represents a single variable used in a MATLAB program along with some static type information obtained from Tamer analyses.
 - **IR.Visitor**: Visitor pattern base class for IR nodes.
 - **IR.TamerXmlReader**: Implements the serialized XML reading logic.
 - **FunctionTable**: Locates methods implementing MATLAB built-in functions in assemblies and performs overload resolution.
 - **FunctionEmitter**: Main code generation class, generates the CIL from the IR representation of a MATLAB method.
 - **PortableClassLibrary**: Implements an ugly hack to patch assemblies and make them portable class libraries, since System.Reflection.Emit does not support this.

### McCli.Compiler.CommandLine ###
A basic command line inferface to the compiler.

### McCli.Tests ###
Implements a few code generation unit tests.

### McCli.BenchmarkRunner ###
A command line utility to run benchmarks (programs with an entry point accepting a single double value) and report execution time.

### CliKit ###
A framework for CIL bytecode generation wrapping the System.Reflection.Emit APIs. A core benefit is the ability to perform partial CIL verification as the bytecode is being emitted in order to catch compiler bugs more easily.
