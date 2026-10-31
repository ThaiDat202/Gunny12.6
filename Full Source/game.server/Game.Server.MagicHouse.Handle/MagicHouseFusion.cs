using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.MagicHouse.Handle;

[MagicHouseHandleAttbute(7)]
public class MagicHouseFusion : IMagicHouseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		MagicFusionDataInfo magicFusionDataInfo = MagicFusionDataMgr.FindMagicFusionData(iD);
		bool flag2 = false;
		if (magicFusionDataInfo != null && magicFusionDataInfo.Type == 1)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(magicFusionDataInfo.ItemID);
			if (itemTemplateInfo == null)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseFusion.DataError"));
			}
			else
			{
				int num2 = 0;
				int num3 = 0;
				if (magicFusionDataInfo.Item1 > 0)
				{
					if (Player.GetItemByTemplateID(magicFusionDataInfo.Item1) != null)
					{
						num2++;
					}
					num3++;
				}
				if (magicFusionDataInfo.Item2 > 0)
				{
					if (Player.GetItemByTemplateID(magicFusionDataInfo.Item2) != null)
					{
						num2++;
					}
					num3++;
				}
				if (magicFusionDataInfo.Item3 > 0)
				{
					if (Player.GetItemByTemplateID(magicFusionDataInfo.Item3) != null)
					{
						num2++;
					}
					num3++;
				}
				if (magicFusionDataInfo.Item4 > 0)
				{
					if (Player.GetItemByTemplateID(magicFusionDataInfo.Item4) != null)
					{
						num2++;
					}
					num3++;
				}
				if (num2 == num3)
				{
					if (magicFusionDataInfo.Item1 > 0)
					{
						Player.RemoveTemplate(magicFusionDataInfo.Item1, magicFusionDataInfo.Item1Count);
					}
					if (magicFusionDataInfo.Item2 > 0)
					{
						Player.RemoveTemplate(magicFusionDataInfo.Item2, magicFusionDataInfo.Item2Count);
					}
					if (magicFusionDataInfo.Item3 > 0)
					{
						Player.RemoveTemplate(magicFusionDataInfo.Item3, magicFusionDataInfo.Item3Count);
					}
					if (magicFusionDataInfo.Item4 > 0)
					{
						Player.RemoveTemplate(magicFusionDataInfo.Item4, magicFusionDataInfo.Item4Count);
					}
					int num4 = magicFusionDataInfo.NeedKey * num;
					int num5 = magicFusionDataInfo.NeedGold * num;
					flag2 = Player.MagicHouse.Info.essence >= num4 && Player.PlayerCharacter.Gold >= num5;
					if (flag2)
					{
						Player.MagicHouse.Info.essence -= num4;
						Player.RemoveGold(num5);
						Player.MagicHouse.UpdateMessage();
						Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseFusion.Success"));
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, num, 105);
						itemInfo.IsBinds = true;
						Player.AddTemplate(itemInfo);
					}
					else
					{
						Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseFusion.NotEnoughtEssence"));
					}
				}
				else
				{
					Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseFusion.NotEnoughtMaterial"));
				}
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseFusion.WrongItem"));
		}
		GSPacketIn gSPacketIn = new GSPacketIn(84, Player.PlayerId);
		gSPacketIn.WriteInt(4);
		gSPacketIn.WriteInt(7);
		gSPacketIn.WriteBoolean(flag2);
		Player.SendTCP(gSPacketIn);
		return false;
	}
}
