﻿using System.Diagnostics;

void Solve()
{
    var sw = Stopwatch.StartNew();

    var grid = ReadGridFromInput("test.txt");

    var (i, j) = FindStartIndex(grid);

    ScanSurroundings(ref grid, i, j, 0);

    var result = (grid.Max(x => x.Max(x => x.DistanceFromStart)) + 1) / 2;

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

void ScanSurroundings(ref Tile[][] grid, int i, int j, int currentDistanceFromStart)
{
    if (i == grid.Length || i < 0
        || j == grid[i].Length || j < 0)
        return;

    if (grid[i][j].Visited)
        return;

    grid[i][j].Visited = true;

    if (grid[i][j].Type is not TileType.Start && (grid[i][j].DistanceFromStart is null || grid[i][j].DistanceFromStart < currentDistanceFromStart))
    {
        grid[i][j].DistanceFromStart = ++currentDistanceFromStart;
    }

    if (i > 0 && (grid[i - 1][j].From == Cardinal.South || grid[i - 1][j].To == Cardinal.South))
        ScanSurroundings(ref grid, i - 1, j, currentDistanceFromStart);

    if (j > 0 && (grid[i][j - 1].From == Cardinal.East || grid[i][j - 1].To == Cardinal.East))
        ScanSurroundings(ref grid, i, j - 1, currentDistanceFromStart);

    if (i < grid.Length - 1 && (grid[i + 1][j].From == Cardinal.North || grid[i + 1][j].To == Cardinal.North))
        ScanSurroundings(ref grid, i + 1, j, currentDistanceFromStart);

    if (j < grid.Length - 1 && (grid[i][j + 1].From == Cardinal.West || grid[i][j + 1].To == Cardinal.West))
        ScanSurroundings(ref grid, i, j + 1, currentDistanceFromStart);
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
    public int? DistanceFromStart = null;
    public bool Visited = false;
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