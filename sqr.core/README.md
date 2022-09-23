# Sqript
## Better, faster, cooler

## Usage & Features

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
```cpp
module MyModule;

import('sqript/core/http');
```

### Basics
#### Variables
##### Dynamic Declaration 
```cpp
*~ x;
```

##### Typed Declaration 
```cpp
Number x;
```

##### Reference Declaration
```cpp
*~ &r;
Number &r;
```

##### Assign by Value
```cpp
*~ a <~ 10;
*~ x <~ 'hello';
x <~ a;
```

##### Assign by Reference
```cpp
*~ a <~ 10;
*~ b <& a;
```

#### Operators

#### Conditions

#### Loops

### Types

### Funqtions
#### Syntax
```cpp
// all flags in [...] are optional 
[ReturnType[&]]funqtion <name> ([ParamType]<param1>[? (=OptionalParam)], ... { 
	return ...;
});

<name>(<param1>, ...);
```

#### Example Dynamic Funqtion
```cpp
// fq is an alias
fq add (a, b { 
	return a + b;
});
*~ x <~ add(3, 10);
```

#### Example Typed Funqtion
```cpp
//                                we don't have to type all params
String funqtion concat (String a, b { 
	return a + b;
});
String x <~ concat('hello', 'world');
```

#### Inline
```cpp
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
```cpp
(str_$) -> ("$");
```
Would change all occurences of (str_*) to ("*")
### Logical
#### Simple
#### Groups

## Qonfig
