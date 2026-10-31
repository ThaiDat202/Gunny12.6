using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.DevilTurn;

public class DevilTurnPacketsOut
{
	public void SendUpdateJackpot(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteLong(DevilTurnMgr.JackBox);
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateInfo(GamePlayer player)
	{
		int num = 0;
		if (player.PlayerCharacter.DevilTurn.getGiftProgress != "")
		{
			string[] array = player.PlayerCharacter.DevilTurn.getGiftProgress.Split(',');
			string[] array2 = array;
			foreach (string value in array2)
			{
				num += 1 << Convert.ToInt32(value);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.lotteryCount);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.myRankScore);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.beadCount1);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.beadCount2);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.beadCount3);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.beadCount4);
		gSPacketIn.WriteInt(player.PlayerCharacter.DevilTurn.beadCount5);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteByte((byte)player.PlayerCharacter.DevilTurn.freeTimes);
		DateTime dateTime = new DateTime(1970, 1, 1);
		int val = Convert.ToInt32((player.PlayerCharacter.DevilTurn.freeDate - dateTime).TotalMilliseconds / 1000.0);
		if (player.PlayerCharacter.DevilTurn.freeDate < DateTime.Now)
		{
			val = 0;
		}
		gSPacketIn.WriteInt(val);
		player.SendTCP(gSPacketIn);
		DevilTurnMgr.UpdateDevilTurnRank(player.PlayerCharacter.ID, GameServer.Instance.Configuration.AreaId);
		SendUpdateBoxList(player);
		SendUpdateRankList(player);
	}

	public void SendUpdateBoxList(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(24);
		List<Sys_Users_ListBox> list = player.PlayerCharacter.ListBoxs.Where((Sys_Users_ListBox p) => p.Count > 0).ToList();
		gSPacketIn.WriteInt(list.Count);
		foreach (Sys_Users_ListBox item in list)
		{
			if (item.OpenDate.Year > 1970 && item.OpenDate < DateTime.Now)
			{
				item.Count--;
				item.OpenDate = Convert.ToDateTime("1970-01-01");
			}
			if (item.Count < 0)
			{
				item.Count = 0;
			}
			DateTime dateTime = Convert.ToDateTime("1970-01-01 07:00:00");
			int val = 0;
			if (item.OpenDate > DateTime.Now)
			{
				val = Convert.ToInt32((item.OpenDate - dateTime).TotalMilliseconds / 1000.0);
			}
			gSPacketIn.WriteInt(item.BoxId);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(val);
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateRankList(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(player.PlayerCharacter.DvRank);
		gSPacketIn.WriteShort((short)DevilTurnMgr.DevilTurn_Ranks.Count);
		int num = 1;
		foreach (Sys_Users_DevilTurn_Rank item in DevilTurnMgr.DevilTurn_Ranks.OrderByDescending((Sys_Users_DevilTurn_Rank p) => p.Score))
		{
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteString(item.NickName);
			gSPacketIn.WriteString(item.ServerName);
			gSPacketIn.WriteInt(item.Score);
			gSPacketIn.WriteInt(num);
			num++;
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateDiceBoxView(GamePlayer player, Sys_Users_ListBox box)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(23);
		gSPacketIn.WriteByte((byte)box.state);
		gSPacketIn.WriteByte((byte)box.boxIndex);
		gSPacketIn.WriteInt(box.subtractMin);
		gSPacketIn.WriteByte((byte)box.min);
		gSPacketIn.WriteByte((byte)box.max);
		gSPacketIn.WriteInt(box.subtractMax);
		gSPacketIn.WriteInt(box.needmoney);
		gSPacketIn.WriteByte((byte)box.currentIndex);
		player.SendTCP(gSPacketIn);
	}

	public void SendDiceResult(GamePlayer player, byte from, byte to)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(16);
		gSPacketIn.WriteByte(from);
		gSPacketIn.WriteByte(to);
		player.SendTCP(gSPacketIn);
	}

	public void SendDevilTurnOpen(GamePlayer player)
	{
		int num = (int)(Convert.ToDateTime(GameProperties.DevilTreasureEndDate) - DateTime.Now).TotalDays;
		byte val = 0;
		if (num == 1)
		{
			val = 2;
		}
		else if (num > 1)
		{
			val = 1;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(608, player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteByte(val);
		player.SendTCP(gSPacketIn);
	}
}
