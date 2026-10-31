using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(145)]
public class TreasureLostEnter : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(145);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(50);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(5);
		gSPacketIn.WriteInt(4000);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(val: false);
		int i = 0;
		int[] array = new int[5] { 5523, 9523, 13356, 13562, 13561 };
		for (; i < 5; i++)
		{
			gSPacketIn.WriteInt(array[i]);
			gSPacketIn.WriteBoolean(val: true);
		}
		gSPacketIn.WriteString("-13,0,201310,201310,0,0,201310,-1,-2,-11,-1,-2,-10,-1,-2,0,0,-1,-2,-3,-4,-5,-10,0,0,0,201310,-1,-2,-3,-4,-5,-10,0,0,0,201310,-1,-2,-3,-4,-5,-10,0,0,0,201310,-1,-2,-2,-5,-4,-5");
		gSPacketIn.WriteInt(5);
		gSPacketIn.WriteInt(10);
		Player.Out.SendTCP(gSPacketIn);
		return true;
	}
}
