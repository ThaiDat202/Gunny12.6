using System;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.EquipAmulet.Handle;

[EquipAmuletHandleAttbute(3)]
public class EquipAmuletActivate : IEquipAmuletCommandHadler
{
	private Random rand = new Random();

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		int[] array = new int[num];
		for (int i = 0; i < num; i++)
		{
			array[i] = packet.ReadInt();
		}
		ItemInfo itemAt = player.EquipBag.GetItemAt(18);
		if (itemAt != null)
		{
			AmuletItemInfo amuletItemInfo = AmuletItemMgr.FindAmuletItem(itemAt.StrengthenLevel);
			if (amuletItemInfo != null)
			{
				AmuletPhaseItemInfo amuletPhaseItemInfo = AmuletPhaseItemMgr.FindAmuletPhaseItem(amuletItemInfo.WashsStep);
				if (amuletPhaseItemInfo == null)
				{
					player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.LoadDataFail"));
					return 0;
				}
				if (player.MoneyDirect(amuletPhaseItemInfo.LockPrice * num))
				{
					if (player.PlayerCharacter.stive >= amuletPhaseItemInfo.Expend)
					{
						AmuletGradeItemInfo amuletGradeItemInfo = AmuletGradeItemMgr.FindAmuletGradeByGp(itemAt.StrengthenExp);
						int i = 0;
						while (i < 4)
						{
							int num2 = rand.Next(1, 10);
							if (!player.newAmuletType.Contains(num2) && !array.Contains(num2))
							{
								player.newAmuletType[i] = num2;
								if (amuletGradeItemInfo == null)
								{
									player.newAmuletValue[i] = rand.Next(1, 10000);
								}
								else
								{
									player.newAmuletValue[i] = rand.Next(amuletGradeItemInfo.Minvalue, amuletGradeItemInfo.Maxvalue);
								}
								i++;
							}
						}
						for (i = 0; i < 4; i++)
						{
							switch (i)
							{
							case 0:
								if (array.Contains(itemAt.Hole1))
								{
									player.newAmuletType[i] = itemAt.Hole1;
									player.newAmuletValue[i] = itemAt.Hole5;
								}
								break;
							case 1:
								if (array.Contains(itemAt.Hole2))
								{
									player.newAmuletType[i] = itemAt.Hole2;
									player.newAmuletValue[i] = itemAt.Hole6;
								}
								break;
							case 2:
								if (array.Contains(itemAt.Hole3))
								{
									player.newAmuletType[i] = itemAt.Hole3;
									player.newAmuletValue[i] = itemAt.Hole5Exp;
								}
								break;
							case 3:
								if (array.Contains(itemAt.Hole4))
								{
									player.newAmuletType[i] = itemAt.Hole4;
									player.newAmuletValue[i] = itemAt.Hole6Exp;
								}
								break;
							}
						}
						itemAt.StrengthenExp++;
						GSPacketIn gSPacketIn = new GSPacketIn(386);
						gSPacketIn.WriteByte(3);
						gSPacketIn.WriteInt(itemAt.StrengthenExp);
						gSPacketIn.WriteInt(player.WashFightPower);
						gSPacketIn.WriteInt(player.PlayerCharacter.stive);
						for (i = 0; i < 4; i++)
						{
							gSPacketIn.WriteInt(player.newAmuletType[i]);
							gSPacketIn.WriteInt(player.newAmuletValue[i]);
						}
						player.SendTCP(gSPacketIn);
						player.PlayerCharacter.stive -= amuletPhaseItemInfo.Expend;
						player.EquipBag.UpdatePlayerProperties();
					}
					else
					{
						player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.StiveNotEnough"));
					}
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.StiveFail"));
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.LoadDataFail"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("EquipAmuletActivate.NotFoundAmulet"));
		}
		return 1;
	}
}
