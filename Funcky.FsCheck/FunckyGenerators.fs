namespace Funcky.FsCheck

open System
open System.Collections.Generic
open System.ComponentModel
open FsCheck
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

[<Sealed>]
[<AbstractClass>]
type FunckyGenerators =
    static member either<'a, 'b>() =
        (Arb.fromGen << Gen.oneof) [
            Arb.generate<'a> |> Gen.map Either<'a, 'b>.Left
            Arb.generate<'b> |> Gen.map Either<'a, 'b>.Right]

    static member result<'a>() =
        (Arb.fromGen << Gen.oneof) [
            Arb.generate<'a> |> Gen.map Result.Ok
            Arb.generate<string> |> Gen.map (EquatableException >> Result<'a>.Error)]

    static member tuple2<'a, 'b>() =
       gen { let! value1 = Arb.generate<'a>
             let! value2 = Arb.generate<'b>
             return ValueTuple.Create(value1, value2) } |> Arb.fromGen

#if PRIORITY_QUEUE
    static member priorityQueue<'a, 'priority>() =
        gen { let! values = Arb.generate<List<ValueTuple<'a, 'priority>>>
              return PriorityQueue(values) } |> Arb.fromGen
#endif

    [<CompiledName("Register")>]
    static member register() = Arb.registerByType typeof<FunckyGenerators> |> ignore
