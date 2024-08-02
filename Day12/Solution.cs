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
    public void Part1_Test1()
    {
        var result = SolvePart1("test1.txt");
        Assert.That(result, Is.EqualTo(2));
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2("test.txt");
        Assert.That(result, Is.EqualTo(525152));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1("input.txt");
    }

    [Test]
    public void Part2_Input()
    {
        SolvePart2("input.txt");
    }

    long SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var records = ReadRecords(inputPath);

        var result = records.Select(x => CalculateArrangements(x.Conditions, x.GroupSizes, []))
            .Sum();

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");

        return result;
    }

    long SolvePart2(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var records = ReadUnfoldedRecords(inputPath);

        var result = records.Select(x => CalculateArrangements(x.Conditions, x.GroupSizes, []))
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

    IEnumerable<Record> ReadUnfoldedRecords(string filePath)
    {
        using var reader = new StreamReader(filePath);

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line is not null)
            {
                var inputs = line.Split(' ');

                yield return new Record(string.Join('?', Enumerable.Repeat($"{inputs[0]}", 5)),
                    string.Join(',', Enumerable.Repeat($"{inputs[1]}", 5))
                    .Split(',')
                    .Select(int.Parse)
                    .ToList());
            }
        }
    }

    long CalculateArrangements(string conditions, List<int> groupSizes, Dictionary<string, long> memo)
    {
        if (groupSizes.Count is 0)
        {
            if (conditions.Contains('#'))
                return 0;
            else
                return 1;
        }

        if (conditions.All(x => x is not '.')
            && conditions.Length == groupSizes.First()
            && groupSizes.Count == 1)
            return 1;

        if (conditions.Count(x => x is not '.') < groupSizes.Sum())
            return 0;

        var totalArrangements = 0L;

        if (memo.TryGetValue(GetMemoKey(conditions, groupSizes), out totalArrangements))
            return totalArrangements;

        if (conditions[..groupSizes.First()].All(x => x is not '.')
            && conditions.Length > groupSizes.First()
            && conditions[groupSizes.First()] is not '#')
        {
            totalArrangements += CalculateArrangements(conditions[(groupSizes.First() + 1)..], groupSizes[1..], memo);
        }

        if (conditions[0] is not '#')
            totalArrangements += CalculateArrangements(conditions[1..], groupSizes, memo);

        memo.TryAdd(GetMemoKey(conditions, groupSizes), totalArrangements);

        return totalArrangements;
    }

    string GetMemoKey(string conditions, List<int> groupSizes)
    {
        return $"{conditions}{string.Join(',', groupSizes)}";
    }

    struct Record(string conditions, List<int> groupSizes)
    {
        public string Conditions { get; set; } = conditions;
        public List<int> GroupSizes { get; set; } = groupSizes;

    }
}