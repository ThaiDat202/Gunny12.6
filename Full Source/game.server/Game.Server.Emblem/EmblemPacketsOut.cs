using System.Collections.Generic;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Emblem;

public class EmblemPacketsOut
{
	public void SendUpdateEmblem(GamePlayer player, List<Sys_Users_Emblem> items)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(655, player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(items.Count);
		foreach (Sys_Users_Emblem item in items)
		{
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteInt(item.tempId);
			gSPacketIn.WriteBoolean(item.isExist);
			gSPacketIn.WriteInt(item.equipPos);
			gSPacketIn.WriteInt(item.equipIndex);
			gSPacketIn.WriteInt(item.mainType);
			gSPacketIn.WriteInt(item.mainValue);
			if (!string.IsNullOrEmpty(item.strvalue))
			{
				string[] array = item.strvalue.Split('|');
				gSPacketIn.WriteInt(array.Length);
				string[] array2 = array;
				foreach (string text in array2)
				{
					gSPacketIn.WriteInt(int.Parse(text.Split(',')[0]));
					gSPacketIn.WriteInt(int.Parse(text.Split(',')[1]));
				}
			}
			else
			{
				gSPacketIn.WriteInt(0);
			}
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateEmblem(GamePlayer player, Sys_Users_Emblem userEmblemInfo)
	{
		SendUpdateEmblem(player, new List<Sys_Users_Emblem> { userEmblemInfo });
	}

	public void SendUpdateEmblem(GamePlayer player)
	{
		SendUpdateEmblem(player, player.EmblemInfos);
	}

	public void SendUpdateScroll(GamePlayer player, List<Sys_Users_Scroll> scrollInfos)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(655, player.PlayerId);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(scrollInfos.Count);
		foreach (Sys_Users_Scroll scrollInfo in scrollInfos)
		{
			gSPacketIn.WriteInt(scrollInfo.TemplateId);
			gSPacketIn.WriteInt(scrollInfo.isBind ? 1 : 0);
			gSPacketIn.WriteInt(scrollInfo.Count);
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendComposeSuccess(GamePlayer player, bool isSuccess, int newId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(655, player.PlayerId);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteBoolean(isSuccess);
		gSPacketIn.WriteInt(newId);
		player.SendTCP(gSPacketIn);
	}

	public void SendForgiveSuccess(GamePlayer player, int id)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(655, player.PlayerId);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(id);
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateScroll(GamePlayer player, Sys_Users_Scroll userScrollInfo)
	{
		SendUpdateScroll(player, new List<Sys_Users_Scroll> { userScrollInfo });
	}

	public void SendUpdateScroll(GamePlayer player)
	{
		SendUpdateScroll(player, player.ScrollInfos);
	}
}
