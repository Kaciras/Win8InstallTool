using BenchmarkDotNet.Running;
using System;
using Win8InstallTool.Benchmark;

BenchmarkRunner.Run<RegFilePerf>();
Console.ReadKey();
