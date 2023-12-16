using System.Diagnostics;
using System.Text.RegularExpressions;

Dictionary<string, char> DigitMappingsToWords = new()
{
	{ "zero", '0' },
	{ "one" , '1' },
	{ "two", '2' },
	{ "three", '3' },
	{ "four", '4' },
	{ "five", '5' },
	{ "six", '6' },
	{ "seven", '7' },
	{ "eight", '8' },
	{ "nine", '9' }
};

List<string> ReadLinesFromInput(string fileName)
{
	using var reader = new StreamReader(fileName);
	List<string> lines = [];

	while (!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if (line is not null)
			lines.Add(line);
	}

	return lines;
}

int GetNumberFromLine(string line)
{
	char firstDigit = '0';
	char lastDigit = '0';

	//Split with digit chars as delimiters, but keep them as their own splits
	string pattern = "([0-9])";
	var lineChunks = Regex.Split(line, pattern).Where(s => !string.IsNullOrWhiteSpace(s));

	//Find first digit
	foreach ( var lineChunk in lineChunks)
	{
		if(lineChunk.Length is 1 && char.IsDigit(lineChunk[0]))
		{
			firstDigit = lineChunk[0];
			break;
		}

		var digit = TryParseSemantically(lineChunk, Ocurrence.First);
		
		if(digit is not '/')
		{
			firstDigit = digit;
			break;
		}
	}

	//Find last digit
	foreach (var lineChunk in lineChunks.Reverse())
	{
		if (lineChunk.Length is 1 && char.IsDigit(lineChunk[0]))
		{
			lastDigit = lineChunk[0];
			break;
		}

		var digit = TryParseSemantically(lineChunk, Ocurrence.Last);

		if (digit is not '/')
		{
			lastDigit = digit;
			break;
		}
			
	}

	return int.Parse($"{firstDigit}{lastDigit}");
}

char TryParseSemantically(string lineChunk, Ocurrence ocurrence)
{
	var numberWords = new SortedDictionary<int, char>();

	foreach (var numberWordPair in DigitMappingsToWords)
	{
		var indexes = AllIndexesOf(lineChunk, numberWordPair.Key);

		foreach (var index in indexes)
			numberWords[index] = numberWordPair.Value;

	}

	if (numberWords.Count is 0)
		return '/';

	return ocurrence switch
	{
		Ocurrence.First => numberWords.First().Value,
		Ocurrence.Last => numberWords.Last().Value,
		_ => '/'
	};
}

IEnumerable<int> AllIndexesOf(string str, string value)
{
	if (String.IsNullOrEmpty(value))
		throw new ArgumentException("the string to find may not be empty", nameof(value));

	for (int index = 0; ; index += value.Length)
	{
		index = str.IndexOf(value, index);
		if (index == -1)
			break;
		yield return index;
	}
}

void Solve(List<string> lines)
{
	List<int> numbersFromLines = lines.Select(line => GetNumberFromLine(line)).ToList();
	var sum = numbersFromLines.Sum(n => n);

	Console.WriteLine(sum);
}


var sw = Stopwatch.StartNew();

var lines = ReadLinesFromInput("input.txt");
Solve(lines);

sw.Stop();
Console.WriteLine(sw.ElapsedMilliseconds);

enum Ocurrence
{
	First,
	Last
};