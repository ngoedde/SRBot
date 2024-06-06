using System.Diagnostics;

namespace SRNetwork.Common.Extensions;

public static class StopwatchExtensions
{
    public static long GetMicroseconds(this Stopwatch watch) => watch.ElapsedTicks / Stopwatch.Frequency * 1000000000;
}