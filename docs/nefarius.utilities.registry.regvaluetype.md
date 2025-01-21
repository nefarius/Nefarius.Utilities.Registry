# RegValueType

Namespace: Nefarius.Utilities.Registry

Represents possible data types of a [RegValue](./nefarius.utilities.registry.regvalue.md).

```csharp
public sealed class RegValueType : Ardalis.SmartEnum.SmartEnum`1[[Nefarius.Utilities.Registry.RegValueType, Nefarius.Utilities.Registry, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null]], Ardalis.SmartEnum.ISmartEnum, System.IEquatable`1[[Ardalis.SmartEnum.SmartEnum`2[[Nefarius.Utilities.Registry.RegValueType, Nefarius.Utilities.Registry, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Ardalis.SmartEnum, Version=8.1.0.0, Culture=neutral, PublicKeyToken=null]], System.IComparable`1[[Ardalis.SmartEnum.SmartEnum`2[[Nefarius.Utilities.Registry.RegValueType, Nefarius.Utilities.Registry, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null],[System.Int32, System.Private.CoreLib, Version=9.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]], Ardalis.SmartEnum, Version=8.1.0.0, Culture=neutral, PublicKeyToken=null]]
```

Inheritance [Object](https://docs.microsoft.com/en-us/dotnet/api/system.object) → SmartEnum&lt;RegValueType, Int32&gt; → SmartEnum&lt;RegValueType&gt; → [RegValueType](./nefarius.utilities.registry.regvaluetype.md)<br>
Implements ISmartEnum, [IEquatable&lt;SmartEnum&lt;RegValueType, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.iequatable-1), [IComparable&lt;SmartEnum&lt;RegValueType, Int32&gt;&gt;](https://docs.microsoft.com/en-us/dotnet/api/system.icomparable-1)

## Fields

### <a id="fields-binary"/>**Binary**

Binary data in any form.

```csharp
public static RegValueType Binary;
```

### <a id="fields-dword"/>**Dword**

A 32-bit number.

```csharp
public static RegValueType Dword;
```

### <a id="fields-expandsz"/>**ExpandSz**

A null-terminated string that contains unexpanded references to environment variables (for example, "%PATH%"). It
 will be a Unicode or ANSI string depending on whether you use the Unicode or ANSI functions. To expand the
 environment variable references, use the ExpandEnvironmentStrings function.

```csharp
public static RegValueType ExpandSz;
```

### <a id="fields-fullresourcedescriptor"/>**FullResourceDescriptor**

A list of hardware resources that a physical device is using, detected and written into the \HardwareDescription
 tree by the system.

```csharp
public static RegValueType FullResourceDescriptor;
```

### <a id="fields-link"/>**Link**

A null-terminated Unicode string that contains the target path of a symbolic link that was created by calling the
 RegCreateKeyEx function with REG_OPTION_CREATE_LINK.

```csharp
public static RegValueType Link;
```

### <a id="fields-multisz"/>**MultiSz**

A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
 String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
 terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
 factored into the length of the string.

```csharp
public static RegValueType MultiSz;
```

### <a id="fields-none"/>**None**

No defined value type.

```csharp
public static RegValueType None;
```

### <a id="fields-qword"/>**Qword**

A 64-bit number.

```csharp
public static RegValueType Qword;
```

### <a id="fields-resourcelist"/>**ResourceList**

A device driver's list of hardware resources, used by the driver or one of the physical devices it controls, in the
 \ResourceMap tree.

```csharp
public static RegValueType ResourceList;
```

### <a id="fields-resourcerequirementslist"/>**ResourceRequirementsList**

A device driver's list of possible hardware resources it or one of the physical devices it controls can use, from
 which the system writes a subset into the \ResourceMap tree.

```csharp
public static RegValueType ResourceRequirementsList;
```

### <a id="fields-sz"/>**Sz**

A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use the Unicode
 or ANSI functions.

```csharp
public static RegValueType Sz;
```

## Properties

### <a id="properties-encodedtype"/>**EncodedType**

Gets the encoded string for the type.

```csharp
public string EncodedType { get; private set; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-name"/>**Name**

```csharp
public string Name { get; }
```

#### Property Value

[String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>

### <a id="properties-value"/>**Value**

```csharp
public int Value { get; }
```

#### Property Value

[Int32](https://docs.microsoft.com/en-us/dotnet/api/system.int32)<br>

## Methods

### <a id="methods-fromencodedtype"/>**FromEncodedType(String)**

Retrieves a [RegValueType](./nefarius.utilities.registry.regvaluetype.md) from an encoded value string.

```csharp
public static RegValueType FromEncodedType(string encodedType)
```

#### Parameters

`encodedType` [String](https://docs.microsoft.com/en-us/dotnet/api/system.string)<br>
The encoded value string from the parser.

#### Returns

A [RegValueType](./nefarius.utilities.registry.regvaluetype.md) or null.
