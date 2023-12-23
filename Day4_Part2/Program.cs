using System.Diagnostics;
using System.Text.RegularExpressions;

List<Card> ReadCardsFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var cards = new List<Card>();

	while (!reader.EndOfStream)
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

int GetTotalNumberOfScratchcards(List<Card> cards)
{
	var sum = 0;

	for (int i = 0; i < cards.Count; i++)
	{
		var numOfWinningNumbers = cards[i].PlayerNumbers.Count(x => cards[i].WinningNumbers.Contains(x));

		if (numOfWinningNumbers > 0)
		{
			var maxIndex = Math.Clamp(i + numOfWinningNumbers + 1, i, cards.Count);
			var minIndex = Math.Clamp(i+1, i, cards.Count);

			foreach (var card in cards[minIndex..maxIndex]) 
			{
				card.NumberOfCopies += cards[i].NumberOfCopies;
			}
		}

		sum += cards[i].NumberOfCopies;
	}

	return sum;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var cards = ReadCardsFromInput("input.txt");
	var result = GetTotalNumberOfScratchcards(cards);

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();


record Card
{
	public int ID { get; init; }
	public int NumberOfCopies { get; set; } = 1;
	public IEnumerable<int> PlayerNumbers { get; init; } = null!;
	public IEnumerable<int> WinningNumbers { get; init; } = null!;
}

partial class Program
{
	[GeneratedRegex(@"^Card\s+(\d+):\s+((?:\d+\s*)+) \|\s+((?:\d+\s*)+)$")]
	private static partial Regex CardRegex();
}