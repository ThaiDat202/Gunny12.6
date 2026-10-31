using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class WindMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly int[] int_0 = new int[9] { 1001, 1002, 1003, 1004, 1005, 1005, 1007, 1008, 1009 };

	private static readonly int[] int_1 = new int[11]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10
	};

	private static readonly string[] string_0 = new string[11]
	{
		".", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"0"
	};

	private static readonly Color[] color_0 = new Color[8]
	{
		Color.Yellow,
		Color.Red,
		Color.Blue,
		Color.Green,
		Color.Orange,
		Color.Aqua,
		Color.DarkCyan,
		Color.Purple
	};

	private static readonly string[] string_1 = new string[3] { "Verdana", "Comic Sans MS", "Tahoma" };

	private static Dictionary<int, WindInfo> dictionary_0;

	private static Random random_0;

	public static byte[] CreateImage(string randomcode)
	{
		int num = 20;
		Bitmap bitmap = new Bitmap(16, 29);
		Graphics graphics = Graphics.FromImage(bitmap);
		int num2 = random_0.Next(string_1.Length);
		FontFamily fontFamily = new FontFamily(string_1[num2]);
		GraphicsPath graphicsPath = new GraphicsPath();
		Pen pen = new Pen(Color.Black, 5f);
		int num3 = random_0.Next(2);
		Color white = Color.White;
		Color burlyWood = Color.BurlyWood;
		Color color;
		Color color2;
		if (num3 != 1)
		{
			color = color_0[random_0.Next(6)];
			color2 = Color.White;
		}
		else
		{
			color2 = color_0[random_0.Next(6)];
			color = Color.White;
		}
		int num4 = random_0.Next(4);
		LinearGradientBrush linearGradientBrush = new LinearGradientBrush(new Rectangle(10, 10, 16, 26), color, color2, num4 switch
		{
			1 => LinearGradientMode.BackwardDiagonal, 
			2 => LinearGradientMode.ForwardDiagonal, 
			3 => LinearGradientMode.Vertical, 
			_ => LinearGradientMode.Horizontal, 
		});
		try
		{
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			StringFormat format = new StringFormat(StringFormatFlags.NoClip)
			{
				Alignment = StringAlignment.Center,
				LineAlignment = StringAlignment.Center
			};
			graphicsPath.AddString(randomcode, fontFamily, 1, 16f, new Point(1, 1), format);
			pen.LineJoin = LineJoin.Round;
			Point point = new Point(7, 12);
			float num5 = random_0.Next(-num, num);
			graphics.TranslateTransform(point.X, point.Y);
			graphics.RotateTransform(num5);
			graphics.DrawPath(pen, graphicsPath);
			graphics.FillPath(linearGradientBrush, graphicsPath);
			graphics.RotateTransform(0f - num5);
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Png);
			return memoryStream.ToArray();
		}
		finally
		{
			graphics.Dispose();
			bitmap.Dispose();
			fontFamily.Dispose();
			graphicsPath.Dispose();
			pen.Dispose();
			linearGradientBrush.Dispose();
		}
	}

	public static List<WindInfo> GetWind()
	{
		List<WindInfo> list = new List<WindInfo>();
		for (int i = 0; i < dictionary_0.Values.Count; i++)
		{
			list.Add(dictionary_0[i]);
		}
		if (list.Count > 0)
		{
			return list;
		}
		return null;
	}

	public static byte GetWindID(int wind, int pos)
	{
		if (wind < 10)
		{
			switch (pos)
			{
			case 1:
				return 10;
			case 3:
				return (byte)((wind == 0) ? 10 : ((byte)wind));
			}
		}
		if (wind >= 10 && wind < 20)
		{
			switch (pos)
			{
			case 1:
				return 1;
			case 3:
				return (byte)((wind - 10 == 0) ? 10 : ((byte)(wind - 10)));
			}
		}
		if (wind >= 20 && wind < 30)
		{
			switch (pos)
			{
			case 1:
				return 2;
			case 3:
				return (byte)((wind - 20 == 0) ? 10 : ((byte)(wind - 20)));
			}
		}
		if (wind >= 30 && wind < 40)
		{
			switch (pos)
			{
			case 1:
				return 3;
			case 3:
				return (byte)((wind - 30 == 0) ? 10 : ((byte)(wind - 30)));
			}
		}
		if (wind >= 40 && wind < 50)
		{
			switch (pos)
			{
			case 1:
				return 4;
			case 3:
				return (byte)((wind - 40 == 0) ? 10 : ((byte)(wind - 40)));
			}
		}
		if (wind >= 50 && wind < 60)
		{
			switch (pos)
			{
			case 1:
				return 5;
			case 3:
				return (byte)((wind - 50 == 0) ? 10 : ((byte)(wind - 50)));
			}
		}
		if (wind >= 60 && wind < 70)
		{
			switch (pos)
			{
			case 1:
				return 6;
			case 3:
				return (byte)((wind - 60 == 0) ? 10 : ((byte)(wind - 60)));
			}
		}
		return 0;
	}

	public static bool Init()
	{
		try
		{
			dictionary_0 = new Dictionary<int, WindInfo>();
			random_0 = new Random();
			return smethod_0(dictionary_0);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("WindInfoMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, WindInfo> ljEUxP05LfjbFcqfVa = new Dictionary<int, WindInfo>();
			if (smethod_0(ljEUxP05LfjbFcqfVa))
			{
				try
				{
					dictionary_0 = ljEUxP05LfjbFcqfVa;
					return true;
				}
				catch
				{
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("WindMgr", exception);
			}
		}
		return false;
	}

	private static bool smethod_0(Dictionary<int, WindInfo> ljEUxP05LfjbFcqfVa)
	{
		int[] array = int_1;
		foreach (int num in array)
		{
			WindInfo windInfo = new WindInfo();
			byte[] array2 = CreateImage(string_0[num]);
			if (array2 == null || array2.Length == 0)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Load Wind Error!");
				}
				return false;
			}
			windInfo.WindID = num;
			windInfo.WindPic = array2;
			if (!ljEUxP05LfjbFcqfVa.ContainsKey(num))
			{
				ljEUxP05LfjbFcqfVa.Add(num, windInfo);
			}
		}
		return true;
	}
}
