using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(30, "打开物品")]
public class Picc : IPacketHandler
{
	public int PICC_PRICE = 10000;

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		int num = packet.ReadInt();
		ActiveInfo singleActive = ActiveMgr.GetSingleActive(id);
		if (singleActive != null)
		{
			List<ActiveAwardInfo> activeAward = ActiveMgr.GetActiveAward(singleActive.ActiveID);
			if (activeAward.Count > 0)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(activeAward[0].ItemID);
				if (itemTemplateInfo != null)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 103);
					using (new PlayerBussiness())
					{
						if (client.Player.MoneyDirect(PICC_PRICE) && client.Player.AddTemplate(itemInfo))
						{
							client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Picc.Msg1", itemTemplateInfo.Name));
							if (client.Player.SaveNewItemToDatabase())
							{
								string translation = LanguageMgr.GetTranslation("Picc.Notice", client.Player.PlayerCharacter.NickName, itemInfo.TemplateID, PICC_PRICE);
								GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, client.Player.ZoneId, null);
								GameServer.Instance.LoginServer.SendPacket(packet2);
							}
						}
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Picc.Msg3"));
				}
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Picc.Msg3"));
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Picc.Msg4"));
		}
		return 1;
	}
}
