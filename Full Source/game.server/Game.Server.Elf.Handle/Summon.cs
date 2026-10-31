using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using MoreLinq;
using SqlDataProvider.Data;

namespace Game.Server.Elf.Handle;

[ElfHandle(8)]
public class Summon : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		int num3 = -1;
		List<ItemInfo> list = new List<ItemInfo>();
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		switch (num)
		{
		case 3:
			num3 = 12241;
			break;
		case 4:
			num3 = 12242;
			break;
		}
		if (num3 == -1)
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.Summon.Failed"));
			return 0;
		}
		if (!player.RemoveTemplate(num3, num2))
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.Summon.ServerSideError"));
			return 0;
		}
		SpecialItemBoxInfo specialValue = new SpecialItemBoxInfo();
		for (int j = 0; j < num2; j++)
		{
			List<ItemInfo> list2 = new List<ItemInfo>();
			ItemBoxMgr.CreateItemBox(num3, list2, ref specialValue);
			foreach (ItemInfo item in list2)
			{
				list.Add(item);
			}
		}
		ElfMgr.Out.SendSummonResult(player, list);
		foreach (ItemInfo item2 in list)
		{
			if (!dictionary.Keys.Contains(item2.TemplateID))
			{
				dictionary.Add(item2.TemplateID, item2);
			}
			else
			{
				dictionary[item2.TemplateID].Count += item2.Count;
			}
		}
		dictionary.ForEach(delegate(KeyValuePair<int, ItemInfo> i)
		{
			if (i.Value.Template.MaxCount == 1)
			{
				for (int k = 0; k < i.Value.Count; k++)
				{
					ItemInfo itemInfo = i.Value.Clone();
					itemInfo.Count = 1;
					player.AddTemplate(itemInfo, i.Value.Template.BagType, itemInfo.Count, eGameView.OtherTypeGet, i.Value.Template.Name);
				}
			}
			else
			{
				player.AddTemplate(i.Value, i.Value.Template.BagType, i.Value.Count, eGameView.OtherTypeGet, i.Value.Template.Name);
			}
		});
		return 0;
	}

	public int RandomElf(int starLevel)
	{
		Random random = new Random();
		int num = 0;
		int result = 0;
		if (starLevel == 3)
		{
			num = random.Next(1, 5);
			result = 1122188 + 3 * (num - 1);
		}
		if (starLevel == 4)
		{
			num = random.Next(1, 5);
			result = 1122189 + 3 * (num - 1);
		}
		if (starLevel == 5)
		{
			num = random.Next(1, 5);
			result = 1122190 + 3 * (num - 1);
		}
		return result;
	}
}
