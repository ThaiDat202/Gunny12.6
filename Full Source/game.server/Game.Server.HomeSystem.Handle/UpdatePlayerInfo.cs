using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(5)]
public class UpdatePlayerInfo : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		return true;
	}
}
