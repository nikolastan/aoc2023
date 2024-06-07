using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;


async Task Solve()
{
	var stopwatch = Stopwatch.StartNew();

	ReadDocumentsFromInput("input.txt");

	var ZNodes = AllNodes.Where(node => node.Name.EndsWith('Z'));

	var ZToZPaths = new ConcurrentBag<Path>();
	var findZZPaths = ZNodes
		.Select(node => Task.Run(() =>
		{
			var shortestPathToZ = FindShortestPathToZ(node);
			ZToZPaths.Add(shortestPathToZ);
		}))
		.ToList();

	await Task.WhenAll(findZZPaths);

	var result = CalculateLCM(ZToZPaths.Select(path => path.TotalSteps).ToList());

	stopwatch.Stop();

	Console.WriteLine($"Result: {ZToZPaths.First().TotalSteps}, Time: {stopwatch.ElapsedMilliseconds}ms");
}

await Solve();
Path FindShortestPathToZ(Node start, int startingInstructionIndex = 0)
{
	int totalSteps = 0;
	int currentInstructionIndex = startingInstructionIndex;

	var currentNode = start;

	do
	{
		currentInstructionIndex %= Instructions.Length;

		currentNode = Instructions[currentInstructionIndex] == 'R'
			? currentNode.RightNext
			: currentNode.LeftNext;

		currentInstructionIndex++;
		totalSteps++;
	} 
	while (!currentNode.Name.EndsWith('Z'));

	currentInstructionIndex %= Instructions.Length;

	return new Path(start, currentNode, startingInstructionIndex, currentInstructionIndex, totalSteps);
}

void ReadDocumentsFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	Instructions = reader.ReadLine()!.ToCharArray();

	reader.ReadLine(); //empty row 

	var nodesRegex = NodesRegex();

	while (!reader.EndOfStream)
	{
		ParseLine(reader.ReadLine()!, nodesRegex);
	}
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

long CalculateLCM(List<int> numbers)
{
	var product = 1L;

	foreach (var num in numbers)
	{
		product = FindLCM(product, num);
	}

	return product;
}

long FindLCM(long num1, int num2)
{
	var a = Math.Abs(num1);
	var b = Math.Abs(num2);

	a /= FindGCD(a, b);
	return a * b;
}

long FindGCD(long a, long b)
{
	if(a== 0)
		return b;

	return FindGCD(b % a, a);
}

class Path(Node start, Node end, int startingInstIndex, int currentInstIndex, int totalSteps)
{
	public readonly Node Start = start;
	public Node End = end;
	public int StartingInstructionIndex = startingInstIndex;
	public int CurrentInstructionIndex = currentInstIndex;
	public int TotalSteps = totalSteps;
}

class Node(string name)
{
	public string Name { get; init; } = name;
	public Node LeftNext { get; set; } = null!;
	public Node RightNext { get; set; } = null!;
}

partial class Program
{
	[GeneratedRegex(@"^(\w+) = \((\w+), (\w+)\)$")]
	private static partial Regex NodesRegex();
	private static List<Node> AllNodes = [];
	private static char[] Instructions = null!;
	private static ConcurrentBag<Path> ViablePaths = [];
}

