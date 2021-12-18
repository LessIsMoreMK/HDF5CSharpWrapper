using System;
using System.Linq;

namespace HDF5CSharpWrapper.Helpers
{
    public static class SequenceEqualsExtension
    {
        public static bool SequenceEquals<T>(this T[,] a, T[,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());

        public static bool SequenceEqualsThreeDim<T>(this T[,,] a, T[,,] b) => a.Rank == b.Rank
            && Enumerable.Range(0, a.Rank).All(d => a.GetLength(d) == b.GetLength(d))
            && a.Cast<T>().SequenceEqual(b.Cast<T>());
    }
}
