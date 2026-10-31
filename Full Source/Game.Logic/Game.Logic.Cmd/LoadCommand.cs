using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(16, "游戏加载进度")]
public class LoadCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		player.LoadingProcess = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(16);
		if (game.GameState == eGameState.Loading)
		{
			if (player.LoadingProcess >= 100)
			{
				game.CheckState(0);
			}
			gSPacketIn.WriteInt(player.LoadingProcess);
			gSPacketIn.WriteInt(player.PlayerDetail.ZoneId);
			gSPacketIn.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
			game.SendToAll(gSPacketIn);
		}
		else if (game.RoomType == eRoomType.CampBattleBattle && game.GameSecondState == eGameState.Loading)
		{
			gSPacketIn.WriteInt(player.LoadingProcess);
			gSPacketIn.WriteInt(player.PlayerDetail.ZoneId);
			gSPacketIn.WriteInt(player.PlayerDetail.PlayerCharacter.ID);
			player.PlayerDetail.SendTCP(gSPacketIn);
			if (player.LoadingProcess >= 100)
			{
				(game as PVEGame).StartGameNewPlayer(player);
			}
		}
	}
}
