using System;
using BenchmarkDotNet.Running;
using Win8InstallTool.Benchmark;

BenchmarkRunner.Run<RegFilePerf>();
Console.ReadKey();
