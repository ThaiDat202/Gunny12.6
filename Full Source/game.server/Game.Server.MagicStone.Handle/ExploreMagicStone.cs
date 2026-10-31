using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;

namespace Game.Server.MagicStone.Handle;

[MagicStoneHandleAttbute(1)]
public class ExploreMagicStone : IMagicStoneCommandHadler
{
	public static Random random = new Random();

	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		int num2 = packet.ReadInt();
		if (num2 > 999)
		{
			return false;
		}
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		PlayerMagicStoneInventory magicStoneBag = Player.MagicStoneBag;
		string[] array = GameProperties.OpenMagicBoxMoney.Split(',');
		if (array.Length == 0)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ExploreMagicStone.Msg1"));
			return false;
		}
		int value = int.Parse(array[0]);
		int value2 = int.Parse(array[1]);
		int minValue = 1;
		int maxValue = 3;
		switch (num)
		{
		case 2:
			minValue = 1;
			maxValue = 4;
			break;
		case 3:
			minValue = 2;
			maxValue = 5;
			break;
		}
		int num3 = magicStoneBag.FindEmptySlotCount();
		if (num3 == 0)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ExploreMagicStone.Msg2"));
			return false;
		}
		if (num2 > num3)
		{
			num2 = num3;
		}
		MoneyType type = ((!flag) ? MoneyType.Money : MoneyType.DDTMoney);
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < num2; i++)
		{
			int num4 = random.Next(minValue, maxValue);
			if (num4 == 4)
			{
				num4 = 3;
			}
			List<ItemTemplateInfo> list2 = ItemMgr.FindMagicStoneTemplate(random.Next(minValue, maxValue));
			if (list2.Count == 0)
			{
				return false;
			}
			if (Player.MoneyDirect(type, value))
			{
				int index = random.Next(list2.Count);
				ItemInfo itemInfo = magicStoneBag.CreateMagicstone(list2[index], 1);
				itemInfo.IsBinds = flag;
				int place = magicStoneBag.FindFirstEmptySlot();
				if (magicStoneBag.AddItemTo(itemInfo, place))
				{
					Player.AddScoreMagicstone(value2);
				}
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(258, Player.PlayerId);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(Player.Extra.Info.ScoreMagicstone);
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
