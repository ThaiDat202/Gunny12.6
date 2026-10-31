using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(71, "小喇叭")]
public class SmallBugleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int templateId = 11101;
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SmallBugleHandler.Msg"));
			return 1;
		}
		if (itemByTemplateID != null)
		{
			if (client.Player.PlayerCharacter.IsBanChat && client.Player.PlayerCharacter.BanChatEndDate > DateTime.Now)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				client.Out.SendMessage(eMessageType.ALERT, "Termina em " + (client.Player.PlayerCharacter.BanChatEndDate - DateTime.Now).ToString());
				return 1;
			}
			client.Player.PlayerCharacter.IsBanChat = false;
			client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			int num = packet.ReadInt();
			string text = packet.ReadString();
			string text2 = packet.ReadString();
			if (client.Player.LastChatMsg == text2)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ChatHandler.Msg"));
				return 2;
			}
			if (text2.Length > 150)
			{
				client.Player.LastChatTime = DateTime.Now;
				return 1;
			}
			GSPacketIn gSPacketIn = new GSPacketIn(71);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteString(text2);
			client.Player.LastChatTime = DateTime.Now;
			client.Player.LastChatMsg = text2;
			client.Player.OnUsingItem(itemByTemplateID.TemplateID, 1);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		}
		else
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SmallBugleHandler.Msg"));
			client.Player.LastChatTime = DateTime.Now.AddDays(7.0);
		}
		return 0;
	}
}
