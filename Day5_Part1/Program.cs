using System.Diagnostics;
using System.Text.RegularExpressions;

Almanac ReadAlmanacFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);

	var almanac = new Almanac();

	var line = reader.ReadLine();

	var seedsRegex = SeedsRegex();

	almanac.Seeds = seedsRegex.Match(line!).Groups[1].Value
				.Split(' ', StringSplitOptions.RemoveEmptyEntries)
				.Select(long.Parse)
				.ToList();

	Map currentMap = null!;

	while (!reader.EndOfStream)
	{
		line = reader.ReadLine();

		if(!string.IsNullOrWhiteSpace(line))
		{
			if (char.IsLetter(line[0]))
			{
				currentMap = new Map();
				almanac.Maps.Add(currentMap);
			}

			if (char.IsDigit(line[0]))
			{
				var values = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

				currentMap.MapRanges.Add(new MapRange()
				{
					DestinationStart = long.Parse(values[0]),
					SourceStart = long.Parse(values[1]),
					RangeLength = long.Parse(values[2])
				});
			}
		}

		
	}

	return almanac;
}

long PassSeedThroughMaps(in List<Map> maps, long seed)
{
	var currentSeed = seed;

	foreach (var map in maps)
	{
		MapRange? rangeContainingSeed = map.MapRanges
			.Where(x => currentSeed >= x.SourceStart && currentSeed <= x.SourceStart + x.RangeLength)
			.SingleOrDefault();

		if(rangeContainingSeed is not null)
		{
			currentSeed = rangeContainingSeed.Value.DestinationStart + currentSeed - rangeContainingSeed.Value.SourceStart;
		}
	}

	return currentSeed;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var almanac = ReadAlmanacFromInput("input.txt");

	var result = almanac.Seeds
		.Select(seed => PassSeedThroughMaps(in almanac.Maps, seed))
		.Min();

	sw.Stop();

	Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

class Map
{
	public List<MapRange> MapRanges = [];
}

struct MapRange
{
	public long DestinationStart;
	public long SourceStart;
	public long RangeLength;
}

struct Almanac
{
	public List<Map> Maps = [];
	public List<long> Seeds = null!;

	public Almanac()
	{
	}
}

partial class Program
{
	[GeneratedRegex(@"^seeds:\s+((?:\d+\s*)+)$")]
	private static partial Regex SeedsRegex();
}