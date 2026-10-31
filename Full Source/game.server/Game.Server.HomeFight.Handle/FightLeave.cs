using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight.Handle;

[HomeFightHandleAttbute(3)]
public class FightLeave : IHomeFightCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(291);
		gSPacketIn.WriteByte(3);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
