using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(12)]
public class HouseEnter : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = false;
		flag = ((num != Player.PlayerId) ? (Player.Home.FriendData != null) : (Player.Home.Data != null));
		GSPacketIn gSPacketIn = new GSPacketIn(285);
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteBoolean(flag);
		if (flag)
		{
			gSPacketIn.WriteInt(num);
		}
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
