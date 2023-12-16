using System.Diagnostics;

var ogBag = new Bag
{
	Red = 12,
	Green = 13,
	Blue = 14
};

List<string> ReadLinesFromInput(string fileName)
{
	using var reader = new StreamReader(fileName);
	List<string> lines = [];

	while (!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if (line is not null)
			lines.Add(line);
	}

	return lines;
}
KeyValuePair<int, Bag> ParseBagFromLine(string line)
{
	var gameInfo = GetGameInfo(line);

	var totalCubes = gameInfo.Value
		.Split(';')
		.Select(x => x.Replace(" ", ""))
		.SelectMany(x => x.Split(','))
		.Select(GetCubesInfo)
		.GroupBy(x => x.Key)
		.Select(group => new KeyValuePair<string, int>(group.Key, group.Max(x => x.Value)));

	return new KeyValuePair<int, Bag>(gameInfo.Key, new Bag()
	{
		Red = totalCubes.First(x => x.Key == "red").Value,
		Green = totalCubes.First(x => x.Key == "green").Value,
		Blue = totalCubes.First(x => x.Key == "blue").Value
	});
}

KeyValuePair<int, string> GetGameInfo(string line)
{
	var digits = line[..line.IndexOf(':')].Where(char.IsDigit);
	string gameID = string.Concat(digits);
	string gameInfo = line[(line.IndexOf(':') + 1)..];

	return new KeyValuePair<int, string>(int.Parse(gameID), gameInfo);
}

KeyValuePair<string, int> GetCubesInfo(string rawData)
{
	var digits = rawData.Where(char.IsDigit);
	string number = string.Concat(digits);
	var cubeColor = rawData.Replace(number, "");

	return new KeyValuePair<string, int>(cubeColor, int.Parse(number));
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var lines = ReadLinesFromInput("input.txt");
	var bags = lines.Select(ParseBagFromLine);

	var result = bags
		.Where(bag => bag.Value.Red <= ogBag.Red
		&& bag.Value.Green <= ogBag.Green
		&& bag.Value.Blue <= ogBag.Blue)
		.Sum(x => x.Key);

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

struct Bag
{
	public int Red, Green, Blue;
}