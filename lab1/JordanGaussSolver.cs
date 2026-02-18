namespace lab1;

public class JordanGaussSolver(Fraction[,] matrix)
{
    private readonly int rows = matrix.GetLength(0);
    private readonly int cols = matrix.GetLength(1);

    public void Solve()
    {
        var pivotRow = 0;
        var variableCount = cols - 1;

        Console.WriteLine($"\n--- Исходная матрица ---");
        PrintMatrix();

        for (var col = 0; col < variableCount && pivotRow < rows; col++)
        {
            var maxRow = FindMaxRowInColumn(col, pivotRow);
            var maxVal = matrix[maxRow, col].Abs;

            if (maxVal.IsZero)
                continue;

            if (maxRow != pivotRow)
            {
                SwapRows(pivotRow, maxRow);
                Console.WriteLine($"\n--- Перестановка строк {pivotRow + 1} и {maxRow + 1} ---");
                PrintMatrix();
            }

            var pivotElement = matrix[pivotRow, col];
            DividePivotRow(pivotRow, pivotElement);
            Console.WriteLine($"\n--- Делим строку на главный элемент ---");
            PrintMatrix();

            CalculateColumn(col, pivotRow);

            Console.WriteLine($"\n--- Результат после обработки x{col + 1} ---");
            PrintMatrix();

            pivotRow++;
        }

        AnalyzeResults(variableCount);
    }

    /// <summary>
    /// Поиск строки с ведущим элементов
    /// </summary>
    /// <param name="col">Колонка для поиска ведущего элемента</param>
    /// <param name="startRow">Строка, с которой начинается поиск</param>
    /// <returns>Индекс строки с ведущим элементом</returns>
    private int FindMaxRowInColumn(int col, int startRow)
    {
        var maxRow = startRow;
        var maxVal = matrix[startRow, col].Abs;

        for (var i = startRow + 1; i < rows; i++)
        {
            if (matrix[i, col].Abs > maxVal)
            {
                maxVal = matrix[i, col].Abs;
                maxRow = i;
            }
        }

        return maxRow;
    }

    /// <summary>
    /// Деление строки на ведущий элемент
    /// </summary>
    /// <param name="row">ИНдекс строки</param>
    /// <param name="pivot">Ведущий элемент</param>
    private void DividePivotRow(int row, Fraction pivot)
    {
        for (var j = 0; j < cols; j++)
        {
            matrix[row, j] /= pivot;
        }
    }

    /// <summary>
    /// Расчет значений методом прямоугольника(учитывая то, что у ведущей строки ведущий элемент равен 1)
    /// ik - элемент в том же столбце, что и ведущий элемент.
    /// ij - пересчитываемый элемент.
    /// kj - элемент в той же строке, что и ведущий элемент.
    /// </summary>
    /// <remarks>
    /// New = Old - (CornerCol * CornerRow)
    /// (знаменатель равен 1, т.к. мы заранее нормировали ведущую строку)
    /// </remarks>
    /// <param name="pivotCol">Номер ведущей колонки</param>
    /// <param name="pivotRow">Номер ведущей строки</param>
    private void CalculateColumn(int pivotCol, int pivotRow)
    {
        for (var i = 0; i < rows; i++)
        {
            if (i == pivotRow)
                continue;

            var ik = matrix[i, pivotCol];
            if (ik.IsZero)
                continue;

            for (var j = pivotCol; j < cols; j++)
            {
                var ij = matrix[i, j];
                var kj = matrix[pivotRow, j];
                matrix[i, j] = ij - (ik * kj);
            }

            // обнуление элемента в столбце главного элемента
            matrix[i, pivotCol] = new Fraction(0);
        }
    }

    /// <summary>
    /// Ответ в зависимости от ранга матрицы
    /// </summary>
    /// <param name="variableCount"></param>
    private void AnalyzeResults(int variableCount)
    {
        var hasNoSolution = false;
        var rank = 0;
        var isBasicVariable = new bool[variableCount];
        //ранг матрицы
        for (var i = 0; i < rows; i++)
        {
            var isRowAllZeros = true;
            var pivotIndex = -1;

            for (var j = 0; j < variableCount; j++)
            {
                if (matrix[i, j].IsZero) continue;
                if (pivotIndex == -1) pivotIndex = j;
                isRowAllZeros = false;
            }

            if (!isRowAllZeros)
            {
                rank++;
                if (pivotIndex != -1) isBasicVariable[pivotIndex] = true;
            }
            else
            {
                if (!matrix[i, cols - 1].IsZero)
                {
                    hasNoSolution = true;
                }
            }
        }

        Console.WriteLine("\n===============================================================");
        if (hasNoSolution)
        {
            Console.WriteLine("Система не имеет решений).");
        }
        else if (rank < variableCount)
        {
            Console.WriteLine("Система имеет множество решений.");
            PrintGeneralSolution(variableCount, isBasicVariable);
        }
        else
        {
            Console.WriteLine("Система имеет единственное решение:");
            for (var i = 0; i < variableCount; i++)
            {
                if (i < rows)
                    Console.WriteLine($"x{i + 1} = {matrix[i, cols - 1]}");
            }
        }
    }

    /// <summary>
    /// Перестановка строк местами
    /// </summary>
    /// <param name="row1">Строка 1</param>
    /// <param name="row2">Строка 2</param>
    private void SwapRows(int row1, int row2)
    {
        for (var j = 0; j < cols; j++)
        {
            (matrix[row1, j], matrix[row2, j]) = (matrix[row2, j], matrix[row1, j]);
        }
    }

    /// <summary>
    /// Вывод матрицы в консоль
    /// </summary>
    private void PrintMatrix()
    {
        for (var i = 0; i < rows; i++)
        {
            for (var j = 0; j < cols; j++)
            {
                if (j == cols - 1) Console.Write("| ");
                Console.Write($"{matrix[i, j],10}\t");
            }

            Console.WriteLine();
        }
    }

    private void PrintGeneralSolution(int variableCount, bool[] isBasicVariable)
    {
        for (var i = 0; i < rows; i++)
        {
            var pivotCol = -1;
            for (var j = 0; j < variableCount; j++)
            {
                if (matrix[i, j].IsZero) continue;
                pivotCol = j;
                break;
            }

            if (pivotCol == -1) continue;

            Console.Write($"x{pivotCol + 1} = ");

            var constantTerm = matrix[i, cols - 1];
            var termPrinted = false;

            if (!constantTerm.IsZero)
            {
                Console.Write($"{constantTerm} ");
                termPrinted = true;
            }

            for (var j = pivotCol + 1; j < variableCount; j++)
            {
                var coeff = matrix[i, j];

                if (coeff.IsZero) continue;
                var movedCoeff = new Fraction(-coeff.Numerator, coeff.Denominator);

                if (movedCoeff.Numerator > 0)
                {
                    Console.Write(termPrinted ? "+ " : "");
                }
                else
                {
                    Console.Write("- ");
                    movedCoeff = movedCoeff.Abs;
                }

                Console.Write(movedCoeff.Numerator == movedCoeff.Denominator
                    ? $"x{j + 1} "
                    : $"{movedCoeff}*x{j + 1} ");

                termPrinted = true;
            }

            if (!termPrinted)
            {
                Console.Write("0");
            }

            Console.WriteLine();
        }
    }
}