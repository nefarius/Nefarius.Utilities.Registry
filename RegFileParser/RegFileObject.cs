using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Nefarius.Utilities.Registry
{
    /// <summary>
    ///     The main reg file parsing class.
    ///     Reads the given reg file and stores the content as
    ///     a Dictionary of registry keys and values as a Dictionary of registry values <see cref="RegValueObject" />
    /// </summary>
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "UnusedMember.Global")]
    public class RegFileObject
    {
        #region Private Fields

        /// <summary>
        ///     The full path of the reg file to be imported
        /// </summary>
        private string _path;

        /// <summary>
        ///     Raw content of the reg file
        /// </summary>
        private string _content;

        #endregion

        #region Public Properties

        /// <summary>
        ///     Gets or sets the full path of the reg file
        /// </summary>
        public string FullPath
        {
            get => _path;
            set
            {
                _path = value;
                FileName = Path.GetFileName(_path);
            }
        }

        /// <summary>
        ///     Gets the name of the reg file
        /// </summary>
        public string FileName { get; private set; }

        /// <summary>
        ///     Gets the dictionary containing all entries
        /// </summary>
        public Dictionary<String, Dictionary<string, RegValueObject>> RegValues { get; }

        /// <summary>
        ///     Gets or sets the encoding schema of the reg file (UTF8 or Default)
        /// </summary>
        public string Encoding { get; set; }

        #endregion

        #region Constructors

        public RegFileObject()
        {
            _path = "";
            FileName = "";
            Encoding = "UTF8";
            RegValues = new Dictionary<String, Dictionary<string, RegValueObject>>();
        }

        public RegFileObject(string RegFileName)
        {
            _path = RegFileName;
            FileName = Path.GetFileName(_path);
            Encoding = "UTF8";
            RegValues = new Dictionary<String, Dictionary<string, RegValueObject>>();
            Read();
        }

        #endregion

        #region Private Methods

        /// <summary>
        ///     Imports the reg file
        /// </summary>
        public void Read()
        {
            if (!File.Exists(_path))
            {
                throw new ArgumentException("Provided file path doesn't exist.");
            }

            _content = File.ReadAllText(_path);
            Encoding = GetEncoding();

            Dictionary<String, Dictionary<String, String>> normalizedContent = null;
            try
            {
                normalizedContent = ParseFile();
            }
            catch (Exception ex)
            {
                throw new Exception("Error reading reg file.", ex);
            }

            if (normalizedContent == null)
            {
                throw new Exception("Error normalizing reg file content.");
            }

            foreach (KeyValuePair<string, Dictionary<string, string>> entry in normalizedContent)
            {
                Dictionary<string, RegValueObject> regValueList = new Dictionary<string, RegValueObject>();

                foreach (KeyValuePair<string, string> item in entry.Value)
                {
                    try
                    {
                        regValueList.Add(item.Key, new RegValueObject(entry.Key, item.Key, item.Value, Encoding));
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"Exception thrown on processing string {item}", ex);
                    }
                }

                RegValues.Add(entry.Key, regValueList);
            }
        }

        /// <summary>
        ///     Parses the reg file for reg keys and reg values
        /// </summary>
        /// <returns>A Dictionary with reg keys as Dictionary keys and a Dictionary of (valuename, valuedata)</returns>
        private Dictionary<String, Dictionary<String, String>> ParseFile()
        {
            Dictionary<string, Dictionary<string, string>> retValue =
                new Dictionary<string, Dictionary<string, string>>();

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
                throw new Exception("Exception thrown on parsing reg file.", ex);
            }

            return retValue;
        }

        /// <summary>
        ///     Creates a flat Dictionary using given searcn pattern
        /// </summary>
        /// <param name="content">The content string to be parsed</param>
        /// <returns>A Dictionary with retrieved keys and remaining content</returns>
        private static Dictionary<String, String> NormalizeKeysDictionary(String content)
        {
            string searchPattern = "^[\t ]*\\[.+\\][\r\n]+";
            MatchCollection matches = Regex.Matches(content, searchPattern, RegexOptions.Multiline);

            int startIndex = 0;
            int lengthIndex = 0;
            Dictionary<string, string> dictKeys = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                try
                {
                    //Retrieve key
                    string sKey = match.Value;
                    //change proposed by Jenda27
                    //if (sKey.EndsWith("\r\n")) sKey = sKey.Substring(0, sKey.Length - 2);
                    while (sKey.EndsWith("\r\n"))
                    {
                        sKey = sKey.Substring(0, sKey.Length - 2);
                    }

                    if (sKey.EndsWith("="))
                    {
                        sKey = sKey.Substring(0, sKey.Length - 1);
                    }

                    sKey = StripeBraces(sKey);
                    sKey = sKey == "@" ? "" : StripeLeadingChars(sKey, "\"");

                    //Retrieve value
                    startIndex = match.Index + match.Length;
                    Match nextMatch = match.NextMatch();
                    lengthIndex = (nextMatch.Success ? nextMatch.Index : content.Length) - startIndex;
                    string sValue = content.Substring(startIndex, lengthIndex);
                    //Removing the ending CR
                    //change suggested by Jenda27
                    //if (sValue.EndsWith("\r\n")) sValue = sValue.Substring(0, sValue.Length - 2);
                    while (sValue.EndsWith("\r\n"))
                    {
                        sValue = sValue.Substring(0, sValue.Length - 2);
                    }

                    //fix for the double key names issue
                    //dictKeys.Add(sKey, sValue);
                    if (dictKeys.ContainsKey(sKey))
                    {
                        string tmpcontent = dictKeys[sKey];
                        StringBuilder tmpsb = new StringBuilder(tmpcontent);
                        if (!tmpcontent.EndsWith(Environment.NewLine))
                        {
                            tmpsb.AppendLine();
                        }

                        tmpsb.Append(sValue);
                        dictKeys[sKey] = tmpsb.ToString();
                    }
                    else
                    {
                        dictKeys.Add(sKey, sValue);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception thrown on processing string {match.Value}", ex);
                }
            }

            return dictKeys;
        }

        /// <summary>
        ///     Creates a flat Dictionary using given search pattern
        /// </summary>
        /// <param name="input">The content string to be parsed</param>
        /// <returns>A Dictionary with retrieved keys and remaining content</returns>
        private Dictionary<string, string> NormalizeValuesDictionary(string input)
        {
            const string searchPattern = @"^[\t ]*("".+""|@)=(""[^""]*""|[^""]+)";
            MatchCollection matches = Regex.Matches(input, searchPattern, RegexOptions.Multiline);

            Dictionary<string, string> dictKeys = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                try
                {
                    //Retrieve key
                    string sKey = match.Groups[1].Value;

                    //Retrieve value
                    string sValue = match.Groups[2].Value;

                    //Removing the ending CR
                    while (sKey.EndsWith("\r\n"))
                    {
                        sKey = sKey.Substring(0, sKey.Length - 2);
                    }

                    sKey = sKey == "@" ? "" : StripeLeadingChars(sKey, "\"");

                    while (sValue.EndsWith("\r\n"))
                    {
                        sValue = sValue.Substring(0, sValue.Length - 2);
                    }

                    if (dictKeys.ContainsKey(sKey))
                    {
                        string tmpcontent = dictKeys[sKey];
                        StringBuilder tmpsb = new StringBuilder(tmpcontent);
                        if (!tmpcontent.EndsWith(Environment.NewLine))
                        {
                            tmpsb.AppendLine();
                        }

                        tmpsb.Append(sValue);
                        dictKeys[sKey] = tmpsb.ToString();
                    }
                    else
                    {
                        dictKeys.Add(sKey, sValue);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Exception thrown on processing string {match.Value}", ex);
                }
            }

            return dictKeys;
        }

        /// <summary>
        ///     Removes the leading and ending characters from the given string
        /// </summary>
        /// <param name="sLine">given string</param>
        /// <param name="leadChar"></param>
        /// <returns>edited string</returns>
        /// <remarks></remarks>
        private static string StripeLeadingChars(string sLine, string leadChar)
        {
            string tmpvalue = sLine.Trim();
            if (tmpvalue.StartsWith(leadChar) & tmpvalue.EndsWith(leadChar))
            {
                return tmpvalue.Substring(1, tmpvalue.Length - 2);
            }

            return tmpvalue;
        }

        /// <summary>
        ///     Removes the leading and ending parenthesis from the given string
        /// </summary>
        /// <param name="line">given string</param>
        /// <returns>edited string</returns>
        /// <remarks></remarks>
        private static string StripeBraces(string line)
        {
            string tmpvalue = line.Trim();
            if (tmpvalue.StartsWith("[") & tmpvalue.EndsWith("]"))
            {
                return tmpvalue.Substring(1, tmpvalue.Length - 2);
            }

            return tmpvalue;
        }

        /// <summary>
        ///     Retrieves the encoding of the reg file, checking the word "REGEDIT4"
        /// </summary>
        /// <returns></returns>
        private string GetEncoding()
        {
            return Regex.IsMatch(_content, "([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline)
                ? "ANSI"
                : "UTF8";
        }

        #endregion
    }
}