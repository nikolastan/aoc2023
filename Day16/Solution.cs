using NUnit.Framework;
using System.Diagnostics;

namespace Day16;

public class Solution
{
    static Tile[,] Grid = new Tile[1,1];

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

    public int SolvePart1(string inputPath)
    {
        var sw = Stopwatch.StartNew();

        Grid = ReadInput(inputPath);

        gridDim = Grid.GetLength(1);

        BeamQueue.RunQueue();

		var result = 0;

		foreach (var tile in Grid)
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

    enum TileType
    {
        Empty, SplitterHorizontal, SplitterVertical, MirrorBack, MirrorForward // . -  | \ /
    }

    class Tile
    {
        public bool Energized { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public TileType TileType { get; set; }

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

    static HashSet<(Tile, Direction)> crossroads = [];

    class Beam
    {
        public Beam(Tile origin, Direction direction)
        {
            StartTile = origin;
            Direction = direction;

            _currentTile = StartTile;
        }

        public Tile StartTile { get; set; }
        Tile _currentTile;
        public Direction Direction { get; set; }

        public bool HandleCurrentTile(Tile tile)
        {
            switch (tile.TileType)
            {
                case TileType.Empty:
                    return false;
                case TileType.SplitterVertical:
				case TileType.SplitterHorizontal:
					{
						crossroads.Add((_currentTile, Direction));
						var died = HandleSplitter(tile);
                        return died;
                    }
                case TileType.MirrorBack:
                case TileType.MirrorForward:
                    {
						crossroads.Add((_currentTile, Direction));
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
                            BeamQueue.Enqueue(new Beam(Grid[tile.X, tile.Y-1], Direction.Left));

						if (Tile.IsValid(tile.X, tile.Y + 1))
							BeamQueue.Enqueue(new Beam(Grid[tile.X, tile.Y + 1], Direction.Right));

                        return true;
					}
				case TileType.SplitterVertical when Direction is Direction.Left or Direction.Right:
                    {

						if (Tile.IsValid(tile.X - 1, tile.Y))
							BeamQueue.Enqueue(new Beam(Grid[tile.X-1, tile.Y], Direction.Up));

						if (Tile.IsValid(tile.X + 1, tile.Y))
							BeamQueue.Enqueue(new Beam(Grid[tile.X + 1, tile.Y], Direction.Down));

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
				if (crossroads.Contains((_currentTile, Direction)))
				{
                    return;
				}

				Grid[_currentTile.X, _currentTile.Y].Energized = true;

                if (HandleCurrentTile(_currentTile))
                    return;

				switch (Direction)
				{
					case Direction.Up when _currentTile.X - 1 >= 0:
						_currentTile = Grid[_currentTile.X - 1, _currentTile.Y];
						break;
					case Direction.Down when _currentTile.X + 1 < gridDim:
						_currentTile = Grid[_currentTile.X + 1, _currentTile.Y];
						break;
					case Direction.Left when _currentTile.Y - 1 >= 0:
						_currentTile = Grid[_currentTile.X, _currentTile.Y - 1];
						break;
					case Direction.Right when _currentTile.Y + 1 < gridDim:
						_currentTile = Grid[_currentTile.X, _currentTile.Y + 1];
						break;
					default:
                        return;
				}
			}
        }
	}

    static class BeamQueue
    {
        static Queue<Beam> beams = new Queue<Beam>();

        public static void Enqueue(Beam beam)
        {
            beams.Enqueue(beam);
        }

        public static void RunQueue()
        {
            Enqueue(new Beam(Grid[0, 0], Direction.Right));

            while (beams.TryDequeue(out Beam beam))
            {
                beam?.RunBeam();
            }
        }
    }
}
