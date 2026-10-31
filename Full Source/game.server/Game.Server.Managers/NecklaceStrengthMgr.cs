using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;
using MoreLinq;
using SqlDataProvider.Data;

namespace Game.Server.Managers;

public static class NecklaceStrengthMgr
{
	public static Dictionary<int, StrengThenExp> NecklaceExps = new Dictionary<int, StrengThenExp>();

	public static List<TS_NecklaceCasting> ts_NecklaceCasting;

	private static ServerData db = new ServerData();

	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static bool Init()
	{
		try
		{
			ts_NecklaceCasting = db.TS_NecklaceCasting.ToList();
			db.StrengThenExp.ForEach(delegate(StrengThenExp e)
			{
				if (!NecklaceExps.ContainsKey(e.Level))
				{
					NecklaceExps.Add(e.Level, e);
				}
			});
			return true;
		}
		catch (Exception arg)
		{
			if (Log.IsErrorEnabled)
			{
				Log.ErrorFormat("Init error: {0}", arg);
			}
			return false;
		}
	}

	public static int GetNecklaceLevelByGP(int exp)
	{
		foreach (KeyValuePair<int, StrengThenExp> necklaceExp in NecklaceExps)
		{
			if (necklaceExp.Value.NecklaceStrengthExp > exp)
			{
				return necklaceExp.Key - 1;
			}
		}
		return -1;
	}

	public static StrengThenExp FindStrengThenExpInfo(int level)
	{
		return NecklaceExps.ContainsKey(level) ? NecklaceExps[level] : null;
	}

	public static void UpdateNecklaceInfo(GamePlayer player)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(11160);
		int necklaceLevelByGP = GetNecklaceLevelByGP(player.PlayerCharacter.necklaceExp);
		int num = ((necklaceLevelByGP != -1) ? (FindStrengThenExpInfo(necklaceLevelByGP + 1).NecklaceStrengthExp - player.PlayerCharacter.necklaceExp) : 0);
		int needCount = (int)Math.Ceiling((float)num / (float)itemTemplateInfo.Property2);
		SendNecklaceInfo(player, 0, needCount);
	}

	public static void SendNecklaceInfo(GamePlayer player, int addExp, int needCount)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(95, player.PlayerId);
		gSPacketIn.WriteInt(player.PlayerCharacter.necklaceExp);
		gSPacketIn.WriteInt(addExp);
		gSPacketIn.WriteInt(needCount);
		player.SendTCP(gSPacketIn);
	}

	public static void SendNecklaceCastInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(661, player.PlayerId);
		gSPacketIn.WriteInt(player.PlayerCharacter.necklaceCastLevel);
		player.SendTCP(gSPacketIn);
	}
}
