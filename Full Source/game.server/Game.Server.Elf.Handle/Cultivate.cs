using System;
using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Elf.Handle;

[ElfHandle(2)]
public class Cultivate : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		Sys_Users_Elf sys_Users_Elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (sys_Users_Elf == null)
		{
			player.SendMessage("Elf.Cultivate.NotFound");
			return 0;
		}
		Dictionary<int, TS_UpgradeTemplate> dictionary = ElfMgr.UpgradeList(sys_Users_Elf);
		if (!dictionary.ContainsKey(sys_Users_Elf.Grade + 1))
		{
			player.SendMessage("Elf.Cultivate.MaxLevel");
			return 0;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num);
		if (itemTemplateInfo == null)
		{
			throw new NullReferenceException($"item is null with templateId = {num}");
		}
		int property = itemTemplateInfo.Property2;
		int num2 = 1;
		if (flag)
		{
			num2 = (int)Math.Ceiling(((float)dictionary[sys_Users_Elf.Grade].Data - (float)sys_Users_Elf.Exp) / (float)property);
			int allItemCount = player.GetAllItemCount(num);
			if (allItemCount < num2)
			{
				num2 = allItemCount;
			}
		}
		if (!player.RemoveTemplate(num, num2))
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.Cultivate.NoItem"));
			return 0;
		}
		sys_Users_Elf.Exp += property * num2;
		int data = dictionary[sys_Users_Elf.Grade].Data;
		while (sys_Users_Elf.Exp >= data)
		{
			sys_Users_Elf.Grade++;
			sys_Users_Elf.Exp -= data;
			ElfMgr.CalculateGradeProps(sys_Users_Elf);
			if (dictionary.ContainsKey(sys_Users_Elf.Grade))
			{
				data = dictionary[sys_Users_Elf.Grade].Data;
				continue;
			}
			sys_Users_Elf.Exp = 0;
			break;
		}
		if (sys_Users_Elf.CultivateGoodFeling < 20)
		{
			sys_Users_Elf.GoodFeling++;
			sys_Users_Elf.CultivateGoodFeling++;
			ElfMgr.Out.SendUpdateFeeling(player, sys_Users_Elf);
		}
		ElfMgr.Out.SendUpdateGrade(player, sys_Users_Elf);
		player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.Cultivate.Success"));
		return 0;
	}
}
