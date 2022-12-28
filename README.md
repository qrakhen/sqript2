# Sqript
## Better, faster, cooler

This is just here for completionist's sake if anyone was to ever ask where the 2 went.

![Sqript Command Line Interface](https://i.imgur.com/M36EDwW.png)

## Usage & Features

### Summary
 * Comes with a syntax-highlighting 'micro-IDE'
 * Platform Independent
 * Infinitely Expandable using the Library SDK
 * Highly Customizable
 * Custom Types & Classes
 * Modules including import/export features
 * Lightweight
 * Directly integratable into any other program
 * Special
 * Fun

### Consider This
#### Aliases
First of all, it is important to note that aliases exist as a fundamental part of Sqript.
If you don't like the usage of a certain keyword or operator (i.e. `*~`),
you can either add your own alias, or use the default text version.

In all following examples, `*~` is used instead of the default `var`.

More on aliases on the Extras Section of this readme.

#### Dynamic vs. Static Types
Sqript, by default, can work with both dynamic and static types at the same time.
This behaviour can be configured to your needs, i.e. to enforce the usage of types,
or to disable types entirely.

## Demo
```js
module MyModule;

import('sqript/core/http');
```

### Basics
#### Variables
##### Dynamic Declaration 
```js
*~ x;
```

##### Typed Declaration 
```js
Number x;
```

##### Reference Declaration
```js
*~ &r;
Number &r;
```

##### Assign by Value
```js
*~ a <~ 10;
*~ x <~ 'hello';
x <~ a;
```

##### Assign by Reference
```js
*~ a <~ 10;
*~ b <& a;
```

#### Operators

#### Conditions

#### Loops

### Types

### Funqtions
#### Syntax
```js
// all flags in [...] are optional 
[ReturnType[&]]funqtion <name> ([ParamType]<param1>[? (=OptionalParam)], ... { 
	return ...;
});

<name>(<param1>, ...);
```

#### Example Dynamic Funqtion
```js
// fq is an alias
fq add (a, b { 
	return a + b;
});
*~ x <~ add(3, 10);
```

#### Example Typed Funqtion
```js
//                                we don't have to type all params
String funqtion concat (String a, b { 
	return a + b;
});
String x <~ concat('hello', 'world');
```

#### Inline
```js
*~ f <~ ~:(x { return x; });
f(5);
```

### Qlasses

### Modules

### Built-In

## Examples
### Bubble Sort

### Vector Class

## Libraries
### Intro
#### SDK
#### Included

## Aliases
### Precompiled
#### Simple
#### Placeholder
```js
(str_$) -> ("$");
```
Would change all occurences of (str_*) to ("*")
### Logical
#### Simple
#### Groups

## Qonfig
