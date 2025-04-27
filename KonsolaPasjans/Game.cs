using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KonsolaPasjans
{
	// Game of Solitaire
	public class Game
    {
		private static string DECK_END_CARD =	"╔═════╗" +
												"║ ◣─╮ ║" +
												"║   │ ║" +
												"║ ╰─╯ ║" +
												"╚═════╝";
		private static string PILE_CARD_TYPE =	"╔═════╗" +
												"║     ║" +
												"║  !  ║" +
												"║     ║" +
												"╚═════╝";
		private List<Card> Deck { get; set; } = new List<Card>();
		private List<Card> DiscardPile { get; set; } = new List<Card>();
		private List<Card>[] cards = new List<Card>[7];
		public bool IsHardDifficulty = false;
		//public List<Card>

		public Game() { }
		private void InitializeDeck()
		{
			foreach (CardColor color in Enum.GetValues(typeof(CardColor)))
			{
				for (int i = 1; i <= 13; i++)
				{
					Deck.Add(new Card((CardValue)i, color));
				}
			}
			ShuffleDeck();
			// Distributing cards to columns
			for (int i = 0; i < 7; i++)
			{
				cards[i] = new List<Card>();
				for (int j = 0; j <= i; j++)
				{
					cards[i].Add(Deck[0]);
					Deck.RemoveAt(0);
				}
				cards[i][i].IsFaceUp = true;
			}
		}
		private void ShuffleDeck()
		{
			//Shuffling the deck
			Random rand = new Random();
			Deck = Deck.OrderBy(x => rand.Next()).ToList();
		}
		public void Start()
		{
			InitializeDeck();
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			// Add game logic here
			Display();
		}
		public void Display()
		{
			Console.Clear();
			CardColor[] colors = (CardColor[])Enum.GetValues(typeof(CardColor));
			int x = 0, y = 9 * 3;
			for (int i = 0; i < colors.Length; i++)
			{
				Console.SetCursorPosition(x, y + i*8);
			}
			for (int i = 0; i < cards.Length; i++)
			{
				DisplayCardColumn(i);
			}
		}
		public void DisplayCardColumn(int column)
		{
			int x = 9 * column, y = 5;
			if (column < 0 || column > cards.Length) throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and cards.Length");
			foreach (var card in cards[column])
			{
				card.Display(x, y);
				y += 2;
			}
		}
	}
}
