using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace KonsolaPasjans.Ranking
{
    public class RankingManager
    {
        public static List<RankingInfo> AllInfo { get; private set; } = new List<RankingInfo>();
		private static string filePath = "ranking.csv";
		public static void Load()
		{
			if (File.Exists(filePath))
			{
				using (StreamReader sr = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath)))
				{
					string line;
					while ((line = sr.ReadLine()) != null)
					{
						if (line.Contains(";"))
						{
							string[] data = line.Split(';');
							if (data.Length == 4)
							{
								try
								{
									string playerName = Encoding.UTF8.GetString(Convert.FromBase64String(data[0])); // Decode Base64 to get the original player name
									int moves = int.Parse(data[1]);
									DateTime date = DateTime.ParseExact(data[2], "yyyy-MM-dd HH:mm:ss", null);
									bool hardMode = bool.Parse(data[3]); // Assuming the last field is a boolean indicating Hard Mode
									RankingInfo rankingInfo = new RankingInfo(playerName, moves, date, hardMode);
									AllInfo.Add(rankingInfo);
								}
								catch (Exception ex)
								{
									Debug.WriteLine($"Error parsing line: {line}. Exception: {ex.Message}");
								}
							}
							else
							{
								Debug.WriteLine($"Invalid line format: {line}");
							}
						}
					}
				}
			}
			else
			{
				Debug.WriteLine("Ranking file not found, starting with an empty ranking.");
			}
		}
        public static void Save()
		{
			File.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,filePath));
			StreamWriter writer = new StreamWriter(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, filePath));
			foreach (RankingInfo item in AllInfo)
			{
				// Format: PlayerName,Moves,Date
				string playernamesafe = Convert.ToBase64String(Encoding.UTF8.GetBytes(item.PlayerName)); // Replace commas to avoid CSV issues
				string line = $"{playernamesafe};{item.Moves};{item.Date.ToString("yyyy-MM-dd HH:mm:ss")};{item.HardMode}";
				writer.WriteLine(line);
			}
			writer.Close();
		}
        public static void Add(string playerName, int moves, bool HardMode)
		{
			RankingInfo rankingInfo = AllInfo.Find(r => r.PlayerName == playerName);
			if (rankingInfo != null)
			{
				AllInfo.Remove(rankingInfo);
				rankingInfo.Moves = moves;
				rankingInfo.Date = DateTime.Now;
			}
			else
			{
				rankingInfo = new RankingInfo(playerName, moves, DateTime.Now, HardMode);
			}
			AllInfo.Add(rankingInfo);
			Save();
		}
		public static RankingInfo[] GetTop5()
		{
			AllInfo.Sort((x, y) => x.Moves.CompareTo(y.Moves));
			List<RankingInfo> top5 = new List<RankingInfo>(AllInfo);
			if (top5.Count > 5)
			{
				top5 = top5.GetRange(0, 5);
			}
			return top5.ToArray();
		}
		public static RankingInfo[] GetEntries()
		{
			return AllInfo.ToArray();
		}
	}
}
