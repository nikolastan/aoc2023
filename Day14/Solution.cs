using NUnit.Framework;
using System.Diagnostics;
using Utility;
using static Utility.Enums;

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

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(64));
    }

    [Test]
    public void Part2_Input()
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

    int SolvePart2(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var grid = File.ReadLines(inputPath)
            .Select(s => s.ToCharArray())
            .ToArray();

        for (int i = 0; i < 1_000_000_000; i++)
        {
            var tempGrid = grid;

            grid = Tilt(grid, Cardinal.North);
            grid = Tilt(grid, Cardinal.West);
            grid = Tilt(grid, Cardinal.South);
            grid = Tilt(grid, Cardinal.East);

            if (Enumerable.SequenceEqual(
                tempGrid.SelectMany(x => x),
                grid.SelectMany(x => x)))
                break;
        }

        var result = grid
            .Reverse()
            .Select((row, index) => new { row, index })
            .Sum(x => x.row.Count(c => c == 'O') * (x.index + 1));

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    char[][] Tilt(char[][] grid, Cardinal side)
    {
        bool descending = false;
        bool transposed = false;

        if (side is Cardinal.North or Cardinal.West)
            descending = true;

        if (side is Cardinal.North or Cardinal.South)
        {
            transposed = true;
            grid = Matrix.Transpose(grid);
        }

        grid = grid
            .Select(row => new string(row).Split('#'))
            .Select(rowParts => rowParts.Select(rowPart => descending ? rowPart.OrderDescending() : rowPart.Order()))
            .Select(sortedStrings => sortedStrings.Select(s => s.ToArray()))
            .Select(sortedRowParts => sortedRowParts.Aggregate((current, next) => [.. current, '#', .. next]))
            .ToArray();

        return transposed ? Matrix.Transpose(grid) : grid;
    }
}
