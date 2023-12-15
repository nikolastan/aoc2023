List<string> ReadLinesFromInput(string fileName)
{
	using var reader = new StreamReader(fileName);
	List<string> lines = [];

	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if(line is not null)
			lines.Add(line);
	}

	return lines;
}

int GetNumberFromLine(string line)
{
	char firstDigit = '0';
	char lastDigit = '0';

	var reversedLine = line.Reverse();

	//Find first digit
	foreach(char c in line)
	{
		if(char.IsDigit(c))
		{
			firstDigit = c;
			break;
		}
	}

	//Find last digit
	foreach (char c in reversedLine)
	{
		if (char.IsDigit(c))
		{
			lastDigit = c;
			break;
		}
	}

	return int.Parse($"{firstDigit}{lastDigit}");
}

void Solve(List<string> lines)
{
	List<int> numbersFromLines = lines.Select(line =>  GetNumberFromLine(line)).ToList();
	var sum = numbersFromLines.Sum(n => n);

	Console.WriteLine(sum);
}


var lines = ReadLinesFromInput("input.txt");
Solve(lines);