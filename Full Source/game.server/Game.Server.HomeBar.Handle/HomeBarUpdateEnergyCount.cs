using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(6)]
public class HomeBarUpdateEnergyCount : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(294);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(Player.Home.Data.energyCount);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
