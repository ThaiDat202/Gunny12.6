using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple.Handle;

[HomeTempleHandleAttbute(0)]
public class Level : IHomeTempleCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (player.Actives.Info == null)
		{
			return 0;
		}
		HomeTempleMgr.Out.SendHomeTempleInfo(player);
		return 1;
	}
}
