using System.Collections.Generic;
using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using MoreLinq;
using SqlDataProvider.Data;

namespace Game.Server.Elf;

public class ElfPacketsOut
{
    //__onUpdateElf, addElf
    public void SendUpdateElf(GamePlayer player, ICollection<Sys_Users_Elf> items)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteBoolean(player.PlayerCharacter.IsHideElf);
		gSPacketIn.WriteInt(items.Count);
		foreach (Sys_Users_Elf item in items)
		{
			gSPacketIn.WriteInt(item.ElfID);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(item.TempID);
			gSPacketIn.WriteInt(item.Exp);
			gSPacketIn.WriteInt(item.Grade);
			gSPacketIn.WriteInt(item.HuoBase);
			gSPacketIn.WriteInt(item.AttackBase);
			gSPacketIn.WriteInt(item.HuoBase);
			gSPacketIn.WriteInt(item.ShuiBase);
			gSPacketIn.WriteInt(item.TuBase);
			gSPacketIn.WriteInt(item.FengBase);
			if (GameProperties.VERSION >= 12100)
			{
                gSPacketIn.WriteInt(item.GuangBase); //_loc7_.guangBase = param1.pkg.readInt();
                gSPacketIn.WriteInt(item.AnBase); //_loc7_.anBase = param1.pkg.readInt();
            }
			gSPacketIn.WriteInt(item.HpValue);
			gSPacketIn.WriteInt(item.AttackValue);
			gSPacketIn.WriteInt(item.HuoValue);
			gSPacketIn.WriteInt(item.ShuiValue);
			gSPacketIn.WriteInt(item.TuValue);
			gSPacketIn.WriteInt(item.FengValue);
            if (GameProperties.VERSION >= 12100)
            {
                gSPacketIn.WriteInt(item.GuangValue); //_loc7_.guangValue = param1.pkg.readInt();
                gSPacketIn.WriteInt(item.AnValue); //_loc7_.anValue = param1.pkg.readInt();
            }
            gSPacketIn.WriteInt(item.HpGrowth);
			gSPacketIn.WriteInt(item.AttackGrowth);
			gSPacketIn.WriteInt(item.HuoGrowth);
			gSPacketIn.WriteInt(item.ShuiGrowth);
			gSPacketIn.WriteInt(item.TuGrowth);
			gSPacketIn.WriteInt(item.FengGrowth);
            if (GameProperties.VERSION >= 12100)
            {
                gSPacketIn.WriteInt(item.GuangGrowth); //_loc7_.guangGrowth = param1.pkg.readInt();
                gSPacketIn.WriteInt(item.AnGrowth); //_loc7_.anGrowth = param1.pkg.readInt();
            }
            gSPacketIn.WriteInt(item.GoodFeling);
			gSPacketIn.WriteBoolean(item.IsFight);
			gSPacketIn.WriteInt(item.Quality);
			gSPacketIn.WriteString(item.SkillList);
			gSPacketIn.WriteInt(item.DungeonGoodFeling);
			gSPacketIn.WriteInt(item.GameGoodFeling);
			gSPacketIn.WriteInt(item.CultivateGoodFeling);
            if (GameProperties.VERSION >= 12100)
            {
                if (player.PlayerCharacter.IsHideElf)
                {
                    gSPacketIn.WriteInt(0);
                }
                else
                {
                    gSPacketIn.WriteInt(item.ElfEquipId);
                }
            }
        }
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateElf(GamePlayer player, Sys_Users_Elf elfInfo)
	{
		SendUpdateElf(player, new List<Sys_Users_Elf> { elfInfo });
	}

	public void SendUpdateElf(GamePlayer player)
	{
		SendUpdateElf(player, player.PlayerCharacter.ElfInfo);
	}

