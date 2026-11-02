using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Toolchains.InProcess.Emit;

// BenchmarkDotNet 0.15.8 does not know the .NET 11 runtime moniker yet and refuses to validate an out-of-process job on it.
// The in-process toolchain runs the benchmarks inside this (Release, net11.0) process instead.
var config = ManualConfig.Create(DefaultConfig.Instance)
    .AddJob(Job.ShortRun.WithToolchain(InProcessEmitToolchain.Instance));

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, config);
