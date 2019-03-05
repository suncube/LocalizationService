using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Localization
{
	public class CSVReader
	{
		public static string[,] SplitCsvGrid(string csvText)
		{
			var lines = Regex.Split(csvText, System.Environment.NewLine);
			int width = 0;
			for (int i = 0; i < lines.Length; i++)
			{
				string[] row = SplitCsvLine(lines[i]);
				width = Mathf.Max(width, row.Length);
			}

			string[,] outputGrid = new string[width, lines.Length + 1];
			for (int y = 0; y < lines.Length; y++)
			{
				string[] row = SplitCsvLine(lines[y]);
				for (int x = 0; x < row.Length; x++)
				{
					outputGrid[x, y] = row[x];
					outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
				}
			}

			return outputGrid;
		}

		public static string[] SplitCsvLine(string line)
		{
			return (from Match m in Regex.Matches(line,
				@"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
				System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
				select m.Groups[1].Value).ToArray();
		}
	}
}