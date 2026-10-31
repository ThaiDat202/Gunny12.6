using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(94)]
public class BoguAdventureOut : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.Actives.SaveBoguAdventureDatabase();
		return true;
	}
}
