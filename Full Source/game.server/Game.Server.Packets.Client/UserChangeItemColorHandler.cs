using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(182, "改变物品颜色")]
public class UserChangeItemColorHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eMessageType type = eMessageType.Normal;
		string translateId = "UserChangeItemColorHandler.Success";
		int num = packet.ReadInt();
		int slot = packet.ReadInt();
		int num2 = packet.ReadInt();
		int slot2 = packet.ReadInt();
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		int num3 = packet.ReadInt();
		ItemInfo itemAt = client.Player.EquipBag.GetItemAt(slot2);
		ItemInfo itemAt2 = client.Player.PropBag.GetItemAt(slot);
		if (itemAt != null)
		{
			client.Player.BeginChanges();
			try
			{
				bool flag = false;
				if (itemAt2 != null && itemAt2.IsValidItem())
				{
					client.Player.RemoveTemplate(itemAt2.TemplateID, 1);
					flag = true;
				}
				else
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num3);
					List<ShopItemInfo> list = ShopMgr.FindShopbyTemplatID(num3);
					int num4 = 0;
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].APrice1 == -1 && list[i].AValue1 != 0)
						{
							num4 = list[i].AValue1;
						}
					}
					if (num4 <= client.Player.PlayerCharacter.Money)
					{
						client.Player.RemoveMoney(num4);
						flag = true;
					}
				}
				if (flag)
				{
					itemAt.Color = ((text == null) ? "" : text);
					itemAt.Skin = ((text2 == null) ? "" : text2);
					client.Player.EquipBag.UpdateItem(itemAt);
				}
			}
			finally
			{
				client.Player.CommitChanges();
			}
		}
		client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		return 0;
	}
}
