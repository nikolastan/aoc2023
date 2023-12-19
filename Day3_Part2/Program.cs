using System.Diagnostics;

char[][] ReadEngineMatrixFromInput(string filePath)
{
	List<char[]> result = [];

	using var reader = new StreamReader(filePath);

	while (!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if (line is not null)
		{
			//Add first row of padding
			if (result.Count is 0)
			{
				var paddingRow = FormPaddingRow(line.Length + 2);
				result.Add(paddingRow);
			}

			//Adding dots to the beginning and end of read line
			result.Add(['.', .. line, '.']);

			//Add last row of padding
			if (reader.EndOfStream)
			{
				var paddingRow = FormPaddingRow(line.Length + 2);
				result.Add(paddingRow);
			}
		}
	}

	return [.. result];
}

char[] FormPaddingRow(int rowLength)
{
	var paddingRow = new char[rowLength];
	for (int i = 0; i < paddingRow.Length; i++)
		paddingRow[i] = '.';

	return paddingRow;
}

IEnumerable<KeyValuePair<int, int>> GetPartNumbersAround(in char[][] matrix, int iPos, int jPos)
{
	var partNumbers = new List<KeyValuePair<int, int>>();

	for (int i = iPos - 1; i < iPos + 2; i++)
	{
		for (int j = jPos - 1; j < jPos + 2; j++)
		{
			if (char.IsDigit(matrix[i][j]))
			{
				var number = "";
				var numberStartPos = j - 1;
				while(char.IsDigit(matrix[i][numberStartPos]))
				{
					numberStartPos--;
				}
				numberStartPos++;
				while (char.IsDigit(matrix[i][numberStartPos]))
				{
					number += matrix[i][numberStartPos];
					numberStartPos++;
				}

				partNumbers.Add(new KeyValuePair<int, int>(i + j, int.Parse(number)));
			}
		}
	}

	return partNumbers.GroupBy(x => x.Value).Select(g => g.First());
}

int GetSumOfAllGears(in char[][] engineMatrix)
{
	int sum = 0;

	for (int i = 1; i < engineMatrix.Length - 1; i++)
	{
		for (int j = 1; j < engineMatrix[i].Length - 1; j++)
		{
			if (engineMatrix[i][j] is '*')
			{
				var partNumbersAround = GetPartNumbersAround(in engineMatrix, i, j);
				if(partNumbersAround.Count() is 2)
				{
					sum += partNumbersAround.First().Value * partNumbersAround.Last().Value;
				}

			}
		}
	}

	return sum;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var engineMatrix = ReadEngineMatrixFromInput("input.txt");

	var sum = GetSumOfAllGears(in engineMatrix);

	sw.Stop();

	Console.WriteLine($"Result: {sum}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();