using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(133, "场景用户离开")]
public class LatentEnergyHandler : IPacketHandler
{
	public static Random random = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int place = packet.ReadInt();
		int num3 = -1;
		int num4 = -1;
		ItemInfo itemAt = client.Player.GetItemAt((eBageType)num2, place);
		if (!itemAt.CanLatentEnergy())
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LatentEnergyHandler.Msg1"));
			return 0;
		}
		ItemInfo itemInfo = null;
		PlayerInventory inventory = client.Player.GetInventory((eBageType)num2);
		string translation = LanguageMgr.GetTranslation("LatentEnergyHandler.Msg2");
		GSPacketIn gSPacketIn = new GSPacketIn(133, client.Player.PlayerCharacter.ID);
		if (num == 1)
		{
			num3 = packet.ReadInt();
			num4 = packet.ReadInt();
			itemInfo = client.Player.GetItemAt((eBageType)num3, num4);
			if (itemInfo == null || itemInfo.Count < 1)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("LatentEnergyHandler.Msg3"));
				return 0;
			}
			int num5 = int.Parse(itemAt.latentEnergyCurStr.Split(',')[0]);
			ItemTemplateInfo template = itemInfo.Template;
			if (itemAt.IsValidLatentEnergy() || num5 >= template.Property3 - 5 || num5 <= template.Property2 - 5)
			{
				itemAt.ResetLatentEnergy();
			}
			string text = random.Next(template.Property2, template.Property3).ToString();
			for (int i = 1; i < 4; i++)
			{
				text = text + "," + random.Next(template.Property2, template.Property3);
			}
			if (itemAt.latentEnergyCurStr.Split(',')[0] == "0")
			{
				itemAt.latentEnergyCurStr = text;
			}
			itemAt.latentEnergyNewStr = text;
			itemAt.latentEnergyEndTime = DateTime.Now.AddDays(7.0);
			PlayerInventory inventory2 = client.Player.GetInventory((eBageType)num3);
			inventory2.RemoveCountFromStack(itemInfo, 1);
		}
		else
		{
			itemAt.latentEnergyCurStr = itemAt.latentEnergyNewStr;
			translation = LanguageMgr.GetTranslation("LatentEnergyHandler.Msg4");
		}
		gSPacketIn.WriteInt(itemAt.Place);
		gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
		gSPacketIn.WriteString(itemAt.latentEnergyNewStr);
		gSPacketIn.WriteDateTime(itemAt.latentEnergyEndTime);
		itemAt.IsBinds = true;
		inventory.UpdateItem(itemAt);
		client.Player.EquipBag.UpdatePlayerProperties();
		client.Out.SendTCP(gSPacketIn);
		client.Player.SendMessage(translation);
		return 0;
	}
}
