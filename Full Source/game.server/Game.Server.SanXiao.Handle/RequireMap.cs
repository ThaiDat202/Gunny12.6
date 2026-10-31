using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.SanXiao.Handle;

[SanXiaoHandleAttbute(2)]
public class RequireMap : ISanXiaoCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		player.Actives.SendSXRequireMap();
		return 1;
	}
}
