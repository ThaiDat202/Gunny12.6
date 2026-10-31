using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(6)]
public class BombKingStartBattle : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (BombKingMgr.IsBombKingOpen)
		{
			if (Player.MainWeapon != null)
			{
				if (BombKingMgr.Checkbattle(Player.PlayerId))
				{
					int rankType = 1;
					BombKingPlayerInfo bombKingPlayerInfo = BombKingMgr.GetBombKingPlayerInfo(Player.PlayerId);
					if (bombKingPlayerInfo != null)
					{
						rankType = bombKingPlayerInfo.RankType;
					}
					Player.battleId = BombKingMgr.GetBattlaId(Player.PlayerId, rankType);
					Player.battleStage = 2;
					Player.BombKingDelay = (int)BombKingMgr.BombKingBeginTime.Subtract(DateTime.Now).TotalSeconds / 10 - 1;
					if (Player.battleStage == 2)
					{
						if (Player.CurrentRoom != null)
						{
							Player.CurrentRoom.RemovePlayerUnsafe(Player);
						}
						RoomMgr.CreateBombKingRoom(Player);
					}
				}
				else
				{
					Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BombKing.Msg.Battle.NotAllowJoin"));
				}
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
			}
		}
		else
		{
			Player.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("BombKing.Msg.Battle.NoStart"));
		}
		return false;
	}
}
