using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolaPasjans
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.ResetColor();
            Console.Clear();
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
