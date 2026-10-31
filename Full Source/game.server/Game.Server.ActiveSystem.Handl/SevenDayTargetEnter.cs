using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(81)]
public class SevenDayTargetEnter : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(81);
		return true;
	}
}
