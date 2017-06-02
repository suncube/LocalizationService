using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public class CSVReader
{
    // splite is [,]
    // splits a CSV file into a 2D string array
    public static string[,] SplitCsvGrid(string csvText)
    {
       // string[] lines = csvText.Split("\n"[0]);
        var lines = Regex.Split(csvText, System.Environment.NewLine);
        // finds the max width of row
        int width = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            string[] row = SplitCsvLine(lines[i]);
            width = Mathf.Max(width, row.Length);
        }

        string[,] outputGrid = new string[width, lines.Length+1];
        for (int y = 0; y < lines.Length; y++)
        {
            string[] row = SplitCsvLine(lines[y]);
            for (int x = 0; x < row.Length; x++)
            {
                outputGrid[x, y] = row[x];
                outputGrid[x, y] = outputGrid[x, y].Replace("\"\"", "\"");
            }
        }
       // DebugOutputGrid(outputGrid);

        return outputGrid;
    }

    //public static void DebugOutputGrid(string[,] grid)
    //{
    //    string textOutput = "";
    //    for (int y = 0; y <= grid.GetUpperBound(1); y++)
    //    {
    //        for (int x = 0; x <= grid.GetUpperBound(0); x++)
    //        {

    //            textOutput += grid[x, y];
    //            textOutput += "|";
    //        }
    //        textOutput += "\n";
    //    }
    //    Debug.Log(textOutput);
    //}

    // splits a CSV row 
    public static string[] SplitCsvLine(string line)
    {
        return (from Match m in Regex.Matches(line,
            @"(((?<x>(?=[,\r\n]+))|""(?<x>([^""]|"""")+)""|(?<x>[^,\r\n]+)),?)",
            System.Text.RegularExpressions.RegexOptions.ExplicitCapture)
            select m.Groups[1].Value).ToArray();
    }
}
