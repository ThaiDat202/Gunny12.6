using System;
using System.Drawing;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.GuildBattle;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Packets.Client;

[PacketHandler(153, "场景用户离开")]
public class ConsortiaBattleHander : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		UserGuildBattleInfo userGuildBattleInfo = GameMgr.GuildBattle.FindUser(client.Player.PlayerId);
		BaseRoom currentRoom = client.Player.CurrentRoom;
		if (currentRoom == null || userGuildBattleInfo == null || currentRoom.RoomType != eRoomType.ConsortiaBattle)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.GuildBattle.Error"));
			return 0;
		}
		switch (b)
		{
		case 5:
			currentRoom.RemovePlayerUnsafe(client.Player);
			break;
		case 3:
			GameMgr.GuildBattle.SendAllPlayerList(userGuildBattleInfo, toAll: false);
			break;
		case 4:
		{
			int x = packet.ReadInt();
			int y = packet.ReadInt();
			string moveStr = packet.ReadString();
			userGuildBattleInfo.Postion = new Point(x, y);
			GameMgr.GuildBattle.SendPlayerMove(userGuildBattleInfo, moveStr);
			break;
		}
		case 21:
			GameMgr.GuildBattle.SendAllPlayerList(userGuildBattleInfo, toAll: true);
			break;
		case 6:
		{
			int playerId = packet.ReadInt();
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			GamePlayer playerById = WorldMgr.GetPlayerById(playerId);
			if (playerById != null && playerById.IsActive && playerById.CurrentRoom != null && !playerById.CurrentRoom.IsPlaying)
			{
				GameMgr.GuildBattle.ChallengeGame(client.Player, playerById);
			}
			else
			{
				client.Player.SendMessage("Lỗi bắt cặp.");
			}
			break;
		}
		case 16:
		{
			byte type = packet.ReadByte();
			if (userGuildBattleInfo != null)
			{
				GameMgr.GuildBattle.SendUpdateScore(userGuildBattleInfo, type);
			}
			break;
		}
		case 17:
		{
			int num = packet.ReadInt();
			bool flag = packet.ReadBoolean();
			switch (num)
			{
			case 1:
				if (client.Player.PlayerCharacter.Money >= 30000)
				{
					client.Player.RemoveMoney(30000);
					client.Player.PlayerCharacter.ActivePowFirstGame = true;
				}
				break;
			case 2:
				if (client.Player.PlayerCharacter.Money >= 300000)
				{
					client.Player.RemoveMoney(300000);
					userGuildBattleInfo.DupeScoreConsortiaBattle = true;
				}
				break;
			case 3:
				if (userGuildBattleInfo.IsDead && client.Player.PlayerCharacter.Money >= 10000)
				{
					client.Player.RemoveMoney(10000);
					GameMgr.GuildBattle.QuickRevive(userGuildBattleInfo, stay: false);
				}
				break;
			case 4:
				if (userGuildBattleInfo.IsDead && client.Player.PlayerCharacter.Money >= 80000)
				{
					client.Player.RemoveMoney(80000);
					GameMgr.GuildBattle.QuickRevive(userGuildBattleInfo, stay: true);
				}
				break;
			}
			GameMgr.GuildBattle.SendUpdateSceneInfo(userGuildBattleInfo);
			break;
		}
		default:
		{
			ConsBatPackageType consBatPackageType = (ConsBatPackageType)b;
			Console.WriteLine("ConsortiaBattleType." + consBatPackageType);
			break;
		}
		}
		return 0;
	}
}
