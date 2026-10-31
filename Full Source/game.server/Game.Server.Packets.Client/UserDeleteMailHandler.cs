using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(112, "删除邮件")]
public class UserDeleteMailHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(112, client.Player.PlayerCharacter.ID);
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		int num = packet.ReadInt();
		gSPacketIn.WriteInt(num);
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			MailInfo mailSingle = playerBussiness.GetMailSingle(client.Player.PlayerCharacter.ID, num);
			if (mailSingle != null)
			{
				if (mailSingle.Annex1 != string.Empty || mailSingle.Annex2 != string.Empty || mailSingle.Annex3 != string.Empty || mailSingle.Annex4 != string.Empty || mailSingle.Annex5 != string.Empty)
				{
					client.Player.SendMessage("当前邮件中物品还未领取,删除失败.");
					gSPacketIn.WriteBoolean(val: false);
					client.Out.SendTCP(gSPacketIn);
					return 0;
				}
				if (playerBussiness.DeleteMail(client.Player.PlayerCharacter.ID, num, out var senderID))
				{
					client.Out.SendMailResponse(senderID, eMailRespose.Receiver);
					gSPacketIn.WriteBoolean(val: true);
				}
				else
				{
					gSPacketIn.WriteBoolean(val: false);
				}
			}
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
