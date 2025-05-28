using System;
using System.Text;

namespace KonsolaPasjans
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.UTF8;
			Console.ResetColor();
			while (true)
			{
				Console.CursorVisible = false;
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.SetWindowSize(80, 36);
				Console.Clear();
				Console.WriteLine("Witaj w Pasjansie!");
				Console.WriteLine("Wybierasz przy użyciu strzałek góra, dół i klawisza Enter");
				int val = ContextMenu.SummonAt(0, 2, "Wybierz opcję:", new string[] { "Nowa Gra", "Samouczek", "Wyjdź z gry" }, ConsoleColor.Yellow, true);
				switch (val)
				{
					case 0: // Nowa Gra
						StartNewGame();
						break;
					case 1: // Samouczek
						TutorialMenu();
						break;
					case -1:
					case 2: // Wyjdź z gry
						Exit();
						break;
				}
			}
		}
		static private void StartNewGame()
		{
			Console.Clear();
			int val = ContextMenu.SummonAt(0, 2, "Wybierz poziom trudności:", new string[] { "Prosty", "Trudny", "Powrót" }, ConsoleColor.Yellow, true);
			if (val == 2 || val == -1)
			{
				return; // Return to main menu
			}
			Game game = new Game();
			game.IsHardDifficulty = val == 1;
			game.Start();
		}
		static private void Exit()
		{
			Console.Clear();
			Console.WriteLine("Dziękujemy za grę w Pasjansa!");
			Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć.");
			Console.ReadKey();
			Environment.Exit(0);
		}
		static private void TutorialMenu()
		{
			Tutorial[] tutorials = new Tutorial[]
			{
				new Tutorials.TutorialCursor(),
				new Tutorials.TutorialSingleCardMoves(),
			};
			while (true)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.SetWindowSize(80, 36);
				Console.Clear();
				Console.WriteLine("Menu Samouczków");
				Console.WriteLine("Wybierasz przy użyciu strzałek góra, dół i klawisza Enter");

				string[] options = new string[tutorials.Length + 1];
				options[0] = "Powrót";
				for (int i = 1; i <= tutorials.Length; i++)
				{
					options[i] = (tutorials[i-1].IsCompleted ? "[X] " : "[ ] ") + tutorials[i-1].Name;
				}
				int val = ContextMenu.SummonAt(0, 2, "Wybierz samouczek:", options, ConsoleColor.Yellow, true);
				switch (val)
				{
					case -1:
					case 0:
						return;
					default:
						tutorials[val - 1].Start();
						if (tutorials[val-1].IsCompleted)
						{
							Console.ForegroundColor = ConsoleColor.White;
							Console.BackgroundColor = ConsoleColor.Black;
							Console.Clear();
							Console.WriteLine("Samouczek ukończony. Naciśnij dowolny klawisz, aby kontynuować.");
							Console.ReadKey();
						}
						break;
				}
			}
		}
	}
}
