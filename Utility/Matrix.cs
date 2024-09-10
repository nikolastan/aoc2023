namespace Utility;

public static class Matrix
{
    public static T[][] Transpose<T>(T[][] matrix)
    {
        var transposedMatrix = new T[matrix[0].Length][];

        for (int i = 0; i < matrix[0].Length; i++)
            transposedMatrix[i] = new T[matrix.Length];

        for (int i = 0; i < matrix.Length; i++)
        {
            for (int j = 0; j < matrix[0].Length; j++)
                transposedMatrix[j][i] = matrix[i][j];
        }


        return transposedMatrix;
    }

    public interface IClonable<T>
    {
        T Clone();
    }

    public static T[,] Copy<T>(this T[,] matrix) 
    {
        var matLen0 = matrix.GetLength(0);
        var matLen1 = matrix.GetLength(1);

		var copy = new T[matLen0, matLen1];

        for(int i = 0; i<matLen0; i++)
            for(int j = 0; j<matLen1; j++)
                copy[i,j] = matrix[i, j];

        return copy;
    }
}
