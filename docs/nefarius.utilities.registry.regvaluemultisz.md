# RegValueMultiSz

Namespace: Nefarius.Utilities.Registry

A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
 String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
 terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
 factored into the length of the string.

```csharp
public sealed class RegValueMultiSz : RegValue
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RegValue](./nefarius.utilities.registry.regvalue.md) → [RegValueMultiSz](./nefarius.utilities.registry.regvaluemultisz.md)

## Properties

### <a id="properties-encoding"/>**Encoding**

The text encoding of the data value.

```csharp
public Encoding Encoding { get; }
```

#### Property Value

[Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

### <a id="properties-entry"/>**Entry**

Registry value name

```csharp
public string Entry { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-parentkey"/>**ParentKey**

Registry value parent key

```csharp
public string ParentKey { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-parentkeywithoutroot"/>**ParentKeyWithoutRoot**

Parent key without root.

```csharp
public string ParentKeyWithoutRoot { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-root"/>**Root**

Registry value root hive

```csharp
public string Root { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-type"/>**Type**

Registry value type

```csharp
public RegValueType Type { get; }
```

#### Property Value

[RegValueType](./nefarius.utilities.registry.regvaluetype.md)<br>

### <a id="properties-value"/>**Value**

A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
 String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
 terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
 factored into the length of the string.

```csharp
public IEnumerable<String> Value { get; }
```

#### Property Value

[IEnumerable&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ienumerable-1)<br>

### <a id="properties-value"/>**Value**

Registry value data (REG_SZ).

```csharp
public string Value { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
