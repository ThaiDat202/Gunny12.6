using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(1)]
internal class DoubleReward : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		if (Player.PropBag.GetItemByTemplateID(0, 11916) == null)
		{
			return 0;
		}
		if (flag && !Player.PlayerCharacter.LabyrinthInfo.isUseDoubleProp && Player.RemoveTemplate(11916, 1))
		{
			Player.PlayerCharacter.LabyrinthInfo.isUseDoubleProp = true;
		}
		Player.Out.SendLabyrinthUpdataInfo(Player);
		return 0;
	}
}
