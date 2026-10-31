using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(72, "大喇叭")]
public class BigBugleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int templateId = packet.ReadInt();
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("BigBugleHandler.Msg"));
			return 1;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(72);
		if (itemByTemplateID != null)
		{
			if (client.Player.PlayerCharacter.IsBanChat && client.Player.PlayerCharacter.BanChatEndDate > DateTime.Now)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				client.Out.SendMessage(eMessageType.ALERT, "禁言时间结束于: " + (client.Player.PlayerCharacter.BanChatEndDate - DateTime.Now).ToString());
				return 1;
			}
			client.Player.PlayerCharacter.IsBanChat = false;
			int num = packet.ReadInt();
			string text = packet.ReadString();
			string text2 = packet.ReadString();
			if (client.Player.LastChatMsg == text2)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ChatHandler.Msg"));
				return 2;
			}
			int num2 = 150;
			if (text2.Length > num2)
			{
				client.Player.LastChatTime = DateTime.Now;
				return 1;
			}
			client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			gSPacketIn.WriteInt(itemByTemplateID.Template.Property2);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteString(text2);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			client.Player.LastChatTime = DateTime.Now;
			client.Player.LastChatMsg = text2;
			client.Player.OnUsingItem(itemByTemplateID.TemplateID, 1);
		}
		else
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("BigBugleHandler.Msg"));
			client.Player.LastChatTime = DateTime.Now.AddDays(7.0);
		}
		return 0;
	}
}
