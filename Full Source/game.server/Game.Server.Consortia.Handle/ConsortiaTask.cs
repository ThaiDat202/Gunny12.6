using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(22)]
public class ConsortiaTask : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.ConsortiaTaskHandler?.ProcessData(Player, packet);
		return 0;
	}
}
