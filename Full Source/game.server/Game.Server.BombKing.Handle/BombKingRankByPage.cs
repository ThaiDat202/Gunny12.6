using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(5)]
public class BombKingRankByPage : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int val = 0;
		int rank = 0;
		int Score = 0;
		List<BombKingPlayerInfo> list = new List<BombKingPlayerInfo>();
		if (BombKingMgr.Week == 1 || BombKingMgr.Week == 2)
		{
			list = ((num != 0) ? BombKingMgr.GetListTop16BombKingPlayerInfo() : BombKingMgr.GetListTop100BombKingPlayerInfo());
			BombKingMgr.GetBombKingPlayerInfo(Player.PlayerCharacter.ID, ref rank, ref Score, num);
		}
		if (list.Count > 0)
		{
			val = ((list.Count > 100) ? 13 : ((list.Count % 8 != 0) ? (list.Count / 8 + 1) : (list.Count / 8)));
		}
		int num3 = 0;
		num3 = ((num2 * 8 <= list.Count) ? 8 : (list.Count - (num2 - 1) * 8));
		GSPacketIn gSPacketIn = new GSPacketIn(263);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt((num != 0) ? 1 : 0);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(num2);
		gSPacketIn.WriteInt(num3);
		for (int i = (num2 - 1) * 8; i < num3 + (num2 - 1) * 8; i++)
		{
			gSPacketIn.WriteInt(i + 1);
			gSPacketIn.WriteInt(list[i].UserID);
			gSPacketIn.WriteInt(list[i].AreaID);
			gSPacketIn.WriteString(list[i].NickName);
			gSPacketIn.WriteString(list[i].AreaName);
			gSPacketIn.WriteInt(list[i].vipType);
			gSPacketIn.WriteInt(list[i].vipLevel);
			if (num == 0)
			{
				gSPacketIn.WriteInt(list[i].myScore);
			}
			else
			{
				gSPacketIn.WriteInt(list[i].Score);
			}
		}
		gSPacketIn.WriteInt(rank);
		gSPacketIn.WriteInt(Score);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
