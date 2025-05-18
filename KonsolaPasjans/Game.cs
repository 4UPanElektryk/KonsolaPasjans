using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KonsolaPasjans
{
	// Game of Solitaire
	public class Game
    {
		private List<Card> Deck { get; set; } = new List<Card>();
		private List<Card> DiscardPile { get; set; } = new List<Card>();
		private List<Card>[] cards = new List<Card>[7];

		private Card[] CardsSelected { get
			{
				if (previousCardSelection == 0)
				{
					//Deck
					return new Card[] { Deck[0] };
				}
				else if (previousCardSelection == 1)
				{
					// Discard pile
					return new Card[] { DiscardPile.Last() };
				}
				else if (previousCardSelection > 1 && previousCardSelection < 6)
				{
					// Foundation (Only 1 card)
					return new Card[] { new Card(foundation[previousCardSelection - 2], (CardColor)(previousCardSelection - 2)) };
				}
				else if (previousCardSelection > 5)
				{
					// Column
					List<Card> Source = cards[previousCardSelection - 6];
					return Source.Skip(Source.Count - previousCardCount).ToArray();
				}
				return null; 
			} }
		private ManagedMoveHistory history = new ManagedMoveHistory(3);
		private CardValue[] foundation = new CardValue[4];
		public bool IsHardDifficulty = false;
		private int selected = 0;
		private int previousCardSelection = -1;
		private int previousCardCount = 0;
		private bool isDeckSelected { get { return selected == 0; } }
		private bool isDiscardSelected { get { return selected == 1; } }
		private bool areCardsSelected { get { return selected > 5;  } }
		private bool isCardBeingMoved { get { return previousCardSelection != -1; } }

		public Game() { }
		public void Start()
		{
			InitializeDeck();
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Display();
			while (true)
			{
				#region Display Logic
				for (int i = 0; i < cards.Length; i++)
				{
					if (cards[i].Count > 0)
					{
						cards[i][cards[i].Count - 1].IsSelected = false;
					}
				}
				if (selected > 5)
				{
					cards[selected - 6][cards[selected - 6].Count - 1].IsSelected = true;
				}
				Display();
				#endregion
				#region User Input
				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.LeftArrow)
				{
					selected--;
					if (selected < 0)
					{
						selected = 12;
					}
					if (DiscardPile.Count == 0 && selected == 1)
					{
						selected--;
					}
				}
				else if (key.Key == ConsoleKey.RightArrow)
				{
					selected++;
					if (selected > 12)
					{
						selected = 0;
					}
					if (DiscardPile.Count == 0 && selected == 1)
					{
						selected++;
					}
				}
				else if (key.Key == ConsoleKey.UpArrow)
				{
					selected -= 7;
					if (selected < 0)
					{
						selected +=12;
					}
					if (DiscardPile.Count == 0 && selected == 1)
					{
						selected--;
					}
				}
				else if (key.Key == ConsoleKey.DownArrow)
				{
					selected += 7;
					if (selected > 12)
					{
						selected -= 12;
					}
					if (DiscardPile.Count == 0 && selected == 1)
					{
						selected++;
					}
				}
				else if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.Enter)
				{
					Debug.WriteLine($"Selected {selected}");
					if (selected > 5)
					{
						cards[selected - 6][cards[selected - 6].Count - 1].IsSelected = false;
					}
					RunAction();
				}
				#endregion
			}
		}
		private void DoMove(Move move)
		{
			Card[] movedcards = null;
			if (move.From == 0)
			{
				//Deck
				movedcards = new Card[] { Deck[0] };
				Deck.RemoveAt(0);
			}
			else if (move.From > 5)
			{
				// Column
				movedcards = cards[move.From - 6].Skip(cards[move.From - 6].Count - move.Cards).ToArray();
				cards[move.From - 6].RemoveRange(cards[move.From - 6].Count - move.Cards, move.Cards);
			}
			else if (move.From > 1)
			{
				// Foundation (Only 1 card)
				movedcards = new Card[] { new Card(foundation[move.From - 2], (CardColor)(move.From - 2)) };
				foundation[move.From - 2] -= 1;
			}
			else if (move.From == 1)
			{ 
				// Discard pile
				movedcards = DiscardPile.Skip(DiscardPile.Count - move.Cards).ToArray(); 
				DiscardPile.RemoveRange(DiscardPile.Count - move.Cards, move.Cards);
			}

			if (movedcards == null)
			{
				throw new ArgumentNullException(nameof(movedcards), "Invalid Card Selection! Moved cards cannot be null");
			}

			if (move.To == 0)
			{
				// Deck
				Deck.AddRange(movedcards);
			}
			else if (move.To == 1)
			{
				// Discard pile
				DiscardPile.AddRange(movedcards);
			}
			else if (move.To > 5)
			{
				// Column
				cards[move.To - 6].AddRange(movedcards);
			}
			else if (move.To > 1)
			{
				// Foundation
				foundation[move.To - 2] = movedcards[0].Value;
			}

		}
		private void UndoMove()
		{
			if (history.Count == 0)
			{
				Console.WriteLine("No moves to undo.");
				return;
			}
			Move last = history.RemoveLast();
			DoMove(last.Undo());
		}
		private bool FindIfValidTarget()
		{
			Card[] movedcards = CardsSelected;


			if (selected == 0 || selected == 1)
			{
				Debug.WriteLine("Cannot move to the Deck or the Discar Pile");
				return false;
			}
			if (selected > 5)
			{
				Debug.WriteLine("Column Selected Target");
				// Column
				if (cards[selected - 6].Count == 0)
				{
					Debug.WriteLineIf(CardsSelected[0].Value == CardValue.King, "Moving To Empty Slot because king");
					return CardsSelected[0].Value == CardValue.King;
				}
				if (cards[selected - 6][cards[selected - 6].Count - 1].IsValidTarget(CardsSelected[0]))
				{
					Debug.WriteLine($"Moving From {previousCardSelection} to Column {selected - 6}");
					return true;
				}
			}
			else if (selected > 1)
			{
				if (previousCardCount > 1)
				{
					Debug.WriteLine("Too many Cards");
					return false;
				}
				// Foundation
				Debug.WriteLine("Moving Card To Foundation");
				return foundation[selected - 2] + 1 == movedcards[0].Value;
			}

			// Invalid target
			Debug.WriteLine("Invalid target");
			return false; 
		}
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
		private void RunAction()
		{
			if (isDeckSelected)
			{
				MoveCard();
				return;
			}
			if (isCardBeingMoved)
			{
				if (areCardsSelected)
				{
					if (previousCardSelection == selected)
					{
						int ret = ContextMenu.SummonAt(0, 0, "Card Already Selecetd!", options: new string[] { "Continue", "Cancel" } );
						if (ret == 1) { previousCardSelection = -1; this.selected = 0; }
					}
					else
					{
						int ret = ContextMenu.SummonAt(0, 0, "Confirm move?", options: new string[] { "Yes", "No", "Cancel" });
						if (ret == 1) { previousCardSelection = -1; this.selected = 0; return; }
						else if (ret == 0)
						{
							Debug.WriteLine("Target Card: " + cards[selected - 6].Last().ToString());
							if (FindIfValidTarget())
							{
								Move move = new Move(previousCardSelection, selected, previousCardCount);
								DoMove(move);
								history.Add(move);
								previousCardSelection = -1; selected = 0;
							}
							else
							{
								Console.WriteLine("Invalid target");
							}
						}
					}
				}
				else if(selected > 1)
				{
					if (CardsSelected.Length > 1)
					{
						Debug.WriteLine("Invalid Target For Multiple Cards");
					}
					else
					{
						if (FindIfValidTarget())
						{
							Move move = new Move(previousCardSelection, selected, previousCardCount);
							DoMove(move);
							history.Add(move);
							previousCardSelection = -1; selected = 0;
						}
					}
				}
			}
			else
			{
				if (isDiscardSelected)
				{
					int ret = ContextMenu.SummonAt(0, 0, "Move cards?", options: new string[] { "Single", "Cancel" });
					if (ret == 0)
					{
						previousCardCount = 1;
						previousCardSelection = selected;
						Debug.WriteLine(CardsSelected[0].ToString());
					}
					else
					{
						previousCardSelection = -1; this.selected = 0; return;
					}
				}
				if (areCardsSelected)
				{
					int ret = ContextMenu.SummonAt(0,0,"Move cards?", options: new string[] { "Multiple", "Single", "Cancel" });
					if (ret == 1)
					{
						previousCardCount = 1;
						previousCardSelection = selected;
						Debug.WriteLine(CardsSelected[0].ToString());
					}
					else if (ret == 0)
					{
						throw new NotImplementedException("Multiple card move not implemented yet");
						previousCardCount = cards[selected - 6].Count;
					}
					else
					{
						previousCardSelection = -1; this.selected = 0; return;
					}
				}
			}
			/*else if (selected == 1)
			{
				if (DiscardPile.Count != 0)
				{
					Deck.Add(DiscardPile[DiscardPile.Count - 1]);
					DiscardPile.RemoveAt(DiscardPile.Count - 1);
				}
			}
			else if (selected > 5)
			{
				int column = selected - 6;
				if (cards[column].Count != 0)
				{
					cards[column][cards[column].Count - 1].IsSelected = true;
				}
			}
			else
			{
				int column = selected;
				if (foundation[column] == CardValue.None)
				{
					foundation[column] = cards[column][cards[column].Count - 1].Value;
					cards[column].RemoveAt(cards[column].Count - 1);
				}
			}*/
		}
		private void MoveCard(bool singlecard = false)
		{
			if (IsHardDifficulty && !singlecard)
			{
				MoveCard(true);
				MoveCard(true);
				MoveCard(true);
			}
			if (Deck.Count == 0)
			{
				//Deck = DiscardPile;
				for (int i = 0; i < DiscardPile.Count; i++)
				{
					DiscardPile[i].IsFaceUp = false;
				}
				Move move = new Move(1, 0, DiscardPile.Count);
				DoMove(move);
				history.Add(move);
				//DiscardPile = new List<Card>();
				Console.WriteLine("No cards left in the deck.");
				return;
			}
			Deck[0].IsFaceUp = true;
			DiscardPile.Add(Deck[0]);
			Deck.RemoveAt(0);
		}
		#region Screen Display
		private void Display()
		{
			Console.Clear();
			for (int i = 0; i < foundation.Length; i++)
			{
				DisplayFoundation(i);
			}
			for (int i = 0; i < cards.Length; i++)
			{
				DisplayCardColumn(i);
			}
			DisplayDeck();
		}
		private void DisplayCardColumn(int column)
		{
			int x = 9 * column, y = 5;
			if (column < 0 || column > cards.Length) throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and cards.Length");
			cards[column].Last().IsFaceUp = true;
			foreach (var card in cards[column])
			{
				card.Display(x, y);
				y += 2;
			}
		}
		private void DisplayDeck()
		{
			int x = 0, y = 0;
			if (Deck.Count == 0)
			{
				CardSpecial card = new CardSpecial(CardColor.Clubs, true);
				card.IsSelected = isDeckSelected;
				card.Display(x, y);
			}
			else
			{
				Card card = Deck[0];
				card.IsSelected = isDeckSelected;
				card.Display(x, y);
			}
			if (DiscardPile.Count != 0)
			{
				List<Card> c = DiscardPile.Skip(DiscardPile.Count - 3).ToList();
				c[c.Count-1].IsSelected = isDiscardSelected;
				int i = 0;
				foreach(Card card in c)
				{
					Debug.WriteLine(card.ToString() + " " + i);
					card.Display((x + 9) + i * 4,y);
					i++;
				}
			}
		}
		private void DisplayFoundation(int column)
		{
			int x = 30 + 8 * column, y = 0;
			if (column < 0 || column > foundation.Length) throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and foundation.Length");
			if (foundation[column] == CardValue.None)
			{
				CardSpecial card = new CardSpecial((CardColor)column, false);
				card.IsSelected = column + 2 == selected;
				card.Display(x, y);
			}
			else
			{
				Card c = new Card(foundation[column], (CardColor)column);
				c.IsFaceUp = true;
				c.IsSelected = column + 2 == selected;
				c.Display(x, y);
			}
		}
		#endregion
	}
}
