using Game.Base.Packets;
using Game.Server.RingStation.Action;

namespace Game.Server.RingStation.RoomGamePkg.TankHandle;

[GameCommandAttbute(91)]
public class GamePacket : IGameCommandHandler
{
	public bool HandleCommand(RingStationRoomLogicProcessor process, VirtualGamePlayer player, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		if (player.CurRoom.IsAutoBot)
		{
			foreach (VirtualGamePlayer player2 in player.CurRoom.GetPlayers())
			{
				if (b <= 46)
				{
					switch (b)
					{
					case 6:
						player2.NextTurn(packet);
						break;
					case 46:
						player2.AddTurn(packet);
						break;
					}
					continue;
				}
				switch (b)
				{
				case 100:
					player2.CurRoom.RemovePlayer(player);
					break;
				case 101:
					player2.SendCreateGame(packet);
					break;
				case 103:
					player2.AddAction(new PlayerLoadingAction(20, 500));
					break;
				}
			}
		}
		else if (b <= 46)
		{
			switch (b)
			{
			case 6:
				player.NextTurn(packet);
				break;
			case 46:
				player.AddTurn(packet);
				break;
			}
		}
		else
		{
			switch (b)
			{
			case 100:
				player.CurRoom.RemovePlayer(player);
				break;
			case 101:
				player.SendCreateGame(packet);
				break;
			case 103:
				if (!player.CurRoom.IsAutoBot)
				{
					break;
				}
				foreach (VirtualGamePlayer player3 in player.CurRoom.GetPlayers())
				{
					player3.AddAction(new PlayerLoadingAction(20, 500));
				}
				break;
			}
		}
		return true;
	}
}
