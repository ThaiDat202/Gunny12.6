using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.EquipAmulet.Handle;

[EquipAmuletHandleAttbute(2)]
public class EquipAmuletUpgrade : IEquipAmuletCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		bool val = false;
		ItemInfo itemAt = player.GetItemAt(eBageType.EquipBag, 18);
		if (itemAt != null)
		{
			AmuletItemInfo amuletItemInfo = AmuletItemMgr.FindAmuletItem(itemAt.StrengthenLevel);
			if (amuletItemInfo != null)
			{
				int num = ((!flag) ? 1 : amuletItemInfo.GuaranteeTimes);
				int num2 = 0;
				for (int i = 0; i < num; i++)
				{
					if (itemAt.StrengthenLevel == AmuletItemMgr.maxLevel)
					{
						break;
					}
					if (!player.RemoveTemplate(amuletItemInfo.LvlupStep, 1))
					{
						break;
					}
					if (rand.Next(10000000) <= amuletItemInfo.SuccessChance || amuletItemInfo.GuaranteeTimes - itemAt.StrengthenTimes <= 1)
					{
						val = true;
						num2 = itemAt.StrengthenTimes;
						itemAt.StrengthenLevel++;
						itemAt.StrengthenTimes = 0;
						player.EquipBag.UpdatePlayerProperties();
						break;
					}
					itemAt.StrengthenTimes++;
					num2++;
				}
				GSPacketIn gSPacketIn = new GSPacketIn(386);
				gSPacketIn.WriteByte(2);
				gSPacketIn.WriteBoolean(val);
				gSPacketIn.WriteInt(num2);
				gSPacketIn.WriteInt(itemAt.StrengthenLevel);
				gSPacketIn.WriteInt(itemAt.StrengthenTimes);
				player.SendTCP(gSPacketIn);
				player.EquipBag.UpdateItem(itemAt);
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.LoadDataFail"));
			}
		}
		return 1;
	}
}
