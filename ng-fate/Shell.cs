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
        }

        public static void WriteKey(object? value)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(value);
        }

        public static void WriteHeading()
        {
            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine(Constants.MESSAGE_TITLE);
            Console.ResetColor();
            Console.WriteLine(Constants.MESSAGE_PUNCH_LINE);
            Console.WriteLine();
        }

        public static void Write(object? value)
        {
            Console.Write(value);
        }

        public static void WriteLine(object? value)
        {
            Console.WriteLine(value);
        }

        public static void Error(object? value)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(value);
        }

        public static void EmptyLine()
        {
            Console.WriteLine();
        }

        public static string? ReadLine()
        {
            return Console.ReadLine();
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
