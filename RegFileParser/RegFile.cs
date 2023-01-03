using System;
using System.Collections.Concurrent;
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
public sealed partial class RegFile
{
    private readonly bool _isStreamOwned;
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
        _isStreamOwned = true;
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

    /// <summary>
    ///     Imports the reg file
    /// </summary>
    private void Read()
    {
        using StreamReader sr = new(_stream);

        _content = sr.ReadToEnd();

        if (_isStreamOwned)
        {
            sr.Dispose();
            _stream.Dispose();
        }

        FileEncoding = GetEncoding();

        ConcurrentDictionary<string, ConcurrentDictionary<string, string>> normalizedContent;

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

        foreach ((string keyName, ConcurrentDictionary<string, string> values) in normalizedContent)
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
    private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> ParseFile()
    {
        ConcurrentDictionary<string, ConcurrentDictionary<string, string>> retValue = new();

        try
        {
            //Get registry keys and values content string
            ConcurrentDictionary<string, string> dictKeys = NormalizeKeysDictionary(_content);

            //Get registry values for a given key
            foreach (KeyValuePair<string, string> item in dictKeys)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    continue;
                }

                ConcurrentDictionary<string, string> dictValues = NormalizeValuesDictionary(item.Value);
                retValue.TryAdd(item.Key, dictValues);
            }
        }
        catch (Exception ex)
        {
            throw new RegFileException("Exception thrown on parsing reg file.", ex);
        }

        return retValue;
    }

    [GeneratedRegex("^[\t ]*\\[.+\\][\r\n]+", RegexOptions.Multiline, "en-US")]
    private static partial Regex RegKeysRegex();

    /// <summary>
    ///     Creates a flat Dictionary using given search pattern
    /// </summary>
    /// <param name="content">The content string to be parsed</param>
    /// <returns>A Dictionary with retrieved keys and remaining content</returns>
    internal static ConcurrentDictionary<string, string> NormalizeKeysDictionary(string content)
    {
        MatchCollection matches = RegKeysRegex().Matches(content);

        ReadOnlySpan<char> input = content.AsSpan();
        ConcurrentDictionary<string, string> dictKeys = new();

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            try
            {
                //Retrieve key
                ReadOnlySpan<char> sKey = match.Value.AsSpan();

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

                while (sValue.EndsWith("\r\n"))
                {
                    sValue = sValue.Slice(0, sValue.Length - 2);
                }

                //fix for the double key names issue
                //dictKeys.Add(sKey, sValue);
                // TODO: see if this can be tuned further
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
                    dictKeys.TryAdd(sKey.ToString(), sValue.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RegFileException($"Exception thrown on processing string {match.Value}", ex);
            }
        }

        return dictKeys;
    }

    [GeneratedRegex(@"^[\t ]*("".+""|@)=(""[^""]*""|[^""]+)", RegexOptions.Multiline, "en-US")]
    private static partial Regex RegValuesRegex();

    /// <summary>
    ///     Creates a flat Dictionary using given search pattern
    /// </summary>
    /// <param name="input">The content string to be parsed</param>
    /// <returns>A Dictionary with retrieved keys and remaining content</returns>
    internal static ConcurrentDictionary<string, string> NormalizeValuesDictionary(string input)
    {
        MatchCollection matches = RegValuesRegex().Matches(input);

        ConcurrentDictionary<string, string> dictKeys = new();

        for (int i = 0; i < matches.Count; i++)
        {
            Match match = matches[i];

            try
            {
                //Retrieve key
                ReadOnlySpan<char> sKey = match.Groups[1].Value.AsSpan();

                //Retrieve value
                ReadOnlySpan<char> sValue = match.Groups[2].Value.AsSpan();

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
                    dictKeys.TryAdd(sKey.ToString(), sValue.ToString());
                }
            }
            catch (Exception ex)
            {
                throw new RegFileException($"Exception thrown on processing string {match.Value}", ex);
            }
        }

        return dictKeys;
    }

    [GeneratedRegex("([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline, "en-US")]
    private static partial Regex RegEncodingRegex();

    /// <summary>
    ///     Retrieves the encoding of the reg file, checking the word "REGEDIT4"
    /// </summary>
    private Encoding GetEncoding()
    {
        return RegEncodingRegex().IsMatch(_content.AsSpan())
            ? Encoding.Default
            : Encoding.UTF8;
    }
}