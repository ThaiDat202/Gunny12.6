using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(2)]
public class ConsortiaDisband : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int consortiaID = Player.PlayerCharacter.ConsortiaID;
		string consortiaName = Player.PlayerCharacter.ConsortiaName;
		bool flag = false;
		string msg = "ConsortiaDisbandHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.DeleteConsortia(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, ref msg))
			{
				flag = true;
				msg = "ConsortiaDisbandHandler.Success1";
				Player.ClearConsortia();
				GameServer.Instance.LoginServer.SendConsortiaDelete(consortiaID);
			}
		}
		string text = LanguageMgr.GetTranslation(msg);
		if (msg == "ConsortiaDisbandHandler.Success1")
		{
			text = text + consortiaName + LanguageMgr.GetTranslation("ConsortiaDisbandHandler.Success2");
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(2);
		if (flag)
		{
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(text);
		}
		else
		{
			gSPacketIn.WriteInt(Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(text);
		}
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
