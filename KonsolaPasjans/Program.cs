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
			Console.Clear();
			Console.SetWindowSize(64,32);
			Game game = new Game();
			Console.WriteLine("Welcome to Solitaire!");
			Console.WriteLine("Press any key to start...");
			Console.ReadKey();
			int val = ContextMenu.SummonAt(0,2, "Choose Difficulty", new string[] { "Easy", "Hard" }, ConsoleColor.Yellow);
			game.IsHardDifficulty = val == 1;
			game.Start();
			Console.ReadKey();
		}
	}
}
