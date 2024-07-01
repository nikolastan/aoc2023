void Solve()
{
	var grid = ReadGridFromInput("test.txt");
}

Solve();


//TODO Convert return value to 2d array for easier traversal
List<List<Tile>> ReadGridFromInput(string filePath)
{
	using var reader = new StreamReader(filePath);
	var grid = new List<List<Tile>>();

	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();
		var tileLine = new List<Tile>();

		if (line is null)
			break;

		foreach(var tileChar in line)
		{
			tileLine.Add(ParseTile(tileChar));
		}

		grid.Add(tileLine);
	}

	return grid;
}

Tile ParseTile(char tileChar)
{
	return tileChar switch
	{
		'|' => new Tile(TileType.Pipe, Cardinal.North, Cardinal.South),
		'-' => new Tile(TileType.Pipe, Cardinal.East, Cardinal.West),
		'L' => new Tile(TileType.Pipe, Cardinal.North, Cardinal.East),
		'J' => new Tile(TileType.Pipe, Cardinal.North, Cardinal.West),
		'7' => new Tile(TileType.Pipe, Cardinal.South, Cardinal.West),
		'F' => new Tile(TileType.Pipe, Cardinal.South, Cardinal.East),
		'.' => new Tile(TileType.Ground),
		'S' => new Tile(TileType.Start),
		_ => throw new ArgumentException("Invalid character.")
	};
}

struct Tile(TileType type, Cardinal? from = null, Cardinal? to = null)
{
	public TileType Type = type;
	public Cardinal? From = from;
	public Cardinal? To = to;
}

enum TileType
{
	Pipe,
	Ground,
	Start
}

enum Cardinal
{
	North,
	South,
	East,
	West
}