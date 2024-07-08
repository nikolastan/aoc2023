void Solve()
{
    var grid = ReadGridFromInput("test.txt");


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

void ScanSurroundings(ref Tile[][] grid, int i, int j, int currentDistanceFromStart)
{
    if (i == grid.Length || i < 0)
        return;

    if (j == grid[j].Length || j < 0)
        return;

    var currentTile = grid[j][i];

    if (currentTile.Type != TileType.Start && currentTile.DistanceFromStart < currentDistanceFromStart)
    {
        currentTile.DistanceFromStart = ++currentDistanceFromStart;
    }


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
    public int DistanceFromStart = 0;
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