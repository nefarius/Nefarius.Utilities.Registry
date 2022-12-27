using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.RegularExpressions;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     A registry value.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
public class RegValue
{
    private string _parentKey;
    private string _parentKeyWithoutRoot;

    /// <summary>
    ///     Overloaded constructor
    /// </summary>
    internal RegValue(string regKeyName, string regValueName, string regValueData, Encoding encoding)
    {
        _parentKey = regKeyName.Trim();
        _parentKeyWithoutRoot = _parentKey;
        Root = GetHive(ref _parentKeyWithoutRoot);
        Entry = regValueName;
        Value = regValueData;
        string tmpStringValue = Value;
        Type = GetRegEntryType(ref tmpStringValue, encoding);
        Value = tmpStringValue;
    }

    /// <summary>
    ///     Registry value name
    /// </summary>
    public string Entry { get; set; }

    /// <summary>
    ///     Registry value parent key
    /// </summary>
    public string ParentKey
    {
        get => _parentKey;
        set
        {
            _parentKey = value;
            _parentKeyWithoutRoot = _parentKey;
            Root = GetHive(ref _parentKeyWithoutRoot);
        }
    }

    /// <summary>
    ///     Registry value root hive
    /// </summary>
    public string Root { get; set; }

    /// <summary>
    ///     Registry value type
    /// </summary>
    public string Type { get; set; }

    /// <summary>
    ///     Registry value data
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    ///     Parent key without root.
    /// </summary>
    public string ParentKeyWithoutRoot
    {
        get => _parentKeyWithoutRoot;
        set => _parentKeyWithoutRoot = value;
    }

    /// <summary>
    ///     Overriden Method
    /// </summary>
    /// <returns>An entry for the [Registry] section of the *.sig signature file</returns>
    public override string ToString()
    {
        return $"{_parentKey}\\\\{Entry}={SetRegEntryType(Type)}{Value}";
    }

    private static string GetHive(ref string subKey)
    {
        string tmpLine = subKey.Trim();

        if (tmpLine.StartsWith("HKEY_LOCAL_MACHINE"))
        {
            subKey = subKey.Substring(18);
            if (subKey.StartsWith("\\"))
            {
                subKey = subKey.Substring(1);
            }

            return "HKEY_LOCAL_MACHINE";
        }

        if (tmpLine.StartsWith("HKEY_CLASSES_ROOT"))
        {
            subKey = subKey.Substring(17);
            if (subKey.StartsWith("\\"))
            {
                subKey = subKey.Substring(1);
            }

            return "HKEY_CLASSES_ROOT";
        }

        if (tmpLine.StartsWith("HKEY_USERS"))
        {
            subKey = subKey.Substring(10);
            if (subKey.StartsWith("\\"))
            {
                subKey = subKey.Substring(1);
            }

            return "HKEY_USERS";
        }

        if (tmpLine.StartsWith("HKEY_CURRENT_CONFIG"))
        {
            subKey = subKey.Substring(19);
            if (subKey.StartsWith("\\"))
            {
                subKey = subKey.Substring(1);
            }

            return "HKEY_CURRENT_CONFIG";
        }

        if (tmpLine.StartsWith("HKEY_CURRENT_USER"))
        {
            subKey = subKey.Substring(17);
            if (subKey.StartsWith("\\"))
            {
                subKey = subKey.Substring(1);
            }

            return "HKEY_CURRENT_USER";
        }

        return null;
    }

