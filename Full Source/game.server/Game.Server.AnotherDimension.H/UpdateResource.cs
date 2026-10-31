using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension.Handle;

[AnotherDimensionHandleAttbute(4)]
public class UpdateResource : IAnotherDimensionCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteInt(4);
		int num = 0;
		gSPacketIn.WriteInt(0);
		for (int i = 0; i < num; i++)
		{
		}
		int num2 = 0;
		gSPacketIn.WriteInt(0);
		for (int j = 0; j < num2; j++)
		{
		}
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
