using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(3)]
public class ConsortiaRenegade : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		bool val = false;
		string nickName = "";
		string msg = ((num == Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed");
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.DeleteConsortiaUser(Player.PlayerCharacter.ID, num, Player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
			{
				msg = ((num == Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess");
				int consortiaID = Player.PlayerCharacter.ConsortiaID;
				if (num == Player.PlayerCharacter.ID)
				{
					Player.ClearConsortia();
					Player.Out.SendMailResponse(Player.PlayerCharacter.ID, eMailRespose.Receiver);
					consortiaBussiness.ConsortiaEventAdd(consortiaID, 8, Player.PlayerCharacter.NickName, "");
				}
				else
				{
					consortiaBussiness.ConsortiaEventAdd(consortiaID, 7, nickName, Player.PlayerCharacter.NickName);
				}
				GameServer.Instance.LoginServer.SendConsortiaUserDelete(num, consortiaID, num != Player.PlayerCharacter.ID, nickName, Player.PlayerCharacter.NickName);
				val = true;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
