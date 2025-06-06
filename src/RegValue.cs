﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

using Nefarius.Utilities.Registry.Util;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     A registry value.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
[SuppressMessage("ReSharper", "InconsistentNaming")]
public class RegValue
{
    private const string Hklm = "HKEY_LOCAL_MACHINE";
    private const string Hkcr = "HKEY_CLASSES_ROOT";
    private const string Hkus = "HKEY_USERS";
    private const string Hkcc = "HKEY_CURRENT_CONFIG";
    private const string Hkcu = "HKEY_CURRENT_USER";

    private readonly int _parentKeyWithoutRootIndex;
    private readonly (int start, int length) _rootIndex;

    /// <summary>
    ///     Full value data.
    /// </summary>
    protected readonly string _valueData;

    /// <summary>
    ///     The offset where the actual value data starts.
    /// </summary>
    protected readonly int _valueStartIndex;

    /// <summary>
    ///     Overloaded constructor
    /// </summary>
    internal RegValue(string keyName, string valueName, RegValueType valueType, string valueData, Encoding encoding)
    {
        Encoding = encoding;
        Entry = valueName;
        Type = valueType;
        ParentKey = keyName.Trim();

        _rootIndex = GetHive(ParentKey, out _parentKeyWithoutRootIndex);
        _valueData = valueData;
        _valueStartIndex = Type.EncodedType?.Length ?? 0;
    }

    /// <summary>
    ///     The text encoding of the data value.
    /// </summary>
    public Encoding Encoding { get; }

    /// <summary>
    ///     Registry value name
    /// </summary>
    public string Entry { get; }

    /// <summary>
    ///     Registry value parent key
    /// </summary>
    public string ParentKey { get; }

    /// <summary>
    ///     Registry value root hive
    /// </summary>
    public string Root => ParentKey.AsSpan().Slice(_rootIndex.start, _rootIndex.length).ToString();

    /// <summary>
    ///     Registry value type
    /// </summary>
    public RegValueType Type { get; }

    /// <summary>
    ///     Registry value data (REG_SZ).
    /// </summary>
    public string Value
    {
        get
        {
            string line = _valueData.AsSpan().Slice(_valueStartIndex).ToString();

            try
            {
                line = Regex.Unescape(line);
            }
            catch (ArgumentException) { }

            return line.StripLeadingChars("\"").ToString();
        }
    }

    /// <summary>
    ///     Parent key without root.
    /// </summary>
    public string ParentKeyWithoutRoot => ParentKey.AsSpan().Slice(_parentKeyWithoutRootIndex).ToString();

    /// <summary>
    ///     Gets the string without prefix or special characters.
    /// </summary>
    protected string RefinedValueString =>
        _valueData.AsSpan()
            .Slice(_valueStartIndex)
            .ToString()
            .StripContinueChar();

    /// <summary>
    ///     Overriden Method
    /// </summary>
    /// <returns>An entry for the [Registry] section of the *.sig signature file</returns>
    public override string ToString()
    {
        return $"{ParentKey}\\{Entry}={Type.EncodedType}{Value}";
    }

    private static (int start, int length) GetHive(string subKey, out int keyWithoutRootStartIndex)
    {
        ReadOnlySpan<char> sKey = subKey.AsSpan();
        ReadOnlySpan<char> tKey = sKey.Trim();

        if (tKey.StartsWith(Hklm))
        {
            keyWithoutRootStartIndex = Hklm.Length;
            ReadOnlySpan<char> slice = sKey.Slice(keyWithoutRootStartIndex);
            if (slice.StartsWith("\\"))
            {
                keyWithoutRootStartIndex += 1;
            }

            return (0, Hklm.Length);
        }

        if (tKey.StartsWith(Hkcr))
        {
            keyWithoutRootStartIndex = Hkcr.Length;
            ReadOnlySpan<char> slice = sKey.Slice(keyWithoutRootStartIndex);
            if (slice.StartsWith("\\"))
            {
                keyWithoutRootStartIndex += 1;
            }

            return (0, Hkcr.Length);
        }

        if (tKey.StartsWith(Hkus))
        {
            keyWithoutRootStartIndex = Hkus.Length;
            ReadOnlySpan<char> slice = sKey.Slice(keyWithoutRootStartIndex);
            if (slice.StartsWith("\\"))
            {
                keyWithoutRootStartIndex += 1;
            }

            return (0, Hkus.Length);
        }

        if (tKey.StartsWith(Hkcc))
        {
            keyWithoutRootStartIndex = Hkcc.Length;
            ReadOnlySpan<char> slice = sKey.Slice(keyWithoutRootStartIndex);
            if (slice.StartsWith("\\"))
            {
                keyWithoutRootStartIndex += 1;
            }

            return (0, Hkcc.Length);
        }

        if (tKey.StartsWith(Hkcu))
        {
            keyWithoutRootStartIndex = Hkcu.Length;
            ReadOnlySpan<char> slice = sKey.Slice(keyWithoutRootStartIndex);
            if (slice.StartsWith("\\"))
            {
                keyWithoutRootStartIndex += 1;
            }

            return (0, Hkcu.Length);
        }

        keyWithoutRootStartIndex = 0;
        return (0, 0);
    }

    /// <summary>
    ///     Converts the byte arrays (saved as array of string) into string
    /// </summary>
    internal static string GetStringRepresentation(IReadOnlyList<string> stringArray, Encoding encoding)
    {
        if (stringArray.Count <= 1)
        {
            return string.Empty;
        }

        StringBuilder sb = new();

        if (Equals(encoding, Encoding.UTF8))
        {
            for (int i = 0; i < stringArray.Count - 2; i += 2)
            {
                string tmpCharacter = stringArray[i + 1] + stringArray[i];
                if (tmpCharacter == "0000")
                {
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    char tmpChar = Convert.ToChar(Convert.ToInt32(tmpCharacter, 16));
                    sb.Append(tmpChar);
                }
            }
        }
        else
        {
            for (int i = 0; i < stringArray.Count - 1; i += 1)
            {
                if (stringArray[i] == "00")
                {
                    sb.Append(Environment.NewLine);
                }
                else
                {
                    char tmpChar = Convert.ToChar(Convert.ToInt32(stringArray[i], 16));
                    sb.Append(tmpChar);
                }
            }
        }

        return sb.ToString();
    }
}