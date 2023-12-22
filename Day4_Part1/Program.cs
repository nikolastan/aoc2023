using System.Diagnostics;
using System.Text.RegularExpressions;

List<Card> ReadCardsFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var cards = new List<Card>();

	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if (line is not null)
			cards.Add(CreateCard(line));
	}

	static Card CreateCard(string line)
	{
		var regex = CardRegex();
		var capturedGroups = regex.Matches(line).ToArray()[0].Groups
			?? throw new InvalidOperationException("Can't read card from this line.");

		var card = new Card()
		{
			ID = int.Parse(capturedGroups[1].Value),
			PlayerNumbers = capturedGroups[2].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).AsEnumerable(),
			WinningNumbers = capturedGroups[3].Value.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).AsEnumerable()
		};



		return card;
	}

	return cards;
}

int GetTotalPoints(IEnumerable<Card> cards)
{
	var sum = 0;

	foreach(var card in cards)
	{
		var numOfWinningNumbers =  card.PlayerNumbers.Count(x => card.WinningNumbers.Contains(x));

		if(numOfWinningNumbers > 0)
			sum += 1 << numOfWinningNumbers - 1;
	}

	return sum;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var cards = ReadCardsFromInput("input.txt");
	var result = GetTotalPoints(cards);

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();


record Card
{
	public int ID { get; init; }
	public IEnumerable<int> PlayerNumbers { get; init; } = null!;
	public IEnumerable<int> WinningNumbers { get; init; } = null!;
}

partial class Program
{
	[GeneratedRegex(@"^Card\s+(\d+):\s+((?:\d+\s*)+) \|\s+((?:\d+\s*)+)$")]
	private static partial Regex CardRegex();
}