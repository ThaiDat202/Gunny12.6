using System;
using System.IO;

public class PlayerLogs
{
	public static void WriteLine(string logs, int type)
	{
		string text = $"{Environment.CurrentDirectory}\\PlayerLogs\\";
		string text2 = type switch
		{
			1 => "ChatLogs.txt", 
			2 => "ResetActiveLogs.txt", 
			3 => "PetSealLogs.txt", 
			_ => "", 
		};
		if (!File.Exists(text + text2))
		{
			FileInfo fileInfo = new FileInfo(text + text2);
			FileStream fileStream = fileInfo.Create();
			fileStream.Close();
		}
		StreamWriter streamWriter = File.AppendText(text + text2);
		streamWriter.WriteLine(logs);
		streamWriter.Flush();
		streamWriter.Close();
	}
}
