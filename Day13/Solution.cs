using NUnit.Framework;
using System.Diagnostics;
using Utility;

namespace Day13;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(405));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1(@"Inputs/input.txt");
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(400));
    }

    [Test]
    public void Part2_Input()
    {
        SolvePart2(@"Inputs/input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var result = 0;

        foreach (var grid in ReadInput(inputPath))
        {
            var patternsByRows = grid
            .Select(row => new string(row))
            .ToList();

            var rowMirrorIndex = FindMirroredRowIndex(patternsByRows);

            if (rowMirrorIndex != -1)
            {
                result += rowMirrorIndex * 100;
                continue;
            }
            var patternsByColumns = Matrix.Transpose(grid)
                .Select(col => new string(col))
                .ToList();

            var colMirrorIndex = FindMirroredRowIndex(patternsByColumns);
            result += colMirrorIndex;
        }

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");

        return result;
    }

    int SolvePart2(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var result = 0;

        foreach (var grid in ReadInput(inputPath))
        {
            var patternsByRows = grid
            .Select(row => new string(row))
            .ToList();

            var rowMirrorIndex = FindSmudgedMirrorIndex(patternsByRows);

            if (rowMirrorIndex != -1)
            {
                result += rowMirrorIndex * 100;
                continue;
            }
            var patternsByColumns = Matrix.Transpose(grid)
                .Select(col => new string(col))
                .ToList();

            var colMirrorIndex = FindSmudgedMirrorIndex(patternsByColumns);
            result += colMirrorIndex;
        }

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms.");

        return result;
    }

    int FindMirroredRowIndex(List<string> patterns)
    {
        for (int i = 0; i < patterns.Count - 1; i++)
        {
            var current = i;
            var mirrored = i + 1;

            while (patterns[current] == patterns[mirrored])
            {
                if (current is 0 || mirrored == patterns.Count - 1)
                    return i + 1;

                current--;
                mirrored++;
            }
        }

        return -1;
    }

    //Find pattern where there is exactly one different character between mirrorred rows
    int FindSmudgedMirrorIndex(List<string> patterns)
    {
        for (int i = 0; i < patterns.Count - 1; i++)
        {
            var current = i;
            var mirrored = i + 1;

            var differences = 0;

            while (patterns[current] == patterns[mirrored]
                || AreDifferentByOneChar(patterns[current], patterns[mirrored], ref differences))
            {
                if (differences > 1)
                    break;

                if (current is 0 || mirrored == patterns.Count - 1)
                {
                    if (differences is 1)
                        return i + 1;
                    else
                        break;
                }

                current--;
                mirrored++;
            }
        }

        return -1;
    }

    bool AreDifferentByOneChar(string s1, string s2, ref int differenceNum)
    {
        var result = s1.Zip(s2, (a, b) => a != b).Count(t => t) == 1;

        if (result)
            differenceNum++;

        return result;
    }

    IEnumerable<char[][]> ReadInput(string inputPath)
    {
        using var reader = new StreamReader(inputPath);

        var grid = new List<char[]>();

        while (!reader.EndOfStream)
        {
            var line = reader.ReadLine();

            if (line is not null)
            {
                if (line is "")
                {
                    yield return grid.ToArray();
                    grid = [];
                }
                else
                    grid.Add(line.ToCharArray());
            }
        }

        yield return grid.ToArray();
    }
}
