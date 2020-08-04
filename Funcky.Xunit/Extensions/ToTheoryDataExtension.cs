using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Funcky.Xunit
{
    public static class ToTheoryDataExtension
    {
        public static TheoryData<T1> ToTheoryData<T1>(this IEnumerable<T1> enumerable)
            => enumerable.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);

        public static TheoryData<T1> ToTheoryData<T1>(this IEnumerable<Tuple<T1>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);

        public static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<Tuple<T1, T2>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3> ToTheoryData<T1, T2, T3>(this IEnumerable<Tuple<T1, T2, T3>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<Tuple<T1, T2, T3, T4>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5> ToTheoryData<T1, T2, T3, T4, T5>(this IEnumerable<Tuple<T1, T2, T3, T4, T5>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6> ToTheoryData<T1, T2, T3, T4, T5, T6>(this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7> ToTheoryData<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(), AddElementToTheoryData);

        public static TheoryData<T1> ToTheoryData<T1>(this IEnumerable<ValueTuple<T1>> enumerable)
            => enumerable.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);

        public static TheoryData<T1, T2> ToTheoryData<T1, T2>(this IEnumerable<(T1 Item1, T2 Item2)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3> ToTheoryData<T1, T2, T3>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4> ToTheoryData<T1, T2, T3, T4>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5> ToTheoryData<T1, T2, T3, T4, T5>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6> ToTheoryData<T1, T2, T3, T4, T5, T6>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7> ToTheoryData<T1, T2, T3, T4, T5, T6, T7>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(), AddElementToTheoryData);

        public static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(this IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9, T10 Item10)> enumerable)
            => enumerable.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(), AddElementToTheoryData);

        private static TheoryData<T1> AddElementToTheoryData<T1>(TheoryData<T1> theoryData, T1 element)
        {
            theoryData.Add(element);

            return theoryData;
        }

        private static TheoryData<T1> AddElementToTheoryData<T1>(TheoryData<T1> theoryData, Tuple<T1> tuple)
        {
            theoryData.Add(tuple.Item1);

            return theoryData;
        }

        private static TheoryData<T1, T2> AddElementToTheoryData<T1, T2>(TheoryData<T1, T2> theoryData, Tuple<T1, T2> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3> AddElementToTheoryData<T1, T2, T3>(TheoryData<T1, T2, T3> theoryData, Tuple<T1, T2, T3> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4> AddElementToTheoryData<T1, T2, T3, T4>(TheoryData<T1, T2, T3, T4> theoryData, Tuple<T1, T2, T3, T4> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5> AddElementToTheoryData<T1, T2, T3, T4, T5>(TheoryData<T1, T2, T3, T4, T5> theoryData, Tuple<T1, T2, T3, T4, T5> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6> AddElementToTheoryData<T1, T2, T3, T4, T5, T6>(TheoryData<T1, T2, T3, T4, T5, T6> theoryData, Tuple<T1, T2, T3, T4, T5, T6> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7>(TheoryData<T1, T2, T3, T4, T5, T6, T7> theoryData, Tuple<T1, T2, T3, T4, T5, T6, T7> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> theoryData, Tuple<T1, T2, T3, T4, T5, T6, T7, T8> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> theoryData, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest.Item1, tuple.Rest.Item2);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> theoryData, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>> tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest.Item1, tuple.Rest.Item2, tuple.Rest.Item3);

            return theoryData;
        }

        private static TheoryData<T1> AddElementToTheoryData<T1>(TheoryData<T1> theoryData, ValueTuple<T1> tuple)
        {
            theoryData.Add(tuple.Item1);

            return theoryData;
        }

        private static TheoryData<T1, T2> AddElementToTheoryData<T1, T2>(TheoryData<T1, T2> theoryData, (T1 Item1, T2 Item2) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3> AddElementToTheoryData<T1, T2, T3>(TheoryData<T1, T2, T3> theoryData, (T1 Item1, T2 Item2, T3 Item3) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4> AddElementToTheoryData<T1, T2, T3, T4>(TheoryData<T1, T2, T3, T4> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5> AddElementToTheoryData<T1, T2, T3, T4, T5>(TheoryData<T1, T2, T3, T4, T5> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6> AddElementToTheoryData<T1, T2, T3, T4, T5, T6>(TheoryData<T1, T2, T3, T4, T5, T6> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7>(TheoryData<T1, T2, T3, T4, T5, T6, T7> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9);

            return theoryData;
        }

        private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> theoryData, (T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9, T10 Item10) tuple)
        {
            theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Item8, tuple.Item9, tuple.Item10);

            return theoryData;
        }
    }
}
