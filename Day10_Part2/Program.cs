using System.Diagnostics;
using static Utility.Enums;

void Solve()
{
    var sw = Stopwatch.StartNew();

    Grid = ReadGridFromInput("input.txt");

    var (i, j) = FindStartIndex();

    TraceLoop(i, j);

    var result = CountPointsInsideLoop();

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

(int, int) FindStartIndex()
{
    var rowLength = Grid.Length;
    var colLength = Grid[0].Length;

    for (int i = 0; i < rowLength; i++)
        for (int j = 0; j < colLength; j++)
            if (Grid[i][j].Type == TileType.Start)
                return (i, j);

    throw new InvalidOperationException("There is no Start tile in input. Check your input.");
}

void TraceLoop(int i, int j)
{
    bool changed;

    do
    {
        changed = false;

        Grid[i][j].PartOfLoop = true;

        if (i > 0
            && ArePipesConnectedBySides(Grid[i - 1][j], Grid[i][j], Cardinal.South, Cardinal.North)
            && !Grid[i - 1][j].PartOfLoop)
        {
            i--;
            changed = true;
        }
        else if (j > 0
            && ArePipesConnectedBySides(Grid[i][j - 1], Grid[i][j], Cardinal.East, Cardinal.West)
            && !Grid[i][j - 1].PartOfLoop)
        {
            j--;
            changed = true;
        }
        else if (i < Grid.Length - 1
            && ArePipesConnectedBySides(Grid[i + 1][j], Grid[i][j], Cardinal.North, Cardinal.South)
            && !Grid[i + 1][j].PartOfLoop)
        {
            i++;
            changed = true;
        }
        else if (j < Grid[0].Length - 1
            && ArePipesConnectedBySides(Grid[i][j + 1], Grid[i][j], Cardinal.West, Cardinal.East)
            && !Grid[i][j + 1].PartOfLoop)
        {
            j++;
            changed = true;
        }
    }
    while (changed);
}

int CountPointsInsideLoop()
{
    var count = 0;

    for (int i = 0; i < Grid.Length; i++)
        for (int j = 0; j < Grid[i].Length; j++)
        {
            if (!Grid[i][j].PartOfLoop && IsInsideLoop(i, j))
                count++;
        }

    return count;
}

bool IsInsideLoop(int i, int j)
{
    var totalEdgeCount = 0;
    var prevColIndex = -1;
    var isCurrentlyOnEdge = false;
    var currentEdgeLenght = 0;

    while (j < Grid[0].Length)
    {
        var currentTile = Grid[i][j];
        if (currentTile.PartOfLoop)
        {
            if (!isCurrentlyOnEdge)
            {
                currentEdgeLenght = 0;
                totalEdgeCount++;
                isCurrentlyOnEdge = true;
                prevColIndex = j;
            }
            else
            {
                currentEdgeLenght++;

                if (((j < Grid[0].Length - 1 && !ArePipesConnected(currentTile, Grid[i][j + 1], Cardinal.East)) || j == Grid[0].Length - 1)
                && prevColIndex != -1
                && currentEdgeLenght > 0)
                {
                    if ((i > 0
                    && ArePipesConnected(Grid[i][prevColIndex], Grid[i - 1][prevColIndex], Cardinal.North)
                    && Grid[i - 1][prevColIndex].PartOfLoop
                    && ArePipesConnected(currentTile, Grid[i - 1][j], Cardinal.North)
                    && Grid[i - 1][j].PartOfLoop)

                    ||

                    (i < Grid.Length - 1
                        && ArePipesConnected(Grid[i][prevColIndex], Grid[i + 1][prevColIndex], Cardinal.South)
                        && Grid[i + 1][prevColIndex].PartOfLoop
                        && ArePipesConnected(currentTile, Grid[i + 1][j], Cardinal.South)
                        && Grid[i + 1][j].PartOfLoop))
                    {
                        totalEdgeCount++;
                        isCurrentlyOnEdge = false;
                    }
                }
            }

            if (j < Grid[0].Length - 1 && !ArePipesConnected(currentTile, Grid[i][j + 1], Cardinal.East))
                isCurrentlyOnEdge = false;
        }
        else
        {
            isCurrentlyOnEdge = false;
        }

        j++;
    }

    if (totalEdgeCount % 2 is 1)
        return true;

    return false;

}

bool ArePipesConnectedBySides(Tile tile1, Tile tile2, Cardinal from, Cardinal to)
{
    return (tile1.From == from || tile1.To == from || tile1.Type == TileType.Start)
        && (tile2.From == to || tile2.To == to || tile2.Type == TileType.Start);
}

bool ArePipesConnected(Tile tile1, Tile tile2, Cardinal goingTo)
{


    return goingTo switch
    {
        Cardinal.East => ArePipesConnectedBySides(tile1, tile2, Cardinal.East, Cardinal.West),
        Cardinal.West => ArePipesConnectedBySides(tile1, tile2, Cardinal.West, Cardinal.East),
        Cardinal.North => ArePipesConnectedBySides(tile1, tile2, Cardinal.North, Cardinal.South),
        Cardinal.South => ArePipesConnectedBySides(tile1, tile2, Cardinal.South, Cardinal.North),
        _ => false,
    };
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

partial class Program
{
    private static Tile[][] Grid { get; set; } = null!;
}