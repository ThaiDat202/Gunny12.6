using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using log4net;
using SqlDataProvider.Data;

namespace Game.Logic;

public class BallMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, BallInfo> m_infos;

	private static Dictionary<int, Tile> m_tiles;

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, BallInfo> dictionary = LoadFromDatabase();
			Dictionary<int, Tile> dictionary2 = LoadFromFiles(dictionary);
			if (dictionary.Values.Count > 0 && dictionary2.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_infos, dictionary);
				Interlocked.Exchange(ref m_tiles, dictionary2);
				return true;
			}
			Console.WriteLine("ballcount: " + dictionary.Count + "|" + dictionary2.Count);
		}
		catch (Exception exception)
		{
			log.Error("Ball Mgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, BallInfo> LoadFromDatabase()
	{
		Dictionary<int, BallInfo> dictionary = new Dictionary<int, BallInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			BallInfo[] allBall = produceBussiness.GetAllBall();
			BallInfo[] array = allBall;
			foreach (BallInfo ballInfo in array)
			{
				if (!dictionary.ContainsKey(ballInfo.ID))
				{
					dictionary.Add(ballInfo.ID, ballInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
	{
		Dictionary<int, Tile> dictionary = new Dictionary<int, Tile>();
		foreach (BallInfo value in list.Values)
		{
			if (value.HasTunnel)
			{
				string text = $"bomb\\{value.ID}.bomb";
				Tile tile = null;
				if (File.Exists(text))
				{
					tile = new Tile(text, digable: false);
				}
				dictionary.Add(value.ID, tile);
				if (tile == null && value.Crater != string.Empty && value.Crater != "0")
				{
					log.ErrorFormat("can't find bomb file:{0}", text);
				}
			}
		}
		return dictionary;
	}

	public static BallInfo FindBall(int id)
	{
		if (m_infos.ContainsKey(id))
		{
			return m_infos[id];
		}
		return null;
	}

	public static Tile FindTile(int id)
	{
		if (m_tiles.ContainsKey(id))
		{
			return m_tiles[id];
		}
		return null;
	}

	public static BombType GetBallType(int ballId)
	{
		switch (ballId)
		{
		case 1:
		case 56:
		case 99:
			return BombType.FORZEN;
		case 3:
			return BombType.FLY;
		case 5:
		case 59:
		case 64:
		case 97:
		case 98:
		case 120:
		case 298:
		case 10009:
			return BombType.CURE;
		case 110:
		case 117:
			return BombType.WORLDCUP;
		case 128:
		case 129:
			return BombType.CATCHINSECT;
		default:
			return BombType.Normal;
		}
	}
}
