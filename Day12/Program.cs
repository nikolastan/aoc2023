void SolvePart1()
{

}

SolvePart1();

IEnumerable<string> ReadRecords(string filePath)
{
    using var reader = new StreamReader(filePath);

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (line is not null)
            yield return line.Trim('.');
    }
}