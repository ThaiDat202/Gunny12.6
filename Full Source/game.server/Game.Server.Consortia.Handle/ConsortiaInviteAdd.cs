using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(11)]
public class ConsortiaInviteAdd : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		string text = packet.ReadString();
		bool val = false;
		string msg = "ConsortiaInviteAddHandler.Failed";
		if (string.IsNullOrEmpty(text))
		{
			return 0;
		}
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaInviteUserInfo consortiaInviteUserInfo = new ConsortiaInviteUserInfo();
			consortiaInviteUserInfo.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
			consortiaInviteUserInfo.ConsortiaName = Player.PlayerCharacter.ConsortiaName;
			consortiaInviteUserInfo.InviteDate = DateTime.Now;
			consortiaInviteUserInfo.InviteID = Player.PlayerCharacter.ID;
			consortiaInviteUserInfo.InviteName = Player.PlayerCharacter.NickName;
			consortiaInviteUserInfo.IsExist = true;
			consortiaInviteUserInfo.Remark = "";
			consortiaInviteUserInfo.UserID = 0;
			consortiaInviteUserInfo.UserName = text;
			if (consortiaBussiness.AddConsortiaInviteUsers(consortiaInviteUserInfo, ref msg))
			{
				msg = "ConsortiaInviteAddHandler.Success";
				val = true;
				GameServer.Instance.LoginServer.SendConsortiaInvite(consortiaInviteUserInfo.ID, consortiaInviteUserInfo.UserID, consortiaInviteUserInfo.UserName, consortiaInviteUserInfo.InviteID, consortiaInviteUserInfo.InviteName, consortiaInviteUserInfo.ConsortiaName, consortiaInviteUserInfo.ConsortiaID);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteString(text);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
