using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(18)]
public class SingleRoomBegin : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (Player.MainWeapon == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
			return false;
		}
		switch (num)
		{
		case 7:
		case 8:
		case 9:
		case 10:
		case 38:
			RoomMgr.CreateGroupBattleRoom(Player, num);
			break;
		case 1:
			RoomMgr.CreateEncounterRoom(Player, eRoomType.Encounter);
			break;
		case 2:
		{
			ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(Player.PlayerCharacter.ConsortiaID);
			int bossLevel = 10;
			if (consortiaById != null)
			{
				bossLevel = consortiaById.callBossLevel;
			}
			RoomMgr.CreateConsortiaBossRoom(Player, eRoomType.ConsortiaBoss, bossLevel);
			break;
		}
		case 3:
			if (Player.CanActive("campbattle"))
			{
				RoomMgr.CreateBattleRoom(Player, eRoomType.FightGround);
			}
			break;
		case 20:
			Player.ClearFootballCard();
			RoomMgr.CreateFightFootballTimeRoom(Player, eRoomType.FightFootballTime);
			break;
		case 21:
			RoomMgr.CreateSurvivalRoom(Player, num);
			break;
		case 4:
			if (Player.CanActive("guildbattle"))
			{
				if (Player.CurrentRoom != null)
				{
					Player.CurrentRoom.RemovePlayerUnsafe(Player);
				}
				if (!Player.IsActive)
				{
					return false;
				}
				RoomMgr.WaitingRoom.RemovePlayer(Player);
				RoomMgr.CreateConsortiaBattleRoom(Player);
			}
			break;
		case 22:
			if (Player.CurrentRoom != null)
			{
				Player.CurrentRoom.RemovePlayerUnsafe(Player);
			}
			if (!Player.IsActive)
			{
				return false;
			}
			RoomMgr.WaitingRoom.RemovePlayer(Player);
			RoomMgr.CreateConsortiaDomainRoom(Player);
			break;
		case 5:
		case 17:
			if (Player.CanActive("campbattle"))
			{
				Console.WriteLine("single camp battle room");
				if (Player.CurrentRoom != null)
				{
					Player.CurrentRoom.RemovePlayerUnsafe(Player);
				}
				if (!Player.IsActive)
				{
					return false;
				}
				RoomMgr.WaitingRoom.RemovePlayer(Player);
				RoomMgr.CreateCampBattleRoom(Player);
			}
			break;
		case 60:
			if (Player.PlayerCharacter.horseRaceCanRaceTime <= 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHaveTimes"));
				return false;
			}
			if (Player.PlayerCharacter.MountsType <= 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHorse"));
				return false;
			}
			if (Player.PlayerCharacter.MountLv <= 0)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("GameServer.HorseGame.NoHorseLevel"));
				return false;
			}
			RoomMgr.CreateHorseRaceRoom(Player);
			break;
		default:
			Console.WriteLine("SINGLE_ROOM_BEGIN: {0}", num);
			break;
		}
		return true;
	}
}
