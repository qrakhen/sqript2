# Sqript
## Better, faster, cooler

## Usage & Features

### Aliases
First of all, it is important to note that aliases exist.
If you don't like the usage of a certain keyword or operator (i.e. `*~`),
you can either add your own alias, or use the default text version.

In all following examples, `*~` is used instead of the default `var`.

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

// typed reference
Number &c <& a; 
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

## Qonfig
