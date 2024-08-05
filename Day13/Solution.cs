using NUnit.Framework;
using System.Diagnostics;
using Utility;

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

        var result = 0;

        foreach (var grid in ReadInput(inputPath))
        {
            var patternsByRows = grid
            .Select(row => new string(row))
            .ToList();

            var patternsByColumns = Matrix.TransposeMatrix(grid)
                .Select(col => new string(col))
                .ToList();

            var colMirrorIndex = FindMirroredRowIndex(patternsByColumns);
            var rowMirrorIndex = FindMirroredRowIndex(patternsByRows);

            result += colMirrorIndex + rowMirrorIndex * 100;
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
                current--;
                mirrored++;

                if (current is 0 || mirrored == patterns.Count - 1)
                    return i;
            }
        }

        return -1;
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

                grid.Add(line.ToCharArray());
            }
        }
    }


}
