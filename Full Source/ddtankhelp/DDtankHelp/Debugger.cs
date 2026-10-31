using System;
using System.IO;

namespace DDtankHelp;

public class Debugger
{
	private static readonly object syncLock = new object();

	public static void getStackTrace(string logNamer)
	{
		lock (syncLock)
		{
			Log(Environment.StackTrace, logNamer + ".txt");
		}
	}

	public static void getStackTrace(string logNamer, string desc)
	{
		lock (syncLock)
		{
			Log(Environment.StackTrace + " Descrição: " + desc, logNamer + ".txt");
		}
	}

	public static string getStackTrace()
	{
		return Environment.StackTrace;
	}

	public static void Log(string logMessage, string fileName)
	{
		if (!Directory.Exists(Environment.CurrentDirectory + "/logsHelp/"))
		{
			Directory.CreateDirectory(Environment.CurrentDirectory + "/logsHelp/");
		}
		StreamWriter streamWriter = File.AppendText(Environment.CurrentDirectory + "/logsHelp/" + fileName);
		streamWriter.Write("\r\nLog Entry : ");
		streamWriter.WriteLine("{0} {1}", DateTime.Now.ToLongTimeString(), DateTime.Now.ToLongDateString());
		streamWriter.WriteLine("  :");
		streamWriter.WriteLine("  :{0}", logMessage);
		streamWriter.WriteLine("-------------------------------");
		streamWriter.Flush();
		streamWriter.Close();
		streamWriter.Dispose();
	}
}
