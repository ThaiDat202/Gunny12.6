using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(119, "物品比较")]
public class GetLinkGoodsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		ItemInfo itemInfo = null;
		int num = packet.ReadInt();
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		GSPacketIn gSPacketIn = new GSPacketIn(119, client.Player.PlayerCharacter.ID);
		string nickName = client.Player.PlayerCharacter.NickName;
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		gSPacketIn.WriteInt(num);
		switch (num)
		{
		case 4:
			gSPacketIn.WriteString(nickName);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		case 5:
			text = packet.ReadString();
			gSPacketIn.WriteString(nickName);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		default:
			if (text2 == "")
			{
				text2 = text;
			}
			itemInfo = playerBussiness.GetUserItemSingle(Convert.ToInt32(text2));
			if (itemInfo != null)
			{
				gSPacketIn.WriteString(nickName);
				gSPacketIn.WriteInt(itemInfo.TemplateID);
				gSPacketIn.WriteInt(itemInfo.ItemID);
				gSPacketIn.WriteInt(itemInfo.StrengthenLevel);
				gSPacketIn.WriteInt(itemInfo.AttackCompose);
				gSPacketIn.WriteInt(itemInfo.AgilityCompose);
				gSPacketIn.WriteInt(itemInfo.LuckCompose);
				gSPacketIn.WriteInt(itemInfo.DefendCompose);
				gSPacketIn.WriteInt(itemInfo.MagicAttack);
				gSPacketIn.WriteInt(itemInfo.MagicDefence);
				gSPacketIn.WriteInt(itemInfo.ValidDate);
				gSPacketIn.WriteBoolean(itemInfo.IsBinds);
				gSPacketIn.WriteBoolean(itemInfo.IsJudge);
				gSPacketIn.WriteBoolean(itemInfo.IsUsed);
				if (itemInfo.IsUsed)
				{
					gSPacketIn.WriteString(itemInfo.BeginDate.ToString());
				}
				gSPacketIn.WriteInt(itemInfo.Hole1);
				gSPacketIn.WriteInt(itemInfo.Hole2);
				gSPacketIn.WriteInt(itemInfo.Hole3);
				gSPacketIn.WriteInt(itemInfo.Hole4);
				gSPacketIn.WriteInt(itemInfo.Hole5);
				gSPacketIn.WriteInt(itemInfo.Hole6);
				gSPacketIn.WriteString(itemInfo.Template.Hole);
				gSPacketIn.WriteString(itemInfo.Template.Pic);
				gSPacketIn.WriteInt(itemInfo.RefineryLevel);
				gSPacketIn.WriteDateTime(DateTime.Now);
				gSPacketIn.WriteByte((byte)itemInfo.Hole5Level);
				gSPacketIn.WriteInt(itemInfo.Hole5Exp);
				gSPacketIn.WriteByte((byte)itemInfo.Hole6Level);
				gSPacketIn.WriteInt(itemInfo.Hole6Exp);
				gSPacketIn.WriteBoolean(itemInfo.GoldValidDate());
				if (itemInfo.GoldValidDate())
				{
					gSPacketIn.WriteInt(itemInfo.goldValidDate);
					gSPacketIn.WriteDateTime(itemInfo.goldBeginTime);
				}
				gSPacketIn.WriteInt(itemInfo.MagicLevel);
				client.Out.SendTCP(gSPacketIn);
			}
			return 1;
		}
	}
}
