using NUnit.Framework;
using System.Diagnostics;
using Utility;

namespace Day14;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(136));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1(@"Inputs/input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var grid = File.ReadLines(inputPath)
            .Select(s => s.ToCharArray())
            .ToArray();

        var transposedGrid = Matrix.Transpose(grid);

        var tiltedGrid = Matrix.Transpose(transposedGrid
            .Select(row => new string(row).Split('#'))
            .Select(rowParts => rowParts.Select(rowPart => rowPart.OrderDescending().ToArray()))
            .Select(sortedRowParts => sortedRowParts.Aggregate((current, next) => [.. current, '#', .. next]))
            .ToArray());

        var result = tiltedGrid
            .Reverse()
            .Select((row, index) => new { row, index })
            .Sum(x => x.row.Count(c => c == 'O') * (x.index + 1));

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }
}
