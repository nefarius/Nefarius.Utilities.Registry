# RegFile

Namespace: Nefarius.Utilities.Registry

The main reg file parsing class. Reads the given reg file and stores the content as a Dictionary of registry keys
 and values as a Dictionary of registry values [RegValue](./nefarius.utilities.registry.regvalue.md).

```csharp
public sealed class RegFile
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RegFile](./nefarius.utilities.registry.regfile.md)

## Properties

### <a id="properties-fileencoding"/>**FileEncoding**

Gets or sets the encoding schema of the reg file (UTF8 or Default)

```csharp
public Encoding FileEncoding { get; private set; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### <a id="properties-regvalues"/>**RegValues**

Gets the dictionary containing all entries

```csharp
public Dictionary<String, Dictionary<String, RegValue>> RegValues { get; }
```

#### Property Value

[Dictionary&lt;String, Dictionary&lt;String, RegValue&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.dictionary-2)<br>

## Constructors

### <a id="constructors-.ctor"/>**RegFile(Stream)**

New instance of [RegFile](./nefarius.utilities.registry.regfile.md).

```csharp
public RegFile(Stream stream)
```

#### Parameters

`stream` [Stream](https://docs.microsoft.com/en-us/dotnet/api/system.io.stream)<br>
The stream to read from.

**Remarks:**

If this instance gets disposed, the provided stream will be disposed as well.

### <a id="constructors-.ctor"/>**RegFile(String)**

New instance of [RegFile](./nefarius.utilities.registry.regfile.md).

```csharp
public RegFile(string regFileName)
```

#### Parameters

`regFileName` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The full path to the file to parse.

## Methods

### <a id="methods-normalizekeysdictionary"/>**NormalizeKeysDictionary(String)**

Creates a flat Dictionary using given search pattern

```csharp
internal static ConcurrentDictionary<String, String> NormalizeKeysDictionary(string content)
```

#### Parameters

`content` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The content string to be parsed

#### Returns

A Dictionary with retrieved keys and remaining content

### <a id="methods-normalizevaluesdictionary"/>**NormalizeValuesDictionary(String)**

Creates a flat Dictionary using given search pattern

```csharp
internal static ConcurrentDictionary<String, String> NormalizeValuesDictionary(string input)
```

#### Parameters

`input` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The content string to be parsed

#### Returns

A Dictionary with retrieved keys and remaining content
