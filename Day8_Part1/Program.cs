using System.Diagnostics;

Documents ReadDocumentsFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var instructions = new List<char>();

	var currentChar = (char)reader.Read();

	while (currentChar is not '\n')
	{
		instructions.Add(currentChar);
		currentChar = (char)reader.Read();
	}

	reader.ReadLine(); //empty row 

	
}

record Node
{
	public string Destination { get; init; } = string.Empty;
	public string LeftPath { get; init; } = string.Empty;
	public string RightPath { get; init; } = string.Empty;

	public Node(string line)
	{
		//^(\w+) = \((\w+), (\w+)\)$ <- regex for reading nodes
	}
}

class Documents
{
	char[] Instructions { get; init; } = [];
	SortedDictionary<string, Node> Network { get; init; } = [];
}

