using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(215, "场景用户离开")]
public class CaddyConvertedHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		string title = packet.ReadString();
		string content = packet.ReadString();
		bool flag = false;
		int riches = 1000;
		string translateId = "ConsortiaRichiUpdateHandler.Failed";
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
			MailInfo mailInfo = new MailInfo();
			ConsortiaUserInfo[] array = allMemberByConsortia;
			foreach (ConsortiaUserInfo consortiaUserInfo in array)
			{
				mailInfo.SenderID = client.Player.PlayerCharacter.ID;
				mailInfo.Sender = "Sua associação " + consortiaInfo.ConsortiaName;
				mailInfo.ReceiverID = consortiaUserInfo.UserID;
				mailInfo.Receiver = consortiaUserInfo.UserName;
				mailInfo.Title = title;
				mailInfo.Content = content;
				mailInfo.Type = 59;
				if (consortiaUserInfo.UserID != client.Player.PlayerCharacter.ID && playerBussiness.SendMail(mailInfo))
				{
					translateId = "ConsortiaRichiUpdateHandler.Success";
					flag = true;
					if (consortiaUserInfo.State != 0)
					{
						client.Player.Out.SendMailResponse(consortiaUserInfo.UserID, eMailRespose.Receiver);
					}
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
				}
				if (!flag)
				{
					translateId = "ConsortiaRichiUpdateHandler.Success";
					flag = true;
					if (consortiaUserInfo.State != 0)
					{
						client.Player.Out.SendMailResponse(consortiaUserInfo.UserID, eMailRespose.Receiver);
					}
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
				}
			}
		}
		if (flag)
		{
			using ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
			consortiaBussiness.ConsortiaRichRemove(client.Player.PlayerCharacter.ConsortiaID, ref riches);
		}
		client.Out.SendConsortiaMail(flag, client.Player.PlayerCharacter.ID);
		client.Player.SendMessage(LanguageMgr.GetTranslation(translateId));
		return 0;
	}
}
