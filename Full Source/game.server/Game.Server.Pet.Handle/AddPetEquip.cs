using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(20)]
public class AddPetEquip : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int bageType = packet.ReadInt();
		int slot = packet.ReadInt();
		int place = packet.ReadInt();
		PlayerInventory inventory = player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (itemAt != null && itemAt.IsEquipPet() && itemAt.IsValidItem())
		{
			if (player.PetBag.CheckEqPetLevel(place, itemAt))
			{
				player.SendMessage(LanguageMgr.GetTranslation("AddPetEquip.WrongLevel"));
				return false;
			}
			if (!itemAt.IsUsed)
			{
				itemAt.IsUsed = true;
				itemAt.BeginDate = DateTime.Now;
			}
			if (player.PetBag.AddEqPet(place, itemAt))
			{
				inventory.TakeOutItem(itemAt);
				player.PetBag.OnChangedPetEquip(place);
				player.SendMessage(LanguageMgr.GetTranslation("AddPetEquip.Success"));
				foreach (ItemAwakenPet itemAwaken in player.PetBag.ItemAwakens)
				{
					if (itemAwaken.itemId == itemAt.ItemID && itemAwaken.petId == PetMgr.FindPetTemplate(player.Pet.TemplateID).KindID)
					{
						player.Pet.Skill = PetMgr.UpdateAwakenSkill(player.Pet.Skill, player.Pet.TemplateID, itemAwaken.skillId1, itemAwaken.skillId2);
						player.Pet.SkillEquip = PetMgr.ActiveEquipSkill(player.Pet.Level, player.PlayerCharacter.VIPLevel);
						player.PetBag.UpdatePet(player.Pet);
						player.PetBag.SaveToDatabase(saveAdopt: false);
						player.SendMessage("Kỹ năng thức tỉnh đã được kích hoạt.");
					}
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("AddPetEquip.Fail"));
			}
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("AddPetEquip.WrongItem"));
		}
		return false;
	}
}
