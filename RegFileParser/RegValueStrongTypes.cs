using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     A 32-bit number.
/// </summary>
public sealed class RegValueDword : RegValue
{
    internal RegValueDword(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     A 32-bit number.
    /// </summary>
    public new Int32 Value => int.Parse(_valueData);
}

/// <summary>
///     A 64-bit number.
/// </summary>
public sealed class RegValueQword : RegValue
{
    internal RegValueQword(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     A 64-bit number.
    /// </summary>
    public new Int64 Value => Int64.Parse(_valueData);
}

/// <summary>
///     Binary data in any form.
/// </summary>
public sealed class RegValueBinary : RegValue
{
    internal RegValueBinary(string keyName, string valueName, RegValueType valueType, string valueData,
        Encoding encoding) : base(keyName, valueName, valueType, valueData, encoding)
    {
    }

    /// <summary>
    ///     Binary data in any form.
    /// </summary>
    public new IEnumerable<byte> Value => _valueData.Split(',').Select(v => Convert.ToByte(v, 16));
}
