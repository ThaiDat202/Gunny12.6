using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(18)]
public class RevertPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		int value = Convert.ToInt32(PetMgr.FindConfig("RecycleCost").Value);
		if (player.MoneyDirect(value))
		{
			UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
			if (petAt == null)
			{
				return false;
			}
			petAt.BaseProp = JsonConvert.SerializeObject(petAt);
			UsersPetInfo usersPetInfo = JsonConvert.DeserializeObject<UsersPetInfo>(petAt.BaseProp);
			if (usersPetInfo == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg7"));
				return false;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334100);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			itemInfo.IsBinds = true;
			itemInfo.DefendCompose = petAt.GP;
			itemInfo.AgilityCompose = petAt.MaxGP;
			itemInfo.Hole1 = petAt.breakGrade;
			itemInfo.Hole2 = petAt.breakAttack;
			itemInfo.Hole3 = petAt.breakDefence;
			itemInfo.Hole4 = petAt.breakAgility;
			itemInfo.Hole5 = petAt.breakLuck;
			itemInfo.Blood = petAt.breakBlood;
			if (!player.PropBag.AddTemplate(itemInfo, 1))
			{
				player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
				player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			petAt.breakGrade = usersPetInfo.breakGrade;
			petAt.breakAttack = usersPetInfo.breakAttack;
			petAt.breakDefence = usersPetInfo.breakDefence;
			petAt.breakAgility = usersPetInfo.breakAgility;
			petAt.breakLuck = usersPetInfo.breakLuck;
			petAt.breakBlood = usersPetInfo.breakBlood;
			petAt.Attack = usersPetInfo.Attack;
			petAt.Defence = usersPetInfo.Defence;
			petAt.Agility = usersPetInfo.Agility;
			petAt.Luck = usersPetInfo.Luck;
			petAt.Blood = usersPetInfo.Blood;
			petAt.TemplateID = usersPetInfo.TemplateID;
			petAt.Skill = usersPetInfo.Skill;
			petAt.SkillEquip = usersPetInfo.SkillEquip;
			petAt.Skill = PetMgr.UpdateSkillPet(1, petAt.TemplateID, player.PlayerCharacter.Grade);
			petAt.SkillEquip = PetMgr.ActiveEquipSkill(1, player.PlayerCharacter.VIPLevel);
			petAt.GP = 0;
			petAt.Level = 1;
			petAt.MaxGP = 55;
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg8"));
			player.PetBag.UpdatePet(petAt);
			player.PetBag.SaveToDatabase(saveAdopt: false);
		}
		return false;
	}
}
