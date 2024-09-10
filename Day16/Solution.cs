using NUnit.Framework;
using System.Diagnostics;
using Utility;

namespace Day16;

public class Solution
{
    static int gridDim;

    [Test]
    public void Part1_Example()
    {
        var result = SolvePart1(@"Inputs/example.txt");
        Assert.That(result, Is.EqualTo(46));
    }

    [Test]
    public void Part1_Input()
    {
        SolvePart1(@"Inputs/input.txt");
	}

	[Test]
	public async Task Part2_Example()
	{
		var result = await SolvePart2(@"Inputs/example.txt");
		Assert.That(result, Is.EqualTo(51));
	}

	[Test]
	public async Task Part2_Input()
	{
		await SolvePart2(@"Inputs/input.txt");
	}

	public int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        var map = ReadInput(inputPath);

        gridDim = map.GetLength(1);

        var grid = new Grid(map);

        grid.BeamQueue.RunQueue(grid, grid.Map[0,0], Direction.Right);

		var result = 0;

		foreach (var tile in grid.Map)
		{
			if (tile.Energized)
				result++;
		}

   //     for(int i = 0; i < gridDim; i++)
   //     {
   //         for(int j = 0; j < gridDim; j++)
   //             Console.Write(Grid[i,j].Energized ? '#' : '.');

			//Console.Write('\n');
   //     }    

		sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    public async Task<int> SolvePart2(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        sw.Start();

		var map = ReadInput(inputPath);
		gridDim = map.GetLength(1);
        
        var tasks = new List<Task<int>>();

        for (int i = 0; i < gridDim; i++)
        {
            var j = i;

            tasks.Add(Task.Run(() =>
            {
                var grid = new Grid(map.Copy());
                grid.BeamQueue.RunQueue(grid, grid.Map[j, 0], Direction.Right);
                return CountEnergizedTiles(grid.Map);
            }));
            tasks.Add(Task.Run(() =>
            {
                var grid = new Grid(map.Copy());
                grid.BeamQueue.RunQueue(grid, grid.Map[gridDim - 1, j], Direction.Up);
                return CountEnergizedTiles(grid.Map);
            }));
            tasks.Add(Task.Run(() =>
            {
                var grid = new Grid(map.Copy());
                grid.BeamQueue.RunQueue(grid, grid.Map[j, gridDim - 1], Direction.Left);
                return CountEnergizedTiles(grid.Map);
            }));
            tasks.Add(Task.Run(() =>
            {
                var grid = new Grid(map.Copy());
                grid.BeamQueue.RunQueue(grid, grid.Map[0, j], Direction.Down);
                return CountEnergizedTiles(grid.Map);
            }));
        }

        var result = (await Task.WhenAll(tasks)).Max();

		sw.Stop();

        Console.WriteLine($"Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");

        return result;
    }

    Tile[,] ReadInput(string inputPath)
    {
        var input = File.ReadAllLines(inputPath);

        var grid = new Tile[input.Length, input[0].Length];

        for (int i = 0; i < input.Length; i++)
        {
            for (int j = 0; j < input[0].Length; j++)
            {
                var ch = input[i][j];
                TileType type = ch switch
                {
                    '.' => TileType.Empty,
                    '-' => TileType.SplitterHorizontal,
                    '|' => TileType.SplitterVertical,
                    '/' => TileType.MirrorForward,
                    '\\' => TileType.MirrorBack,
                    _ => throw new ArgumentException($"Wrong char type: passed {ch}")
                };

                grid[i, j] = new Tile(i,j,type);
            }
        }

        return grid;
    }

    int CountEnergizedTiles(Tile[,] map)
    {
        var result = 0;

        for (int i = 0; i < gridDim ;i++)
        {
            for(int j = 0;j < gridDim ;j++)
            {
				if (map[i,j].Energized)
					result++;
			}
        }

        Debug.WriteLine(result);
        return result;
    }
    enum TileType
    {
        Empty, SplitterHorizontal, SplitterVertical, MirrorBack, MirrorForward // . -  | \ /
    }

    struct Tile
    {
        public bool Energized { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TileType TileType { get; set; }

        //public Tile Copy()
        //{
        //    return new Tile(X, Y, TileType);
        //}

        public Tile(int x, int y, TileType type)
        {
            X = x;
            Y = y;
            TileType = type;
        }

        public static bool IsValid(int x, int y)
        {
            if(x < 0 || y < 0 || x >= gridDim ||  y >= gridDim)
                return false;

            return true;
        }
	}

    enum Direction { Up, Down, Left, Right }

    class Grid(Tile[,] map)
    {
		public HashSet<(Tile, Direction)> Crossroads = [];
        public BeamQueue BeamQueue = new();

        public Tile[,] Map = map;
	}