	public void SendUpdateElfFight(GamePlayer player, Sys_Users_Elf item)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(6);
		gSPacketIn.WriteInt(item.ElfID);
		gSPacketIn.WriteBoolean(item.IsFight);
		player.SendTCP(gSPacketIn);
	}

	public void SendDiscardElf(GamePlayer player, int elfId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(elfId);
		gSPacketIn.WriteBoolean(val: false);
		player.SendTCP(gSPacketIn);
	}

	public void SendSummonResult(GamePlayer player, List<ItemInfo> items)
	{
		GSPacketIn packet = new GSPacketIn(664, player.PlayerId);
		packet.WriteByte(8);
		packet.WriteInt(player.GetSummonCount(1));
		packet.WriteInt(player.GetSummonCount(2));
		packet.WriteInt(items.Count);
		items.ForEach(delegate(ItemInfo i)
		{
			packet.WriteInt(i.TemplateID);
			packet.WriteInt(i.Count);
			packet.WriteBoolean(i.IsBinds);
		});
		player.SendTCP(packet);
	}

	public void SendSkillBooks(GamePlayer player)
	{
		GSPacketIn packet = new GSPacketIn(664, player.PlayerId);
		packet.WriteByte(9);
		packet.WriteInt(player.PlayerCharacter.SkillBooks.Count);
		player.PlayerCharacter.SkillBooks.ForEach(delegate(Sys_Users_SkillBooks i)
		{
			packet.WriteInt(i.TemplateID);
			packet.WriteInt(i.IsBind ? 1 : 0);
			packet.WriteInt(i.Count);
		});
		player.SendTCP(packet);
	}

	public void SendSkillBooks(GamePlayer player, Sys_Users_SkillBooks book)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(book.TemplateID);
		gSPacketIn.WriteInt(book.IsBind ? 1 : 0);
		gSPacketIn.WriteInt(book.Count);
		player.SendTCP(gSPacketIn);
	}

    //__onAddGrowth
    public void SendUpdateProps(GamePlayer player, Sys_Users_Elf elf)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(4);
		gSPacketIn.WriteInt(elf.ElfID);
		gSPacketIn.WriteInt(elf.HpValue);
		gSPacketIn.WriteInt(elf.AttackValue);
		gSPacketIn.WriteInt(elf.HuoValue);
		gSPacketIn.WriteInt(elf.ShuiValue);
		gSPacketIn.WriteInt(elf.TuValue);
		gSPacketIn.WriteInt(elf.FengValue);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(elf.GuangValue); //_loc4_.guangValue = param1.pkg.readInt();
            gSPacketIn.WriteInt(elf.AnValue); //_loc4_.anValue = param1.pkg.readInt();
        }
        gSPacketIn.WriteInt(elf.HpGrowth);
		gSPacketIn.WriteInt(elf.AttackGrowth);
		gSPacketIn.WriteInt(elf.HuoGrowth);
		gSPacketIn.WriteInt(elf.ShuiGrowth);
		gSPacketIn.WriteInt(elf.TuGrowth);
		gSPacketIn.WriteInt(elf.FengGrowth);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(elf.GuangGrowth); //_loc4_.guangGrowth = param1.pkg.readInt();
            gSPacketIn.WriteInt(elf.AnGrowth); //_loc4_.anGrowth = param1.pkg.readInt();
        }
        gSPacketIn.WriteInt(elf.Quality);
		player.SendTCP(gSPacketIn);
	}

    //__onExerciseSave
    public void SendExerciseProps(GamePlayer player, Sys_Users_Elf elf)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(13);
		gSPacketIn.WriteInt(elf.ElfID);
		gSPacketIn.WriteInt(elf.HpValue);
		gSPacketIn.WriteInt(elf.AttackValue);
		gSPacketIn.WriteInt(elf.HuoValue);
		gSPacketIn.WriteInt(elf.ShuiValue);
		gSPacketIn.WriteInt(elf.TuValue);
		gSPacketIn.WriteInt(elf.FengValue);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(elf.GuangValue); //_loc3_.guangValue = param1.pkg.readInt();
            gSPacketIn.WriteInt(elf.AnValue); //_loc3_.anValue = param1.pkg.readInt();
        }
        gSPacketIn.WriteInt(elf.HpGrowth);
		gSPacketIn.WriteInt(elf.AttackGrowth);
		gSPacketIn.WriteInt(elf.HuoGrowth);
		gSPacketIn.WriteInt(elf.ShuiGrowth);
		gSPacketIn.WriteInt(elf.TuGrowth);
		gSPacketIn.WriteInt(elf.FengGrowth);
        if (GameProperties.VERSION >= 12100)
        {
            gSPacketIn.WriteInt(elf.GuangGrowth); //_loc3_.guangGrowth = param1.pkg.readInt();
            gSPacketIn.WriteInt(elf.AnGrowth); //_loc3_.anGrowth = param1.pkg.readInt();
        }
        gSPacketIn.WriteInt(elf.Quality);
		player.SendTCP(gSPacketIn);
	}

    //__onUpdateCultivate
    public void SendUpdateGrade(GamePlayer player, Sys_Users_Elf elf)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(elf.ElfID);
		gSPacketIn.WriteInt(elf.Exp);
		gSPacketIn.WriteInt(elf.Grade);
		gSPacketIn.WriteInt(elf.HpValue);
		gSPacketIn.WriteInt(elf.AttackValue);
		gSPacketIn.WriteInt(elf.HuoValue);
		gSPacketIn.WriteInt(elf.ShuiValue);
		gSPacketIn.WriteInt(elf.TuValue);
		gSPacketIn.WriteInt(elf.FengValue);
		if (GameProperties.VERSION >= 12100)
		{
            gSPacketIn.WriteInt(elf.GuangValue); //_loc3_.guangValue = param1.pkg.readInt();
            gSPacketIn.WriteInt(elf.AnValue); //_loc3_.anValue = param1.pkg.readInt();
        }
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateSkill(GamePlayer player, Sys_Users_Elf elf)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(elf.ElfID);
		gSPacketIn.WriteString(elf.SkillList);
		player.SendTCP(gSPacketIn);
	}

	public void SendUpdateFeeling(GamePlayer player, Sys_Users_Elf elf)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(15);
		gSPacketIn.WriteInt(elf.ElfID);
		gSPacketIn.WriteInt(elf.GoodFeling);
		gSPacketIn.WriteInt(elf.DungeonGoodFeling);
		gSPacketIn.WriteInt(elf.GameGoodFeling);
		gSPacketIn.WriteInt(elf.CultivateGoodFeling);
		player.SendTCP(gSPacketIn);
	}

	public void SendElfCoin(GamePlayer player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(664, player.PlayerId);
		gSPacketIn.WriteByte(14);
		gSPacketIn.WriteInt(player.Extra.Info.Coin);
		player.SendTCP(gSPacketIn);
	}

    public void SendUpdateElfEquipLvInfo(GamePlayer player)
    {
        GSPacketIn gSPacketIns = new GSPacketIn(707, player.PlayerId);
        gSPacketIns.WriteByte(1);

        gSPacketIns.WriteInt(player.PlayerCharacter.ElfInfo.Count);

        var elfTemplateInfo = ElfMgr.Elfs;
        foreach (Sys_Users_Elf item in player.PlayerCharacter.ElfInfo)
        {
            gSPacketIns.WriteInt(elfTemplateInfo.FirstOrDefault((Elf_Template_Info p) => p.TemplateID == item.TempID).ElfType);
            gSPacketIns.WriteInt(1); //Level
        }
        player.SendTCP(gSPacketIns);
    }

    public void SendUpdateElfEquipInfo(GamePlayer player)
    {
        GSPacketIn gSPacketIns = new GSPacketIn(707, player.PlayerId);
        gSPacketIns.WriteByte(2);
        gSPacketIns.WriteInt(player.PlayerCharacter.ElfInfo.Count);

		var elfTemplateInfo = ElfMgr.Elfs;
        foreach (Sys_Users_Elf item in player.PlayerCharacter.ElfInfo)
        {
            gSPacketIns.WriteInt(item.ElfID);
            gSPacketIns.WriteInt(elfTemplateInfo.FirstOrDefault((Elf_Template_Info p)=>p.TemplateID == item.TempID).ElfType);
            gSPacketIns.WriteInt(item.Quality);
        }
        player.SendTCP(gSPacketIns);
    }
}
