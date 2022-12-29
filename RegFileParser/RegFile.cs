using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Nefarius.Utilities.Registry.Util;

namespace Nefarius.Utilities.Registry;

/// <summary>
///     A <see cref="RegFile" /> exception.
/// </summary>
public sealed class RegFileException : Exception
{
    internal RegFileException(string message) : base(message)
    {
    }

    internal RegFileException(string message, Exception inner) : base(message, inner)
    {
    }
}

/// <summary>
///     The main reg file parsing class. Reads the given reg file and stores the content as a Dictionary of registry keys
///     and values as a Dictionary of registry values <see cref="RegValue" />.
/// </summary>
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
[SuppressMessage("ReSharper", "ReplaceSliceWithRangeIndexer")]
public sealed class RegFile : IDisposable
{
    private readonly Stream _stream;
    private string _content;

    /// <summary>
    ///     New instance of <see cref="RegFile" />.
    /// </summary>
    /// <param name="stream">The stream to read from.</param>
    /// <remarks>If this instance gets disposed, the provided stream will be disposed as well.</remarks>
    public RegFile(Stream stream)
    {
        _stream = stream;
        Read();
    }

    /// <summary>
    ///     New instance of <see cref="RegFile" />.
    /// </summary>
    /// <param name="regFileName">The full path to the file to parse.</param>
    public RegFile(string regFileName)
    {
        _stream = File.OpenRead(regFileName);
        Read();
    }

    /// <summary>
    ///     Gets the dictionary containing all entries
    /// </summary>
    public Dictionary<string, Dictionary<string, RegValue>> RegValues { get; } = new();

    /// <summary>
    ///     Gets or sets the encoding schema of the reg file (UTF8 or Default)
    /// </summary>
    public Encoding FileEncoding { get; private set; } = Encoding.UTF8;

    /// <inheritdoc />
    public void Dispose()
    {
        _stream?.Dispose();
    }

    /// <summary>
    ///     Imports the reg file
    /// </summary>
    private void Read()
    {
        using StreamReader sr = new(_stream);

        _content = sr.ReadToEnd();
        FileEncoding = GetEncoding();

        Dictionary<string, Dictionary<string, string>> normalizedContent;

        try
        {
            normalizedContent = ParseFile();
        }
        catch (Exception ex)
        {
            throw new RegFileException("Error reading reg file.", ex);
        }

        if (normalizedContent == null)
        {
            throw new RegFileException("Error normalizing reg file content.");
        }

        foreach ((string keyName, Dictionary<string, string> values) in normalizedContent)
        {
            Dictionary<string, RegValue> regValueList = new();

            foreach (KeyValuePair<string, string> item in values)
            {
                try
                {
                    RegValueType type = RegValueType.FromEncodedType(item.Value);
                    string valueName = item.Key;
                    string valueData = item.Value;

                    type
                        // DWORD
                        .When(RegValueType.Dword).Then(() => regValueList.Add(item.Key,
                            new RegValueDword(keyName, valueName, type, valueData, FileEncoding)))
                        // QWORD
                        .When(RegValueType.Qword).Then(() => regValueList.Add(item.Key,
                            new RegValueQword(keyName, valueName, type, valueData, FileEncoding)))
                        // Binary
                        .When(RegValueType.Binary).Then(() => regValueList.Add(item.Key,
                            new RegValueBinary(keyName, valueName, type, valueData, FileEncoding)))
                        // String list
                        .When(RegValueType.MultiSz).Then(() => regValueList.Add(item.Key,
                            new RegValueMultiSz(keyName, valueName, type, valueData, FileEncoding)))
                        // String
                        .When(RegValueType.Sz).Then(() => regValueList.Add(item.Key,
                            new RegValueSz(keyName, valueName, type, valueData, FileEncoding)))
                        // No value
                        .When(RegValueType.None).Then(() => regValueList.Add(item.Key,
                            new RegValueNone(keyName, valueName, type, valueData, FileEncoding)))
                        // Fallback (value will remain string type)
                        .Default(() =>
                            regValueList.Add(item.Key,
                                new RegValue(keyName, valueName, type, valueData, FileEncoding)));
                }
                catch (Exception ex)
                {
                    throw new RegFileException($"Exception thrown on processing string {item}", ex);
                }
            }

            RegValues.Add(keyName, regValueList);
        }
    }

