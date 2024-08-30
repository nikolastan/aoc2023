using NUnit.Framework;
using System.Diagnostics;

namespace Day16;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(46));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1(@"Inputs/input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var result = 46;

        var grid = ReadInput(inputPath);

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    char[,] ReadInput(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var grid = new char[input.Length, input[0].Length];

        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                grid[i, j] = input[i][j];
            }
        }

        return grid;
    }

    class Tile
    {

    }
}
