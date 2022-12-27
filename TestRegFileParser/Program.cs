using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Nefarius.Utilities.Registry;

namespace TestRegFileParser;

internal class Program
{
    private static string _outputPath = Environment.ExpandEnvironmentVariables("%Temp%\\regfile.txt");

    private static void Main(string[] args)
    {
        Console.WriteLine("Registry File Parser");

        if (args.Length == 0)
        {
            Console.WriteLine("Missing parameter(s).");
            Console.WriteLine("Syntax:");
            Console.WriteLine("RegFileParser <reg file path> [<output file path>]");
            Console.WriteLine();

            //Wait for ENTER key to close program
            Console.WriteLine("Press 'ENTER' to exit program.");
            Console.ReadLine();

            return;
        }

        if (!File.Exists(args[0]))
        {
            Console.WriteLine("File '{0}' not found.", args[0]);

            //Wait for ENTER key to close program
            Console.WriteLine("Press 'ENTER' to exit program.");
            Console.ReadLine();
            return;
        }

        if (args.Length == 2)
        {
            _outputPath = args[1];
        }

        try
        {
            RegFile regfile = new(args[0]);
            Console.WriteLine("Reg file has been imported.");

            //For proofing purpose generate new txt file with imported content
            Int32 count = 0;
            StringBuilder sb = new();
            foreach (KeyValuePair<String, Dictionary<String, RegValue>> entry in regfile.RegValues)
            {
                sb.AppendLine($@"[{entry.Key}]");
                foreach (KeyValuePair<String, RegValue> item in entry.Value)
                {
                    if (String.IsNullOrEmpty(item.Value.Entry))
                    {
                        sb.Append("@=");
                    }
                    else
                    {
                        sb.AppendFormat("\"{0}\"=", item.Value.Entry);
                    }

                    if (item.Value.Type == RegValueType.Sz)
                    {
                        sb.AppendLine($"\"{item.Value.Value}\"");
                    }
                    else
                    {
                        sb.AppendLine($"[{item.Value.Type}] {item.Value.Value}");
                    }

                    count++;
                }

                sb.AppendLine();
            }

            File.WriteAllText(_outputPath, sb.ToString());
            Console.WriteLine("Content file generated as '{0}'", _outputPath);

            Console.WriteLine("Reg file contains {0} keys and {1} values.", regfile.RegValues.Count, count);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception thrown\n{0}", ex);
        }

        //Wait for ENTER key to close program
        Console.WriteLine("Press 'ENTER' to exit program.");
        Console.ReadLine();
    }
}