using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(12)]
public class ConsortiaInvitePass : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID != 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		bool val = false;
		int consortiaID = 0;
		string consortiaName = "";
		string msg = "ConsortiaInvitePassHandler.Failed";
		int tempID = 0;
		string tempName = "";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			int consortiaRepute = 0;
			ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
			if (consortiaBussiness.PassConsortiaInviteUsers(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, consortiaUserInfo, ref tempID, ref tempName, ref consortiaRepute))
			{
				Player.PlayerCharacter.ConsortiaID = consortiaID;
				Player.PlayerCharacter.ConsortiaName = consortiaName;
				Player.PlayerCharacter.DutyLevel = consortiaUserInfo.Level;
				Player.PlayerCharacter.DutyName = consortiaUserInfo.DutyName;
				Player.PlayerCharacter.Right = consortiaUserInfo.Right;
				ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(consortiaID);
				if (consortiaInfo != null)
				{
					Player.PlayerCharacter.ConsortiaLevel = consortiaInfo.Level;
				}
				msg = "ConsortiaInvitePassHandler.Success";
				val = true;
				consortiaUserInfo.UserID = Player.PlayerCharacter.ID;
				consortiaUserInfo.UserName = Player.PlayerCharacter.NickName;
				consortiaUserInfo.Grade = Player.PlayerCharacter.Grade;
				consortiaUserInfo.Offer = Player.PlayerCharacter.Offer;
				consortiaUserInfo.RichesOffer = Player.PlayerCharacter.RichesOffer;
				consortiaUserInfo.RichesRob = Player.PlayerCharacter.RichesRob;
				consortiaUserInfo.Win = Player.PlayerCharacter.Win;
				consortiaUserInfo.Total = Player.PlayerCharacter.Total;
				consortiaUserInfo.Escape = Player.PlayerCharacter.Escape;
				consortiaUserInfo.ConsortiaID = consortiaID;
				consortiaUserInfo.ConsortiaName = consortiaName;
				GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, consortiaUserInfo, isInvite: true, consortiaRepute, Player.PlayerCharacter.UserName, Player.PlayerCharacter.FightPower, Player.PlayerCharacter.Offer);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteInt(num);
		packet.WriteBoolean(val);
		packet.WriteInt(consortiaID);
		packet.WriteString(consortiaName);
		packet.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
