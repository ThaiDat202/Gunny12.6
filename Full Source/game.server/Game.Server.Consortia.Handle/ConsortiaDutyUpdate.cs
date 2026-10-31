using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(10)]
public class ConsortiaDutyUpdate : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int dutyID = packet.ReadInt();
		int num = packet.ReadByte();
		string msg = "ConsortiaDutyUpdateHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaDutyInfo consortiaDutyInfo = new ConsortiaDutyInfo();
			consortiaDutyInfo.ConsortiaID = Player.PlayerCharacter.ConsortiaID;
			consortiaDutyInfo.DutyID = dutyID;
			consortiaDutyInfo.IsExist = true;
			consortiaDutyInfo.DutyName = "";
			switch (num)
			{
			case 1:
				return 1;
			case 2:
				consortiaDutyInfo.DutyName = packet.ReadString();
				if (string.IsNullOrEmpty(consortiaDutyInfo.DutyName) || Encoding.Default.GetByteCount(consortiaDutyInfo.DutyName) > 10)
				{
					Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long"));
					return 1;
				}
				consortiaDutyInfo.Right = packet.ReadInt();
				break;
			}
			if (consortiaBussiness.UpdateConsortiaDuty(consortiaDutyInfo, Player.PlayerCharacter.ID, num, ref msg))
			{
				dutyID = consortiaDutyInfo.DutyID;
				msg = "ConsortiaDutyUpdateHandler.Success";
				GameServer.Instance.LoginServer.SendConsortiaDuty(consortiaDutyInfo, num, Player.PlayerCharacter.ConsortiaID);
			}
		}
		return 0;
	}
}
