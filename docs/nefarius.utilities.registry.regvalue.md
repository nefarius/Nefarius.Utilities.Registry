# RegValue

Namespace: Nefarius.Utilities.Registry

A registry value.

```csharp
public class RegValue
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RegValue](./nefarius.utilities.registry.regvalue.md)

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

Registry value data (REG_SZ).

```csharp
public string Value { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

## Methods

### <a id="methods-getstringrepresentation"/>**GetStringRepresentation(IReadOnlyList&lt;String&gt;, Encoding)**

Converts the byte arrays (saved as array of string) into string

```csharp
internal static string GetStringRepresentation(IReadOnlyList<String> stringArray, Encoding encoding)
```

#### Parameters

`stringArray` [IReadOnlyList&lt;String&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.collections.generic.ireadonlylist-1)<br>

`encoding` [Encoding](https://docs.microsoft.com/en-us/dotnet/api/system.text.encoding)<br>

#### Returns

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)

### <a id="methods-tostring"/>**ToString()**

Overriden Method

```csharp
public string ToString()
```

#### Returns

An entry for the [Registry] section of the *.sig signature file
