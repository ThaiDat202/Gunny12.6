using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HorseAmulet.Handle;

[HorseAmuletHandleAttbute(7)]
public class HorseAmuletReplace : IHorseAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		return 1;
	}
}
