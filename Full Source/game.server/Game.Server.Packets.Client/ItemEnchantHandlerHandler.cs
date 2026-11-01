using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(280, "物品附魔")]
public class ItemEnchantHandlerHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemAt2 = client.Player.StoreBag.GetItemAt(1);
		if (itemAt != null && itemAt.TemplateID == 11166)
		{
			if (itemAt2 != null && itemAt2.IsEnchant())
			{
				int magicLevel = itemAt2.MagicLevel;
				int magicExp = itemAt2.MagicExp;
				bool flag2 = false;
				int num = 1;
				MagicItemTemplateInfo magicItemTemplateInfo = MagicItemTemplateMgr.FindMagicItemTemplate(magicLevel + 1);
				if (magicItemTemplateInfo != null)
				{
					int property = itemAt.Template.Property2;
					if (flag)
					{
						int num2 = ((magicItemTemplateInfo.Exp - magicExp < itemAt.Template.Property2) ? itemAt.Template.Property2 : (magicItemTemplateInfo.Exp - magicExp));
						num = ((num2 / itemAt.Template.Property2 <= 0) ? 1 : (num2 / itemAt.Template.Property2));
					}
					if (num > itemAt.Count)
					{
						num = itemAt.Count;
					}
					itemAt2.MagicExp += property * num;
					if (itemAt2.MagicExp >= magicItemTemplateInfo.Exp)
					{
						itemAt2.MagicLevel++;
						flag2 = true;
					}
					if (flag2)
					{
						GSPacketIn gSPacketIn = packet.Clone();
						gSPacketIn.ClearContext();
						gSPacketIn.WriteInt(itemAt2.MagicExp);
						gSPacketIn.WriteBoolean(flag2);
						client.Player.SendTCP(gSPacketIn);
						client.Player.EquipBag.UpdatePlayerProperties();
					}
					else
					{
						client.Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemEnchant.AddExpSuccess", property * num));
					}
					if (!itemAt2.IsBinds)
					{
						itemAt2.IsBinds = true;
					}
					client.Player.StoreBag.UpdateItem(itemAt2);
					client.Player.StoreBag.RemoveCountFromStack(itemAt, num);
				}
				else
				{
					client.Player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemEnchant.DataError"));
				}
			}
			else
			{
				client.Player.SendMessage(eMessageType.Normal, "Vật phẩm này không thể sử dụng vào lúc này.");
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemEnchant.TheseCategoryIDCannotEnchant"));
		}
		return 0;
	}
}
