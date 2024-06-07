using System.Collections.Concurrent;
using System.Diagnostics;
using System.Text.RegularExpressions;

async Task Solve()
{
	var stopwatch = Stopwatch.StartNew();

	ReadDocumentsFromInput("input.txt");

	var ANodes = AllNodes.Where(node => node.Name.EndsWith('A'));

	var AToZPaths = new ConcurrentBag<Path>();
	var findAZPaths = ANodes
		.Select(node => Task.Run(() =>
		{
			var shortestPathToZ = FindShortestPathToZ(node);
			AToZPaths.Add(shortestPathToZ);
		}))
		.ToList();

	await Task.WhenAll(findAZPaths);

	while(AToZPaths.Select(path => path.End).Any(end => !end.Name.EndsWith('Z'))
		|| AToZPaths.Select(path => path.TotalSteps).Distinct().Skip(1).Any())
	{
		var currentMaxSteps = AToZPaths.Max(path => path.TotalSteps);

		var findNextZInPaths = AToZPaths.Where(path => path.TotalSteps != currentMaxSteps)
			.Select(path => Task.Run(() =>
			{
				var viableNextPath = ViablePaths
					.SingleOrDefault(viablePath => viablePath.Start == path.End
					&& viablePath.StartingInstructionIndex == path.CurrentInstructionIndex);

				if(viableNextPath is null)
				{
					viableNextPath = FindShortestPathToZ(path.End, path.CurrentInstructionIndex);
					ViablePaths.Add(viableNextPath);
				}

				path.End = viableNextPath.End;
				path.CurrentInstructionIndex = viableNextPath.CurrentInstructionIndex;
				path.TotalSteps += viableNextPath.TotalSteps;
			}));

		await Task.WhenAll(findNextZInPaths);
	}

	stopwatch.Stop();

	Console.WriteLine($"Result: {AToZPaths.First().TotalSteps}, Time: {stopwatch.ElapsedMilliseconds}ms");
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

class Path(Node start, Node end, int startingInstIndex, int currentInstIndex, long totalSteps)
{
	public readonly Node Start = start;
	public Node End = end;
	public int StartingInstructionIndex = startingInstIndex;
	public int CurrentInstructionIndex = currentInstIndex;
	public long TotalSteps = totalSteps;
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

