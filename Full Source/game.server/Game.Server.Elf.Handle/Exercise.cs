using System;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Elf.Handle;

[ElfHandle(3)]
public class Exercise : IElfCommandHandler
{
	public static int ItemId = 12247;

	public static int NeedCount = 5;

	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		Random random = new Random();
		Sys_Users_Elf elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (elf == null)
		{
			player.SendMessage("Elf.Exercise.NotFound");
			return 0;
		}
		if (!player.RemoveTemplate(ItemId, NeedCount))
		{
			player.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Elf.Exercise.NoItem"));
			return 0;
		}
		Elf_Template_Info elf_Template_Info = ElfMgr.Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
		if (elf_Template_Info == null)
		{
			throw new NullReferenceException($"elfTemplate is null with templateId = {elf.TempID}");
		}
		int num = int.Parse(elf_Template_Info.HPGrowths.Split(',')[1]);
		int num2 = int.Parse(elf_Template_Info.AtckGrowths.Split(',')[1]);
		int num3 = int.Parse(elf_Template_Info.FireGrowths.Split(',')[1]);
		int num4 = int.Parse(elf_Template_Info.WindGrowths.Split(',')[1]);
		int num5 = int.Parse(elf_Template_Info.WateGrowths.Split(',')[1]);
		int num6 = int.Parse(elf_Template_Info.LandGrowths.Split(',')[1]);
		GSPacketIn gSPacketIn = new GSPacketIn(664);
		gSPacketIn.WriteByte(3);
		int num7 = random.Next(1, 7);
		int num8 = 0;
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(num7);
		for (int i = 0; i < num7; i++)
		{
			num8 = random.Next(-10, 10);
			switch (i)
			{
			case 0:
				if (num8 + elf.HpGrowth > num)
				{
					elf.HpGrowth = num;
				}
				else
				{
					elf.HpGrowth += num8;
				}
				break;
			case 1:
				if (num8 + elf.AttackGrowth > num2)
				{
					elf.AttackGrowth = num2;
				}
				else
				{
					elf.AttackGrowth += num8;
				}
				break;
			case 2:
				if (num8 + elf.HuoGrowth > num3)
				{
					elf.HuoGrowth = num3;
				}
				else
				{
					elf.HuoGrowth += num8;
				}
				break;
			case 3:
				if (num8 + elf.ShuiGrowth > num5)
				{
					elf.ShuiGrowth = num5;
				}
				else
				{
					elf.ShuiGrowth += num8;
				}
				break;
			case 4:
				if (num8 + elf.TuGrowth > num6)
				{
					elf.TuGrowth = num6;
				}
				else
				{
					elf.TuGrowth += num8;
				}
				break;
			case 5:
				if (num8 + elf.FengGrowth > num4)
				{
					elf.FengGrowth = num4;
				}
				else
				{
					elf.FengGrowth += num8;
				}
				break;
			}
			gSPacketIn.WriteInt(num8);
		}
		player.Out.SendTCP(gSPacketIn);
		elf.Quality = ElfMgr.CalculateQuality(elf);
		ElfMgr.CalculateGradeProps(elf, elf.Grade - 1);
		return 0;
	}
}
