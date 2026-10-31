using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(31, "场景用户离开")]
public class GoodsExchangeHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int i = 0;
		ActiveInfo singleActive = ActiveMgr.GetSingleActive(id);
		List<ItemInfo> list = new List<ItemInfo>();
		StringBuilder stringBuilder = new StringBuilder();
		if (num < 1 || num2 < 1 || num > 999 || num2 > 999)
		{
			client.Out.SendMessage(eMessageType.Normal, "Quantidade de Troca Invalida");
			return 0;
		}
		for (; i < num2; i++)
		{
			int num3 = packet.ReadInt();
			int place = packet.ReadInt();
			int bagType = packet.ReadInt();
			ItemInfo itemAt = client.Player.GetItemAt((eBageType)bagType, place);
			if (itemAt != null && itemAt.TemplateID == num3)
			{
				list.Add(itemAt);
				continue;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num3);
			stringBuilder.Append(itemTemplateInfo.Name + ", ");
		}
		int index = packet.ReadInt();
		if (list.Count == num2)
		{
			StringBuilder stringBuilder2 = new StringBuilder();
			List<ActiveConvertItemInfo> list2 = new List<ActiveConvertItemInfo>();
			foreach (ItemInfo item in list)
			{
				ActiveConvertItemInfo activeConvertItem = ActiveMgr.GetActiveConvertItem(id, item.TemplateID, index);
				if (activeConvertItem == null)
				{
					break;
				}
				if (client.Player.GetItemCount(item.TemplateID) >= activeConvertItem.ItemCount * num && item.TemplateID == activeConvertItem.TemplateID)
				{
					list2.Add(activeConvertItem);
				}
			}
			if (list2.Count == num2)
			{
				foreach (ActiveConvertItemInfo item2 in list2)
				{
					client.Player.RemoveTemplate(item2.TemplateID, item2.ItemCount * num);
				}
				List<ActiveConvertItemInfo> activeConvertItemAward = ActiveMgr.GetActiveConvertItemAward(id, index);
				foreach (ActiveConvertItemInfo item3 in activeConvertItemAward)
				{
					ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(item3.TemplateID);
					if (itemTemplateInfo2 != null)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo2, 1, 103);
						itemInfo.IsBinds = item3.IsBind;
						itemInfo.Count = item3.ItemCount;
						itemInfo.ValidDate = item3.ValidDate;
						if (item3.LimitValue > 0)
						{
							itemInfo.StrengthenLevel = item3.LimitValue;
						}
						client.Player.AddTemplate(itemInfo, itemInfo.Template.BagType, item3.ItemCount * num, eGameView.RouletteTypeGet);
						stringBuilder2.Append(itemTemplateInfo2.Name + " x" + item3.ItemCount + ". ");
					}
				}
				if (stringBuilder2.Length > 0)
				{
					client.Out.SendMessage(eMessageType.Normal, stringBuilder2.ToString());
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GoodsExchangeHandler.Msg2"));
				}
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GoodsExchangeHandler.ItemsExchangeNotEnought"));
			}
		}
		else
		{
			string text = stringBuilder.ToString();
			text = text.Substring(0, text.LastIndexOf(","));
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("GoodsExchangeHandler.Msg1", text));
		}
		return 0;
	}
}
