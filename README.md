# Refit IEnumerable Query Bug

Tested using Refit Version `4.7.51`, which was released on August 20th, 2019.

## Explanation

When using a container class for query string properties, Refit serializes IEnumerable members that are **Not** objects, incorrectly. 

Note: It is important that the IEnumerable be in a container class to reproduce this bug.

IEnumerables of types `int`, `bool`, `enum`, etc are not processed correctly, but other types which inherit from type `object`, will serialize correctly (with the exception being type `string`, which also will not serialize correctly for some reason).

## Defining Correct vs Incorrect

Using the Query String object, "MyQuery", as defined in [Program.cs](.Program.cs), we should get the URL: 

```https://example.com/api/v1/example/5?ListInts[]=32&ListInts[]=42&ListInts[]=52&ListBools[]=True&ListBools[]=False&ListEnums[]=Second&ListEnums[]=Third&ListStrings[]=Bird&ListStrings[]=word```

Instead we get:

```https://example.com/api/v1/example/5?ListInts[]=System.Collections.Generic.List`1[System.Int32]&ListBools[]=System.Collections.Generic.List`1[System.Boolean]&ListEnums[]=System.Collections.Generic.List`1[RefitIEnumerableBug.ExampleEnum]&ListStrings[]=System.Collections.Generic.List`1[System.String]```

## Workaround:

This can be worked around by making the items within the IEnumerable property, nullable. This works because the Type `Nullable` inherits from the base `object` class. The type `string` is a reference type, so cannot be made nullable, meaning that this workaround does not work for `IEnumerable<string>`.

To see the workaround in action, switch to the "nullable" branch ( `git checkout nullable` ).