using NUnit.Framework;
using System.Diagnostics;

namespace Day17;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(102));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1(@"Inputs/input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var grid = File.ReadLines(inputPath)
            .Select(line => line.Select(x => (int)x).ToArray())
            .ToArray();



        var result = 102;

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    int CalculateMinRoad(int[][] map, int i, int j, Direction currentDirection, int directionCount)
    {

    }

    enum Direction
    {
        Down, Left, Right
    }
}
