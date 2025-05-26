using System.Diagnostics;
namespace KonsolaPasjans
{
	public class ManagedMoveHistory
	{
		private Move[] moves;
		private int MoveCount;
		public int Count { get { return MoveCount; } }
		public int TotalCount;
		public Move this[int index]
		{
			get { return moves[index]; }
			set { moves[index] = value; }
		}
		public ManagedMoveHistory(int maxsize) {
			moves = new Move[maxsize];
			MoveCount = 0;
			TotalCount = 0;
		}
		public void Add(Move move)
		{
			TotalCount++;
			if (MoveCount < moves.Length)
			{
				moves[MoveCount] = move;
				MoveCount++;
			}
			else
			{
				Debug.WriteLine("Move history is full. Dropping old move!");
				for (int i = 0; i < moves.Length - 1; i++)
				{
					moves[i] = moves[i + 1];
				}
				moves[moves.Length - 1] = move;
			}
		}
		public Move RemoveLast()
		{
			if (MoveCount > 0)
			{
				MoveCount--;
				Move lastMove = moves[MoveCount];
				moves[MoveCount] = null;
				return lastMove;
			}
			else
			{
				Debug.WriteLine("Move history is empty. Cannot remove last move!");
				return null;
			}
		}
	}
}
