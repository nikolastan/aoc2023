using NUnit.Framework;
using System.Diagnostics;

namespace Day12;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1("test.txt");
        Assert.That(result, Is.EqualTo(21));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1("input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var records = ReadRecords(inputPath);

        //var result = 0;

        //foreach (var item in records)
        //{
        //    result += CalculateTotalArrangements(item.Conditions, item.GroupSizes);
        //}

        var result = records.Select(x => CalculateArrangements(x.Conditions, x.GroupSizes))
            .Sum();

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");

        return result;
    }

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

    int CalculateArrangements(string conditions, List<int> groupSizes)
    {
        if (groupSizes.Count is 0)
            return 1;

        if (conditions.All(x => x is not '.')
            && conditions.Length == groupSizes.First()
            && groupSizes.Count == 1)
            return 1;

        if (conditions.Count(x => x is not '.') < groupSizes.Sum())
            return 0;

        var totalArrangements = 0;

        if (conditions[..groupSizes.First()].All(x => x is not '.')
            && conditions.Length > groupSizes.First()
            && conditions[groupSizes.First()] is not '#')
        {
            totalArrangements += CalculateArrangements(conditions[(groupSizes.First() + 1)..], groupSizes[1..]);
        }

        if (conditions[0] is not '#')
            totalArrangements += CalculateArrangements(conditions[1..], groupSizes);

        return totalArrangements;
    }

    struct Record(string conditions, List<int> groupSizes)
    {
        public string Conditions { get; set; } = conditions;
        public List<int> GroupSizes { get; set; } = groupSizes;

    }
}