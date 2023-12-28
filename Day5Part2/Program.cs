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
				.Select((value, index) => new {Index = index, Value = value})
				.GroupBy(x => x.Index / 2)
				.Select(g => new SeedRange()
				{
					RangeStart = g.ElementAt(0).Value,
					RangeLength = g.ElementAt(1).Value
				})
				.ToList();

	Map currentMap = null!;

	while (!reader.EndOfStream)
	{
		line = reader.ReadLine();

		if (!string.IsNullOrWhiteSpace(line))
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

//TODO Fix diff method
IEnumerable<SeedRange?> RangeDiff(long range1Start, long range1Length, long range2Start, long range2Length)
{
	long range1End = range1Start + range1Length;
	long range2End = range2Start + range2Length;

	//Situation where 2 ranges don't intersect
	if (range1End < range2Start || range1Start > range2End)
	{
		yield return new SeedRange() { RangeStart = range1Start, RangeLength = range1Length };
	}
	else
	{
		var points = CreateSortedPointsList(range1Start, range1End, range2Start, range2End);

		if (points.First().Value is 1)
		{
			var rangeStart = points.First().Key; 
			var rangeEnd = points[1].Key;

			if (rangeStart == rangeEnd)
				yield return null;

			yield return new SeedRange() { RangeLength = rangeEnd - rangeStart, RangeStart = rangeStart };
		}

		if (points.Last().Value is 1)
		{
			var rangeStart = points[3].Key;
			var rangeEnd = points.Last().Key;

			if (rangeStart == rangeEnd)
				yield return null;

			yield return new SeedRange() { RangeLength = rangeEnd - rangeStart, RangeStart = rangeStart };
		}
	}
}

IEnumerable<SeedRange> RangeIntersect(long range1Start, long range1Length, long range2Start, long range2Length)
{
	long range1End = range1Start + range1Length;
	long range2End = range2Start + range2Length;

	//Situation where 2 ranges don't intersect
	if (range1End < range2Start || range1Start > range2End)
	{
		yield break;
	}
	else
	{
		var points = CreateSortedPointsList(range1Start, range1End, range2Start, range2End);

		var resultRangeStart = points[1].Key;
		var resultRangeEnd = points[2].Key;

		if (resultRangeStart == resultRangeEnd)
			yield break;

		yield return new SeedRange() { RangeLength = resultRangeEnd - resultRangeStart, RangeStart = resultRangeStart };
	}
}

List<KeyValuePair<long, int>> CreateSortedPointsList(long range1Start, long range1End, long range2Start, long range2End)
{
	var points = new List<KeyValuePair<long, int>>()
		{
			KeyValuePair.Create(range1Start, 1),
			KeyValuePair.Create(range2Start, 2),
			KeyValuePair.Create(range1End, 1),
			KeyValuePair.Create(range2End, 2)
		};

	points.Sort((x, y) => x.Key.CompareTo(y.Key));

	return points;
}

long DetermineLowestDestination(Almanac almanac)
{
	var destinationRanges = almanac.Seeds;

	foreach(var map in almanac.Maps)
	{
		var newDestinations = new List<SeedRange>();

		foreach(var destination in destinationRanges) 
		{
			var mappedIntersects = map.MapRanges
				.SelectMany(mapRange => RangeIntersect(destination.RangeStart, destination.RangeLength, mapRange.SourceStart, mapRange.RangeLength)
					.Select(i => new SeedRange() { RangeStart = mapRange.DestinationStart, RangeLength = i.RangeLength}));

			if(mappedIntersects.Any(x => x.RangeLength == destination.RangeLength))
			{
				newDestinations.Add(mappedIntersects.Single(x => x.RangeLength == destination.RangeLength));
				continue;
			}
			else
			{
				//TODO Determine differences between destination and intersects (sources, not mapped), add them to newDestinations
			}
		}

		destinationRanges = newDestinations;
	}

	destinationRanges.Sort((x, y) => x.RangeStart.CompareTo(y.RangeStart));

	return destinationRanges.First().RangeStart;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var almanac = ReadAlmanacFromInput("test.txt");

	var result = DetermineLowestDestination(almanac);

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

struct SeedRange
{
	public long RangeStart;
	public long RangeLength;
}

class Almanac
{
	public List<Map> Maps = [];
	public List<SeedRange> Seeds = null!;

	public Almanac()
	{
	}
}

partial class Program
{
	[GeneratedRegex(@"^seeds:\s+((?:\d+\s*)+)$")]
	private static partial Regex SeedsRegex();
}