    /// <summary>
    ///     Retrieves the reg value type, parsing the prefix of the value
    /// </summary>
    internal static string GetRegEntryType(ref string sTextLine, Encoding textEncoding)
    {
        if (sTextLine.StartsWith("hex(a):"))
        {
            sTextLine = sTextLine.Substring(7);
            return "REG_RESOURCE_REQUIREMENTS_LIST";
        }

        if (sTextLine.StartsWith("hex(b):"))
        {
            sTextLine = sTextLine.Substring(7);
            return "REG_QWORD";
        }

        if (sTextLine.StartsWith("dword:"))
        {
            sTextLine = Convert.ToInt32(sTextLine.Substring(6), 16).ToString();
            return "REG_DWORD";
        }

        if (sTextLine.StartsWith("hex(7):"))
        {
            sTextLine = StripeContinueChar(sTextLine.Substring(7));
            sTextLine = GetStringRepresentation(sTextLine.Split(','), textEncoding);
            return "REG_MULTI_SZ";
        }

        if (sTextLine.StartsWith("hex(6):"))
        {
            sTextLine = StripeContinueChar(sTextLine.Substring(7));
            sTextLine = GetStringRepresentation(sTextLine.Split(','), textEncoding);
            return "REG_LINK";
        }

        if (sTextLine.StartsWith("hex(2):"))
        {
            sTextLine = StripeContinueChar(sTextLine.Substring(7));
            sTextLine = GetStringRepresentation(sTextLine.Split(','), textEncoding);
            return "REG_EXPAND_SZ";
        }

        if (sTextLine.StartsWith("hex(0):"))
        {
            sTextLine = sTextLine.Substring(7);
            return "REG_NONE";
        }

        if (sTextLine.StartsWith("hex:"))
        {
            sTextLine = StripeContinueChar(sTextLine.Substring(4));
            if (sTextLine.EndsWith(","))
            {
                sTextLine = sTextLine.Substring(0, sTextLine.Length - 1);
            }

            return "REG_BINARY";
        }

        sTextLine = Regex.Unescape(sTextLine);
        sTextLine = StripeLeadingChars(sTextLine, "\"");
        return "REG_SZ";
    }

    internal static string SetRegEntryType(string sRegDataType)
    {
        switch (sRegDataType)
        {
            case "REG_QWORD":
                return "hex(b):";

            case "REG_RESOURCE_REQUIREMENTS_LIST":
                return "hex(a):";

            case "REG_FULL_RESOURCE_DESCRIPTOR":
                return "hex(9):";

            case "REG_RESOURCE_LIST":
                return "hex(8):";

            case "REG_MULTI_SZ":
                return "hex(7):";

            case "REG_LINK":
                return "hex(6):";

            case "REG_DWORD":
                return "dword:";

            case "REG_EXPAND_SZ":
                return "hex(2):";

            case "REG_NONE":
                return "hex(0):";

            case "REG_BINARY":
                return "hex:";

            case "REG_SZ":
                return null;

            default:
                return null;
        }
        /*
        hex: REG_BINARY
        hex(0): REG_NONE
        hex(1): REG_SZ
        hex(2): EXPAND_SZ
        hex(3): REG_BINARY
        hex(4): REG_DWORD
        hex(5): REG_DWORD_BIG_ENDIAN ; invalid type ?
        hex(6): REG_LINK
        hex(7): REG_MULTI_SZ
        hex(8): REG_RESOURCE_LIST
        hex(9): REG_FULL_RESOURCE_DESCRIPTOR
        hex(a): REG_RESOURCE_REQUIREMENTS_LIST
        hex(b): REG_QWORD
        */
    }

    /// <summary>
    ///     Removes the leading and ending characters from the given string
    /// </summary>
    internal static string StripeLeadingChars(string line, string leadChar)
    {
        string value = line.Trim();
        if (value.StartsWith(leadChar) & value.EndsWith(leadChar))
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }

    /// <summary>
    ///     Removes the leading and ending parenthesis from the given string
    /// </summary>
    internal static string StripeBraces(string line)
    {
        string value = line.Trim();
        if (value.StartsWith("[") & value.EndsWith("]"))
        {
            return value.Substring(1, value.Length - 2);
        }

        return value;
    }

    /// <summary>
    ///     Removes the ending backslashes from the given string
    /// </summary>
    internal static string StripeContinueChar(string line)
    {
        return Regex.Replace(line, "\\\\\r\n[ ]*", string.Empty);
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