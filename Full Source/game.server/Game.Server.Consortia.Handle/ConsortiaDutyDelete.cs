using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(9)]
public class ConsortiaDutyDelete : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		int num = packet.ReadInt();
		bool val = false;
		string msg = "ConsortiaDutyDeleteHandler.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.DeleteConsortiaDuty(num, Player.PlayerCharacter.ID, Player.PlayerCharacter.ConsortiaID, ref msg))
			{
				msg = "ConsortiaDutyDeleteHandler.Success";
				val = true;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
