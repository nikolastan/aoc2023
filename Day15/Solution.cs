﻿using NUnit.Framework;
using System.Diagnostics;
using System.Text;

namespace Day15;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(1320));
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
        Assert.That(result, Is.EqualTo(145));
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
        foreach (var input in ReadInput(inputPath))
        {
            result += HashInput(input);
        }

        sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    int SolvePart2(string inputPath)
    {
        var input = ReadInput(inputPath)
            .Select(Parse);

        var
    }



    IEnumerable<string> ReadInput(string inputPath)
    {
        using var reader = new StreamReader(inputPath);

        List<char> input = [];

        while (!reader.EndOfStream)
        {
            var nextChar = (char)reader.Read();

            if (nextChar is ',')
            {
                yield return new string(input.ToArray());
                input.Clear();
                continue;
            }

            if (nextChar is '\n')
                continue;

            input.Add(nextChar);
        }

        yield return new string(input.ToArray());
    }

    int HashInput(string input)
    {
        var value = 0;

        var ASCIIValues = Encoding.ASCII.GetBytes(input);

        foreach (byte b in ASCIIValues)
        {
            value += b;
            value *= 17;
            value %= 256;
        }

        return value;
    }

    Operation Parse(string input)
    {
        return input.Contains('=')
            ? new Operation(input[..input.IndexOf('=')], OperationType.Add, int.Parse(input[(input.IndexOf('=') + 1)..]))
            : new Operation(input[..input.IndexOf('-')], OperationType.Remove, int.Parse(input[(input.IndexOf('-') + 1)..]));
    }

    struct Operation(string label, OperationType type, int? focalLength = null)
    {
        public string Label { get; set; } = label;
        public OperationType Type { get; set; } = type;
        public int? FocalLength { get; set; } = focalLength;
    }

    enum OperationType
    {
        Remove,
        Add
    }
}
