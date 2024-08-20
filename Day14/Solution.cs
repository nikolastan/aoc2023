using NUnit.Framework;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using static Utility.Enums;

namespace Day14;

public class Solution
{
    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(136));
    }

    [Test]
    public void Part1_Input()
    {
        var result = SolvePart1(@"Inputs/input.txt");
    }

    [Test]
    public void Part2_Example()
    {
        var result = SolvePart2(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(64));
    }

    [Test]
    public void Part2_Input()
    {
        var result = SolvePart2(@"Inputs/input.txt");
    }

    int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var grid = File.ReadLines(inputPath)
            .Select(s => s.ToCharArray())
			.Select(s => s.Select(c => new Tile(c)).ToArray())
			.ToArray();

        Tilt(ref grid, Cardinal.North);

        var result = grid //tiltedGrid
            .Reverse()
            .Select((row, index) => new { row, index })
            .Sum(x => x.row.Count(c => c.Type is TileType.Movable) * (x.index + 1));


		sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    int SolvePart2(string inputPath)
	{
		var sw = Stopwatch.StartNew();

		var totalCycles = 1_000_000_000;

		var grid = File.ReadLines(inputPath)
			.Select(s => s.ToCharArray())
			.Select(s => s.Select(c => new Tile(c)).ToArray())
			.ToArray();

		var memo = new Dictionary<string, int>();

		bool foundLoop = false;

		for (int i = 0; i < totalCycles; i++)
		{
			Tilt(ref grid, Cardinal.North);
			Tilt(ref grid, Cardinal.West);
			Tilt(ref grid, Cardinal.South);
			Tilt(ref grid, Cardinal.East);
			//PrintGrid(grid);

			if (foundLoop)
				continue;

			var bytes = ASCIIEncoding.ASCII.GetBytes(grid.SelectMany(x => x.Select(x => x.OriginalChar)).ToArray());
			var hash = Convert.ToHexString(MD5.HashData(bytes));

			if (!memo.TryAdd(hash, i))
			{
				memo.TryGetValue(hash, out int loopStartIndex);
				var loopDuration = i - loopStartIndex;
				i = totalCycles - ((totalCycles - loopStartIndex) % loopDuration);
				foundLoop = true;
			}
		}

		var result = grid
			.Reverse()
			.Select((row, index) => new { row, index })
			.Sum(x => x.row.Count(c => c.Type == TileType.Movable) * (x.index + 1));

		sw.Stop();

		Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

		return result;
	}

	//North -> j-i, 0
	//West -> i-j, 0
	//South -> j-i, n
	//East -> i-j, n

	void Tilt(ref Tile[][] grid, Cardinal side)
    {
        int startIndex;
        int endIndex;
        int step;

        DetermineParameters(side, in grid, out startIndex, out endIndex, out step);

        if (side is Cardinal.West or Cardinal.East)
        {
            for (int i = 0; i < grid.Length; i++)
            {
                int nextMovableIndex = startIndex;
                for (int j = startIndex; (j <= endIndex && step > 0) || (j >= endIndex && step < 0); j += step)
                {
					if (grid[i][j].Type is TileType.NonMovable)
						nextMovableIndex = j + step;

					if (grid[i][j].Type is TileType.Movable)
					{
						if (nextMovableIndex != j && nextMovableIndex >= 0)
						{
							grid[i][nextMovableIndex].Type = TileType.Movable;
							grid[i][j].Type = TileType.Ground;
						}

						nextMovableIndex += step;
					}
				}
            }
        }
        else
        {
			for (int j = 0; j < grid[0].Length; j++)
			{
                int nextMovableIndex = startIndex;
				for (int i = startIndex; (i <= endIndex && step > 0) || (i >= endIndex && step < 0); i += step)
				{
					if (grid[i][j].Type is TileType.NonMovable)
						nextMovableIndex = i + step;

					if (grid[i][j].Type is TileType.Movable)
					{
						if (nextMovableIndex != i && nextMovableIndex >= 0)
						{
							grid[nextMovableIndex][j].Type = TileType.Movable;
							grid[i][j].Type = TileType.Ground;
						}

						nextMovableIndex += step;
					}
				}
			}
		}
    }

    void DetermineParameters(Cardinal side, in Tile[][] grid, out int startIndex, out int endIndex, out int step)
    {
		switch (side)
		{
			case Cardinal.North:
				startIndex = 0;
				endIndex = grid[0].Length - 1;
				step = 1;
				break;
			case Cardinal.West:
				startIndex = 0;
				endIndex = grid.Length - 1;
				step = 1;
				break;
			case Cardinal.South:
				startIndex = grid[0].Length - 1;
				endIndex = 0;
				step = -1;
				break;
			case Cardinal.East:
				startIndex = grid.Length - 1;
				endIndex = 0;
				step = -1;
				break;
			default:
				throw new ArgumentException($"Invalid side {side}.");
		}
	}

	void PrintGrid(Tile[][] grid)
	{
		for(int i = 0; i<grid.Length; i++)
		{
			for (int j = 0; j < grid[0].Length; j++)
				Debug.Write(grid[i][j].OriginalChar);

			Debug.Write('\n');
		}

		Debug.Write('\n');
	}

    struct Tile
    {
		public TileType _tileType;
        public TileType Type { get => _tileType;
			set
			{
				_originalChar = value switch
				{
					TileType.Movable => 'O',
					TileType.NonMovable => '#',
					TileType.Ground => '.',
					_ => throw new InvalidOperationException($"Invalid character {value}")
				};

				_tileType = value;
			}
		}

        private char _originalChar;
        public char OriginalChar { get => _originalChar;
            set
            {
				Type = value switch
				{
					'O' => TileType.Movable,
					'#' => TileType.NonMovable,
					_ => TileType.Ground,
				};

				_originalChar = value;
			}
        }

        public Tile(char c)
        {
            OriginalChar = c;
		}
	}

    enum TileType
    {
        Movable,
        NonMovable,
        Ground
    }
}
