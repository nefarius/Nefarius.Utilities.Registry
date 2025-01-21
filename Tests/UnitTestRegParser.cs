using System.Diagnostics;

using Nefarius.Utilities.Registry;

namespace Tests;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void ParseBigFile()
    {
        Stopwatch sw = Stopwatch.StartNew();
        
        RegFile file = new(@"D:\Development\GitHub\Nefarius.Utilities.Registry\Dumps\00_software_export.reg");
        
        sw.Stop();
        
        TestContext.Out.WriteLine($"Parsing done in {sw.Elapsed}");
        
        Assert.Pass();
    }

    [Test]
    public void TestParsingRegFiles()
    {
        var testFiles = Directory.GetFiles(
            Path.Combine(TestContext.CurrentContext.TestDirectory, @"..\..\Dumps"),
            "*.reg",
            SearchOption.AllDirectories
        ).Skip(1).ToArray();

        // TODO: not all files satisfy the condition below, fix either the files or the test!
        
        Assert.That(testFiles, Is.Not.Empty);
        
        foreach (string testFile in testFiles)
        {
            RegFile file = new(testFile);
            
            TestContext.Out.WriteLine($"Processing file {testFile}");

            KeyValuePair<string, Dictionary<string, RegValue>> cachedEntries = file.RegValues.First();

            Dictionary<string, RegValueBinary> binValues = cachedEntries.Value
                .Where(v => v.Value.Type == RegValueType.Binary)
                .ToDictionary(pair => pair.Key, pair => (RegValueBinary)pair.Value);

            Assert.That(binValues, Is.Not.Empty);

            KeyValuePair<string, RegValueBinary>? recordEntry =
                binValues.FirstOrDefault(pair => pair.Value.Value.First() == 0x36);

            Assert.That(recordEntry, Is.Not.Null);
            
            byte[] recordBlob = recordEntry.Value.Value.Value.ToArray();

            Assert.That(recordBlob, Is.Not.Empty);
        }
    }
}