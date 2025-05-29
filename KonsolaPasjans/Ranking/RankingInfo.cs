using System;

namespace KonsolaPasjans.Ranking
{
    public class RankingInfo
    {
		public string PlayerName { get; set; }
		public int Moves { get; set; }
		public DateTime Date { get; set; }
		public bool HardMode { get; set; }
		public RankingInfo(string playerName, int moves, DateTime date, bool hardMode)
		{
			PlayerName = playerName;
			Moves = moves;
			Date = date;
			HardMode = hardMode;
		}
		public override string ToString()
		{
			if (HardMode)
			{
				return $"{PlayerName} - {Moves} ruchów w Trudnej grze - {Date.ToShortDateString()}";
			}
			return $"{PlayerName} - {Moves} ruchów w Prostej grze - {Date.ToShortDateString()}";
		}
	}
}
