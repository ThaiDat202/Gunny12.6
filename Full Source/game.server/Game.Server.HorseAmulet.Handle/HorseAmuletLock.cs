using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HorseAmulet.Handle;

[HorseAmuletHandleAttbute(4)]
public class HorseAmuletLock : IHorseAmuletCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		ItemInfo itemAt = player.HorseAmuletBag.GetItemAt(packet.ReadInt());
		if (itemAt != null)
		{
			itemAt.cellLocked = !itemAt.cellLocked;
			player.HorseAmuletBag.UpdateItem(itemAt);
		}
		return 1;
	}
}
