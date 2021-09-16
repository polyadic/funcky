## CartesianProduct

In mathematics, specifically set theory, the Cartesian product of two sets A and B, denoted A×B, is the set of all ordered pairs (a, b) where a is in A and b is in B.

The CartesianProduct extension function returns all possible pairs of two given IEnumerables.

There are two overloads, one which lets you choose

![cartesian-product with marbles](cartesian-product.svg "Marble me!")

### Examples

Two sequences as input:

``` 
smiles = [😀, 😐, 🙄]
fruits = [🍉, 🍌, 🍇, 🍓]
``` 

The cartesian products of smiles and fruits:

``` 
smiles × fruits => [[😀, 🍉], [😀, 🍌], [😀, 🍇], [😀, 🍓], 
                    [😐, 🍉], [😐, 🍌], [😐, 🍇], [😐, 🍓], 
				    [🙄, 🍉], [🙄, 🍌], [🙄, 🍇], [🙄, 🍓]]
```

In this C# example you see how all Playing cards are in fact a cartesian product of a suit and a value.

This example uses the overload with our own selector, because we just want a sequence of strings.

```cs
var suits = ImmutableList.Create("♠", "♣", "♥", "♦");
var values = 
  ImmutableList.Create("2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A");

var allCards = suits.CartesianProduct(values, (suit, value) => $"{value}{suit}");
``` 
