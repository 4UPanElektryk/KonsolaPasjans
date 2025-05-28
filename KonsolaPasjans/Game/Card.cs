using System;

namespace KonsolaPasjans
{
	public class Card
	{
		public CardValue Value { get; set; }
		public CardColor Color { get; set; }
		public bool IsFaceUp { get; set; } = false;
		public bool IsSelected { get; set; } = false;
		public Card(CardValue value, CardColor color)
		{
			Value = value;
			Color = color;
		}
		public Card(Card card)
		{
			this.Value = card.Value;
			this.Color = card.Color;
			this.IsFaceUp = card.IsFaceUp;
			this.IsSelected = card.IsSelected;
		}
		public override string ToString()
		{
			return $"{Value} of {Color}";
		}
		public virtual void Display(int x, int y)
		{
			char color = ColorToChar(Color);
			string val = !((int)Value == 1 || (int)Value > 10) ? ((int)Value).ToString() : (Value.ToString()[0]).ToString();
			ConsoleColor cardColor = (int)Color % 2 == 0 ? ConsoleColor.Red : ConsoleColor.Black;
			Console.ForegroundColor = IsFaceUp ? cardColor : ConsoleColor.Gray;
			if (IsSelected) Console.ForegroundColor = ConsoleColor.Yellow;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			if (IsFaceUp)
			{

				Console.SetCursorPosition(x, y++); Console.Write($"╔═════╗");
				Console.SetCursorPosition(x, y++); Console.Write($"║ {val+color,-3} ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║     ║");
				Console.SetCursorPosition(x, y++); Console.Write($"║ {color+val,3} ║");
				Console.SetCursorPosition(x, y++); Console.Write($"╚═════╝");
			}
			else
			{
				Console.SetCursorPosition(x, y++); Console.Write("╔═════╗");
				Console.SetCursorPosition(x, y++); Console.Write("║ XXX ║");
				Console.SetCursorPosition(x, y++); Console.Write("║ XXX ║");
				Console.SetCursorPosition(x, y++); Console.Write("║ XXX ║");
				Console.SetCursorPosition(x, y++); Console.Write("╚═════╝");
			}
		}
		protected static char ColorToChar(CardColor color) { 
			switch(color)
			{
				case CardColor.Hearts:
					return '♥';
				case CardColor.Diamonds:
					return '♦';
				case CardColor.Clubs:
					return '♣';
				case CardColor.Spades:
					return '♠';
				case CardColor.None:
					return ' ';
				default:
					throw new ArgumentOutOfRangeException(nameof(color), color, null);
			}
		}
		public virtual bool IsValidTarget(Card card)
		{
			bool isLocalRed = (int)this.Color % 2 == 0;
			bool isTargetRed = (int)card.Color % 2 == 0;
			if (isLocalRed != isTargetRed)
			{
				if (this.Value == (card.Value+1))
				{
					return true;
				}
				return false;
			}
			return false;
		}
	}
}
