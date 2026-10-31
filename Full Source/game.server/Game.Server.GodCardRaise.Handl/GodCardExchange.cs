using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GodCardRaise.Handle;

[GodCardRaiseHandleAttbute(23)]
public class GodCardExchange : IGodCardRaiseCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(329);
		gSPacketIn.WriteByte(23);
		gSPacketIn.WriteInt(num);
		GodCardGroupInfo godCardGroupInfo = GodCardGroupMgr.FindGodCardGroup(num);
		if (godCardGroupInfo == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("GodCardOperateCard.Fail"));
		}
		else
		{
			if (godCardGroupInfo.ExchangeTimes - player.Actives.FindCardExchangesCount(num) <= 0)
			{
				player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.End"));
				return 0;
			}
			gSPacketIn.WriteInt(player.Actives.SaveListCardExchanges(num, 1));
			GodCardGroupDetailInfo[] array = GodCardGroupMgr.FindGodCardGroupDetail(godCardGroupInfo.GroupID).ToArray();
			gSPacketIn.WriteInt(array.Length);
			if (!player.Actives.CheckCardExchangesCount(array))
			{
				GodCardGroupDetailInfo[] array2 = array;
				foreach (GodCardGroupDetailInfo godCardGroupDetailInfo in array2)
				{
					gSPacketIn.WriteInt(godCardGroupDetailInfo.CardID);
					gSPacketIn.WriteInt(player.Actives.FindCardCount(godCardGroupDetailInfo.CardID));
				}
			}
			else
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(godCardGroupInfo.GiftID);
				if (itemTemplateInfo != null && itemTemplateInfo != null)
				{
					if (!player.FindEmptySlot(itemTemplateInfo.BagType))
					{
						player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.BagFull"));
						return 0;
					}
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
					itemInfo.IsBinds = true;
					player.AddTemplate(itemInfo);
					player.SendMessage(LanguageMgr.GetTranslation("GodCardExchange.Success", itemTemplateInfo.Name, itemInfo.Count));
				}
				GodCardGroupDetailInfo[] array2 = array;
				foreach (GodCardGroupDetailInfo godCardGroupDetailInfo2 in array2)
				{
					gSPacketIn.WriteInt(godCardGroupDetailInfo2.CardID);
					gSPacketIn.WriteInt(player.Actives.RemoveListCard(godCardGroupDetailInfo2.CardID, godCardGroupDetailInfo2.Number));
				}
			}
			player.Out.SendTCP(gSPacketIn);
		}
		return 1;
	}
}
