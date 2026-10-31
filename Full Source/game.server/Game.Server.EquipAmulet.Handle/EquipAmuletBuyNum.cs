using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.EquipAmulet.Handle;

[EquipAmuletHandleAttbute(1)]
public class EquipAmuletBuyNum : IEquipAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(386);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(player.PlayerCharacter.buyStiveNum);
		player.SendTCP(gSPacketIn);
		return 1;
	}
}
