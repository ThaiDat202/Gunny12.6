using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeFight.Handle;

[HomeFightHandleAttbute(1)]
public class FightEnter : IHomeFightCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(291);
		gSPacketIn.WriteByte(1);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
