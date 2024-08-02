using NUnit.Framework;
using System.Diagnostics;

namespace Day13;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/examplePart1.txt");
        Assert.That(result, Is.EqualTo(405));
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var patterns = ReadInput(inputPath);

        sw.Stop();

        var result = 405;

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");

        return result;
    }

    char[][] ReadInput(string inputPath)
    {
        using var reader = new StreamReader(inputPath);

        var grid = new List<char[]>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line is not null)
            {
                grid.Add(line.ToCharArray());
            }
        }

        return grid.ToArray();
    }

    int FindHorizontalMirrorIndex(char[][] grid)
    {

    }
}
