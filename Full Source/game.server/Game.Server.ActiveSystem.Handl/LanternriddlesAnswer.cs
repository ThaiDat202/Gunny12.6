using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(40)]
public class LanternriddlesAnswer : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		packet.ReadInt();
		packet.ReadInt();
		int option = packet.ReadInt();
		ActiveSystemMgr.LanternriddlesAnswer(Player.PlayerCharacter.ID, option);
		return true;
	}
}
