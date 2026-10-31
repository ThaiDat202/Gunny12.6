using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(0)]
public class ConsortiaTryin : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID != 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		bool val = false;
		string msg = "ConsortiaApplyLoginHandler.ADD_Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaApplyUserInfo consortiaApplyUserInfo = new ConsortiaApplyUserInfo();
			consortiaApplyUserInfo.ApplyDate = DateTime.Now;
			consortiaApplyUserInfo.ConsortiaID = num;
			consortiaApplyUserInfo.ConsortiaName = "";
			consortiaApplyUserInfo.IsExist = true;
			consortiaApplyUserInfo.Remark = "";
			consortiaApplyUserInfo.UserID = Player.PlayerCharacter.ID;
			consortiaApplyUserInfo.UserName = Player.PlayerCharacter.NickName;
			if (consortiaBussiness.AddConsortiaApplyUsers(consortiaApplyUserInfo, ref msg))
			{
				msg = ((num != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success");
				val = true;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(0);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
