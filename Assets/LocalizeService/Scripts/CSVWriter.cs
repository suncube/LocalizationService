using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Localization
{
	public class CSVWriter : StreamWriter
	{
		public CSVWriter(string filename)
			: base(filename, false, System.Text.Encoding.UTF8)
		{
		}

		public void WriteRow(CSVRow row)
		{
			StringBuilder builder = new StringBuilder();
			bool firstColumn = true;
			foreach (string value in row)
			{
				if (!firstColumn)
					builder.Append(',');

				if (value.IndexOfAny(new char[] {'"', ','}) != -1)
					builder.AppendFormat("\"{0}\"", value.Replace("\"", "\"\""));
				else
					builder.Append(value);
				firstColumn = false;
			}
			row.LineText = builder.ToString();
			WriteLine(row.LineText);
		}
	}


	public class CSVRow : List<string>
	{
		public string LineText { get; set; }
	}
}