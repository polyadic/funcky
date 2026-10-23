#if !XUNIT_V3
using System.ComponentModel;
using Xunit;

namespace Funcky.Extensions;

public static class ToTheoryDataExtension
{
    extension<T1>(IEnumerable<T1> source)
    {
        [Pure]
        public TheoryData<T1> ToTheoryData()
            => source.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);
    }

    extension<T1>(IEnumerable<Tuple<T1>> source)
    {
        [Pure]
        public TheoryData<T1> ToTheoryData()
            => source.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);
    }

    extension<T1, T2>(IEnumerable<Tuple<T1, T2>> source)
    {
        [Pure]
        public TheoryData<T1, T2> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3>(IEnumerable<Tuple<T1, T2, T3>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4>(IEnumerable<Tuple<T1, T2, T3, T4>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5>(IEnumerable<Tuple<T1, T2, T3, T4, T5>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, T8>> source)
        where T8 : notnull
    {
        [Pure]
        [Obsolete("This overload is incorrect, the last element has to be a tuple itself.")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9>>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IEnumerable<Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8, T9, T10>>> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(), AddElementToTheoryData);
    }

    extension<T1>(IEnumerable<ValueTuple<T1>> source)
    {
        [Pure]
        public TheoryData<T1> ToTheoryData()
            => source.Aggregate(new TheoryData<T1>(), AddElementToTheoryData);
    }

    extension<T1, T2>(IEnumerable<(T1 Item1, T2 Item2)> source)
    {
        [Pure]
        public TheoryData<T1, T2> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, T9>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9>(), AddElementToTheoryData);
    }

    extension<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(IEnumerable<(T1 Item1, T2 Item2, T3 Item3, T4 Item4, T5 Item5, T6 Item6, T7 Item7, T8 Item8, T9 Item9, T10 Item10)> source)
    {
        [Pure]
        public TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> ToTheoryData()
            => source.Aggregate(new TheoryData<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(), AddElementToTheoryData);
    }

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
        where T8 : notnull
    {
        theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest);

        return theoryData;
    }

    private static TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> AddElementToTheoryData<T1, T2, T3, T4, T5, T6, T7, T8>(TheoryData<T1, T2, T3, T4, T5, T6, T7, T8> theoryData, Tuple<T1, T2, T3, T4, T5, T6, T7, Tuple<T8>> tuple)
    {
        theoryData.Add(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7, tuple.Rest.Item1);

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
#endif
