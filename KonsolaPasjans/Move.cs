namespace KonsolaPasjans
{
    public class Move
    {
        // Selection index
        public int From { get; set; }
		public int To { get; set; }
        // Card Memory
        public int Cards { get; set; }

		public Move(int from, int to, int cards)
		{
			From = from;
			To = to;
			this.Cards = cards;
		}
	}
}
