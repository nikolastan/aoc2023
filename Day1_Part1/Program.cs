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
	char firstDigit = line.FirstOrDefault(c => char.IsDigit(c), '0');
	char lastDigit = line.LastOrDefault(c => char.IsDigit(c), '0');

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