using System.Diagnostics;
using System.Text.RegularExpressions;

void Solve()
{
    var stopwatch = Stopwatch.StartNew();

    var docs = ReadDocumentsFromInput("input.txt");

    var totalSteps = 0;
    var instructionIndex = 0;

    var currentNode = docs.NetworkStart;

    while(currentNode.Name != "ZZZ")
    {
        currentNode = docs.Instructions[instructionIndex % docs.Instructions.Length] == 'R' 
            ? currentNode.RightNext 
            : currentNode.LeftNext;

        instructionIndex++;
        totalSteps++;
    }

    stopwatch.Stop();

    Console.WriteLine($"Result: {totalSteps}, Time: {stopwatch.ElapsedMilliseconds}ms");
}

Solve();

Documents ReadDocumentsFromInput(string filePath)
{
    using var reader = new StreamReader(filePath);

    var instructions = reader.ReadLine()!.ToCharArray();

    reader.ReadLine(); //empty row 

    var nodesRegex = NodesRegex();

    while (!reader.EndOfStream)
    {
        ParseLine(reader.ReadLine()!, nodesRegex);
    }

    var networkStart = AllNodes.Single(node => node.Name == "AAA");

    return new Documents(instructions, networkStart);

}

void ParseLine(string line, Regex nodesRegex)
{
    var match = nodesRegex.Match(line);

    if (match.Success)
    {
        var newNode = AllNodes.SingleOrDefault(node => node.Name == match.Groups[1].Value);

        if (newNode is null)
        {
            newNode = new Node(match.Groups[1].Value);
            AllNodes.Add(newNode);
        }

        var leftNode = AllNodes.SingleOrDefault(node => node.Name == match.Groups[2].Value);

        if (leftNode is null)
        {
            leftNode = new Node(match.Groups[2].Value);
            AllNodes.Add(leftNode);
        }

        newNode.LeftNext = leftNode;

		var rightNode = AllNodes.SingleOrDefault(node => node.Name == match.Groups[3].Value);

		if (rightNode is null)
        {
            rightNode = new Node(match.Groups[3].Value);
            AllNodes.Add(rightNode);
        }

        newNode.RightNext = rightNode;
    }
    else
    {
        throw new ArgumentException("Invalid line.");
    }
}

class Node(string name)
{
    public string Name { get; init; } = name;
    public Node LeftNext { get; set; } = null!;
    public Node RightNext { get; set; } = null!;
}

class Documents(char[] instructions, Node networkStart)
{
    public char[] Instructions { get; init; } = instructions;
    public Node NetworkStart { get; init; } = networkStart;
}

partial class Program
{
    [GeneratedRegex(@"^(\w+) = \((\w+), (\w+)\)$")]
    private static partial Regex NodesRegex();
    private static List<Node> AllNodes = [];
}

