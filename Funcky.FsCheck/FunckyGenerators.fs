namespace Funcky.FsCheck

open System
#if PRIORITY_QUEUE
open System.Collections.Generic
#endif
open FsCheck
open FsCheck.FSharp
open FsCheck.Xunit
open Funcky.Monads
open Microsoft.FSharp.Core

type EquatableException =
    inherit Exception

    new (message) = { inherit Exception(message) }

    interface IEquatable<EquatableException> with
        member this.Equals(other) = other.Message = this.Message

    override this.Equals(other) =
        match other with
        | :? EquatableException as o -> this.Message = o.Message
        | _ -> false

    override this.GetHashCode() = this.Message.GetHashCode()

type NonNull<'a> = NonNull of 'a with
    member x.Get = match x with NonNull r -> r

module FunckyArb =
    let generateNonNull<'a> map = map |> ArbMap.generate<NonNull<'a>> |> Gen.map (fun x -> x.Get)

[<Sealed>]
[<AbstractClass>]
type FunckyGenerators =
    static member either<'a, 'b>(map) =
        (Arb.fromGen << Gen.oneof) [
            map |> FunckyArb.generateNonNull<'a> |> Gen.map Either<'a, 'b>.Left
            map |> FunckyArb.generateNonNull<'b> |> Gen.map Either<'a, 'b>.Right]

    static member result<'a>(map) =
        (Arb.fromGen << Gen.oneof) [
            map |> FunckyArb.generateNonNull<'a> |> Gen.map Result.Ok
            map |> ArbMap.generate<string> |> Gen.map (EquatableException >> Result<'a>.Error)]

    static member tuple2<'a, 'b>(map) =
       map |> ArbMap.arbitrary<Tuple<'a, 'b>> |> Arb.convert TupleExtensions.ToValueTuple TupleExtensions.ToTuple

#if PRIORITY_QUEUE
    static member priorityQueue<'a, 'priority>(map) =
         Arb.fromGen <|
             gen { let! values = map |> ArbMap.generate<List<ValueTuple<'a, 'priority>>>
                   return PriorityQueue(values) }
#endif

    static member nonNull<'a>(map) =
        map |> ArbMap.arbitrary<'a>
            |> Arb.filter (fun x -> not (Object.ReferenceEquals(x, null)))
            |> Arb.convert NonNull _.Get

    static member option<'a>(map) =
        { new Arbitrary<Funcky.Monads.Option<'a>>() with
            override _.Generator =
                Gen.frequency [(1, gen { return Option<'a>.None }); (7, map |> FunckyArb.generateNonNull<'a> |> Gen.map Option.Some)]
            override _.Shrinker o =
                o.Match(none = Seq.empty, some = fun x -> seq { yield Option<'a>.None; for x' in map |> ArbMap.arbitrary<'a> |> Arb.toShrink <| x -> Option.Some x' })
        }

#if INDEX_TYPE
    static member index(map) =
        map |> ArbMap.arbitrary<Tuple<PositiveInt, bool>>
            |> Arb.convert (fun (value, fromEnd) -> Index(value.Get, fromEnd)) (fun x -> (PositiveInt(x.Value), x.IsFromEnd))
#endif

    static member generateLazy<'a>(map) =
        map |> ArbMap.arbitrary<NonNull<'a>>
            |> Arb.convert (fun x -> Lazy.Return(x.Get)) (fun x -> NonNull(x.Value))

    static member generateReader<'env, 'a>(map) =
        Arb.fromGen (
            map |> ArbMap.generate<Func<'env, NonNull<'a>>>
                |> Gen.map (fun f -> fun env -> f.Invoke(env).Get)
                |> Gen.map Reader<'env>.FromFunc)

type FunckyPropertyAttribute() as self =
    inherit PropertyAttribute()

    do
        self.Arbitrary <- List.toArray [typedefof<FunckyGenerators>]
