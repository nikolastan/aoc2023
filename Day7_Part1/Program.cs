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
				Cards = [.. handValues[0].OrderBy(x => Array.IndexOf(Hand.CardOrder, x))],
				Bid = int.Parse(handValues[1])
			};

			hands.Add(hand);
		}
	}

	return hands;
}
void Solve()
{
	var sw = Stopwatch.StartNew();

	var hands = ReadHandsFromInput("input.txt");

	hands.Sort();

	int result = 0;

	for(int i = 0; i< hands.Count; i++)
	{
		result += hands[i].Bid * (i + 1);
	}

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

class Hand : IComparable<Hand>
{
	HandType _handType;
	public HandType HandType
	{
		get
		{
			if (_handType is 0)
				DetermineHandType();

			return _handType;
		}
		private set { _handType = value; }
	}
	public required List<char> Cards { get; init; }
	public int Bid { get; init; }

	public int CompareTo(Hand? other)
	{
		ArgumentNullException.ThrowIfNull(other);

		var handTypeDiff = other.HandType - HandType;

		return handTypeDiff switch
		{
			< 0 or > 0 => handTypeDiff,
			0 => CompareSequentially(other, this)
		};
	}

	void DetermineHandType()
	{
		var uniqueCardsSorted = Cards
			.GroupBy(x => x)
			.OrderByDescending(group => group.Count())
			.Select(group => new { Value = group.Key, Count = group.Count() })
			.ToArray();

		var strongestCard = uniqueCardsSorted[0];
		var secondStrongestCard = uniqueCardsSorted[1] ?? null;

		HandType = (strongestCard.Count, secondStrongestCard?.Count) switch
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

	static int CompareSequentially(Hand x, Hand y)
	{
		int diff = 0;
		var i = 0;

		while(diff is 0 && i != x.Cards.Count)
		{
			diff = Array.IndexOf(CardOrder, x.Cards[i]) - Array.IndexOf(CardOrder, y.Cards[i]);
			i++;
		}

		return diff;

	}

	public readonly static char[] CardOrder = ['A', 'K', 'Q', 'J', 'T', '9', '8', '7', '6', '5', '4', '3', '2'];
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