    class Beam
    {
        Grid grid { get; set; }
        public Tile StartTile { get; set; }
        Tile _currentTile;
        public Direction Direction { get; set; }

		public Beam(Grid grid, Tile origin, Direction direction)
		{
            this.grid = grid;
			StartTile = origin;
			Direction = direction;

			_currentTile = StartTile;
		}

		public bool HandleCurrentTile(Tile tile)
        {
            switch (tile.TileType)
            {
                case TileType.Empty:
                    return false;
                case TileType.SplitterVertical:
				case TileType.SplitterHorizontal:
					{
						grid.Crossroads.Add((_currentTile, Direction));
						var died = HandleSplitter(tile);
                        return died;
                    }
                case TileType.MirrorBack:
                case TileType.MirrorForward:
                    {
						grid.Crossroads.Add((_currentTile, Direction));
						HandleMirror(tile);
                        return false;
                    }
                default:
                    throw new NotImplementedException("What?");
            }
        }

        bool HandleSplitter(Tile tile)
        {
            switch(tile.TileType)
            {
                case TileType.SplitterHorizontal when Direction is Direction.Up or Direction.Down:
                    {
                        if (Tile.IsValid(tile.X, tile.Y - 1))
                            grid.BeamQueue.Enqueue(new Beam(grid, grid.Map[tile.X, tile.Y-1], Direction.Left));

						if (Tile.IsValid(tile.X, tile.Y + 1))
							grid.BeamQueue.Enqueue(new Beam(grid, grid.Map[tile.X, tile.Y + 1], Direction.Right));

                        return true;
					}
				case TileType.SplitterVertical when Direction is Direction.Left or Direction.Right:
                    {

						if (Tile.IsValid(tile.X - 1, tile.Y))
							grid.BeamQueue.Enqueue(new Beam(grid, grid.Map[tile.X-1, tile.Y], Direction.Up));

						if (Tile.IsValid(tile.X + 1, tile.Y))
							grid.BeamQueue.Enqueue(new Beam(grid, grid.Map[tile.X + 1, tile.Y], Direction.Down));

                        return true;
                    }
                default:
                    if (tile.TileType is not (TileType.SplitterHorizontal or TileType.SplitterVertical))
                        throw new ArgumentException($"Passed {tile.TileType} instead of Splitter type.");
                    else
                        return false;
            }
		}

		void HandleMirror(Tile tile)
		{
			switch (tile.TileType)
			{
                case TileType.MirrorBack when Direction is Direction.Up:
				case TileType.MirrorForward when Direction is Direction.Down:
					{
                        Direction = Direction.Left;
						break;
					}
                case TileType.MirrorBack when Direction is Direction.Down:
				case TileType.MirrorForward when Direction is Direction.Up:
					{
						Direction = Direction.Right;
						break;
					}
				case TileType.MirrorBack when Direction is Direction.Left:
                case TileType.MirrorForward when Direction is Direction.Right:
					{
						Direction = Direction.Up;
						break;
					}
				case TileType.MirrorBack when Direction is Direction.Right:
				case TileType.MirrorForward when Direction is Direction.Left:
					{
                        Direction = Direction.Down;
						break;
					}
				default:
					if (tile.TileType is not TileType.MirrorBack or TileType.MirrorForward)
						throw new ArgumentException($"Passed {tile} instead of Mirror type.");
					else
						break;
			}
		}

        public void RunBeam()
        {
            while (true)
			{
				if (grid.Crossroads.Contains((_currentTile, Direction)))
				{
                    return;
				}

				grid.Map[_currentTile.X, _currentTile.Y].Energized = true;

                if (HandleCurrentTile(_currentTile))
                    return;

				switch (Direction)
				{
					case Direction.Up when _currentTile.X - 1 >= 0:
						_currentTile = grid.Map[_currentTile.X - 1, _currentTile.Y];
						break;
					case Direction.Down when _currentTile.X + 1 < gridDim:
						_currentTile = grid.Map[_currentTile.X + 1, _currentTile.Y];
						break;
					case Direction.Left when _currentTile.Y - 1 >= 0:
						_currentTile = grid.Map[_currentTile.X, _currentTile.Y - 1];
						break;
					case Direction.Right when _currentTile.Y + 1 < gridDim:
						_currentTile = grid.Map[_currentTile.X, _currentTile.Y + 1];
						break;
					default:
                        return;
				}
			}
        }
	}

    class BeamQueue
    {
        Queue<Beam> beams = new();

        public void Enqueue(Beam beam)
        {
            beams.Enqueue(beam);
        }

        public void RunQueue(Grid grid, Tile startingPosition, Direction direction)
        {
            Enqueue(new Beam(grid, startingPosition, direction));

            while (beams.TryDequeue(out Beam beam))
            {
                beam?.RunBeam();
            }
        }
    }
}
