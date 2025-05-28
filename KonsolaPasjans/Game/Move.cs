namespace KonsolaPasjans
{
	public class Move
	{
		// Selection index
		public int From { get; set; }
		public int To { get; set; }
		// Card Memory
		public int Cards { get; set; }
		public bool UndoMove { get; set; }
		public bool WasCardBelowCovered { get; set; }

		public Move(int from, int to, int cards)
		{
			From = from;
			To = to;
			this.Cards = cards;
			UndoMove = false;
		}
		public Move Undo()
		{
			return new Move(To, From, Cards) { UndoMove = true, WasCardBelowCovered = this.WasCardBelowCovered };
		}
	}
}
