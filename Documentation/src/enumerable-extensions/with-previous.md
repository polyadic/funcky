## WithPrevious

<picture>
    <picture>
      <source srcset="with-previous-dark.svg" media="(prefers-color-scheme: dark)">
      <img src="with-previous.svg" alt="A marble diagram showing the WithPrevious operation">
    </picture>
</picture>

### Example

```
animals = [ 🦄, 🐺, 🐷, 🦁, 🐵, 🐶 ]

animals.WithPrevious() =>
    [[∅, 🦄],
	 [🦄, 🐺],
	 [🐺, 🐷],
	 [🐷, 🦁],
	 [🦁, 🐵],
	 [🐵, 🐶]]
```
