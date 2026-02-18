namespace lab1;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Введите номер входного файла:");
        var inputStr = Console.ReadLine();
        var fileName = $"tasks/{inputStr}.txt";

        if (!File.Exists(fileName))
        {
            Console.WriteLine("Файл не найден.");
        }

        try
        {
            var data = FileManager.ReadMatrix(fileName);
            var solver = new JordanGaussSolver(data);

            solver.Solve();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Критическая ошибка: {ex.Message}");
        }
    }
}