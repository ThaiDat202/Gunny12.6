using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(7)]
public class ConsotiaApplyState : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		bool flag = packet.ReadBoolean();
		bool val = false;
		string msg = "CONSORTIA_APPLY_STATE.Failed";
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (consortiaBussiness.UpdateConsotiaApplyState(Player.PlayerCharacter.ConsortiaID, Player.PlayerCharacter.ID, flag, ref msg))
			{
				msg = "CONSORTIA_APPLY_STATE.Success";
				val = true;
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteBoolean(flag);
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteString(LanguageMgr.GetTranslation(msg));
		Player.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
