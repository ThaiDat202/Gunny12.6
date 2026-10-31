using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(9, "开始移动")]
public class MoveStartCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (!player.IsAttacking)
		{
			return;
		}
		bool flag = packet.ReadBoolean();
		byte b = packet.ReadByte();
		int x = packet.ReadInt();
		int num = packet.ReadInt();
		byte dir = packet.ReadByte();
		bool flag2 = packet.ReadBoolean();
		short num2 = packet.ReadShort();
		if (game.TurnIndex == num2)
		{
		}
		game.SendPlayerMove(player, b, x, num, dir);
		byte b2 = b;
		byte b3 = b2;
		if ((uint)b3 <= 1u)
		{
			player.SetXY(x, num);
			player.StartMoving();
			if (player.Y - num > 1 || player.IsLiving != flag2)
			{
				game.SendPlayerMove(player, 3, player.X, player.Y, dir, flag2);
			}
		}
	}
}
