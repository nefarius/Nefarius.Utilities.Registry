using System;
using System.Text.RegularExpressions;

namespace Nefarius.Utilities.Registry.Util;

internal static class StringExtensions
{
    /// <summary>
    ///     Removes the ending backslashes from the given string
    /// </summary>
    internal static string StripContinueChar(this string line)
    {
        return Regex.Replace(line, "\\\\\r\n[ ]*", string.Empty);
    }

    /// <summary>
    ///     Removes the leading and ending characters from the given string
    /// </summary>
    internal static ReadOnlySpan<char> StripLeadingChars(this string line, string leadChar)
    {
        return line.AsSpan().StripLeadingChars(leadChar);
    }

    /// <summary>
    ///     Removes the leading and ending characters from the given string
    /// </summary>
    internal static ReadOnlySpan<char> StripLeadingChars(this ReadOnlySpan<char> line, string leadChar)
    {
        ReadOnlySpan<char> value = line.Trim();
        if (value.StartsWith(leadChar) & value.EndsWith(leadChar))
        {
            return value.Slice(1, value.Length - 2);
        }

        return value;
    }

    /// <summary>
    ///     Removes the leading and ending parenthesis from the given string
    /// </summary>
    /// <param name="line">given string</param>
    /// <returns>edited string</returns>
    internal static ReadOnlySpan<char> StripeBraces(this ReadOnlySpan<char> line)
    {
        ReadOnlySpan<char> value = line.Trim();
        if (value.StartsWith("[") & value.EndsWith("]"))
        {
            return value.Slice(1, value.Length - 2);
        }

        return value;
    }
}
