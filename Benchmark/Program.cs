using BenchmarkDotNet.Running;
using Benchmark;
using System;

BenchmarkRunner.Run<RegFilePerf>();
Console.ReadKey();
