using System.Diagnostics;

char[][] ReadEngineMatrixFromInput(string filePath)
{
	List<char[]> result = [];

	using var reader = new StreamReader(filePath);

	while(!reader.EndOfStream)
	{
		var line = reader.ReadLine();

		if(line is not null)
		{
			//Add first row of padding
			if(result.Count is 0)
			{
				var paddingRow = FormPaddingRow(line.Length + 2);
				result.Add(paddingRow);
			}

			//Adding dots to the beginning and end of read line
			result.Add(['.', ..line, '.']);

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

bool AnySymbolsIn(in char[][] matrix, int iPos, int jPos)
{
	for (int i = iPos - 1; i < iPos + 2; i++)
	{
		for (int j = jPos - 1; j < jPos + 2; j++)
		{
			if (!char.IsDigit(matrix[i][j]) && matrix[i][j] is not '.')
				return true;
		}
	}

	return false;
}

int GetSumOfAllParts(in char[][] engineMatrix)
{
	int sum = 0;

	for (int i = 1; i < engineMatrix.Length - 1; i++)
	{
		for (int j = 1; j < engineMatrix[i].Length - 1; j++)
		{
			var number = "";
			var foundPart = false;

			//Getting the whole number
			while (char.IsNumber(engineMatrix[i][j]))
			{

				number += engineMatrix[i][j];
				if (!foundPart && AnySymbolsIn(in engineMatrix, i, j))
				{
					foundPart = true;
				}

				j++;
			}

			if (foundPart)
			{
				sum += int.Parse(number);
			}
		}
	}

	return sum;
}

void Solve()
{
	var sw = Stopwatch.StartNew();

	var engineMatrix = ReadEngineMatrixFromInput("input.txt");

	var sum = GetSumOfAllParts(in engineMatrix);

	sw.Stop();

	Console.WriteLine($"Result: {sum}, Time: {sw.ElapsedMilliseconds}ms");
}

Solve();