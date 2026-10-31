using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeTemple;

public class HomeTemplePacketOut
{
	public void SendHomeTempleInfo(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(297, player.PlayerId);
		gSPacketIn.WriteByte(0);
		gSPacketIn.WriteInt(player.Actives.Info.CurrentSelectIndex);
		gSPacketIn.WriteInt(player.Actives.Info.CurrentLevel);
		gSPacketIn.WriteInt(player.Actives.Info.HomeTempleExp);
		if (GameProperties.VERSION >= 11000)
		{
			gSPacketIn.WriteInt(player.Actives.Info.promoteLevel);
		}
		player.SendTCP(gSPacketIn);
	}

	public void SendHomeTemplePromote(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(297);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(player.Actives.Info.promoteLevel);
		player.SendTCP(gSPacketIn);
	}
}
