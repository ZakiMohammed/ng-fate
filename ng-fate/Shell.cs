namespace ng_fate
{
    static class Shell
    {
        public static void WriteKeyValue(object? key, object? value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{key}: ");
            Console.ResetColor();
            Console.Write(value);
            Console.WriteLine();

            Console.ResetColor();
        }

        public static void WriteKey(object? value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{value}:");

            Console.ResetColor();
        }

        public static void WriteHeading()
        {
            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------");

            Console.ForegroundColor = ConsoleColor.Cyan;

            Console.WriteLine(Constants.MESSAGE_TITLE);
            Console.ResetColor();

            Console.WriteLine($"\n{Constants.MESSAGE_PUNCH_LINE}");
            Console.WriteLine($"\n{Constants.MESSAGE_GITHUB_LINE}");

            Console.WriteLine("-----------------------------------------------------------------------------------------------------------------");
            Console.WriteLine();
        }

        public static void Write(object? value)
        {
            Console.Write(value);
        }

        public static void WriteLine(object? value, ConsoleColor? color = null)
        {
            if (color != null)
                Console.ForegroundColor = color.Value;

            Console.WriteLine(value);

            if (color != null)
                Console.ResetColor();
        }

        public static void Log(object? value)
        {
            Console.Write($"[{DateTime.Now:hh:mm:ss:fff} ");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"LOG");
            Console.ResetColor();
            Console.WriteLine($"] {value}");
        }

        public static void Error(object? value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(value);

            Console.ResetColor();
        }

        public static void Finish()
        {
            Shell.SetForegroundColor(ConsoleColor.Green);
            Shell.WriteLine(Constants.MESSAGE_FINISH);
            Shell.ResetColor();
        }

        public static void EmptyLine()
        {
            Console.WriteLine();
        }

        public static string? ReadLine()
        {
            return Console.ReadLine();
        }

        public static string? AcceptLine(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Write(message);
            ResetColor();
            return ReadLine();
        }

        public static void Clear()
        {
            Console.Clear();
        }

        public static void ResetColor()
        {
            Console.ResetColor();
        }

        public static void SetForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }
    }
}
