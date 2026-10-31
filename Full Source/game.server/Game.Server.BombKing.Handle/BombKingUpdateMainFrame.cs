using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(3)]
public class BombKingUpdateMainFrame : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		BombKingPlayerInfo bombKingPlayerInfo = BombKingMgr.GetBombKingPlayerInfo(Player.PlayerId);
		int val = 0;
		int val2 = 0;
		if (bombKingPlayerInfo != null)
		{
			val = bombKingPlayerInfo.myScore;
			val2 = bombKingPlayerInfo.myRank;
		}
		List<BombKingPlayerInfo> listTop16BombKingPlayerInfo = BombKingMgr.GetListTop16BombKingPlayerInfo();
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(3);
		for (int i = 0; i < 16; i++)
		{
			if (listTop16BombKingPlayerInfo.Count == 0)
			{
				gSPacketIn.WriteBoolean(val: false);
			}
			else if (i < listTop16BombKingPlayerInfo.Count)
			{
				BombKingPlayerInfo bombKingPlayerInfo2 = listTop16BombKingPlayerInfo[i];
				gSPacketIn.WriteBoolean(val: true);
				gSPacketIn.WriteInt(bombKingPlayerInfo2.UserID);
				gSPacketIn.WriteInt(bombKingPlayerInfo2.AreaID);
				gSPacketIn.WriteString(bombKingPlayerInfo2.NickName);
				gSPacketIn.WriteInt(bombKingPlayerInfo2.RankType);
			}
			else
			{
				gSPacketIn.WriteBoolean(val: false);
			}
		}
		for (int j = 0; j < 3; j++)
		{
			BombKingPlayerInfo topBombKing = BombKingMgr.GetTopBombKing(j);
			if (topBombKing == null)
			{
				gSPacketIn.WriteBoolean(val: false);
				continue;
			}
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(topBombKing.UserID);
			gSPacketIn.WriteInt(topBombKing.AreaID);
			gSPacketIn.WriteString(topBombKing.NickName);
			gSPacketIn.WriteString(topBombKing.style);
			gSPacketIn.WriteString(topBombKing.color);
			gSPacketIn.WriteBoolean(topBombKing.sex);
		}
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(val2);
		gSPacketIn.WriteInt(BombKingMgr.battleStage);
		gSPacketIn.WriteInt(BombKingMgr.turn);
		gSPacketIn.WriteInt((BombKingMgr.status == 1) ? 1 : Player.battleStage);
		gSPacketIn.WriteDateTime(BombKingMgr.BombKingBeginTime);
		gSPacketIn.WriteDateTime(BombKingMgr.BombKingEndTime);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
