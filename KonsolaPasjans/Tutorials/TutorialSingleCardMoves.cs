using System;
using System.Collections.Generic;
using System.Linq;

namespace KonsolaPasjans.Tutorials
{
	class TutorialSingleCardMoves : Tutorial
	{
		private List<Card>[] _cardsColumns;
		private int _cursor = 1;
		private int selection = -1; // -1 means no selection
		private bool firstCardSelected = false;

		public TutorialSingleCardMoves() : base()
		{
			Name = "Samouczek Ruchów Kart (1/2)";
		}
		private void Clear()
		{
			_cursor = 1;
			selection = -1;
			firstCardSelected = false;
			IsCompleted = false;
			// Initialization logic if needed
			_cardsColumns = new List<Card>[]{
				new List<Card>()
				{
					new Card(CardValue.Ace, CardColor.Hearts) { IsFaceUp = false },
					new Card(CardValue.Two, CardColor.Diamonds) { IsFaceUp = true }
				},
				new List<Card>()
				{
					new Card(CardValue.Six, CardColor.Diamonds) { IsFaceUp = false },
					new Card(CardValue.Four, CardColor.Spades) { IsFaceUp = false },
					new Card(CardValue.Five, CardColor.Hearts) { IsFaceUp = false },
					new Card(CardValue.Three, CardColor.Clubs) { IsFaceUp = true },
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
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Console.WriteLine("Witaj w samouczku ruchów kart w Pasjansie (1/2)!");
			Console.WriteLine("Użyj strzałek na klawiaturze aby przesunąć kursor");
			Console.WriteLine("Zaznaczona karta będzie się wyświetlana po prawej");
			Console.WriteLine("Zaznaczenie kart można anulować przy użyciu klawisza 'ESC'");
			Console.WriteLine("Powodzenia!");
			// Logic to handle movement tutorial
			Console.ReadKey(true); // Wait for the user to press a key before starting the tutorial
			Console.Clear();
			while (true)
			{
				for (int i = 0; i < _cardsColumns.Length; i++)
				{
					_cardsColumns[i].Last().IsFaceUp = true;
				}
				Draw(); // Draw the current state of the tutorial
				ConsoleKey key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Escape)
				{
					if (selection == -1)
					{
						IsCompleted = false;
						return;
					}
					else
					{
						selection = -1; // Deselect the card
					}
				}
				if (key == ConsoleKey.LeftArrow)
				{
					_cursor--;
					if (_cursor < 0) _cursor = _cardsColumns.Length - 1;
				}
				else if (key == ConsoleKey.RightArrow)
				{
					_cursor++;
					if (_cursor >= _cardsColumns.Length) _cursor = 0;
				}
				else if (key == ConsoleKey.Enter)
				{
					if (IsCompleted)
					{
						return;
					}
					if (selection == -1)
					{
						selection = _cursor; // Select the current card
						firstCardSelected = true;
					}
					else if (_cardsColumns[_cursor].Last().IsValidTarget(_cardsColumns[selection].Last()))
					{
						_cardsColumns[_cursor].Add(_cardsColumns[selection].Last());
						_cardsColumns[selection].RemoveAt(_cardsColumns[selection].Count - 1);
						selection = -1; // Deselect the card
						IsCompleted = true;
					}
				}
			}
		}
		private void Draw()
		{
			int skiplines = 4;
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Console.WriteLine("Zadania:");
			char SelectCard = firstCardSelected ? 'X' : ' ';
			char MoveCard = IsCompleted ? 'X' : ' ';
			Console.WriteLine($"[{SelectCard}] Wybierz kartę do przeniesienia");
			Console.WriteLine($"[{MoveCard}] Przenieś kartę");
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
			if (selection != -1)
			{
				int x = 64, y = 1;
				Card card = new Card(_cardsColumns[selection].Last());
				card.Display(x, y);
			}


			#endregion
		}
	}
}
