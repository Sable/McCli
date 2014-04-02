using System.IO;
using System.Diagnostics;

foreach (var path in Directory.GetFiles(@"Benchmarks", "drv_*.tirxml", SearchOption.AllDirectories))
{
	Process.Start("mcclicd.cmd", path);
}