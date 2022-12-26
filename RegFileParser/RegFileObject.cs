using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text.RegularExpressions;

namespace RegFileParser
{

  /// <summary>
  /// The main reg file parsing class.
  /// Reads the given reg file and stores the content as
  /// a Dictionary of registry keys and values as a Dictionary of registry values <see cref="RegValueObject"/>
  /// </summary>
  public class RegFileObject
  {

    #region Private Fields

    /// <summary>
    /// The full path of the reg file to be imported
    /// </summary>
    private string path;

    /// <summary>
    /// The reg file name
    /// </summary>
    private string filename;

    /// <summary>
    /// Encoding of the reg file (Regedit 4 - ANSI; Regedit 5 - UTF8)
    /// </summary>
    private string encoding;

    /// <summary>
    /// Raw content of the reg file
    /// </summary>
    private string content;
    
    /// <summary>
    /// the dictionary containing parsed registry values
    /// </summary>
    private Dictionary<String,Dictionary<string, RegValueObject>> regvalues;

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets or sets the full path of the reg file
    /// </summary>
    public string FullPath
    {
      get { return path; }
      set 
      { 
        path = value;
        filename = Path.GetFileName(path);
      }
    }

    /// <summary>
    /// Gets the name of the reg file
    /// </summary>
    public string FileName
    {
      get { return filename; }
    }

    /// <summary>
    /// Gets the dictionary containing all entries
    /// </summary>
    public Dictionary<String, Dictionary<string, RegValueObject>> RegValues
    {
      get { return regvalues; }
    }

    /// <summary>
    /// Gets or sets the encoding schema of the reg file (UTF8 or Default)
    /// </summary>
    public string Encoding
    {
      get { return encoding; }
      set { encoding = value; }
    }

    #endregion

    #region Constructors

    public RegFileObject()
    {
      path = "";
      filename = "";
      encoding = "UTF8";
      regvalues = new Dictionary<String, Dictionary<string, RegValueObject>>();
    }

    public RegFileObject(string RegFileName)
    {
      path = RegFileName;
      filename = Path.GetFileName(path);
      encoding = "UTF8";
      regvalues = new Dictionary<String, Dictionary<string, RegValueObject>>();
      Read();
    }

    #endregion

    #region Private Methods

    /// <summary>
    /// Imports the reg file
    /// </summary>
    public void Read()
    {
      Dictionary<String, Dictionary<String, String>> normalizedContent = null;

      if (File.Exists(path))
      {
        content = File.ReadAllText(path);
        encoding = GetEncoding();

        try
        {
          normalizedContent = ParseFile();
        }
        catch (Exception ex)
        {
          throw new Exception("Error reading reg file.",ex);
        }

        if (normalizedContent == null)
          throw new Exception("Error normalizing reg file content.");

        foreach (KeyValuePair<String, Dictionary<String, String>> entry in normalizedContent)
        {
          Dictionary<String, RegValueObject> regValueList = new Dictionary<string, RegValueObject>();

          foreach (KeyValuePair<String, String> item in entry.Value)
          {
            try 
	          {	        
              regValueList.Add(item.Key, new RegValueObject(entry.Key, item.Key, item.Value, this.encoding));
            }
            catch (Exception ex)
            {
              throw new Exception(String.Format("Exception thrown on processing string {0}", item), ex);
            }
          }
          regvalues.Add(entry.Key, regValueList);
        }		

      }
    }

