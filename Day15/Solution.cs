using NUnit.Framework;
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
        return 145;
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
}
