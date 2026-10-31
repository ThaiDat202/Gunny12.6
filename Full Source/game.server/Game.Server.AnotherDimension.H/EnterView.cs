using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.AnotherDimension.Handle;

[AnotherDimensionHandleAttbute(3)]
public class EnterView : IAnotherDimensionCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		Player.SendTCP(gSPacketIn);
		gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteInt(3);
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
		gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteBoolean(val: true);
		int num3 = 0;
		gSPacketIn.WriteInt(0);
		for (int k = 0; k < num3; k++)
		{
		}
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
