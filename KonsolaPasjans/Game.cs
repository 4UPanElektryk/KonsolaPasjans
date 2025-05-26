using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace KonsolaPasjans
{
	// Game of Solitaire
	public class Game
    {

		// Cards And stuff
		private List<Card> Deck { get; set; } = new List<Card>();
		private List<Card> DiscardPile { get; set; } = new List<Card>();
		private List<Card>[] cards = new List<Card>[7];
		private CardValue[] foundation = new CardValue[4];

		// Selection
		private int cursor = 0;
		private int selection = -1;
		private int selectionCount = 0;
		private Card[] selectionCards { 
			get {
				if (selection == 0) { /* Deck */ return new Card[] { Deck[0] }; }
				else if (selection == 1) { /* Discard pile */ return new Card[] { DiscardPile.Last() }; }
				else if (selection > 1 && selection < 6) { /* Foundation (Only 1 card) */ return new Card[] { new Card(foundation[selection - 2], (CardColor)(selection - 2)) { IsFaceUp = true } }; }
				else if (selection > 5) { /* Column */ return cards[selection - 6].Skip(cards[selection - 6].Count - selectionCount).ToArray(); }
				return new Card[0]; 
			} 
		}

		// Game State
		private ManagedMoveHistory history = new ManagedMoveHistory(3);
		public bool IsHardDifficulty = false;
		private bool FullReRender = false;

		private bool isDeckSelected { get { return cursor == 0; } }
		private bool isDiscardSelected { get { return cursor == 1; } }
		private bool isFoundationSelected { get { return cursor > 1 && cursor < 6; } }
		private bool areCardsSelected { get { return cursor > 5;  } }
		private bool isCardBeingMoved { get { return selection != -1; } }

		public Game() { }
		public void Start()
		{
			InitializeDeck();
			Console.CursorVisible = false;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Display();
			while (true)
			{
				Display();
				#region User Input
				ConsoleKeyInfo key = Console.ReadKey(true);
				if (key.Key == ConsoleKey.LeftArrow)
				{
					cursor--;
					if (cursor < 0)
					{
						cursor = 12;
					}
					if (DiscardPile.Count == 0 && cursor == 1)
					{
						cursor--;
					}
				}
				else if (key.Key == ConsoleKey.RightArrow)
				{
					cursor++;
					if (cursor > 12)
					{
						cursor = 0;
					}
					if (DiscardPile.Count == 0 && cursor == 1)
					{
						cursor++;
					}
				}
				else if (key.Key == ConsoleKey.UpArrow)
				{
					if (cursor == 6)
					{
						cursor = 0;
					}
					else if (cursor == 7 || cursor == 8)
					{
						cursor = 1;
					}
					else
					{
						cursor -= 7;
						if (cursor < 0)
						{
							cursor += 12;
						}
					}
					if (DiscardPile.Count == 0 && isDiscardSelected)
					{
						cursor--;
					}
				}
				else if (key.Key == ConsoleKey.DownArrow)
				{
					if (isDeckSelected)
					{
						cursor = 6;
					}
					else if (isDiscardSelected)
					{
						cursor = 7;
					}
					else
					{
						cursor += 7;
						if (cursor > 12)
						{
							cursor -= 12;
						}
						if (DiscardPile.Count == 0 && cursor == 1)
						{
							cursor++;
						}
					}
				}
				else if (key.Key == ConsoleKey.Spacebar || key.Key == ConsoleKey.Enter)
				{
					Debug.WriteLine($"Selected {cursor}");
					if (cursor > 5 && cards[cursor - 6].Count > 0)
					{
						cards[cursor - 6][cards[cursor - 6].Count - 1].IsSelected = false;
					}
					RunAction();
				}
				else if (key.Key == ConsoleKey.Escape)
				{
					if (isCardBeingMoved)
					{
						selection = -1;
						FullReRender = true;
					}
					else
					{
						if (DisplayPauseMenu())
						{
							break; // Exit the game loop
						}
					}
				}
				if (IsGameWon())
				{
					//TODO: Game won logic

					Console.Clear();
					Console.ForegroundColor = ConsoleColor.White;
					Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 2);
					Console.WriteLine("You won the game!");
					Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 1);
					Console.WriteLine($"You made {history.TotalCount} moves");
					Console.SetCursorPosition(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2);
					Console.WriteLine("Press any key to exit...");
					Console.ReadKey(true);
					break; // Exit the game loop
				}
				#endregion
			}
			Console.CursorVisible = true;
		}
		#region Selection Logic
		private bool FindIfValidTarget()
		{
			Card[] movedcards = selectionCards;
			if (isDeckSelected || isDiscardSelected)
			{
				Debug.WriteLine("Cannot move to the Deck or the Discar Pile");
				return false;
			}
			if (areCardsSelected)
			{
				Debug.WriteLine("Column Selected Target");
				// Column
				if (cards[cursor - 6].Count == 0)
				{
					Debug.WriteLineIf(selectionCards[0].Value == CardValue.King, "Moving To Empty Slot because king");
					return selectionCards[0].Value == CardValue.King;
				}
				if (cards[cursor - 6][cards[cursor - 6].Count - 1].IsValidTarget(selectionCards[0]))
				{
					Debug.WriteLine($"Moving From {selection} to Column {cursor - 6}");
					return true;
				}
			}
			else if (cursor > 1)
			{
				if (selectionCount > 1)
				{
					Debug.WriteLine("Too many Cards");
					return false;
				}
				// Foundation
				if (cursor - 2 == (int)movedcards[0].Color)
				{
					Debug.WriteLine("Moving Card To Foundation");
					return foundation[cursor - 2] + 1 == movedcards[0].Value;
				}
				return false;
			}

			// Invalid target
			Debug.WriteLine("Invalid target");
			return false; 
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
					if (selection == cursor)
					{
						// Same Card Selected
						/*int ret = ContextMenu.SummonAt(0, 0, "Card Already Selecetd!", options: new string[] { "Continue", "Cancel" } ); FullReRender = true;
						if (ret == 1) { */
						selection = -1; selectionCount = 0; this.cursor = 0; //}
					}
					else if (FindIfValidTarget())
					{
						// Different Card Confirmation
						/*int ret = ContextMenu.SummonAt(0, 0, "Confirm move?", options: new string[] { "Yes", "No", "Cancel" }); 
						if (ret == 1) { previousCardSelection = -1; this.selected = 0; return; }
						else if (ret == 0)
						{*/
							Move move = new Move(selection, cursor, selectionCount);
							DoMove(move);
							history.Add(move);
							selection = -1; cursor = 0;
						//}
						FullReRender = true;
					}
				}
				else if(cursor > 1)
				{
					// Foundation
					if (selectionCards.Length > 1)
					{
						Debug.WriteLine("Invalid Target For Multiple Cards");
					}
					else if (FindIfValidTarget())
					{
						Move move = new Move(selection, cursor, selectionCount);
						DoMove(move);
						history.Add(move);
						selection = -1; cursor = 0;
						FullReRender = true;
					}
				}
			}
			else
			{
				if (isDiscardSelected)
				{
					selectionCount = 1;
					selection = cursor;
					Debug.WriteLine(selectionCards[0].ToString());
				}
				else if (areCardsSelected && !(cards[cursor - 6].Count == 0)) // if cards are selected and the column is not empty
				{
					if (DiscoverMaxSelection() == 1)
					{
						selectionCount = 1;
						selection = cursor;
						Debug.WriteLine(selectionCards[0].ToString());
					}
					else
					{
						selectionCount = DiscoverMaxSelection();
						selection = cursor;
						Debug.WriteLine(selectionCards[0].ToString());
						int count = 1;
						while (true)
						{
							DisplaySelection(count); // Display the selection with the current count
							ConsoleKeyInfo key = Console.ReadKey(true);
							if(key.Key == ConsoleKey.Escape)
							{
								//exit cancel selection
								selection = -1; 
								this.cursor = 0; 
								break;
							}
							else if (key.Key == ConsoleKey.Enter || key.Key == ConsoleKey.Spacebar)
							{
								//exit selection
								selection = cursor;
								selectionCount = count;
								break;
							}
							else if(key.Key == ConsoleKey.UpArrow)
							{
								count++;
								if (count > selectionCount)
								{
									count = 1;
								}
							}
							else if (key.Key == ConsoleKey.DownArrow)
							{
								count--;
								if (count < 1)
								{
									count = selectionCount;
								}
							}
						}
						FullReRender = true;
					}
				}
				else if (isFoundationSelected && (foundation[cursor - 2] != CardValue.None)) //Foundation Selected and the colums are not empty
				{
					selectionCount = 1;
					selection = cursor;
					Debug.WriteLine(selectionCards[0].ToString());
				}

			}
		}
		#endregion
		#region Game Logic
		private bool IsGameWon()
		{
			// Check if all foundations are filled
			return foundation.All(f => f == CardValue.King);
		}
		/// <summary>
		/// In game pause menu when ESC is pressed
		/// </summary>
		/// <returns>true if game is ended and false if the game should continue</returns>
		private	bool DisplayPauseMenu()
		{
			//center screen pause menu
			Console.Clear();
			int ret = ContextMenu.SummonAt(Console.WindowWidth / 2 - 10, Console.WindowHeight / 2 - 3, "Game Paused", new string[] { "Resume", "Undo Move", "Restart Game", "Exit to Main Menu" }, ConsoleColor.Yellow);
			if (ret == 1)
			{
				UndoMove();
			}
			else if (ret == 2) 
			{
				RestartGame();
			}
			else if (ret == 3)
			{
				return true;
			}
			FullReRender = true;
			return false;
		}
		private void RestartGame()
		{
			// Reset the game state
			Deck.Clear();
			DiscardPile.Clear();
			for (int i = 0; i < cards.Length; i++)
			{
				cards[i].Clear();
			}
			for (int i = 0; i < foundation.Length; i++)
			{
				foundation[i] = CardValue.None;
			}
			cursor = 0;
			selection = -1;
			history = new ManagedMoveHistory(3);
			InitializeDeck();
			FullReRender = true;
		}
		#endregion
		#region Card Logic
		private int DiscoverMaxSelection()
		{
			int n = 1;
			int column = cursor - 6;
			int cardslen = cards[column].Count;
			for (int i = cardslen - 1; i > 0; i--)
			{
				if (!cards[column][i - 1].IsFaceUp)
				{
					break;
				}
				if (cards[column][i - 1].IsValidTarget(cards[column][i]))
				{
					n = cardslen - (i - 1);
				}
			}
			return n;
		}
		private void InitializeDeck()
		{
			for (int j = 0; j <= 3; j++)
			{
				for (int i = 1; i <= 13; i++)
				{
					Deck.Add(new Card((CardValue)i, (CardColor)j));
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
		private void DoMove(Move move)
		{
			Card[] movedcards = null;
			if (move.From == 0)
			{
				//Deck
				movedcards = Deck.Take(move.Cards).ToArray();
				Deck.RemoveRange(0, move.Cards);
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
		private void MoveCard()
		{
			if (Deck.Count == 0)
			{
				FullReRender = true;
				Deck = DiscardPile;
				for (int i = 0; i < DiscardPile.Count; i++)
				{
					DiscardPile[i].IsFaceUp = false;
				}
				//Move move = new Move(1, 0, DiscardPile.Count);
				//DoMove(move);
				//history.Add(move);
				DiscardPile = new List<Card>();
				ShuffleDeck();
				return;
			}
			if (IsHardDifficulty)
			{
				int count = Deck.Count;
				if (count > 3) { count = 3; }
				Move move = new Move(0, 1, count);
				for (int i = 0; i < count; i++)
				{
					Deck[i].IsFaceUp = true;
					Deck[i].IsSelected = false;
				}
				DoMove(move);
				FullReRender = true;
			}
			else
			{

				Deck[0].IsFaceUp = true;
				Move move = new Move(0, 1, 1);
				DiscardPile.Add(Deck[0]);
				Deck.RemoveAt(0);
			}
		}
		#endregion
		#region Screen Display
		private void Display()
		{
			if (FullReRender)
			{
				Console.Clear();
				FullReRender = false;
			}
			for (int i = 0; i < foundation.Length; i++)
			{
				DisplayFoundation(i);
			}
			for (int i = 0; i < cards.Length; i++)
			{
				DisplayCardColumn(i);
			}
			DisplayDeck();
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			for (int i = 0; i < Console.WindowHeight; i++)
			{
				Console.SetCursorPosition(62, i);
				Console.Write("║");
			}
			DisplaySelection();
		}
		private void DisplaySelection(int amount = 0)
		{
			Console.SetCursorPosition(64, 5);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Selection: ");
			int x = 64, y = 6;
			if (selection != -1)
			{
				for (int i = 0; i < selectionCount; i++)
				{
					Card displayCard = new Card(selectionCards[i]);
					if (i >= selectionCount - amount)
					{
						displayCard.IsSelected = true;
					}
					displayCard.Display(x, y);
					y += 2;
				}
			}
		}
		private void DisplayCardColumn(int column)
		{
			int x = 9 * column, y = 5;
			if (column < 0 || column > cards.Length) throw new ArgumentOutOfRangeException(nameof(column), "Column must be between 0 and cards.Length");
			if (cards[column].Count > 0)
			{
				cards[column].Last().IsFaceUp = true;
			}
			if (cards[column].Count == 0)
			{
				Card card = new CardSpecial(CardColor.None, false);
				card.IsSelected = column == cursor - 6;
				card.Display(x, y);
				return;
			}
			cards[column][cards[column].Count - 1].IsSelected = column == cursor - 6;
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
				card.IsSelected = column + 2 == cursor;
				card.Display(x, y);
			}
			else
			{
				Card c = new Card(foundation[column], (CardColor)column);
				c.IsFaceUp = true;
				c.IsSelected = column + 2 == cursor;
				c.Display(x, y);
			}
		}
		#endregion
	}
}
