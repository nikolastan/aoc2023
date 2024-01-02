using System.Diagnostics;

Race ReadRaceFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var timeLine = reader.ReadLine();
	var distanceLine = reader.ReadLine();

	if (timeLine is null || distanceLine is null)
		throw new InvalidOperationException("Invalid input file.");

	var races = new List<Race>();

	return new Race()
	{
		Time = ExtractNumValueFrom(timeLine),
		Distance = ExtractNumValueFrom(distanceLine)
	};
}

static long ExtractNumValueFrom(string line)
{
	return long.Parse(line
		.Split(' ', StringSplitOptions.RemoveEmptyEntries)
		.Select(x => x.Trim())
		.Where(subs => !subs.Any(c => char.IsLetter(c)))
		.Aggregate("", (x, y) => x + y));
		
}

void Solve()
{
	var sw = Stopwatch.StartNew();
	var race = ReadRaceFromInput("input.txt");

	var result = NumberOfWaysToBeatRecord(race);

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time:{sw.ElapsedMilliseconds}ms");
}

int NumberOfWaysToBeatRecord(Race race)
{
	int lowerBound = (int)Math.Floor((race.Time - Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2 + 1);
	int upperBound = (int)Math.Ceiling((race.Time + Math.Sqrt(Math.Pow(race.Time, 2) - 4 * race.Distance)) / 2);

	return upperBound - lowerBound;
}

Solve();

struct Race
{
	public long Time;
	public long Distance;
}