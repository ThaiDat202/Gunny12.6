using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.Academy;

public class AcademyMgr
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly PlayerData PlayerData = new PlayerData();

	private static List<AcademyRequestInfo> Requests;

	public static readonly int Academy_FightPower = 10000000;

	public static readonly int MASTER_STATE = 2;

	public static readonly int MASTER_FULL_STATE = 3;

	public static object m_locker = new object();

	public static bool Init()
	{
		try
		{
			Requests = new List<AcademyRequestInfo>();
			return true;
		}
		catch (Exception arg)
		{
			Log.ErrorFormat("AcademyMgr error: {0}", arg);
			return false;
		}
	}

	public static void AddRequest(AcademyRequestInfo request)
	{
		lock (Requests)
		{
			Requests.Add(request);
		}
	}

	public static AcademyRequestInfo GetRequest(int senderid, int receiveid)
	{
		lock (Requests)
		{
			foreach (AcademyRequestInfo request in Requests)
			{
				if (request.SenderID == senderid && request.ReceiderID == receiveid)
				{
					return request;
				}
			}
			return null;
		}
	}

	public static void RemoveRequest(AcademyRequestInfo request)
	{
		lock (Requests)
		{
			Requests.Remove(request);
		}
	}

	public static void RemoveOldRequest()
	{
		List<AcademyRequestInfo> list = new List<AcademyRequestInfo>();
		lock (Requests)
		{
			foreach (AcademyRequestInfo request in Requests)
			{
				if (request.CreateTime.AddHours(1.0) < DateTime.Now)
				{
					list.Add(request);
				}
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (AcademyRequestInfo item in list)
		{
			RemoveRequest(item);
		}
	}

	public static bool CheckCanMaster(int fightPower)
	{
		return fightPower >= Academy_FightPower;
	}

	public static bool CheckCanApprentice(int fightPower)
	{
		return fightPower < Academy_FightPower;
	}

	public static int CheckDate(DateTime dateTime)
	{
		if (dateTime > DateTime.Now)
		{
			return (int)Math.Ceiling((dateTime - DateTime.Now).TotalHours);
		}
		return 0;
	}

	public static bool AddApprentice(GamePlayer master, GamePlayer apprentice)
	{
		lock (m_locker)
		{
			if (!master.PlayerCharacter.AcademyInfo.AddMasterOrApprentices(apprentice.PlayerCharacter.ID, apprentice.PlayerCharacter.NickName) && apprentice.PlayerCharacter.AcademyInfo.MasterID == 0)
			{
				return false;
			}
			apprentice.PlayerCharacter.AcademyInfo.MasterID = master.PlayerCharacter.ID;
			apprentice.PlayerCharacter.AcademyInfo.AddMasterOrApprentices(master.PlayerCharacter.ID, master.PlayerCharacter.NickName);
			apprentice.Out.SendAcademyApprenticeState(apprentice.PlayerCharacter, -1);
			master.Out.SendAcademyApprenticeState(master.PlayerCharacter, -1);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(70000010), 1, 101);
			itemInfo.IsBinds = true;
			apprentice.SendItemToMail(itemInfo, "获得拜师奖励", "获得拜师奖励", eMailType.Active);
			if (master.PlayerCharacter.AcademyInfo.IsFirst)
			{
				master.PlayerCharacter.AcademyInfo.IsFirst = false;
				ItemInfo item = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(70000011), 1, 101);
				itemInfo.IsBinds = true;
				apprentice.SendItemToMail(item, "获得首次收徒奖励", "获得首次收徒奖励", eMailType.Active);
			}
			PlayerData.Entry(apprentice.PlayerCharacter.AcademyInfo).State = EntityState.Modified;
			PlayerData.Entry(master.PlayerCharacter.AcademyInfo).State = EntityState.Modified;
			PlayerData.SaveChanges();
			apprentice.OnAcademyEvent(master, 0);
			master.OnAcademyEvent(apprentice, 1);
			return true;
		}
	}

	public static bool FireMaster(GamePlayer player, bool isComplete)
	{
		lock (m_locker)
		{
			if (player.PlayerCharacter.AcademyInfo.RemoveMasterOrApprentices(player.PlayerCharacter.AcademyInfo.MasterID))
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(player.PlayerCharacter.AcademyInfo.MasterID);
				PlayerBussiness playerBussiness = new PlayerBussiness();
				PlayerInfo playerInfo = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(player.PlayerCharacter.AcademyInfo.MasterID) : playerById.PlayerCharacter);
				if (playerInfo == null)
				{
					return false;
				}
				if (isComplete)
				{
					playerInfo.AcademyInfo.GraduatesCount++;
					int templateId = 0;
					bool flag = false;
					switch (playerInfo.AcademyInfo.GraduatesCount)
					{
					case 3:
						templateId = 70000017;
						flag = true;
						break;
					case 6:
						templateId = 70000018;
						flag = true;
						break;
					case 9:
						templateId = 70000019;
						flag = true;
						break;
					case 12:
						templateId = 70000020;
						flag = true;
						break;
					case 15:
						templateId = 70000021;
						flag = true;
						break;
					case 20:
						templateId = 70000022;
						flag = true;
						break;
					}
					if (flag)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 101);
						itemInfo.IsBinds = true;
						playerById.SendItemToMail(itemInfo, $"出徒数量:{playerInfo.AcademyInfo.GraduatesCount} 的奖励", $"出徒数量:{playerInfo.AcademyInfo.GraduatesCount} 的奖励", eMailType.Active);
						GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
						for (int i = 0; i < allPlayers.Length; i++)
						{
							allPlayers[i].SendMessage($"恭喜玩家[{playerInfo.NickName}]出徒数量达到{playerInfo.AcademyInfo.GraduatesCount}个,奖励道具:{itemInfo.Template.Name}");
						}
					}
					playerById?.OnAcademyEvent(player, 2);
					player.OnAcademyEvent(null, 3);
				}
				playerInfo.AcademyInfo.RemoveMasterOrApprentices(player.PlayerId);
				PlayerData.Entry(playerInfo.AcademyInfo).State = EntityState.Modified;
				PlayerData.SaveChanges();
				if (playerById != null)
				{
					if (!isComplete)
					{
						playerById.Out.SendAcademySystemNotice($"你已解除与[{player.PlayerCharacter.NickName}]的师徒关系.", isAlert: true);
					}
					playerById.Out.SendAcademyApprenticeState(playerInfo, player.PlayerCharacter.ID);
				}
				player.PlayerCharacter.AcademyInfo.MasterID = 0;
				return true;
			}
			return false;
		}
	}

	public static bool FireApprentice(GamePlayer player, int uid, bool isSilent)
	{
		lock (m_locker)
		{
			if (player.PlayerCharacter.AcademyInfo.RemoveMasterOrApprentices(uid))
			{
				GamePlayer playerById = WorldMgr.GetPlayerById(uid);
				PlayerBussiness playerBussiness = new PlayerBussiness();
				PlayerInfo playerInfo = ((playerById == null) ? playerBussiness.GetUserSingleByUserID(uid) : playerById.PlayerCharacter);
				if (playerInfo == null)
				{
					return false;
				}
				playerInfo.AcademyInfo.RemoveMasterOrApprentices(playerInfo.AcademyInfo.MasterID);
				playerInfo.AcademyInfo.MasterID = 0;
				PlayerData.Entry(playerInfo.AcademyInfo).State = EntityState.Modified;
				PlayerData.SaveChanges();
				if (playerById == null)
				{
					return false;
				}
				if (!isSilent)
				{
					playerById.Out.SendAcademySystemNotice($"你已解除与[{player.PlayerCharacter.NickName}]的师徒关系.", isAlert: true);
				}
				playerById.Out.SendAcademyApprenticeState(playerInfo, uid);
				return true;
			}
			return false;
		}
	}

	public static void SendAwards(GamePlayer player, int fightPower)
	{
		int[] array = new int[4] { 70000013, 70000014, 70000015, 70000016 };
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[0]), 1, 101);
		itemInfo.IsBinds = true;
		player.SendItemToMail(itemInfo, string.Format("战斗力到达{0}的无师自通(师徒)奖励", "100万"), string.Format("战斗力到达{0}的无师自通(师徒)奖励", "100万"), eMailType.Active);
		ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[1]), 1, 101);
		itemInfo2.IsBinds = true;
		player.SendItemToMail(itemInfo2, string.Format("战斗力到达{0}的无师自通(师徒)奖励", "300万"), string.Format("战斗力到达{0}的无师自通(师徒)奖励", "300万"), eMailType.Active);
		ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[2]), 1, 101);
		itemInfo3.IsBinds = true;
		player.SendItemToMail(itemInfo3, string.Format("战斗力到达{0}的无师自通(师徒)奖励", "500万"), string.Format("战斗力到达{0}的无师自通(师徒)奖励", "500万"), eMailType.Active);
		ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[3]), 1, 101);
		itemInfo4.IsBinds = true;
		player.SendItemToMail(itemInfo4, "无师自通(出师礼包)", "无师自通(出师礼包)", eMailType.Active);
		ItemInfo itemInfo5 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(70000010), 1, 101);
		itemInfo5.IsBinds = true;
		player.SendItemToMail(itemInfo5, "无师自通(拜师礼包)", "无师自通(拜师礼包)", eMailType.Active);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		for (int i = 0; i < allPlayers.Length; i++)
		{
			allPlayers[i].SendMessage($"恭喜玩家[{player.PlayerCharacter.NickName}]无师自通,战斗力已达到1000万~~,大家也要再接再厉哦~~!");
		}
	}

	public static void UpdateAwardApp(GamePlayer player, int fightPower)
	{
		int[] array = new int[4] { 70000013, 70000014, 70000015, 70000016 };
		int masterID = player.PlayerCharacter.AcademyInfo.MasterID;
		GamePlayer playerById = WorldMgr.GetPlayerById(masterID);
		if (fightPower >= 1000000 && !player.PlayerCharacter.AcademyInfo.Get100)
		{
			player.PlayerCharacter.AcademyInfo.Get100 = true;
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[0]), 1, 101);
			itemInfo.IsBinds = true;
			player.SendItemToMail(itemInfo, string.Format("战斗力到达{0}的师徒奖励", "100万"), string.Format("战斗力到达{0}的师徒奖励", "100万"), eMailType.Active);
			playerById.SendItemToMail(itemInfo, string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "100万"), string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "100万"), eMailType.Active);
		}
		if (fightPower >= 3000000 && !player.PlayerCharacter.AcademyInfo.Get300)
		{
			player.PlayerCharacter.AcademyInfo.Get300 = true;
			ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[1]), 1, 101);
			itemInfo2.IsBinds = true;
			player.SendItemToMail(itemInfo2, string.Format("战斗力到达{0}的师徒奖励", "300万"), string.Format("战斗力到达{0}的师徒奖励", "300万"), eMailType.Active);
			playerById.SendItemToMail(itemInfo2, string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "300万"), string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "300万"), eMailType.Active);
		}
		if (fightPower >= 5000000 && !player.PlayerCharacter.AcademyInfo.Get500)
		{
			player.PlayerCharacter.AcademyInfo.Get500 = true;
			ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[2]), 1, 101);
			itemInfo3.IsBinds = true;
			player.SendItemToMail(itemInfo3, string.Format("战斗力到达{0}的师徒奖励", "500万"), string.Format("战斗力到达{0}的师徒奖励", "500万"), eMailType.Active);
			playerById.SendItemToMail(itemInfo3, string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "500万"), string.Format("你的徒弟[{0}]战斗力到达{1}的师徒奖励", player.PlayerCharacter.NickName, "500万"), eMailType.Active);
		}
		if (player.PlayerCharacter.FightPower < Academy_FightPower || player.PlayerCharacter.AcademyInfo.Get1000)
		{
			return;
		}
		player.PlayerCharacter.AcademyInfo.Get1000 = true;
		PlayerInfo playerInfo = null;
		if (playerById != null)
		{
			playerById.Out.SendAcademyGradute(player, 1);
			playerInfo = playerById.PlayerCharacter;
		}
		else
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerInfo = playerBussiness.GetUserSingleByUserID(masterID);
		}
		ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(array[3]), 1, 101);
		itemInfo4.IsBinds = true;
		player.SendItemToMail(itemInfo4, string.Format("恭喜你战斗力到达{0}获得出师奖励", "1000万"), string.Format("恭喜你战斗力到达{0}获得出师奖励", "1000万"), eMailType.Active);
		playerById.SendItemToMail(itemInfo4, string.Format("你的徒弟[{0}]战斗力到达{1}获得出师奖励", player.PlayerCharacter.NickName, "1000万"), string.Format("你的徒弟[{0}]战斗力到达{1}获得出师奖励", player.PlayerCharacter.NickName, "1000万"), eMailType.Active);
		FireMaster(player, isComplete: true);
		PlayerData.Entry(player.PlayerCharacter.AcademyInfo).State = EntityState.Modified;
		PlayerData.SaveChanges();
		player.Out.SendAcademyApprenticeState(player.PlayerCharacter, masterID);
		player.Out.SendAcademyGradute(player, 0);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		for (int i = 0; i < allPlayers.Length; i++)
		{
			allPlayers[i].SendMessage($"恭喜[{playerInfo.NickName}]的徒弟[{player.PlayerCharacter.NickName}]战斗力已到达1000万,现已出师,大家也要再接再厉哦~~!");
		}
	}
}
