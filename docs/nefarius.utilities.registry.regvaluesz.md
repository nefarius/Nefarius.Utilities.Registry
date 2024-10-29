# RegValueSz

Namespace: Nefarius.Utilities.Registry

A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use the Unicode
 or ANSI functions.

```csharp
public sealed class RegValueSz : RegValue
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → [RegValue](./nefarius.utilities.registry.regvalue.md) → [RegValueSz](./nefarius.utilities.registry.regvaluesz.md)

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
