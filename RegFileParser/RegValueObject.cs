using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace RegFileParser
{
  [Serializable]
  public class RegValueObject
  {
    private string root;
    private string parentkey;
    private string parentkeywithoutroot;

    private string entry;
    private string value;
    private string type;

    /// <summary>
    /// Parameterless constructor
    /// </summary>
    public RegValueObject()
    {
      root = "";
      parentkey = "";
      parentkeywithoutroot = "";
      entry = "";
      value = "";
      type = "";
    }

    /// <summary>
    /// Overloaded constructor
    /// </summary>
    /// <param name="propertyString">A line from the [Registry] section of the *.sig signature file</param>
    public RegValueObject(string regKeyName, string regValueName, string regValueData, string encoding)
    {
      parentkey = regKeyName.Trim();
      parentkeywithoutroot = parentkey;
      root = GetHive(ref parentkeywithoutroot);
      entry = regValueName;
      value = regValueData;
      type = "";
      string tmpStringValue = value;
      type = GetRegEntryType(ref tmpStringValue, encoding);
      value = tmpStringValue;
    }

    #region Public Methods

    /// <summary>
    /// Overriden Method
    /// </summary>
    /// <returns>An entry for the [Registry] section of the *.sig signature file</returns>
    public override string ToString()
    {
      return String.Format("{0}\\\\{1}={2}{3}", this.parentkey, this.entry, SetRegEntryType(this.type), this.value);
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Regsitry value name
    /// </summary>
    [XmlElement("entry", typeof(string))]
    public string Entry
    {
      get { return entry; }
      set { entry = value; }
    }
    
    /// <summary>
    /// Registry value parent key
    /// </summary>
    [XmlElement("key", typeof(string))]
    public string ParentKey
    {
      get { return parentkey; }
      set 
      { 
        parentkey = value;
        parentkeywithoutroot = parentkey;
        root = GetHive(ref parentkeywithoutroot);
      }
    }
    
    /// <summary>
    /// Registry value root hive
    /// </summary>
    [XmlElement("root", typeof(string))]
    public string Root
    {
      get { return root; }
      set { root = value; }
    }
    
    /// <summary>
    /// Registry value type
    /// </summary>
    [XmlElement("type", typeof(string))]
    public string Type
    {
      get { return type; }
      set { type = value; }
    }
    
    /// <summary>
    /// Registry value data
    /// </summary>
    [XmlElement("value", typeof(string))]
    public string Value
    {
      get { return this.value; }
      set { this.value = value; }
    }

    [XmlElement("value", typeof(string))]
    public string ParentKeyWithoutRoot
    {
      get { return parentkeywithoutroot; }
      set { parentkeywithoutroot = value; }
    }

    #endregion Public Properties

    #region Private Functions

    private string GetHive(ref string skey)
    {
      string tmpLine = skey.Trim();

      if (tmpLine.StartsWith("HKEY_LOCAL_MACHINE"))
      {
        skey = skey.Substring(18);
        if (skey.StartsWith("\\")) skey = skey.Substring(1);
        return "HKEY_LOCAL_MACHINE";
      }
      
      if (tmpLine.StartsWith("HKEY_CLASSES_ROOT"))
      {
        skey = skey.Substring(17);
        if (skey.StartsWith("\\")) skey = skey.Substring(1);
        return "HKEY_CLASSES_ROOT";
      }
      
      if (tmpLine.StartsWith("HKEY_USERS"))
      {
        skey = skey.Substring(10);
        if (skey.StartsWith("\\")) skey = skey.Substring(1);
        return "HKEY_USERS";
      }
      
      if (tmpLine.StartsWith("HKEY_CURRENT_CONFIG")) 
      {
        skey = skey.Substring(19);
        if (skey.StartsWith("\\")) skey = skey.Substring(1);
        return "HKEY_CURRENT_CONFIG"; 
      }
      
      if (tmpLine.StartsWith("HKEY_CURRENT_USER"))
      {
        skey = skey.Substring(17);
        if (skey.StartsWith("\\")) skey = skey.Substring(1);
        return "HKEY_CURRENT_USER";
      }

      return "";
    }

    /// <summary>
    /// Retrieves the reg value type, parsing the prefix of the value
    /// </summary>
    /// <param name="sTextLine">Registry value row string</param>
    /// <returns>Value</returns>
    private string GetRegEntryType(ref string sTextLine, String textEncoding)
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
        sTextLine = GetStringRepresentation(sTextLine.Split(new char[] { ',' }), textEncoding);
        return "REG_MULTI_SZ";
      }

      if (sTextLine.StartsWith("hex(6):"))
      {
        sTextLine = StripeContinueChar(sTextLine.Substring(7));
        sTextLine = GetStringRepresentation(sTextLine.Split(new char[] { ',' }), textEncoding);
        return "REG_LINK";
      }

      if (sTextLine.StartsWith("hex(2):"))
      {
        sTextLine = StripeContinueChar(sTextLine.Substring(7));
        sTextLine = GetStringRepresentation(sTextLine.Split(new char[] { ',' }), textEncoding);
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

    private string SetRegEntryType(string sRegDataType)
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
          return "";

        default:
          return "";
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
    /// Removes the leading and ending characters from the given string
    /// </summary>
    /// <param name="sline">given string</param>
    /// <returns>edited string</returns>
    /// <remarks></remarks>
    private string StripeLeadingChars(string sline, string LeadChar)
    {
      string tmpvalue = sline.Trim();
      if (tmpvalue.StartsWith(LeadChar) & tmpvalue.EndsWith(LeadChar))
      {
        return tmpvalue.Substring(1, tmpvalue.Length - 2);
      }
      return tmpvalue;
    }

    /// <summary>
    /// Removes the leading and ending parenthesis from the given string
    /// </summary>
    /// <param name="sline">given string</param>
    /// <returns>edited string</returns>
    /// <remarks></remarks>
    private string StripeBraces(string sline)
    {
      string tmpvalue = sline.Trim();
      if (tmpvalue.StartsWith("[") & tmpvalue.EndsWith("]"))
      {
        return tmpvalue.Substring(1, tmpvalue.Length - 2);
      }
      return tmpvalue;
    }

    /// <summary>
    /// Removes the ending backslashes from the given string
    /// </summary>
    /// <param name="sline">given string</param>
    /// <returns>edited string</returns>
    /// <remarks></remarks>
    private string StripeContinueChar(string sline)
    {
      String tmpString = Regex.Replace(sline, "\\\\\r\n[ ]*", String.Empty);
      return tmpString;
    }

    /// <summary>
    /// Converts the byte arrays (saved as array of string) into string
    /// </summary>
    /// <param name="stringArray">Array of string</param>
    /// <returns>String value</returns>
    private string GetStringRepresentation(string[] stringArray, string encoding)
    {
      if (stringArray.Length > 1)
      {
        StringBuilder sb = new StringBuilder();

        if ((encoding == "UTF8"))
        {
          for (int i = 0; i < stringArray.Length - 2; i += 2)
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
          for (int i = 0; i < stringArray.Length - 1; i += 1)
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
      else
        return String.Empty;
    }

    #endregion

  }

}
