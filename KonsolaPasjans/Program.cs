using System;
using System.Text;

namespace KonsolaPasjans
{
	class Program
	{
		private static Tutorial[] tutorials;
		static void Main(string[] args)
		{
			Ranking.RankingManager.Load();
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
				int val = ContextMenu.SummonAt(0, 1, "Wybierz opcję przy użyciu strzałek góra, dół oraz zatwierź klawiszem Enter:", new string[] { "Nowa Gra", "Ranking", "Samouczek", "Wyjdź z gry" }, ConsoleColor.Yellow, true);
				switch (val)
				{
					case 0: // Nowa Gra
						StartNewGame();
						break;
					case 1:
						ShowRanking();
						break;
					case 2: // Samouczek
						TutorialMenu();
						break;
					case -1:
					case 3: // Wyjdź z gry
						Exit();
						break;
				}
			}
		}
		static private void ShowRanking()
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);

			Console.WriteLine("Ranking:");
			Console.WriteLine("Imię gracza                    Ruchy  Data       Tryb");
			Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
			foreach (var entry in Ranking.RankingManager.GetEntries())
			{
				string TrybGry = entry.HardMode ? "Trudny" : "Prosty";
				//Console.ForegroundColor = entry.HardMode ? ConsoleColor.Red : ConsoleColor.Green;
				Console.WriteLine($"{entry.PlayerName.PadRight(30)}  {entry.Moves.ToString().PadLeft(4)}  {entry.Date.ToShortDateString()} ({TrybGry})");
				//Console.WriteLine($"{entry.PlayerName.PadRight(30)}  {entry.Moves.ToString().PadLeft(4)} {entry.Date.ToShortDateString()}");
			}
			Console.WriteLine("━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━");
			Console.WriteLine("Naciśnij dowolny klawisz, aby wrócić do menu głównego.");
			Console.ReadKey(true); // Wait for the user to press a key before returning to the main menu
		}
		static private void StartNewGame()
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);
			Console.WriteLine("Wybierz opcję przy użyciu strzałek góra, dół oraz zatwierź klawiszem Enter:");
			int val = ContextMenu.SummonAt(0, 1, "Poziom trudności:", new string[] { "Prosty", "Trudny", "Powrót" }, ConsoleColor.Yellow, true);
			if (val == 2 || val == -1)
			{
				return; // Return to main menu
			}
			Game game = new Game();
			game.IsHardDifficulty = val == 1;
			game.Start();
		}
		static private void TutorialMenu()
		{
			tutorials = new Tutorial[]
			{
				new Tutorials.TutorialCursor(),
				new Tutorials.TutorialSingleCardMoves(),
				new Tutorials.TutorialMultipleCardMoves()
			};
			while (true)
			{
				Console.BackgroundColor = ConsoleColor.Black;
				Console.ForegroundColor = ConsoleColor.Gray;
				Console.SetWindowSize(80, 36);
				Console.Clear();
				Console.WriteLine("Menu Samouczków");
				string[] options = new string[tutorials.Length + 1];
				options[0] = "Powrót";
				for (int i = 1; i <= tutorials.Length; i++)
				{
					options[i] = (tutorials[i-1].IsCompleted ? "[X] " : "[ ] ") + tutorials[i-1].Name;
				}
				int val = ContextMenu.SummonAt(0, 1, "Wybierz opcję przy użyciu strzałek góra, dół oraz zatwierź klawiszem Enter:", options, ConsoleColor.Yellow, true);
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
						}
						break;
				}
			}
		}
		static private void Exit()
		{
			Console.Clear();
			Console.WriteLine("Dziękujemy za grę w Pasjansa!");
			Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć.");
			Console.ReadKey();
			Environment.Exit(0);
		}
	}
}
