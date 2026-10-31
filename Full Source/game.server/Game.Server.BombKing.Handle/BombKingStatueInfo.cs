using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(2)]
public class BombKingStatueInfo : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(2);
		for (int i = 0; i < 3; i++)
		{
			BombKingPlayerInfo topBombKing = BombKingMgr.GetTopBombKing(i);
			if (topBombKing == null)
			{
				gSPacketIn.WriteBoolean(val: false);
				continue;
			}
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteString(topBombKing.NickName);
			gSPacketIn.WriteInt(topBombKing.vipType);
			gSPacketIn.WriteInt(topBombKing.vipLevel);
			gSPacketIn.WriteString(topBombKing.style);
			gSPacketIn.WriteString(topBombKing.color);
			gSPacketIn.WriteBoolean(topBombKing.sex);
			gSPacketIn.WriteString(topBombKing.AreaName);
			gSPacketIn.WriteBoolean(val: true);
		}
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
