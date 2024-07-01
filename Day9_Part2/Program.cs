using System.Diagnostics;

void Solve()
{
	var sw = Stopwatch.StartNew();

	var histories = ReadHistoriesFromInput("input.txt");
	var result = 0;
	foreach (var history in histories)
	{
		result += CalculateNextValue(history);
	}

	sw.Stop();
	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

static IEnumerable<List<int>> ReadHistoriesFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	while (!reader.EndOfStream)
	{
		var newHistory = reader.ReadLine()!
			.Split(' ')
			.Select(int.Parse)
			.ToList();

		yield return newHistory;
	}
}

int CalculateNextValue(List<int> history)
{
	var currentSequence = history;
	var allSequences = new List<List<int>>();

	while (currentSequence.Any(num => num != 0))
	{
		allSequences.Add(currentSequence);
		var newSequence = new List<int>();

		for (int i = 1; i < currentSequence.Count; i++)
		{
			newSequence.Add(currentSequence[i] - currentSequence[i - 1]);
		}

		currentSequence = newSequence;
	}

	allSequences.Reverse();
	var result = 0;

	foreach (var sequence in allSequences)
	{
		result = sequence.First() - result;
	}

	return result;
}