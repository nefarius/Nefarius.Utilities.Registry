using System.Text;

using Nefarius.Utilities.Registry;

string outputPath = Environment.ExpandEnvironmentVariables("%Temp%\\regfile.txt");

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
    outputPath = args[1];
}

try
{
    RegFile regFile = new(args[0]);
    Console.WriteLine("Reg file has been imported.");

    //For proofing purpose generate new txt file with imported content
    Int32 count = 0;
    StringBuilder sb = new();
    foreach (KeyValuePair<string, Dictionary<string, RegValue>> entry in regFile.RegValues)
    {
        sb.AppendLine($@"[{entry.Key}]");
        foreach (KeyValuePair<string, RegValue> item in entry.Value)
        {
            if (string.IsNullOrEmpty(item.Value.Entry))
            {
                sb.Append("@=");
            }
            else
            {
                sb.Append($"\"{item.Value.Entry}\"=");
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
    
    var t = sb.ToString();

    /*
    File.WriteAllText(outputPath, sb.ToString());
    Console.WriteLine("Content file generated as '{0}'", outputPath);

    Console.WriteLine("Reg file contains {0} keys and {1} values.", regFile.RegValues.Count, count);
    */
}
catch (Exception ex)
{
    Console.WriteLine("Exception thrown\n{0}", ex);
}

//Wait for ENTER key to close program
Console.WriteLine("Press 'ENTER' to exit program.");
Console.ReadLine();