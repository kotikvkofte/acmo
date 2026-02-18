namespace lab1;

public static class FileManager
{
    public static Fraction[,] ReadMatrix(string filePath)
    {
        var lines = File.ReadAllLines(filePath)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToArray();

        var rows = lines.Length;
        var cols = lines[0].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;

        var matrix = new Fraction[rows, cols];

        for (var i = 0; i < rows; i++)
        {
            var parts = lines[i].Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            for (var j = 0; j < cols; j++)
            {
                matrix[i, j] = new Fraction(long.Parse(parts[j]));
            }
        }

        return matrix;
    }
}