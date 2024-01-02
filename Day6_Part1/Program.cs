using System.Diagnostics;

List<Race> ReadRacesFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var timeLine = reader.ReadLine();
	var distanceLine = reader.ReadLine();

	if (timeLine is null || distanceLine is null)
		throw new InvalidOperationException("Invalid input file.");

	var times = ExtractNumValuesFrom(timeLine).ToList();
	var distances = ExtractNumValuesFrom(distanceLine).ToList();

	var races = new List<Race>();

	for(int i = 0; i < times.Count; i++)
	{
		races.Add(new Race()
		{
			Time = times[i],
			Distance = distances[i]
		});
	}

	return races;
}

static IEnumerable<int> ExtractNumValuesFrom(string line)
{
	return line
		.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select(x => x.Trim())
		.Where(subs => !subs.Any(c => char.IsLetter(c)))
		.Select(int.Parse);
}

void Solve()
{
	var sw = Stopwatch.StartNew();
	var races = ReadRacesFromInput("input.txt");

	var result = races
		.Select(NumberOfWaysToBeatRecord)
		.Aggregate(1, (x, y) => x * y);

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time:{sw.ElapsedMilliseconds}ms");
}

int NumberOfWaysToBeatRecord(Race race)
{
	int lowerBound = (int) Math.Floor((race.Time - Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2 + 1);
	int upperBound = (int) Math.Ceiling((race.Time + Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2);

	return upperBound - lowerBound;
}

Solve();

struct Race
{
	public int Time;
	public int Distance;
}