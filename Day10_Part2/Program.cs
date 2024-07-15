using System.Diagnostics;

void Solve()
{
    var sw = Stopwatch.StartNew();

    var grid = ReadGridFromInput("input.txt");

    var (i, j) = FindStartIndex(grid);

    ScanSurroundings(ref grid, i, j, 0);



    var result = 0;

    sw.Stop();
    Console.WriteLine($"Result: {result}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();

Tile[][] ReadGridFromInput(string filePath)
{
    using var reader = new StreamReader(filePath);
    var grid = new List<Tile[]>();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (line is null)
            break;

        var tileLine = new Tile[line.Length];

        for (int i = 0; i < line.Length; i++)
        {
            tileLine[i] = ParseTile(line[i]);
        }

        grid.Add(tileLine);
    }

    return [.. grid];
}

(int, int) FindStartIndex(Tile[][] grid)
{
    var rowLength = grid.Length;
    var colLength = grid[0].Length;

    for (int i = 0; i < rowLength; i++)
        for (int j = 0; j < colLength; j++)
            if (grid[i][j].Type == TileType.Start)
                return (i, j);

    throw new InvalidOperationException("There is no Start tile in input. Check your input.");
}

void ScanSurroundings(ref Tile[][] grid, int i, int j)
{
    bool changed;
    do
    {
        changed = false;

        grid[i][j].PartOfLoop = true;

        if (grid[i][j].EligibleForEnclosure)
        {
            grid[i][j].EligibleForEnclosure = false;
        }

        if (i > 0
            && ArePipesConnected(grid[i - 1][j], grid[i][j], Cardinal.South, Cardinal.North)
            && !grid[i - 1][j].PartOfLoop)
        {
            i--;
            changed = true;
            MarkAdjacentForEnclosure(ref grid, i, j);
        }
        else if (j > 0
            && ArePipesConnected(grid[i][j - 1], grid[i][j], Cardinal.East, Cardinal.West)
            && !grid[i][j - 1].PartOfLoop)
        {
            j--;
            changed = true;
        }
        else if (i < grid.Length - 1
            && ArePipesConnected(grid[i + 1][j], grid[i][j], Cardinal.North, Cardinal.South)
            && !grid[i + 1][j].PartOfLoop)
        {
            i++;
            changed = true;
			MarkAdjacentForEnclosure(ref grid, i, j);
		}
        else if (j < grid.Length - 1
            && ArePipesConnected(grid[i][j + 1], grid[i][j], Cardinal.West, Cardinal.East)
            && !grid[i][j + 1].PartOfLoop)
        {
            j++;
            changed = true;
        }
    }
    while (changed);
}

void MarkAdjacentForEnclosure(ref Tile[][] grid, int i, int j)
{
    if(j < grid.Length - 1 && !grid[i][j + 1].PartOfLoop)
    {
        grid[i][j + 1].EligibleForEnclosure = true;
    }

    if(j > 0 && !grid[i][j - 1].PartOfLoop)
    {
        grid[i][j - 1].EligibleForEnclosure = true;
    }
}

bool ArePipesConnected(Tile tile1, Tile tile2, Cardinal from, Cardinal to)
{
    return (tile1.From == from || tile1.To == from) && (tile2.From == to || tile2.To == to || tile2.Type == TileType.Start);
}

int CountEnclosedTiles(ref Tile[][] grid)
{
    var eligibleForEnclosure = grid
        .SelectMany((row, rowIndex) => row.Select((_, colIndex) => new { rowIndex, colIndex }))
        .ToList();

	var totalCount = 0;

    while(eligibleForEnclosure.Count > 0)
    {
        var currentTile = eligibleForEnclosure.First();
        eligibleForEnclosure.Remove(currentTile);

        var rowCount = 1;

        //Cannot work like this, we must check for inner & outer border of loop.
    }

}

bool IsEnclosed(ref Tile[][] grid, int i, int j)
{

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
    public bool PartOfLoop = false;
    public bool EligibleForEnclosure = false;
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