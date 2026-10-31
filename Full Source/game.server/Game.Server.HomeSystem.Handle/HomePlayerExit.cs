using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(2)]
public class HomePlayerExit : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(Player.PlayerId);
		Player.SendTCP(gSPacketIn);
		Player.Home.SaveToDatabase();
		return false;
	}
}
