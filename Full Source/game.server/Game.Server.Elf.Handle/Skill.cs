using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using MoreLinq;

namespace Game.Server.Elf.Handle;

[ElfHandle(5)]
public class Skill : IElfCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int elfId = packet.ReadInt();
		int skillId = packet.ReadInt();
		int num = packet.ReadInt();
		List<int> lockSkill = new List<int>();
		int num2 = packet.ReadInt();
		if (num2 > 7)
		{
			return 0;
		}
		for (int j = 0; j < num2; j++)
		{
			lockSkill.Add(packet.ReadInt());
		}
		Sys_Users_Elf elf = player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.ElfID == elfId);
		if (elf == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.ElfNotFound"));
			return 0;
		}
		Sys_Users_SkillBooks sys_Users_SkillBooks = player.PlayerCharacter.SkillBooks.FirstOrDefault((Sys_Users_SkillBooks b) => b.TemplateID == skillId);
		if (sys_Users_SkillBooks == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.BookNotFound"));
			return 0;
		}
		Elf_Template_Info elf_Template_Info = ElfMgr.Elfs.FirstOrDefault((Elf_Template_Info e) => e.TemplateID == elf.TempID);
		if (elf_Template_Info == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.ServerSideError"));
			return 0;
		}
		Elf_SkillBook elf_SkillBook = ElfMgr.ElfSkills.FirstOrDefault((Elf_SkillBook e) => e.TemplateId == skillId);
		if (elf_SkillBook == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.ServerSideError"));
			return 0;
		}
		List<int> list = (string.IsNullOrEmpty(elf.SkillList) ? new List<int>() : elf.SkillList.Split(',').ToList().Select(int.Parse)
			.ToList());
		if (list.Contains(elf_SkillBook.SkillTempId))
		{
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.AlreadyLearned"));
			return 0;
		}
		int num3 = ElfMgr.Random.Next(0, 1000);
		if (num3 >= elf_SkillBook.Randoms)
		{
			ReduceBook(player, sys_Users_SkillBooks);
			player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.TryAgain"));
			return 0;
		}
		if (list.Count >= int.Parse(elf_Template_Info.MaxSkillCount))
		{
			Dictionary<int, int> skills = new Dictionary<int, int>();
			list.ForEach(delegate(int c)
			{
				skills.Add(c, lockSkill.Contains(c) ? c : 0);
			});
			Dictionary<int, int> dictionary = skills.Where((KeyValuePair<int, int> i) => i.Value == 0).ToDictionary();
			KeyValuePair<int, int> keyValuePair = dictionary.ElementAt(ElfMgr.Random.Next(dictionary.Count));
			skills[keyValuePair.Key] = elf_SkillBook.SkillTempId;
			List<int> newSkills = new List<int>();
			skills.ForEach(delegate(KeyValuePair<int, int> s)
			{
				newSkills.Add((s.Value == 0) ? s.Key : s.Value);
			});
			elf.SkillList = string.Join(",", newSkills);
		}
		else
		{
			list.Add(elf_SkillBook.SkillTempId);
			elf.SkillList = string.Join(",", list);
		}
		ElfMgr.Out.SendUpdateSkill(player, elf);
		ReduceBook(player, sys_Users_SkillBooks);
		player.SendMessage(LanguageMgr.GetTranslation("Elf.Skill.Success"));
		return 0;
	}

	private void ReduceBook(GamePlayer player, Sys_Users_SkillBooks skillBook)
	{
		skillBook.Count--;
		ElfMgr.Out.SendSkillBooks(player, skillBook);
		if (skillBook.Count <= 0)
		{
			player.PlayerCharacter.SkillBooks.Remove(skillBook);
		}
	}
}
