using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension.Handle;

[AnotherDimensionHandleAttbute(6)]
public class UpgradeLevel : IAnotherDimensionCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(2000);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(20000);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(1400);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
