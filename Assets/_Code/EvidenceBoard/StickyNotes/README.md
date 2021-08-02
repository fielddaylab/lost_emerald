# Evidence Board Sticky Note Format

## Basics

```
// this is a comment

:: noteId
@root RootId
@correct NodeId0, NodeId1, NodeId2, ...
Any note text in response.
It can extend to multiple lines too.
```

###  `:: noteId`

You must provide a unique id for the note.
The id itself is not important, as long as it is kept consistent across save data versions.

```
// the location-correct note
:: location-correct

// another note
:: location-incorrect-ab
```

**Note:** The full id of a note is in the format `[filename]-[noteId]` (ex. `L1-Notes-location-correct`)

### `@root`

Specifies which roots this sticky note will be read under.
Can accept multiple values.

```
// Single Root
@root Location

// Multiple Roots
@root Cause, Name
```

### `@correct`

This specifies that this note represents a correct chain of evidence. The evidence chain must contain only the nodes specified along with the `@correct` tag, in any order.

```
// Selects for only A
@correct A

// Selects for B, C, and D in any order
@correct B, C, D
```

### `@hint`

This specifies that this note represents a hint in the chain of evidence. Any one of the provided nodes must be detected at the end of the evidence chain, unless specified by the `@first` or `@anywhere` tags.

**Note**: If no nodes are provided, this will be presented at any combination not already covered by any `correct` nodes for the same root.

```
// Selects for A at the end
@hint A

// Selects for B, after A and C in any order
@hint B
@precededBy A, C
```

### `@incorrect`

This specifies that this note represents an incorrect piece of evidence in the chain of evidence. Any one of the provided nodes must be detected at the end of the evidence chain, unless specified by the `@first` or `@anywhere` tags.

**Note**: If no nodes are provided, this will be presented at any combination not already covered by any `hint` or `correct` nodes for the same root.

```
// Selects for D at the end
@incorrect D

// Selects for E at the start of the chain
@incorrect E
@first
```

## Modifiers

### `@precededBy`

Specifies that one of the nodes specified by `@incorrect` or `@hint` must be preceded by the given unordered seqeunce of nodes.

```
// This is for a hint on D, preceded by A and B in any order
@hint   D
@precededBy A, B
```

### `@first`

Specifies that one of the nodes specified by either `@incorrect` or `@hint` must be the first node in the evidence chain.

This will override the usual 2-depth rule for incorrect notes.
Note that any use of the `@precededBy` tag is unsupported with `@first`.

```
// Selects for D or E at the beginning of the chain
// This can trigger even in a 1-depth evidence chain
@incorrect D, E
@first
```

### `@last`

Specifies that one of the nodes specified by either `@incorrect` or `@hint` must be the last node in the evidence chain.

**Note:** This is the default location setting for a note, and does not need to be specified. 

### `@anywhere`

Specifies that any of the nodes specified by either `@incorrect` or `@hint` must be anywhere in the evidence chain.

```
// Selects for B anywhere in the evidence chain
@incorrect B
@anywhere
```

## Examples

```
:: location-correct
@root Location
@correct Loretta-Location
Yep! This seems like the place!

:: location-incorrect
@root Location
@incorrect
I don't know if these look like coordinates to me...

:: name-correct
@root Name
@correct Chart-Canaller, Loretta-Clipping
Wow! The Loretta, huh?

:: name-hint-chart
@root Name
@hint Chart-Canaller
Hmm, but there are two ships on this chart that were canallers...
Is there any other evidence pointing me towards one of them?

:: name-hint-clipping
@root Name
@hint Chart-Clipping
Hmm, okay, this is a ship name, but how do I know it's the right one?