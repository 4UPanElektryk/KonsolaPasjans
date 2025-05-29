using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KonsolaPasjans.Tutorials
{
    class TutorialMultipleCardMoves : Tutorial
	{
		private List<Card>[] _cardsColumns;
		private int _cursor = 1;
		private int _columnHowMany = 1; // -1 means no selection
		private bool firstCardSelected = false;

		public TutorialMultipleCardMoves() : base()
		{
			Name = "Samouczek Ruchów Kart (2/2)";
		}
		private void Clear()
		{
			_cursor = 1;
			_columnHowMany = 1;
			firstCardSelected = false;
			_cardsColumns = new List<Card>[]{
				new List<Card>()
				{
					new Card(CardValue.Ace, CardColor.Hearts) { IsFaceUp = false },
					new Card(CardValue.Two, CardColor.Diamonds) { IsFaceUp = true }
				},
				new List<Card>()
				{
					new Card(CardValue.Six, CardColor.Clubs) { IsFaceUp = true },
					new Card(CardValue.Five, CardColor.Hearts) { IsFaceUp = true },
					new Card(CardValue.Four, CardColor.Spades) { IsFaceUp = true },
					new Card(CardValue.Three, CardColor.Diamonds) { IsFaceUp = true },
				},
				new List<Card>()
				{
					new Card(CardValue.Seven, CardColor.Clubs) { IsFaceUp = false },
					new Card(CardValue.Eight, CardColor.Spades) { IsFaceUp = false },
					new Card(CardValue.Nine, CardColor.Hearts) { IsFaceUp = true }
				}
			};
		}
		public override void Start()
		{
			Clear();
			Draw();
			_columnHowMany = DiscoverMaxSelection();
			int highlightedCards = 1;
			while (true)
			{
				DisplaySelection(highlightedCards);
				ConsoleKey key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Escape)
				{
					IsCompleted = false;
					return;
				}
				else if (key == ConsoleKey.UpArrow)
				{
					highlightedCards++;
					if (highlightedCards > _columnHowMany)
					{
						highlightedCards = 1;
					}
				}
				else if (key == ConsoleKey.DownArrow)
				{
					highlightedCards--;
					if (highlightedCards <= 0)
					{
						highlightedCards = _columnHowMany;
					}
				}
				else if	(key == ConsoleKey.Enter)
				{
					if (IsCompleted)
					{
						return;
					}
				}
				if (highlightedCards == _columnHowMany)
				{
					IsCompleted = true;
					Draw();
				}
			}
		}
		private void Draw()
		{
			int skiplines = 3;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Console.WriteLine("Zadania:");
			char SelectCard = IsCompleted ? 'X' : ' ';
			Console.WriteLine($"[{SelectCard}] Zaznacz wszystkie karty");
			if (IsCompleted)
			{
				Console.WriteLine("Naciśnij 'Enter' Aby Zakończyć");
			}

			#region Cards Columns
			for (int i = 0; i < _cardsColumns.Length; i++)
			{
				for (int j = 0; j < _cardsColumns[i].Count; j++)
				{
					_cardsColumns[i][j].IsSelected = false;
				}
			}

			for (int i = 0; i < _cardsColumns.Length; i++)
			{
				int x = i * 9; // Adjust the x position for each column
				if (_cardsColumns[i].Count == 0)
				{
					Card card = new CardSpecial(CardColor.None, false);
					card.IsSelected = (i == _cursor); // Highlight the selected card
					card.Display(x, skiplines);
				}
				else
				{
					_cardsColumns[i].Last().IsSelected = (i == _cursor); // Highlight the selected card
				}
				for (int j = 0; j < _cardsColumns[i].Count; j++)
				{
					int y = skiplines + j * 2; // Adjust the y position for each card in the column
					_cardsColumns[i][j].Display(x, y);
				}
			}
			#endregion
			Console.ForegroundColor = ConsoleColor.White;
			for (int i = 0; i < Console.WindowHeight; i++)
			{
				Console.SetCursorPosition(62, i);
				Console.Write("║");
			}
			Console.SetCursorPosition(64, 0);
			Console.WriteLine("Wybrane Karty: ");
			#region Selected Cards


			#endregion
		}
		private void DisplaySelection(int amount = 0)
		{
			Console.SetCursorPosition(64, 0);
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Wybrane Karty: ");
			int x = 64, y = 1;
			Card[] selectionCards = _cardsColumns[_cursor].Skip(_cardsColumns[_cursor].Count - _columnHowMany).ToArray();
			if (_cursor != -1)
			{
				for (int i = 0; i < _columnHowMany; i++)
				{
					Card displayCard = new Card(selectionCards[i]);
					if (i >= _columnHowMany - amount)
					{
						displayCard.IsSelected = true;
					}
					displayCard.Display(x, y);
					y += 2;
				}
			}
		}
		private int DiscoverMaxSelection()
		{
			int n = 1;
			int column = _cursor;
			int cardslen = _cardsColumns[column].Count;
			for (int i = cardslen - 1; i > 0; i--)
			{
				if (!_cardsColumns[column][i - 1].IsFaceUp)
				{
					break;
				}
				if (_cardsColumns[column][i - 1].IsValidTarget(_cardsColumns[column][i]))
				{
					n = cardslen - (i - 1);
				}
			}
			return n;
		}
	}
}
