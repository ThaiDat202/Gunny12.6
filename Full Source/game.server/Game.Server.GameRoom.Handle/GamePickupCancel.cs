using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Game.Server.Rooms;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(11)]
public class GamePickupCancel : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.CurrentRoom != null)
		{
			if (Player.CurrentRoom.BattleServer != null)
			{
				Player.CurrentRoom.BattleServer.RemoveRoom(Player.CurrentRoom);
				if (Player != Player.CurrentRoom.Host)
				{
					Player.CurrentRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
					RoomMgr.UpdatePlayerState(Player, 0);
				}
				else if (Player.CurrentRoom.IsSingle())
				{
					Player.CurrentRoom.RemovePlayerUnsafe(Player);
				}
				else
				{
					RoomMgr.UpdatePlayerState(Player, 2);
				}
			}
			else
			{
				Player.CurrentRoom.RemovePlayerUnsafe(Player);
			}
		}
		if (Player.CurrentHorseRaceRoom != null)
		{
			Player.CurrentHorseRaceRoom.RemovePlayerUnsafe(Player);
		}
		return true;
	}
}