    /// <summary>
    ///     Parses the reg file for reg keys and reg values
    /// </summary>
    /// <returns>A Dictionary with reg keys as Dictionary keys and a Dictionary of (valuename, valuedata)</returns>
    private Dictionary<string, Dictionary<string, string>> ParseFile()
    {
        Dictionary<string, Dictionary<string, string>> retValue = new();

        try
        {
            //Get registry keys and values content string
            //Change proposed by Jenda27
            //Dictionary<String, String> dictKeys = NormalizeDictionary("^[\t ]*\\[.+\\]\r\n", content, true);
            Dictionary<string, string> dictKeys = NormalizeKeysDictionary(_content);

            //Get registry values for a given key
            foreach (KeyValuePair<string, string> item in dictKeys)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    continue;
                }

                //Dictionary<String, String> dictValues = NormalizeDictionary("^[\t ]*(\".+\"|@)=", item.Value, false);
                Dictionary<string, string> dictValues = NormalizeValuesDictionary(item.Value);
                retValue.Add(item.Key, dictValues);
            }
        }
        catch (Exception ex)
        {
            throw new RegFileException("Exception thrown on parsing reg file.", ex);
        }

        return retValue;
    }

    /// <summary>
    ///     Creates a flat Dictionary using given search pattern
    /// </summary>
    /// <param name="content">The content string to be parsed</param>
    /// <returns>A Dictionary with retrieved keys and remaining content</returns>
    internal static Dictionary<string, string> NormalizeKeysDictionary(string content)
    {
        const string searchPattern = "^[\t ]*\\[.+\\][\r\n]+";
        MatchCollection matches = Regex.Matches(content, searchPattern, RegexOptions.Multiline);

        ReadOnlySpan<char> input = content.AsSpan();
        Dictionary<string, string> dictKeys = new();

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            try
            {
                //Retrieve key
                ReadOnlySpan<char> sKey = match.Value.AsSpan();
                //change proposed by Jenda27
                //if (sKey.EndsWith("\r\n")) sKey = sKey.Substring(0, sKey.Length - 2);
                while (sKey.EndsWith("\r\n"))
                {
                    sKey = sKey.Slice(0, sKey.Length - 2);
                }

                if (sKey.EndsWith("="))
                {
                    sKey = sKey.Slice(0, sKey.Length - 1);
                }

                sKey = sKey.StripeBraces();
                sKey = sKey == "@" ? "" : sKey.StripLeadingChars("\"");

                //Retrieve value
                int startIndex = match.Index + match.Length;
                Match nextMatch = match.NextMatch();
                int lengthIndex = (nextMatch.Success ? nextMatch.Index : content.Length) - startIndex;
                ReadOnlySpan<char> sValue = input.Slice(startIndex, lengthIndex);
                //Removing the ending CR
                //change suggested by Jenda27
                //if (sValue.EndsWith("\r\n")) sValue = sValue.Substring(0, sValue.Length - 2);
                while (sValue.EndsWith("\r\n"))
                {
                    sValue = sValue.Slice(0, sValue.Length - 2);
                }

                //fix for the double key names issue
                //dictKeys.Add(sKey, sValue);
                if (dictKeys.ContainsKey(sKey.ToString()))
                {
                    string key = dictKeys[sKey.ToString()];
                    StringBuilder sb = new(key);
                    if (!key.EndsWith(Environment.NewLine))
                    {
                        sb.AppendLine();
                    }

                    sb.Append(sValue);
                    dictKeys[sKey.ToString()] = sb.ToString();
                }
                else
                {
                    dictKeys.Add(sKey.ToString(), sValue.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RegFileException($"Exception thrown on processing string {match.Value}", ex);
            }
        }

        return dictKeys;
    }

    /// <summary>
    ///     Creates a flat Dictionary using given search pattern
    /// </summary>
    /// <param name="input">The content string to be parsed</param>
    /// <returns>A Dictionary with retrieved keys and remaining content</returns>
    internal static Dictionary<string, string> NormalizeValuesDictionary(string input)
    {
        const string searchPattern = @"^[\t ]*("".+""|@)=(""[^""]*""|[^""]+)";
        MatchCollection matches = Regex.Matches(input, searchPattern, RegexOptions.Multiline);

        Dictionary<string, string> dictKeys = new();

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            try
            {
                //Retrieve key
                var sKey = match.Groups[1].Value.AsSpan();

                //Retrieve value
                var sValue = match.Groups[2].Value.AsSpan();

                //Removing the ending CR
                while (sKey.EndsWith("\r\n"))
                {
                    sKey = sKey.Slice(0, sKey.Length - 2);
                }

                sKey = sKey == "@" ? "" : sKey.StripLeadingChars("\"");

                while (sValue.EndsWith("\r\n"))
                {
                    sValue = sValue.Slice(0, sValue.Length - 2);
                }

                if (dictKeys.ContainsKey(sKey.ToString()))
                {
                    string key = dictKeys[sKey.ToString()];
                    StringBuilder sb = new(key);
                    if (!key.EndsWith(Environment.NewLine))
                    {
                        sb.AppendLine();
                    }

                    sb.Append(sValue);
                    dictKeys[sKey.ToString()] = sb.ToString();
                }
                else
                {
                    dictKeys.Add(sKey.ToString(), sValue.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RegFileException($"Exception thrown on processing string {match.Value}", ex);
            }
        }

        return dictKeys;
    }

    /// <summary>
    ///     Retrieves the encoding of the reg file, checking the word "REGEDIT4"
    /// </summary>
    private Encoding GetEncoding()
    {
        return Regex.IsMatch(_content, "([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline)
            ? Encoding.Default
            : Encoding.UTF8;
    }
}