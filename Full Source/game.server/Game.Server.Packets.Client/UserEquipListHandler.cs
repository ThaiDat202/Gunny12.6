using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(74, "获取用户装备")]
public class UserEquipListHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int id = 0;
		string text = "";
		GamePlayer gamePlayer;
		if (flag)
		{
			id = packet.ReadInt();
			gamePlayer = WorldMgr.GetPlayerById(id);
		}
		else
		{
			text = packet.ReadString();
			gamePlayer = WorldMgr.GetClientByPlayerNickName(text);
		}
		PlayerInfo info;
		List<ItemInfo> list;
		List<ItemInfo> list2;
		List<UserGemStone> list3;
		List<ItemInfo> list4;
		ExplorerManualInfo explorerManualInfo;
		List<MarkChipItemInfo> list5;
		List<UsersPetLacrarInfo> list6;
		List<Sys_Users_Emblem> list7;
		ICollection<Sys_Users_Elf> collection;
		Sys_Users_Academy sys_Users_Academy;
		if (gamePlayer != null && id > 0)
		{
			PlayerBussiness playerBussiness = new PlayerBussiness();
			PlayerData playerData = new PlayerData();
			info = gamePlayer.PlayerCharacter;
			list = gamePlayer.EquipBag.GetItems(0, 31);
			list2 = gamePlayer.BeadBag.GetItems(0, 31);
			list3 = gamePlayer.GemStone;
			list4 = gamePlayer.MagicStoneBag.GetItems(0, 31);
			explorerManualInfo = info.explorerManualInfo;
			if (explorerManualInfo == null)
			{
				explorerManualInfo = playerBussiness.InitExplorerManualInfo(id);
			}
			list5 = gamePlayer.Marks.ChipBags[1];
			list6 = gamePlayer.PetBag.lacresPet;
			if (list6 == null)
			{
				list6 = playerBussiness.GetUserLacresPet(id);
			}
			list7 = gamePlayer.EmblemInfos;
			if (list7 == null)
			{
				list7 = playerData.Sys_Users_Emblem.Where((Sys_Users_Emblem p) => p.UserID == info.ID && p.isExist).ToList();
			}
			collection = info.ElfInfo;
			if (collection == null)
			{
				collection = playerData.Sys_Users_Elf.Where((Sys_Users_Elf p) => p.UserID == info.ID).ToList();
			}
			Sys_Users_Team team = info.Team;
			if (team == null)
			{
				info.Team = (from q in playerData.Sys_Users_Team
					join p in playerData.Sys_Users_Team_User on q.teamID equals p.teamID
					where p.UserID == id
					select q).FirstOrDefault();
				if (info.Team == null)
				{
					info.Team = new Sys_Users_Team();
					info.Team.teamID = 0;
				}
			}
			sys_Users_Academy = info.AcademyInfo;
			if (sys_Users_Academy == null)
			{
				sys_Users_Academy = playerData.Sys_Users_Academy.FirstOrDefault((Sys_Users_Academy p) => p.UserID == info.ID);
				if (sys_Users_Academy == null)
				{
					sys_Users_Academy = new Sys_Users_Academy();
					sys_Users_Academy.UserID = info.ID;
					sys_Users_Academy.IsFirst = true;
					sys_Users_Academy.MasterOrApprentices = "";
					sys_Users_Academy.HonourOfMaster = "";
					sys_Users_Academy.FreezesDate = DateTime.Now.AddDays(-7.0);
				}
			}
		}
		else if (id < 0)
		{
			id = Math.Abs(id);
			using PlayerBussiness playerBussiness2 = new PlayerBussiness();
			PlayerData playerData2 = new PlayerData();
			info = playerBussiness2.GetUserSingleByUserID(id);
			if (info == null)
			{
				return 0;
			}
			info.ZoneID = client.Player.ZoneId;
			info.ZoneName = client.Player.ZoneName;
			list = playerBussiness2.GetUserEuqip(id);
			list2 = playerBussiness2.GetUserBeadEuqip(id);
			info.Texp = playerBussiness2.GetUserTexpInfoSingle(id);
			list3 = playerBussiness2.GetSingleGemStones(id);
			list4 = playerBussiness2.GetUserMagicstoneEuqip(id);
			explorerManualInfo = playerBussiness2.InitExplorerManualInfo(id);
			List<MarkChipItemInfo> allUsersMark = playerBussiness2.GetAllUsersMark(id);
			list5 = new List<MarkChipItemInfo>();
			foreach (MarkChipItemInfo item in allUsersMark)
			{
				if (item.Type == 1)
				{
					list5.Add(item);
				}
			}
			list6 = playerBussiness2.GetUserLacresPet(id);
			list7 = playerData2.Sys_Users_Emblem.Where((Sys_Users_Emblem p) => p.UserID == info.ID && p.isExist).ToList();
			collection = playerData2.Sys_Users_Elf.Where((Sys_Users_Elf p) => p.UserID == info.ID).ToList();
			info.Team = (from q in playerData2.Sys_Users_Team
				join p in playerData2.Sys_Users_Team_User on q.teamID equals p.teamID
				where p.UserID == id
				select q).FirstOrDefault();
			if (info.Team == null)
			{
				info.Team = new Sys_Users_Team();
				info.Team.teamID = 0;
			}
			sys_Users_Academy = playerData2.Sys_Users_Academy.FirstOrDefault((Sys_Users_Academy p) => p.UserID == info.ID);
			if (sys_Users_Academy == null)
			{
				sys_Users_Academy = new Sys_Users_Academy();
				sys_Users_Academy.UserID = info.ID;
				sys_Users_Academy.IsFirst = true;
				sys_Users_Academy.MasterOrApprentices = "";
				sys_Users_Academy.HonourOfMaster = "";
				sys_Users_Academy.FreezesDate = DateTime.Now.AddDays(-7.0);
			}
		}
		else
		{
			using PlayerBussiness playerBussiness3 = new PlayerBussiness();
			PlayerData playerData3 = new PlayerData();
			info = playerBussiness3.GetUserSingleByUserID(id);
			if (info == null)
			{
				return 0;
			}
			info.ZoneID = client.Player.ZoneId;
			info.ZoneName = client.Player.ZoneName;
			list = playerBussiness3.GetUserEuqip(id);
			list2 = playerBussiness3.GetUserBeadEuqip(id);
			info.Texp = playerBussiness3.GetUserTexpInfoSingle(id);
			list3 = playerBussiness3.GetSingleGemStones(id);
			list4 = playerBussiness3.GetUserMagicstoneEuqip(id);
			explorerManualInfo = playerBussiness3.InitExplorerManualInfo(id);
			List<MarkChipItemInfo> allUsersMark2 = playerBussiness3.GetAllUsersMark(id);
			list5 = new List<MarkChipItemInfo>();
			foreach (MarkChipItemInfo item2 in allUsersMark2)
			{
				if (item2.Type == 1)
				{
					list5.Add(item2);
				}
			}
			list6 = playerBussiness3.GetUserLacresPet(id);
			list7 = playerData3.Sys_Users_Emblem.Where((Sys_Users_Emblem p) => p.UserID == info.ID && p.isExist).ToList();
			collection = playerData3.Sys_Users_Elf.Where((Sys_Users_Elf p) => p.UserID == info.ID).ToList();
			info.Team = (from q in playerData3.Sys_Users_Team
				join p in playerData3.Sys_Users_Team_User on q.teamID equals p.teamID
				where p.UserID == id
				select q).FirstOrDefault();
			if (info.Team == null)
			{
				info.Team = new Sys_Users_Team();
				info.Team.teamID = 0;
			}
			sys_Users_Academy = playerData3.Sys_Users_Academy.FirstOrDefault((Sys_Users_Academy p) => p.UserID == info.ID);
			if (sys_Users_Academy == null)
			{
				sys_Users_Academy = new Sys_Users_Academy();
				sys_Users_Academy.UserID = info.ID;
				sys_Users_Academy.IsFirst = true;
				sys_Users_Academy.MasterOrApprentices = "";
				sys_Users_Academy.HonourOfMaster = "";
				sys_Users_Academy.FreezesDate = DateTime.Now.AddDays(-7.0);
			}
		}
		if (info != null && list != null && list2 != null && list3 != null && list4 != null && explorerManualInfo != null)
		{
			client.Player.Out.SendUserEquip(info, list, list3, list2, list4, explorerManualInfo, list5, list6, list7, collection, sys_Users_Academy);
			if (info.ID != client.Player.PlayerCharacter.ID)
			{
				client.Player.PlayerProp.ViewOther(info);
			}
		}
		return 0;
	}
}
