using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using log4net;

namespace Game.Server.Emblem;

public class EmblemMgr
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static EmblemPacketsOut Out = new EmblemPacketsOut();

	private static readonly ServerData ServerData = new ServerData();

	public static readonly PlayerData PlayerData = new PlayerData();

	public static List<TS_Emblem> Emblems { get; private set; }

	public static List<TS_Scroll> Scrolls { get; private set; }

	public static bool Init()
	{
		try
		{
			Emblems = ServerData.TS_Emblem.ToList();
			Scrolls = ServerData.TS_Scroll.ToList();
			if (Log.IsDebugEnabled)
			{
				Log.DebugFormat("Total emblems: {0}, Total scrolls: {1}", Emblems.Count, Scrolls.Count);
			}
			return true;
		}
		catch (Exception arg)
		{
			Log.ErrorFormat("EmblemMgr error: {0}", arg);
			return false;
		}
	}

	public static int AddItem(GamePlayer player, int templateId)
	{
		Random random = new Random();
		TS_Emblem tS_Emblem = Emblems.FirstOrDefault((TS_Emblem p) => p.TemplateID == templateId);
		if (tS_Emblem == null)
		{
			return 0;
		}
		string[] array = tS_Emblem.MainValue.Split('-');
		Sys_Users_Emblem sys_Users_Emblem = new Sys_Users_Emblem
		{
			UserID = player.PlayerId,
			tempId = templateId,
			isExist = true,
			equipPos = 0,
			equipIndex = 0,
			mainType = tS_Emblem.MainType,
			mainValue = random.Next(int.Parse(array[0]), int.Parse(array[1]))
		};
		List<string> list = new List<string>();
		List<int> list2 = new List<int>();
		list2 = ((tS_Emblem.MainType > 4 || 1 == 0) ? new List<int> { 5, 6, 7, 8 } : new List<int> { 1, 2, 3, 4 });
		for (int i = 0; i < tS_Emblem.SubCount; i++)
		{
			string[] array2 = tS_Emblem.SubValue.Split('-');
			list.Add(list2[random.Next(list2.Count)] + "," + random.Next(int.Parse(array2[0]), int.Parse(array2[1])));
		}
		sys_Users_Emblem.strvalue = string.Join("|", list);
		PlayerData playerData = new PlayerData();
		playerData.Sys_Users_Emblem.Add(sys_Users_Emblem);
		playerData.SaveChanges();
		player.EmblemInfos.Add(sys_Users_Emblem);
		Out.SendUpdateEmblem(player, sys_Users_Emblem);
		return sys_Users_Emblem.ID;
	}

	public static int AddScrollItem(GamePlayer player, int temlateId, int count)
	{
		TS_Scroll tS_Scroll = Scrolls.FirstOrDefault((TS_Scroll p) => p.TemplateID == temlateId);
		if (tS_Scroll == null)
		{
			return 0;
		}
		Sys_Users_Scroll sys_Users_Scroll = player.ScrollInfos.FirstOrDefault((Sys_Users_Scroll p) => p.TemplateId == temlateId);
		if (sys_Users_Scroll != null)
		{
			sys_Users_Scroll.Count += count;
		}
		else
		{
			sys_Users_Scroll = new Sys_Users_Scroll
			{
				UserID = player.PlayerId,
				TemplateId = temlateId,
				Count = count,
				isBind = true
			};
			PlayerData playerData = new PlayerData();
			playerData.Sys_Users_Scroll.Add(sys_Users_Scroll);
			playerData.SaveChanges();
			player.ScrollInfos.Add(sys_Users_Scroll);
		}
		Out.SendUpdateScroll(player);
		return sys_Users_Scroll.ID;
	}

	public static void SendProInfoHandler(GamePlayer player)
	{
		int fireAtt = 0;
		int waterAtt = 0;
		int windAtt = 0;
		int landAtt = 0;
		int fireDef = 0;
		int waterDef = 0;
		int windDef = 0;
		int landDef = 0;
		List<Sys_Users_Emblem> emblems = player.EmblemInfos.Where((Sys_Users_Emblem p) => p.isExist && p.equipPos != 0).ToList();
		player.PlayerCharacter.EmblemInfoAttackSKill = 0;
		player.PlayerCharacter.EmblemInfoDefendSKill = 0;
		CalculateProps(emblems, ref fireAtt, ref waterAtt, ref windAtt, ref landAtt, ref fireDef, ref waterDef, ref windDef, ref landDef);
		List<int> list = new List<int> { fireAtt, waterAtt, windAtt, landAtt };
		List<int> list2 = new List<int> { fireDef, waterDef, windDef, landDef };
		int num = list.Max();
		player.PlayerCharacter.EmblemInfoAttackMax = num;
		if (num != 0)
		{
			for (int i = 0; i < list.Count; i++)
			{
				if (list[i] == num)
				{
					switch (i)
					{
					case 0:
						player.PlayerCharacter.EmblemInfoAttackSKill = 817;
						player.PlayerCharacter.EmblemInfoAttackSKill_Rate = 3000;
						player.PlayerCharacter.EmblemInfoAttackSKill_Value = Convert.ToInt32((double)fireAtt * 1.6);
						break;
					case 1:
						player.PlayerCharacter.EmblemInfoAttackSKill = 818;
						player.PlayerCharacter.EmblemInfoAttackSKill_Rate = Convert.ToInt32(list.Sum() / 40) * 100;
						player.PlayerCharacter.EmblemInfoAttackSKill_Value = 0;
						break;
					case 2:
						player.PlayerCharacter.EmblemInfoAttackSKill = 819;
						player.PlayerCharacter.EmblemInfoAttackSKill_Rate = 3000;
						player.PlayerCharacter.EmblemInfoAttackSKill_Value = Convert.ToInt32((double)windAtt * 3.2);
						break;
					case 3:
						player.PlayerCharacter.EmblemInfoAttackSKill = 820;
						player.PlayerCharacter.EmblemInfoAttackSKill_Rate = 100;
						player.PlayerCharacter.EmblemInfoAttackSKill_Value = Convert.ToInt32(landAtt * 2);
						break;
					}
					break;
				}
			}
		}
		int num2 = list2.Max();
		player.PlayerCharacter.EmblemInfoDefendMax = num2;
		player.PlayerCharacter.EmblemInfoDefendSKill_Value = Convert.ToInt32((double)list2.Sum() / 1.6);
		player.PlayerCharacter.EmblemInfoDefendSKill_Rate = 20000;
		if (num2 != 0)
		{
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j] == num2)
				{
					switch (j)
					{
					case 0:
						player.PlayerCharacter.EmblemInfoDefendSKill = 821;
						break;
					case 1:
						player.PlayerCharacter.EmblemInfoDefendSKill = 822;
						player.PlayerCharacter.EmblemInfoDefendSKill_Value = Convert.ToInt32(list2.Sum() / 15);
						break;
					case 2:
						player.PlayerCharacter.EmblemInfoDefendSKill = 823;
						break;
					case 3:
						player.PlayerCharacter.EmblemInfoDefendSKill = 824;
						break;
					}
					break;
				}
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(655);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(fireAtt);
		gSPacketIn.WriteInt(windAtt);
		gSPacketIn.WriteInt(landAtt);
		gSPacketIn.WriteInt(waterAtt);
		gSPacketIn.WriteInt(fireDef);
		gSPacketIn.WriteInt(windDef);
		gSPacketIn.WriteInt(landDef);
		gSPacketIn.WriteInt(waterDef);
		for (int k = 0; k < 2; k++)
		{
			if (k == 0)
			{
				gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoAttackSKill);
				if (player.PlayerCharacter.EmblemInfoAttackSKill != 0)
				{
					gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoAttackSKill_Value);
					gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoAttackSKill_Rate);
				}
			}
			else
			{
				gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoDefendSKill);
				if (player.PlayerCharacter.EmblemInfoDefendSKill != 0)
				{
					gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoDefendSKill_Value);
					gSPacketIn.WriteInt(player.PlayerCharacter.EmblemInfoDefendSKill_Rate);
				}
			}
		}
		player.SendTCP(gSPacketIn);
	}

	public static void CalculateProps(List<Sys_Users_Emblem> emblems, ref int fireAtt, ref int waterAtt, ref int windAtt, ref int landAtt, ref int fireDef, ref int waterDef, ref int windDef, ref int landDef)
	{
		foreach (Sys_Users_Emblem emblem in emblems)
		{
			AddTypeValue(emblem.equipPos, emblem.mainType, emblem.mainValue, ref fireAtt, ref waterAtt, ref windAtt, ref landAtt, ref fireDef, ref waterDef, ref windDef, ref landDef);
			if (string.IsNullOrEmpty(emblem.strvalue))
			{
				continue;
			}
			List<string> list = emblem.strvalue.Split('|').ToList();
			foreach (string item in list)
			{
				string[] array = item.Split(',');
				AddTypeValue(emblem.equipPos, int.Parse(array[0]), int.Parse(array[1]), ref fireAtt, ref waterAtt, ref windAtt, ref landAtt, ref fireDef, ref waterDef, ref windDef, ref landDef);
			}
		}
	}

	public static void AddTypeValue(int place, int maintype, int value, ref int fireAtt, ref int waterAtt, ref int windAtt, ref int landAtt, ref int fireDef, ref int waterDef, ref int windDef, ref int landDef)
	{
		switch (place)
		{
		case 1:
		case 2:
			switch (maintype)
			{
			case 1:
				fireAtt += value;
				break;
			case 4:
				waterAtt += value;
				break;
			case 2:
				windAtt += value;
				break;
			case 3:
				landAtt += value;
				break;
			}
			break;
		case 3:
		case 4:
			switch (maintype)
			{
			case 5:
				fireDef += value;
				break;
			case 8:
				waterDef += value;
				break;
			case 6:
				windDef += value;
				break;
			case 7:
				landDef += value;
				break;
			}
			break;
		}
	}
}
