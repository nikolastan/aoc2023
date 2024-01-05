using System.Diagnostics;

List<Hand> ReadHandsFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var hands = new List<Hand>();

	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if(line is not null)
		{
			var handValues = line
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.ToArray();

			Hand hand = new()
			{
				Cards = [.. handValues[0].OrderBy(x => Array.IndexOf(CardOrder, x))],
				Bid = int.Parse(handValues[1])
			};

			hand.HandType = DetermineHandType(hand.Cards);

			hands.Add(hand);
		}
	}

	return hands;
}

static HandType DetermineHandType(IEnumerable<char> hand)
{
	var uniqueCardsSorted = hand
		.GroupBy(x => x)
		.OrderByDescending(group => group.Count())
		.Select(group => new { Value = group.Key, Count = group.Count() })
		.ToArray();

	var strongestCard = uniqueCardsSorted[0];
	var secondStrongestCard = uniqueCardsSorted[1] ?? null;

	return (strongestCard.Count, secondStrongestCard?.Count) switch
	{
		(5, _) => HandType.FiveOfAKind,
		(4, 1) => HandType.FourOfAKind,
		(3, 2) or (2, 3) => HandType.FullHouse,
		(3, 1) => HandType.ThreeOfAKind,
		(2, 2) => HandType.TwoPair,
		(2, 1) => HandType.OnePair,
		(1, 1) => HandType.HighCard,
		_ => throw new InvalidOperationException("This should not be possible!")
	};
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var hands = ReadHandsFromInput("test.txt");

	sw.Stop();

	Console.WriteLine($"Result: {hands}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

class Hand
{
	public HandType HandType { get; set; }
	public List<char> Cards { get; init; } = null!;
	public int Bid { get; init; } 
}

enum HandType
{
	FiveOfAKind = 1,
	FourOfAKind,
	FullHouse,
	ThreeOfAKind,
	TwoPair,
	OnePair,
	HighCard
}

public partial class Program
{
	private readonly static char[] CardOrder = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];
}