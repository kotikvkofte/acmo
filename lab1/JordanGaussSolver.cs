using System.Text;

namespace lab1;

public class JordanGaussSolver
{
    private readonly Fraction[,] _matrix;
    private readonly Fraction[,] _originalMatrix;
    private readonly int _rows;
    private readonly int _cols;
    private readonly bool _silentMode;
    
    public JordanGaussSolver(Fraction[,] matrix, bool silentMode = false)
    {
        _rows = matrix.GetLength(0);
        _cols = matrix.GetLength(1);
        _silentMode = silentMode;
        
        _matrix = new Fraction[_rows, _cols];
        _originalMatrix = new Fraction[_rows, _cols];
        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _cols; j++)
            {
                _matrix[i, j] = matrix[i, j];
                _originalMatrix[i, j] = matrix[i, j];
            }
        }
    }

    public void Solve()
    {
        var pivotRow = 0;
        var variableCount = _cols - 1;

        if (!_silentMode)
        {
            Console.WriteLine($"\n--- Исходная матрица ---");
            PrintMatrix();
        }

        for (var col = 0; col < variableCount && pivotRow < _rows; col++)
        {
            var maxRow = FindMaxRowInColumn(col, pivotRow);
            var maxVal = _matrix[maxRow, col].Abs;

            if (maxVal.IsZero)
                continue;

            if (maxRow != pivotRow)
            {
                SwapRows(pivotRow, maxRow);
                if (!_silentMode)
                {
                    Console.WriteLine($"\n--- Перестановка строк {pivotRow + 1} и {maxRow + 1} ---");
                    PrintMatrix();
                }
            }

            var pivotElement = _matrix[pivotRow, col];
            DividePivotRow(pivotRow, pivotElement);
            
            if (!_silentMode)
            {
                Console.WriteLine($"\n--- Делим строку на главный элемент ---");
                PrintMatrix();
            }

            CalculateColumn(col, pivotRow);

            if (!_silentMode)
            {
                Console.WriteLine($"\n--- Результат после обработки x{col + 1} ---");
                PrintMatrix();
            }

            pivotRow++;
        }
        
        if (!_silentMode)
        {
            AnalyzeResults(variableCount);
        }
    }
    
    private int FindMaxRowInColumn(int col, int startRow)
    {
        var maxRow = startRow;
        var maxVal = _matrix[startRow, col].Abs;
        for (var i = startRow + 1; i < _rows; i++)
        {
            if (_matrix[i, col].Abs < maxVal || _matrix[i, col].Abs == maxVal) continue;
            maxVal = _matrix[i, col].Abs; maxRow = i;
        }
        return maxRow;
    }
    private void DividePivotRow(int row, Fraction pivot) { for (var j = 0; j < _cols; j++) _matrix[row, j] /= pivot; }
    private void CalculateColumn(int pivotCol, int pivotRow)
    {
        for (var i = 0; i < _rows; i++)
        {
            if (i == pivotRow) continue;
            var ik = _matrix[i, pivotCol];
            if (ik.IsZero) continue;
            for (var j = pivotCol; j < _cols; j++)
            {
                _matrix[i, j] -= (ik * _matrix[pivotRow, j]);
            }
            _matrix[i, pivotCol] = new Fraction(0);
        }
    }
    private void SwapRows(int row1, int row2) { for (var j = 0; j < _cols; j++) (_matrix[row1, j], _matrix[row2, j]) = (_matrix[row2, j], _matrix[row1, j]); }

    private void AnalyzeResults(int variableCount)
    {
        var hasNoSolution = false;
        var rank = 0;
        var isBasicVariable = new bool[variableCount];
        
        for (var i = 0; i < _rows; i++)
        {
            var isRowAllZeros = true;
            var pivotIndex = -1;

            for (var j = 0; j < variableCount; j++)
            {
                if (_matrix[i, j].IsZero) continue;
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
                if (!_matrix[i, _cols - 1].IsZero) hasNoSolution = true;
            }
        }

        Console.WriteLine("\n===============================================================");
        if (hasNoSolution)
        {
            Console.WriteLine("Система не имеет решений.");
        }
        else if (rank < variableCount)
        {
            Console.WriteLine("Система имеет множество решений.");
            Console.WriteLine("Одно из базисных решений (текущее):");
            ComputeAndPrintBasicSolution(variableCount, isBasicVariable);
            
            FindAllBasicSolutions(rank, variableCount);
        }
        else
        {
            Console.WriteLine("Система имеет единственное решение:");
            for (var i = 0; i < variableCount; i++)
            {
                 if (i < _rows) Console.WriteLine($"x{i + 1} = {_matrix[i, _cols - 1]}");
            }
        }
    }

    private static List<int[]> GetCombinations(int n, int k)
    {
        var result = new List<int[]>();
        var combination = new int[k];
        Generate(0, 0);
        return result;

        void Generate(int index, int start)
        {
            if (index == k)
            {
                result.Add((int[])combination.Clone());
                return;
            }
            for (var i = start; i < n; i++)
            {
                combination[index] = i;
                Generate(index + 1, i + 1);
            }
        }
    }

    private void FindAllBasicSolutions(int rank, int variableCount)
    {
        Console.WriteLine($"\n--- Поиск ВСЕХ базисных решений (Ранг = {rank}) ---");
        
        var combinations = GetCombinations(variableCount, rank);
        var count = 0;

        foreach (var combo in combinations)
        {
            var subMatrixData = new Fraction[_rows, rank + 1];

            for (var i = 0; i < _rows; i++)
            {
                for (var k = 0; k < rank; k++)
                {
                    subMatrixData[i, k] = _originalMatrix[i, combo[k]];
                }
                subMatrixData[i, rank] = _originalMatrix[i, _cols - 1];
            }

            var subSolver = new JordanGaussSolver(subMatrixData, silentMode: true);
            subSolver.Solve();
            
            if (!subSolver.IsValidBasis(rank)) continue;
            count++;
            PrintSpecificBasicSolution(subSolver._matrix, combo, variableCount);
        }
        
        if (count == 0) Console.WriteLine("Дополнительных базисных решений не найдено.");
    }

    private bool IsValidBasis(int expectedRank)
    {
        var actualRank = 0;
        var subVars = _cols - 1;
        
        for (var i = 0; i < _rows; i++)
        {
            var rowHasPivot = false;
            for (var j = 0; j < subVars; j++)
            {
                if (_matrix[i, j].IsZero) continue;
                rowHasPivot = true; 
                break;
            }
            
            if (rowHasPivot) actualRank++;
            else if (!_matrix[i, _cols - 1].IsZero) return false;
        }

        return actualRank == expectedRank;
    }

    private static void PrintSpecificBasicSolution(Fraction[,] solvedSubMatrix, int[] basisIndices, int totalVars)
    {
        var resultStr = new StringBuilder("X(");
        var resultValues = new string[totalVars];

        for (var i = 0; i < totalVars; i++) resultValues[i] = "0";

        for (var k = 0; k < basisIndices.Length; k++)
        {
            var originalIndex = basisIndices[k];
            
            var val = new Fraction(0);
            for(var r = 0; r < solvedSubMatrix.GetLength(0); r++)
            {
                if (solvedSubMatrix[r, k].IsZero) continue;
                val = solvedSubMatrix[r, solvedSubMatrix.GetLength(1) - 1];
                break;
            }
            resultValues[originalIndex] = val.ToString();
        }

        resultStr.Append(string.Join(", ", resultValues));
        resultStr.Append(')');
        
        Console.Write($"Базис [{string.Join(" ", basisIndices.Select(x=>"x"+(x+1)))}]: ");
        Console.WriteLine(resultStr);
    }

    private void ComputeAndPrintBasicSolution(int variableCount, bool[] isBasicVariable)
    {
        var res = new StringBuilder("X(");
        for (var j = 0; j < variableCount; j++)
        {
            if (!isBasicVariable[j]) res.Append('0');
            else
            {
                var value = _matrix[0, _cols - 1];
                var found = false;
                for (var i = 0; i < _rows; i++)
                {
                    if (_matrix[i, j].IsZero) continue;
                    value = _matrix[i, _cols - 1];
                    found = true;
                    break;
                }
                res.Append(found ? value : new Fraction(0));
            }
            if (j < variableCount - 1) res.Append(", ");
        }
        res.Append(')');
        Console.WriteLine(res.ToString());
    }

    private void PrintMatrix()
    {
        for (var i = 0; i < _rows; i++)
        {
            for (var j = 0; j < _cols; j++)
            {
                if (j == _cols - 1) Console.Write("| ");
                Console.Write($"{_matrix[i, j],10}\t");
            }
            Console.WriteLine();
        }
    }
}