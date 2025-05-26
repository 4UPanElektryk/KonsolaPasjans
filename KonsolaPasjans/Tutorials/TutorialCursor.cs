using System;

namespace KonsolaPasjans.Tutorials
{
    class TutorialCursor : Tutorial
    {
		private Card[] _cards;
		private int selectedCardIndex = 4;
		private bool MovedUp = false;
		private bool MovedDown = false;
		private bool MovedLeft = false;
		private bool MovedRight = false;
		public TutorialCursor() : base()
		{
			Name = "Samouczek Kursora";
			// Initialization logic if needed
			_cards = new Card[9]
			{
				new Card(CardValue.Ace, CardColor.Hearts) { IsFaceUp = true },
				new Card(CardValue.Two, CardColor.Diamonds) { IsFaceUp = true },
				new Card(CardValue.Three, CardColor.Clubs) { IsFaceUp = true },
				new Card(CardValue.Four, CardColor.Spades) { IsFaceUp = true },
				new Card(CardValue.Five, CardColor.Hearts) { IsFaceUp = true },
				new Card(CardValue.Six, CardColor.Diamonds) { IsFaceUp = true },
				new Card(CardValue.Seven, CardColor.Clubs) { IsFaceUp = true },
				new Card(CardValue.Eight, CardColor.Spades) { IsFaceUp = true },
				new Card(CardValue.Nine, CardColor.Hearts) { IsFaceUp = true }
			};
		}
		public override void Start()
		{
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.Clear();
			Console.WriteLine("Witaj w samouczku kursora w Pasjansie!");
			Console.WriteLine("Aby przesunąć kursor, użyj strzałek na klawiaturze.");
			Console.WriteLine("Zaznaczona karta będzie się podświetlała na żółto");
			Console.WriteLine("Aby zakończyć ten samouczek, przemieść kursor przynajmniej raz w każdym kierunku lub naciśnij 'Esc'.");
			Console.WriteLine("Powodzenia!");
			// Logic to handle movement tutorial
			Console.ReadKey(true); // Wait for the user to press a key before starting the tutorial
			Console.Clear();
			while (true)
			{
				Draw(); // Draw the current state of the tutorial
				var key = Console.ReadKey(true).Key;
				if (key == ConsoleKey.Escape)
				{
					IsCompleted = false;
					return; // Exit the tutorial
				}
				else if (key == ConsoleKey.UpArrow)
				{
					MovedUp = true;
					selectedCardIndex -= 3; // Move up in the array
					if (selectedCardIndex < 0) selectedCardIndex += _cards.Length; // Prevent going out of bounds
				}
				else if (key == ConsoleKey.DownArrow)
				{
					MovedDown = true;
					selectedCardIndex += 3; // Move down in the array
					if (selectedCardIndex >= _cards.Length) selectedCardIndex -= _cards.Length; // Prevent going out of bounds
				}
				else if (key == ConsoleKey.LeftArrow)
				{
					MovedLeft = true;
					selectedCardIndex -= 1; // Move left in the array
					if (selectedCardIndex < 0) selectedCardIndex += _cards.Length; // Prevent going out of bounds
				}
				else if (key == ConsoleKey.RightArrow)
				{
					MovedRight = true;
					selectedCardIndex += 1; // Move right in the array
					if (selectedCardIndex >= _cards.Length) selectedCardIndex -= _cards.Length; // Prevent going out of bounds
				}
				else if (key == ConsoleKey.Enter)
				{
					if (IsCompleted)
					{
						Console.WriteLine("Samouczek zakończony. Dziękujemy za udział!");
						return; // Exit the tutorial
					}
				}
				if (MovedDown && MovedUp && MovedLeft && MovedRight)
				{
					IsCompleted = true;
				}
			}
		}
		private void Draw()
		{
			// Quests
			Console.SetCursorPosition(0, 0);
			Console.BackgroundColor = ConsoleColor.DarkGreen;
			Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("Zadania:");
			char MovedUpCompleted = MovedUp ? 'X' : ' ';
			char MovedDownCompleted = MovedDown ? 'X' : ' ';
			char MovedLeftCompleted = MovedLeft ? 'X' : ' ';
			char MovedRightCompleted = MovedRight ? 'X' : ' ';
			Console.WriteLine($"[{MovedUpCompleted}] Przesuń kursor w górę");
			Console.WriteLine($"[{MovedDownCompleted}] Przesuń kursor w dół");
			Console.WriteLine($"[{MovedLeftCompleted}] Przesuń kursor w lewo");
			Console.WriteLine($"[{MovedRightCompleted}] Przesuń kursor w prawo");
			Console.WriteLine($"Naciśnij 'ESC' Aby Przerwać");
			if (IsCompleted)
			{
				Console.WriteLine($"Naciśnij 'Enter' Aby Zakończyć");
			}

			int xlimit = 3;
			for (int i = 0; i < _cards.Length; i++)
			{
				int x = (i % xlimit) * 10; // Calculate x position based on index
				int y = (i / xlimit) * 5 + 7; // Calculate y position based on index
				_cards[i].IsSelected = (i == selectedCardIndex); // Highlight the selected card
				_cards[i].Display(x, y); // Display the card at the calculated position
			}
		}
	}
}
