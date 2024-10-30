using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     A 32-bit number.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueDword : RegValue
{
    internal RegValueDword(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     A 32-bit number.
    /// </summary>
    public new Int32 Value => int.Parse(_valueData.AsSpan().Slice(_valueStartIndex), NumberStyles.HexNumber);
}

/// <summary>
///     A 64-bit number.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueQword : RegValue
{
    internal RegValueQword(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     A 64-bit number.
    /// </summary>
    public new Int64 Value => Int64.Parse(_valueData.AsSpan().Slice(_valueStartIndex), NumberStyles.HexNumber);
}

/// <summary>
///     Binary data in any form.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueBinary : RegValue
{
    internal RegValueBinary(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     Binary data in any form.
    /// </summary>
    public new IEnumerable<byte> Value => RefinedValueString
        .Split(',')
        .Select(v => Convert.ToByte(v, 16));
}

/// <summary>
///     A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
///     String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
///     terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
///     factored into the length of the string.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueMultiSz : RegValue
{
    internal RegValueMultiSz(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     A sequence of null-terminated strings, terminated by an empty string (\0). The following is an example:
    ///     String1\0String2\0String3\0LastString\0\0 The first \0 terminates the first string, the second to the last \0
    ///     terminates the last string, and the final \0 terminates the sequence. Note that the final terminator must be
    ///     factored into the length of the string.
    /// </summary>
    public new IEnumerable<string> Value
    {
        get
        {
            string source = RefinedValueString;

            if (string.IsNullOrEmpty(source))
            {
                return Array.Empty<string>();
            }

            byte[] bytes = source
                .Split(',')
                .Select(v => Convert.ToByte(v, 16))
                .ToArray();

            // Trims away potential redundant NULL-characters and splits at NULL-terminator
            string trimmed = Encoding.Unicode.GetString(bytes).TrimEnd(char.MinValue);

            // Split at NULL-character
            return string.IsNullOrWhiteSpace(trimmed) ? Array.Empty<string>() : trimmed.Split(char.MinValue);
        }
    }
}

/// <summary>
///     A null-terminated string. This will be either a Unicode or an ANSI string, depending on whether you use the Unicode
///     or ANSI functions.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueSz : RegValue
{
    internal RegValueSz(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }
}

/// <summary>
///     No defined value type.
/// </summary>
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public sealed class RegValueNone : RegValue
{
    internal RegValueNone(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     No defined value type.
    /// </summary>
    public new object Value => null;
}