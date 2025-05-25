using System;

namespace KonsolaPasjans
{
    public class ContextMenu
    {
        public static int SummonAt(int x, int y,string Title, string[] options, ConsoleColor color = ConsoleColor.White)
        {
			int max = Title.Length;
			foreach (var item in options)
			{
				max = item.Length > max ? item.Length : max;
			}
            int selectedOption = 0;
			while (true)
            {
				Console.SetCursorPosition(x, y);
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.Write(Title);
				for (int i = 0; i < options.Length; i++)
				{
					Console.SetCursorPosition(x, y + i + 1);
					if (i == selectedOption)
					{
						Console.ForegroundColor = color;
						Console.Write($"> {options[i].PadLeft(max)}");
					}
					else
					{
						Console.ForegroundColor = ConsoleColor.Gray;
						Console.Write($"  {options[i].PadLeft(max)}");
					}
				}
				ConsoleKeyInfo key = Console.ReadKey(true);
				/*if (key.Key == ConsoleKey.Escape)
				{
					return -1;
				}*/
				if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
				{
					return selectedOption;
				}
				if (key.Key == ConsoleKey.UpArrow)
				{
					selectedOption--;
					if (selectedOption < 0)
					{
						selectedOption = options.Length - 1;
					}
				}
				if (key.Key == ConsoleKey.DownArrow)
				{
					selectedOption++;
					if (selectedOption >= options.Length)
					{
						selectedOption = 0;
					}
				}
				if (key.Key == ConsoleKey.End)
				{
					selectedOption = options.Length - 1;
				}
				if (key.Key == ConsoleKey.Home)
				{
					selectedOption = 0;
				}
			}
        }
    }
}
