## Merge

### Examples

Given two sequences which are already ordered the same way:

```
  sequence1 = [1, 2, 7, 9, 14]
  sequence2 = [3, 6, 8, 11, 12, 16]
```

By merging we get one single sequence with the all elements of the given sequences with the same order.
  
```
  sequence1.Merge(sequence2) => 
              [1, 2,       7,    9,         14    ]
              [      3, 6,    8,    11, 12,     16]
              -------------------------------------
              [1, 2, 3, 6, 7, 8, 9, 11, 12, 14, 16]
```
