void SolvePart1()
{
    var records = ReadRecords("test.txt");
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

struct Record(string conditions, List<int> groupSizes)
{
    public string Conditions { get; set; } = conditions;
    public List<int> GroupSizes { get; set; } = groupSizes;
}