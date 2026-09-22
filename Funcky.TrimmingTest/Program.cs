using Funcky;
using Funcky.Extensions;

// Use a few async APIs, so the build verifies that Funcky and Funcky.Async can be referenced
// together on every target framework (on net10.0 Funcky.Async only forwards to Funcky).
var numbers = AsyncSequence.Successors(1, n => ValueTask.FromResult(n + 1)).Take(5);
var first = await numbers.FirstOrNoneAsync();
var sum = (await numbers.MaterializeAsync()).Sum();

Console.WriteLine($"{first} {sum}");
