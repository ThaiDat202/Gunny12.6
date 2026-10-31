using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(374, "大喇叭")]
public class OpenAmuletBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadByte();
		int slot = packet.ReadInt();
		int num = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		if (inventory != null)
		{
			ItemInfo itemAt = inventory.GetItemAt(slot);
			if (itemAt != null && itemAt.Count >= num && itemAt.Template.CategoryID == 68)
			{
				int templateID = itemAt.TemplateID;
				if (inventory.RemoveCountFromStack(itemAt, num))
				{
					List<ItemInfo> list = new List<ItemInfo>();
					SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
					for (int i = 0; i < num; i++)
					{
						ItemBoxMgr.CreateItemBox(templateID, list, ref specialValue);
					}
					if (list.Count > 0)
					{
						client.Player.AddTemplate(list);
						client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.OpenAmuletBox.Success.Msg"));
					}
				}
			}
		}
		return 0;
	}
}
