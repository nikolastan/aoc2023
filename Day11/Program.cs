using System.Diagnostics;
using Utility;

void SolvePart1()
{
    var sw = Stopwatch.StartNew();

    var image = ReadImage("input.txt");
    var expandedImage = ExpandColumns(image);

    var galaxies = FindGalaxies(expandedImage)
        .ToList();

    var result = 0L;

    while (galaxies.Count > 0)
    {
        var current = galaxies.First();
        galaxies.RemoveAt(0);

        result += galaxies
            .Select(x =>
            {
                return Math.Abs(x.Coordinates.Item2 - current.Coordinates.Item2)
                    + Math.Abs(x.Coordinates.Item1 - current.Coordinates.Item1);
            })
            .Sum();
    }

    sw.Stop();

    Console.WriteLine($"PART 1 - Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}ms");
}

void SolvePart2()
{
    var sw = Stopwatch.StartNew();

    var image = ReadImageWithXMultipliers("input.txt");
    var expandedImage = ExpandColumnsWithYMultipliers(image);

    var galaxies = FindGalaxiesFromTiles(expandedImage)
        .ToList();

    var result = 0L;

    while (galaxies.Count > 0)
    {
        var current = galaxies.First();
        galaxies.RemoveAt(0);

        result += galaxies
            .Select(x =>
            {
                return Math.Abs(x.Coordinates.Item2 - current.Coordinates.Item2)
                    + Math.Abs(x.Coordinates.Item1 - current.Coordinates.Item1);
            })
            .Sum();
    }

    sw.Stop();

    Console.WriteLine($"PART 2 - Result: {result}, Time elapsed: {sw.ElapsedMilliseconds}");
}

SolvePart1();
SolvePart2();

char[][] ReadImage(string filePath)
{
    using var reader = new StreamReader(filePath);
    var grid = new List<char[]>();

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (line is null)
            break;

        //We do the row expansion while reading the input (for Part1)
        if (line.All(x => x is '.'))
            grid.Add(line.ToCharArray());

        grid.Add(line.ToCharArray());
    }

    return [.. grid];
}

char[][] ExpandColumns(char[][] data)
{
    var transposedImage = Matrix.TransposeMatrix(data);

    var expandedImage = new List<char[]>();

    foreach (var row in transposedImage)
    {
        expandedImage.Add(row);

        if (row.All(x => x is '.'))
            expandedImage.Add(row);
    }

    return Matrix.TransposeMatrix([.. expandedImage]);

}

IEnumerable<Galaxy> FindGalaxies(char[][] image)
{
    var galaxies = new List<Galaxy>();

    for (int i = 0; i < image.Length; i++)
        for (int j = 0; j < image[i].Length; j++)
            if (image[i][j] is '#')
            {
                yield return new Galaxy((i, j));
            }
}

IEnumerable<Galaxy> FindGalaxiesFromTiles(Tile[][] image)
{
    var galaxies = new List<Galaxy>();

    for (int i = 0; i < image.Length; i++)
        for (int j = 0; j < image[i].Length; j++)
            if (image[i][j].Character is '#')
            {
                var current = image[i][j];
                yield return new Galaxy((current.XMultiplier * 1000000 + i - current.XMultiplier,
                    current.YMultiplier * 1000000 + j - current.YMultiplier));
            }
}
Tile[][] ReadImageWithXMultipliers(string filePath)
{
    using var reader = new StreamReader(filePath);
    var grid = new List<Tile[]>();
    var currentXMultiplier = 0;

    while (!reader.EndOfStream)
    {
        var line = reader.ReadLine();

        if (line is null)
            break;

        //We do the row expansion while reading the input
        if (line.All(x => x is '.'))
        {
            currentXMultiplier++;

        }

        grid.Add(line
            .ToCharArray()
            .Select(c => new Tile(c) { XMultiplier = currentXMultiplier })
            .ToArray());
    }

    return [.. grid];
}

Tile[][] ExpandColumnsWithYMultipliers(Tile[][] data)
{
    var transposedImage = Matrix.TransposeMatrix(data);

    var expandedImage = new List<Tile[]>();

    var currentYMultiplier = 0;

    foreach (var row in transposedImage)
    {
        var newRow = row;
        if (row.All(t => t.Character is '.'))
        {
            currentYMultiplier++;
        }

        newRow = row.Select(t =>
        {
            t.YMultiplier = currentYMultiplier;
            return t;
        }).ToArray();

        expandedImage.Add(newRow);
    }

    return Matrix.TransposeMatrix([.. expandedImage]);
}
struct Galaxy((long, long) coordinates)
{
    public (long, long) Coordinates { get; set; } = coordinates;
}

struct Tile(char character)
{
    public char Character { get; set; } = character;
    public long XMultiplier { get; set; } = 0;
    public long YMultiplier { get; set; } = 0;
}