    /// <summary>
    /// Parses the reg file for reg keys and reg values
    /// </summary>
    /// <returns>A Dictionary with reg keys as Dictionary keys and a Dictionary of (valuename, valuedata)</returns>
    private Dictionary<String, Dictionary<String, String>> ParseFile()
    {
      Dictionary<String, Dictionary<String, String>> retValue = new Dictionary<string, Dictionary<string, string>>();

      try
      {
        //Get registry keys and values content string
        //Change proposed by Jenda27
        //Dictionary<String, String> dictKeys = NormalizeDictionary("^[\t ]*\\[.+\\]\r\n", content, true);
        Dictionary<String, String> dictKeys = NormalizeKeysDictionary(content);

        //Get registry values for a given key
        foreach (KeyValuePair<String, String> item in dictKeys)
        {
          if (string.IsNullOrEmpty(item.Value)) continue;
          //Dictionary<String, String> dictValues = NormalizeDictionary("^[\t ]*(\".+\"|@)=", item.Value, false);
          Dictionary<String, String> dictValues = NormalizeValuesDictionary(item.Value);
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
    /// Creates a flat Dictionary using given searcn pattern
    /// </summary>
    /// <param name="content">The content string to be parsed</param>
    /// <returns>A Dictionary with retrieved keys and remaining content</returns>
    private Dictionary<String, String> NormalizeKeysDictionary(String content)
    {
      string searchPattern = "^[\t ]*\\[.+\\][\r\n]+";
      MatchCollection matches = Regex.Matches(content, searchPattern, RegexOptions.Multiline);

      Int32 startIndex = 0;
      Int32 lengthIndex = 0;
      Dictionary<String, String> dictKeys = new Dictionary<string, string>();

      foreach (Match match in matches)
      {
        try
        {
          //Retrieve key
          String sKey = match.Value;
          //change proposed by Jenda27
          //if (sKey.EndsWith("\r\n")) sKey = sKey.Substring(0, sKey.Length - 2);
          while (sKey.EndsWith("\r\n"))
          {
            sKey = sKey.Substring(0, sKey.Length - 2);
          }
          if (sKey.EndsWith("=")) sKey = sKey.Substring(0, sKey.Length - 1);
          sKey = StripeBraces(sKey);
          if (sKey == "@") 
            sKey = "";
          else
            sKey = StripeLeadingChars(sKey, "\"");
          
          //Retrieve value
          startIndex = match.Index + match.Length;
          Match nextMatch = match.NextMatch();
          lengthIndex = ((nextMatch.Success) ? nextMatch.Index : content.Length) - startIndex;         
          String sValue = content.Substring(startIndex, lengthIndex);
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
            if (!tmpcontent.EndsWith(Environment.NewLine)) tmpsb.AppendLine();
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
          throw new Exception(String.Format("Exception thrown on processing string {0}", match.Value), ex);
        }
      }
      return dictKeys;
    }

        /// <summary>
        /// Creates a flat Dictionary using given searcn pattern
        /// </summary>
        /// <param name="content">The content string to be parsed</param>
        /// <returns>A Dictionary with retrieved keys and remaining content</returns>
        private Dictionary<String, String> NormalizeValuesDictionary(String content)
        {
            string searchPattern = @"^[\t ]*("".+""|@)=(""[^""]*""|[^""]+)";
            MatchCollection matches = Regex.Matches(content, searchPattern, RegexOptions.Multiline);

            Dictionary<String, String> dictKeys = new Dictionary<string, string>();

            foreach (Match match in matches)
            {
                try
                {
                    //Retrieve key
                    String sKey = match.Groups[1].Value;

                    //Retrieve value
                    String sValue = match.Groups[2].Value;

                    //Removing the ending CR
                    while (sKey.EndsWith("\r\n"))
                    {
                        sKey = sKey.Substring(0, sKey.Length - 2);
                    }

                    if (sKey == "@")
                        sKey = "";
                    else
                        sKey = StripeLeadingChars(sKey, "\"");

                    while (sValue.EndsWith("\r\n"))
                    {
                        sValue = sValue.Substring(0, sValue.Length - 2);
                    }

                    if (dictKeys.ContainsKey(sKey))
                    {
                        string tmpcontent = dictKeys[sKey];
                        StringBuilder tmpsb = new StringBuilder(tmpcontent);
                        if (!tmpcontent.EndsWith(Environment.NewLine)) tmpsb.AppendLine();
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
                    throw new Exception(String.Format("Exception thrown on processing string {0}", match.Value), ex);
                }
            }
            return dictKeys;
        }

        /// <summary>
        /// Removes the leading and ending characters from the given string
        /// </summary>
        /// <param name="sLine">given string</param>
        /// <returns>edited string</returns>
        /// <remarks></remarks>
        private string StripeLeadingChars(string sLine, string leadChar)
    {
      string tmpvalue = sLine.Trim();
      if (tmpvalue.StartsWith(leadChar) & tmpvalue.EndsWith(leadChar))
      {
        return tmpvalue.Substring(1, tmpvalue.Length - 2);
      }
      return tmpvalue;
    }

    /// <summary>
    /// Removes the leading and ending parenthesis from the given string
    /// </summary>
    /// <param name="sLine">given string</param>
    /// <returns>edited string</returns>
    /// <remarks></remarks>
    private string StripeBraces(string sLine)
    {
      string tmpvalue = sLine.Trim();
      if (tmpvalue.StartsWith("[") & tmpvalue.EndsWith("]"))
      {
        return tmpvalue.Substring(1, tmpvalue.Length - 2);
      }
      return tmpvalue;
    }

    /// <summary>
    /// Retrieves the ecoding of the reg file, checking the word "REGEDIT4"
    /// </summary>
    /// <returns></returns>
    private string GetEncoding()
    {
      if (Regex.IsMatch(content, "([ ]*(\r\n)*)REGEDIT4", RegexOptions.IgnoreCase | RegexOptions.Singleline))
        return "ANSI";
      else
        return "UTF8";
    }

    #endregion

  }


}
