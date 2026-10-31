using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(4)]
public class ConsortiaTryinPass : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		bool val = false;
		string msg = "ConsortiaApplyLoginPassHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			int consortiaRepute = 0;
			ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
			if (consortiaBussiness.PassConsortiaApplyUsers(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.PlayerCharacter.ConsortiaID, ref msg, consortiaUserInfo, ref consortiaRepute))
			{
				msg = "ConsortiaApplyLoginPassHandler.Success";
				val = true;
				if (consortiaUserInfo.UserID != 0)
				{
					consortiaUserInfo.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
					consortiaUserInfo.ConsortiaName = Player.PlayerCharacter.ConsortiaName;
					consortiaBussiness.ConsortiaEventAdd(Player.PlayerCharacter.ConsortiaID, 6, consortiaUserInfo.UserName, Player.PlayerCharacter.NickName);
					GameServer.Instance.LoginServer.SendConsortiaUserPass(Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, consortiaUserInfo, isInvite: false, consortiaRepute, consortiaUserInfo.LoginName, Player.PlayerCharacter.FightPower, Player.PlayerCharacter.Offer);
				}
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
