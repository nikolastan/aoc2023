void Solve()
{
	var histories = ReadHistoriesFromInput("test.txt");
	foreach (var history in histories)
	{

	}
}

static IEnumerable<List<int>> ReadHistoriesFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	while(!reader.EndOfStream)
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

}