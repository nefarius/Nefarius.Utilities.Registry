using System.Text;

using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

using Nefarius.Utilities.Registry;

[MemoryDiagnoser]
public class ParserBenchmark
{
    [Benchmark]
    public void Parse()
    {
        try
        {
            RegFile regFile = new(@"F:\Downloads\CurrentControlSet_full_export.reg");

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

                    //if (item.Value is RegValueMultiSz multiSz)
                    //{
                    //    Console.WriteLine(multiSz.Value.Count());
                    //}
                }

                sb.AppendLine();
            }

            //string outputPath = Environment.ExpandEnvironmentVariables("%Temp%\\regfile.txt");

            //File.WriteAllText(outputPath, sb.ToString());
            //Console.WriteLine("Content file generated as '{0}'", outputPath);
            //
            //Console.WriteLine("Reg file contains {0} keys and {1} values.", regFile.RegValues.Count, count);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Exception thrown\n{0}", ex);
        }
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Summary summary = BenchmarkRunner.Run<ParserBenchmark>();
    }
}