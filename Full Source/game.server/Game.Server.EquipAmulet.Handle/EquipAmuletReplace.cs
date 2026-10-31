using System;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.EquipAmulet.Handle;

[EquipAmuletHandleAttbute(6)]
public class EquipAmuletReplace : IEquipAmuletCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (num == 1)
		{
			ItemInfo itemAt = player.EquipBag.GetItemAt(18);
			if (itemAt != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(386);
				gSPacketIn.WriteByte(6);
				gSPacketIn.WriteBoolean(val: true);
				for (int i = 0; i < 4; i++)
				{
					switch (i)
					{
					case 0:
						itemAt.Hole1 = player.newAmuletType[i];
						itemAt.Hole5 = player.newAmuletValue[i];
						break;
					case 1:
						itemAt.Hole2 = player.newAmuletType[i];
						itemAt.Hole6 = player.newAmuletValue[i];
						break;
					case 2:
						itemAt.Hole3 = player.newAmuletType[i];
						itemAt.Hole5Exp = player.newAmuletValue[i];
						break;
					case 3:
						itemAt.Hole4 = player.newAmuletType[i];
						itemAt.Hole6Exp = player.newAmuletValue[i];
						break;
					}
				}
				player.EquipBag.UpdateItem(itemAt);
				player.EquipBag.UpdatePlayerProperties();
				player.SendTCP(gSPacketIn);
			}
		}
		return 1;
	}
}
