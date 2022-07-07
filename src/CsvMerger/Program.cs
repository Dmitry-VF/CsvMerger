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

var records = new List<Dictionary<string, string>>();

foreach (var file in files)
{
    string text = File.ReadAllText(file);

    Console.WriteLine($"Checking {Path.GetFileName(file)}");

    var csvEntries = ReadCSV(file);

    var headers = csvEntries[1];

    for (var i = 2; i < csvEntries.Count; i++) //Ignore first row and headers entry, so i = 2
    {
        var entry = csvEntries[i];

        if (entry is null) continue;

        var recs = new List<Dictionary<string, string>>();
        var headerNumber = 0;

        foreach (var item in entry)
        {
            if (headerNumber > headers.Length) continue;

            var d = new Dictionary<string, string>();
            d.Add(!string.IsNullOrEmpty(headers[headerNumber]) ? headers[headerNumber] : String.Empty, item);
            recs.Add(d);
            headerNumber++;
        }

        recs.Add(new Dictionary<string, string> { { "FileName", Path.GetFileNameWithoutExtension(file) } }); //Add cell value with current file name

        records.AddRange(recs);
    }
}

var finalHeaders = CollectUniqueKeys(records);
var resultFileName = $"MaterialList{DateTime.Now.ToString("yyMMdd")}.csv";


using (var writer = new StreamWriter(Path.Combine(input, resultFileName)))
using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
{
    
    foreach (var record in records)
    {
        
        //foreach (var dic in record)
        //{
        //    var key = dic.Key;
            
        //    if (!csv.HeaderRecord.Contains(key) || csv.HeaderRecord.Length == 0 )
        //    {
        //        csv.WriteHeader(key);
        //    }
        // }
    }

    //var hasHeaderBeenWritten = false;
    //foreach (var row in records)
    //{
    //    if (!hasHeaderBeenWritten)
    //    {
    //        foreach (var pair in row)
    //        {

    //            csv.WriteField(pair.Key);
    //        }

    //        hasHeaderBeenWritten = true;

    //        csv.NextRecord();
    //    }

    //    foreach (var pair in row)
    //    {
    //        csv.WriteField(pair.Value);
    //    }

    //    csv.NextRecord();


    writer.WriteLine(csv);
}

//using (var f = File.CreateText(Path.Combine(input, resultFileName)))
//{
//    f.NewLine = "\n";
//    foreach (var entry in mergedCollection)
//    {
//        if (entry is null) continue;

//        f.WriteLine(string.Join(",", entry));
//    }
//}

Console.WriteLine("OK");

List<string> CollectUniqueKeys(List<Dictionary<string, string>> keyValuePairs)
{
    var result = new List<string>();

    foreach(var keyValue in keyValuePairs)
    {
        foreach(var key in keyValue.Keys)
        {
            if(!result.Contains(key))
            {
                result.Add(key);
            }
        }
    }

    return result;
}

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