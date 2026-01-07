namespace SharePointListsGraphApi.Helpers
{
    internal static class MenuHelper
    {
        public static void DisplayMainMenu()
        {
            Console.Clear();
            Console.WriteLine("+============================================================+");
            Console.WriteLine("|          SharePoint List Management Tool                   |");
            Console.WriteLine("+============================================================+");
            Console.WriteLine();
            Console.WriteLine("  1. Add Columns from CSV File");
            Console.WriteLine("  2. Generate Random Test Data");
            Console.WriteLine("  3. Export List to JSON File");
            Console.WriteLine("  4. Display Access Token");
            Console.WriteLine("  5. Exit");
            Console.WriteLine();
            Console.Write("Select an option (1-5): ");
        }

        public static int GetMenuChoice()
        {
            while (true)
            {
                var input = Console.ReadLine();
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= 5)
                {
                    return choice;
                }
                Console.Write("Invalid choice. Please enter a number between 1 and 5: ");
            }
        }

        public static string GetFilePathInput(string prompt)
        {
            Console.Write(prompt);
            var path = Console.ReadLine() ?? string.Empty;
            return path.Trim().Trim('"');
        }

        public static int GetNumberInput(string prompt, int min = 1, int max = 10000)
        {
            while (true)
            {
                Console.Write(prompt);
                var input = Console.ReadLine();
                if (int.TryParse(input, out int number) && number >= min && number <= max)
                {
                    return number;
                }
                Console.WriteLine($"Invalid input. Please enter a number between {min} and {max}.");
            }
        }

        public static void PressKeyToContinue()
        {
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey(true);
        }

        public static void DisplayHeader(string title)
        {
            Console.WriteLine();
            Console.WriteLine("============================================================");
            Console.WriteLine($"  {title}");
            Console.WriteLine("============================================================");
            Console.WriteLine();
        }

        public static void DisplaySuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"✓ {message}");
            Console.ResetColor();
        }

        public static void DisplayError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"✗ {message}");
            Console.ResetColor();
        }

        public static void DisplayInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ℹ {message}");
            Console.ResetColor();
        }
    }
}
