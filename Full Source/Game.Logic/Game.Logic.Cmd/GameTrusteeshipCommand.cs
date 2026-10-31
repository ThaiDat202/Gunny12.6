using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(149, "使用道具")]
public class GameTrusteeshipCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(149);
		gSPacketIn.Parameter2 = player.Id;
		gSPacketIn.WriteInt(game.PlayerCount);
		Player[] allPlayers = game.GetAllPlayers();
		foreach (Player player2 in allPlayers)
		{
			gSPacketIn.WriteInt(player2.Id);
			gSPacketIn.WriteBoolean(val: false);
		}
		player.PlayerDetail.SendTCP(gSPacketIn);
	}
}
