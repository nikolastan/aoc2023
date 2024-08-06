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
}
