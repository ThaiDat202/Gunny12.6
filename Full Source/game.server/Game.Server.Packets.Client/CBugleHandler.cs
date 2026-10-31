using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(73, "大喇叭")]
public class CBugleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int templateId = 11100;
		int clientId = packet.ReadInt();
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CBugleHandler.Msg"));
			return 1;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(73, clientId);
		if (itemByTemplateID != null)
		{
			if (client.Player.PlayerCharacter.IsBanChat && client.Player.PlayerCharacter.BanChatEndDate > DateTime.Now)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				client.Out.SendMessage(eMessageType.ALERT, "Termina em " + (client.Player.PlayerCharacter.BanChatEndDate - DateTime.Now).ToString());
				return 1;
			}
			client.Player.PlayerCharacter.IsBanChat = false;
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
			client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			gSPacketIn.WriteInt(client.Player.ZoneId);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteString(text2);
			gSPacketIn.WriteString(client.Player.ZoneName);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			client.Player.LastChatTime = DateTime.Now;
			client.Player.LastChatMsg = text2;
			client.Player.OnUsingItem(itemByTemplateID.TemplateID, 1);
		}
		else
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("CBugleHandler.Msg"));
			client.Player.LastChatTime = DateTime.Now.AddDays(7.0);
		}
		return 0;
	}
}
