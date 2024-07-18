using System.Diagnostics;

void Solve()
{
    var sw = Stopwatch.StartNew();

    var grid = ReadGridFromInput("test.txt");

    var (i, j) = FindStartIndex(grid);

    TraceLoop(ref grid, i, j);

    var result = CountPointsInsideLoop(ref grid);

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

void TraceLoop(ref Tile[][] grid, int i, int j)
{
    bool changed;

    do
    {
        changed = false;

        grid[i][j].PartOfLoop = true;

        if (i > 0
            && ArePipesConnected(grid[i - 1][j], grid[i][j], Cardinal.South, Cardinal.North)
            && !grid[i - 1][j].PartOfLoop)
        {
            i--;
            changed = true;
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

int CountPointsInsideLoop(ref Tile[][] grid)
{
    var count = 0;

    for (int i = 0; i < grid.Length; i++)
        for (int j = 0; j < grid[i].Length; j++)
        {
            if (!grid[i][j].PartOfLoop && IsInsideLoop(ref grid, i, j))
                count++;
        }

    return count;
}

bool IsInsideLoop(ref Tile[][] grid, int i, int j)
{
    var edgeCount = 0;
    (int, int) lastEdgePoint = (0, 0);
    var isCurrentlyOnEdge = false;

    while (j < grid[0].Length)
    {
        var currentTile = grid[i][j];
        if (currentTile.PartOfLoop)
        {
            if (!isCurrentlyOnEdge)
            {
                edgeCount++;
                isCurrentlyOnEdge = true;
            }

            lastEdgePoint = (i, j);
        }
        else if (isCurrentlyOnEdge)
        {
            isCurrentlyOnEdge = false;
            if (lastEdgePoint.Item1 > 0 && grid[lastEdgePoint.Item1 - 1][lastEdgePoint.Item2].PartOfLoop)
                edgeCount++;
            else if (lastEdgePoint.Item1 < grid.Length - 1 && grid[lastEdgePoint.Item1 + 1][lastEdgePoint.Item2].PartOfLoop)
                edgeCount++;
        }

        j++;
    }

    if (edgeCount % 2 is 1)
        return true;

    return false;

}

bool ArePipesConnected(Tile tile1, Tile tile2, Cardinal from, Cardinal to)
{
    return (tile1.From == from || tile1.To == from) && (tile2.From == to || tile2.To == to || tile2.Type == TileType.Start);
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