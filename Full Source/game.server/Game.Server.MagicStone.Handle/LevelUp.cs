using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.MagicStone.Handle;

[MagicStoneHandleAttbute(2)]
public class LevelUp : IMagicStoneCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.HasBagPassword && Player.PlayerCharacter.IsLocked)
		{
			Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return false;
		}
		PlayerMagicStoneInventory magicStoneBag = Player.MagicStoneBag;
		ItemInfo itemAt = magicStoneBag.GetItemAt(31);
		if (itemAt == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("MagicStone.LevelUp"));
			return false;
		}
		List<int> list = new List<int>();
		int num = 0;
		int num2 = packet.ReadInt();
		MagicStoneInfo magicStoneInfo = MagicStoneMgr.FindMagicStoneByLevel(itemAt.TemplateID, 10);
		if (magicStoneInfo == null)
		{
			Player.SendMessage("Database Error.");
			return false;
		}
		for (int i = 0; i < num2; i++)
		{
			int num3 = packet.ReadInt();
			ItemInfo itemAt2 = magicStoneBag.GetItemAt(num3);
			if (itemAt2 != null && !itemAt2.goodsLock)
			{
				list.Add(num3);
				num += itemAt2.StrengthenExp;
			}
			if (itemAt.StrengthenExp + num >= magicStoneInfo.Exp)
			{
				break;
			}
		}
		itemAt.StrengthenExp += num;
		List<MagicStoneInfo> list2 = MagicStoneMgr.FindMagicStone(itemAt.TemplateID);
		foreach (MagicStoneInfo item in list2)
		{
			if (itemAt.StrengthenExp >= item.Exp)
			{
				itemAt.StrengthenLevel = item.Level;
				if (itemAt.AttackCompose > 0 && item.Attack > itemAt.AttackCompose)
				{
					itemAt.AttackCompose = item.Attack;
				}
				if (itemAt.DefendCompose > 0 && item.Defence > itemAt.DefendCompose)
				{
					itemAt.DefendCompose = item.Defence;
				}
				if (itemAt.AgilityCompose > 0 && item.Agility > itemAt.AgilityCompose)
				{
					itemAt.AgilityCompose = item.Agility;
				}
				if (itemAt.LuckCompose > 0 && item.Luck > itemAt.LuckCompose)
				{
					itemAt.LuckCompose = item.Luck;
				}
				if (itemAt.MagicAttack > 0 && item.MagicAttack > itemAt.MagicAttack)
				{
					itemAt.MagicAttack = item.MagicAttack;
				}
				if (itemAt.MagicDefence > 0 && item.MagicDefence > itemAt.MagicDefence)
				{
					itemAt.MagicDefence = item.MagicDefence;
				}
			}
		}
		magicStoneBag.UpdateItem(itemAt);
		magicStoneBag.RemoveAllItem(list);
		return true;
	}
}
