using System;
using System.Drawing;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.CampBattle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client;

[PacketHandler(146, "场景用户离开")]
public class CampBattleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		CampBattlePlayerInfo player = GameMgr.CampBattle.GetPlayer(client.Player.ZoneId, client.Player.PlayerId);
		BaseRoom currentRoom = client.Player.CurrentRoom;
		if (currentRoom == null || player == null || currentRoom.RoomType != eRoomType.CampBattleBattle)
		{
			return 0;
		}
		switch (b)
		{
		case 21:
			GameMgr.CampBattle.SendPerScoreRank(client.Player);
			break;
		case 6:
			GameMgr.CampBattle.SendCampInitSecen(player);
			break;
		case 3:
			currentRoom.RemovePlayerUnsafe(client.Player);
			break;
		case 5:
		{
			int monsterId = packet.ReadInt();
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			GameMgr.CampBattle.ChallengePVEGame(client.Player, monsterId);
			break;
		}
		case 16:
		{
			int playerId = packet.ReadInt();
			int num3 = packet.ReadInt();
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (!client.Player.CurrentRoom.IsPlaying && playerById != null && playerById.IsActive && playerById.CurrentRoom != null && !playerById.CurrentRoom.IsPlaying && playerById.CurrentRoom.RoomType == eRoomType.CampBattleBattle)
			{
				GameMgr.CampBattle.ChallengePVPGame(client.Player, playerById);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.CampBattle.PVPFight.Error"));
			}
			break;
		}
		case 2:
		{
			int posX = packet.ReadInt();
			int posY = packet.ReadInt();
			int num = packet.ReadInt();
			int num2 = packet.ReadInt();
			player.PosX = posX;
			player.PosY = posY;
			GameMgr.CampBattle.SendPlayerMove(player);
			break;
		}
		case 9:
		{
			CampBattleTeamInfo team = GameMgr.CampBattle.GetTeam(player.Team);
			if (team != null && team.OpenGate)
			{
				Point startPoint = GameMgr.CampBattle.GetStartPoint(1, team.TeamID);
				player.MapId = 1;
				player.PosX = startPoint.X;
				player.PosY = startPoint.Y;
				GameMgr.CampBattle.SendCampInitSecen(player);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("CampBattle.Msg.Monster.CanNotPassMap"));
			}
			break;
		}
		case 17:
		{
			bool isCapture = packet.ReadBoolean();
			if (!GameMgr.CampBattle.CaptureStatue(player, isCapture))
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Campbattle.Msg.Capture.Fail"));
			}
			break;
		}
		default:
		{
			CampPackageType campPackageType = (CampPackageType)b;
			Console.WriteLine("CampBattleHandler." + campPackageType);
			break;
		}
		}
		return 0;
	}
}
