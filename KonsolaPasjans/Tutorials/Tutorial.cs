using System;

namespace KonsolaPasjans
{
    public class Tutorial
    {
		public bool IsCompleted = false;
		public string Name = "Samouczek";
		public Tutorial()
		{
			// Creation Logic
		}
		public virtual void Start()
		{
			Console.WriteLine("Witaj w grze Pasjans!");
			Console.WriteLine("Celem gry jest uporządkowanie kart w stosy według kolorów i wartości.");
			Console.WriteLine("Możesz przesuwać karty między stosami, a także odkrywać karty z talii.");
			Console.WriteLine("Aby rozpocząć grę, naciśnij klawisz 'Enter'. Aby zakończyć, przyciśnij 'Esc'.");
			Console.WriteLine("Powodzenia!");


		}
	}
}
