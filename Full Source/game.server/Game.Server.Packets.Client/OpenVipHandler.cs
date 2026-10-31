using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(92, "开通/续费Vip")]
public class OpenVipHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		string text = packet.ReadString();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		int value = 200000;
		int num2 = 200000;
		int num3 = 500000;
		int num4 = 800000;
		string translation = LanguageMgr.GetTranslation("OpenVipHandler.Msg1");
		string translation2 = LanguageMgr.GetTranslation("OpenVipHandler.Msg1");
		switch (num)
		{
		case 30:
			value = num2;
			break;
		case 90:
			value = num3;
			break;
		case 180:
			value = num4;
			break;
		}
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
		if (client.Player.MoneyDirect(value))
		{
			DateTime ExpireDayOut = DateTime.Now;
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerBussiness.VIPRenewal(text, num, ref ExpireDayOut);
			if (clientByPlayerNickName == null)
			{
				translation = LanguageMgr.GetTranslation("OpenVipHandler.Msg2", text);
			}
			else if (client.Player.PlayerCharacter.NickName == text)
			{
				if (client.Player.PlayerCharacter.typeVIP == 0)
				{
					client.Player.OpenVIP(num, ExpireDayOut);
				}
				else
				{
					client.Player.ContinousVIP(ExpireDayOut);
					translation = LanguageMgr.GetTranslation("OpenVipHandler.Msg3");
				}
				client.Out.SendOpenVIP(client.Player.PlayerCharacter);
			}
			else
			{
				if (clientByPlayerNickName.PlayerCharacter.typeVIP == 0)
				{
					clientByPlayerNickName.OpenVIP(num, ExpireDayOut);
					translation = LanguageMgr.GetTranslation("OpenVipHandler.Msg4", text);
					translation2 = LanguageMgr.GetTranslation("OpenVipHandler.Msg5", client.Player.PlayerCharacter.NickName);
				}
				else
				{
					clientByPlayerNickName.ContinousVIP(ExpireDayOut);
					translation = LanguageMgr.GetTranslation("OpenVipHandler.Msg6", text);
					translation2 = LanguageMgr.GetTranslation("OpenVipHandler.Msg7", client.Player.PlayerCharacter.NickName);
				}
				clientByPlayerNickName.Out.SendOpenVIP(clientByPlayerNickName.PlayerCharacter);
				clientByPlayerNickName.Out.SendMessage(eMessageType.Normal, translation2);
			}
			client.Out.SendMessage(eMessageType.Normal, translation);
		}
		return 0;
	}
}
