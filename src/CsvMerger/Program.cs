// See https://aka.ms/new-console-template for more information

using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

Console.WriteLine("Enter CSV directory path:");
var input = Console.ReadLine();

if (!Directory.Exists(input))
{
    Console.WriteLine("Invalid path. Enter the valid path and try again!");
    Environment.Exit(1);
}

var files = Directory.EnumerateFiles(input, "*.*", SearchOption.TopDirectoryOnly)
    .Where(s => s.EndsWith(".csv") || s.StartsWith("J"));
var mergedCollection = new List<string[]>();

foreach (var file in files)
{
    string text = File.ReadAllText(file);
    if (Path.GetFileName(file).Contains(".bak")) continue;

    Console.WriteLine($"Checking {Path.GetFileName(file)}");

    var csvEntries = ReadCSV(file);
    var result = new List<string[]>();

    var headers = csvEntries[1].ToList();
    headers.Add("FileName");

    result[0] = headers.ToArray();

    for (var i = 2; i < csvEntries.Count; i++) //Ignore first row and headers entry, so i = 2
    {
        var entry = csvEntries[i];

        if (entry is null) continue;

        var rowArray = entry.ToList();

        rowArray.Add(file); //Add cell value with current file name

        result[i - 1] = rowArray.ToArray();
    }

    mergedCollection.AddRange(result);
}

var resultFileName = $"MaterialList{DateTime.Now.ToString("yyMMdd")}.csv";

using (var f = File.CreateText(Path.Combine(input, resultFileName)))
{
    f.NewLine = "\n";
    foreach (var entry in mergedCollection)
    {
        if (entry is null) continue;

        f.WriteLine(string.Join(",", entry));
    }
}

Console.WriteLine("OK");

List<string[]> ReadCSV(string absolutePath)
{
    var result = new List<string[]>();

    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
        HasHeaderRecord = true,
        Mode = CsvMode.NoEscape
    };

    using (var reader = new StreamReader(absolutePath))
    using (var parser = new CsvParser(reader, config))
    {
        while (parser.Read())
        {
            var row = parser.Record;
            result.Add(row);
            if (row == null)
            {
                break;
            }
        }
    }
    return result;
}