using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(21)]
public class DelPetEquip : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int eqPlace = packet.ReadInt();
		if (player.PetBag.RemoveEqPet(num, eqPlace))
		{
			player.PetBag.OnChangedPetEquip(num);
			player.Pet.Skill = PetMgr.UpdateSkillPet(player.Pet.Level, player.Pet.TemplateID, player.Level);
			foreach (PetEquipInfo petEquip in player.Pet.PetEquips)
			{
				foreach (ItemAwakenPet itemAwaken in player.PetBag.ItemAwakens)
				{
					if (itemAwaken.itemId == petEquip.itemID && itemAwaken.petId == PetMgr.FindPetTemplate(player.Pet.TemplateID).KindID)
					{
						player.Pet.Skill = PetMgr.UpdateAwakenSkill(player.Pet.Skill, player.Pet.TemplateID, itemAwaken.skillId1, itemAwaken.skillId2);
					}
				}
			}
			player.Pet.SkillEquip = PetMgr.ActiveEquipSkill(player.Pet.Level, player.PlayerCharacter.VIPLevel);
			player.PetBag.UpdatePet(player.Pet);
			player.PetBag.SaveToDatabase(saveAdopt: false);
		}
		return false;
	}
}
