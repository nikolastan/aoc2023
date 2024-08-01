using System.Diagnostics;

void SolvePart1()
{
    var sw = Stopwatch.StartNew();

    var records = ReadRecords("test.txt");

    var result = 0;

    foreach (var item in records)
    {
        result += CalculateTotalArrangements(item.Conditions, item.GroupSizes);
    }

    //var result = records.Select(x => CalculateTotalArrangements(x.Conditions, x.GroupSizes))
    //    .Sum();

    sw.Stop();

    Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");
}

SolvePart1();

IEnumerable<Record> ReadRecords(string filePath)
{
    using var reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (line is not null)
        {
            var inputs = line.Split(' ');

            yield return new Record(inputs[0].Trim('.'),
                inputs[1]
                .Split(',')
                .Select(int.Parse)
                .ToList());
        }
    }
}

int CalculateTotalArrangements(string conditions, List<int> groupSizes)
{
    if (groupSizes.Count is 0)
        return 1;

    if (conditions.All(x => x is not '.') && conditions.Length == groupSizes.First())
        return 1;

    if (conditions.Count(x => x is not '.') < groupSizes.Sum())
        return 0;

    var totalArrangements = 0;

    if (conditions[..groupSizes.First()].All(x => x is not '.')
        && conditions.Length > groupSizes.First()
        && conditions[groupSizes.First()] is not '#')
    {
        totalArrangements += CalculateTotalArrangements(conditions[(groupSizes.First() + 1)..], groupSizes[1..]);
        for (int i = 1; i <= groupSizes.First(); i++)
        {
            totalArrangements += CalculateTotalArrangements(conditions[i..], groupSizes);
        }
    }
    else
    {
        totalArrangements += CalculateTotalArrangements(conditions[1..], groupSizes);
    }

    return totalArrangements;
}

struct Record(string conditions, List<int> groupSizes)
{
    public string Conditions { get; set; } = conditions;
    public List<int> GroupSizes { get; set; } = groupSizes;
}