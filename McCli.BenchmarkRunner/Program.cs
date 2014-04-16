using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace McCli.BenchmarkRunner
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: {0} path scale", Path.GetFileNameWithoutExtension(Assembly.GetEntryAssembly().Location));
				return;
			}

			string benchmarkPath = Path.GetFullPath(args[0]);
			var benchmarkAssembly = Assembly.LoadFile(benchmarkPath);
			var type = benchmarkAssembly.GetExportedTypes().Single();
			var method = type.GetMethod(type.Name);
			var @delegate = (Action<double>)Delegate.CreateDelegate(typeof(Action<double>), method);

			@delegate(0); // Pre-jit

			double scale = int.Parse(args[1]);

			var stopwatch = Stopwatch.StartNew();
			try
			{
				@delegate(scale);
			}
			catch (Exception exception)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine(exception);
				Console.ResetColor();
				return;
			}

			var duration = stopwatch.Elapsed;
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Completed in {0}", duration);
			Console.ResetColor();
		}
	}
}
