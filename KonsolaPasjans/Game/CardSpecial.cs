using System;

namespace KonsolaPasjans
{
    public class CardSpecial : Card
    {
		/// <summary>
		/// Special card constructor
		/// </summary>
		/// <param name="color"></param>
		/// <param name="IsDeckEndCard"></param>
		public CardSpecial(CardColor color, bool IsDeckEndCard) : base(CardValue.None, color)
		{
			this.IsFaceUp = IsDeckEndCard;
		}
		public override void Display(int x, int y)
		{
			Console.ForegroundColor = IsSelected ? ConsoleColor.Yellow : ConsoleColor.Gray;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			char color = ColorToChar(Color);
			if (this.IsFaceUp)
			{
				Console.SetCursorPosition(x, y++); Console.Write($"╔═════╗");
				Console.SetCursorPosition(x, y++); Console.Write($"║ ▼─╮ ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║   │ ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║ ╰─╯ ║");
				Console.SetCursorPosition(x, y++); Console.Write($"╚═════╝");
			}
			else
			{
				ConsoleColor cardColor = (int)Color % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Black;
				if (Color == CardColor.None) { cardColor = ConsoleColor.White; }
				Console.ForegroundColor = cardColor;
				if (this.IsSelected)
				{
					Console.ForegroundColor = ConsoleColor.Yellow;
				}
				Console.SetCursorPosition(x, y++); Console.Write($"╔═════╗");
				Console.SetCursorPosition(x, y++); Console.Write($"║     ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║  {color}  ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║     ║");
				Console.SetCursorPosition(x, y++); Console.Write($"╚═════╝");
			}
		}
		public override bool IsValidTarget(Card card)
		{
			if (this.IsFaceUp)
			{
				return false;
			}

			if (card.Color == this.Color && card.Value == CardValue.Ace)
			{
				return true;
			}
			return false;
		}
	}
}
