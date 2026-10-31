using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(33)]
public class EatPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int amor = packet.ReadInt();
		int num = packet.ReadInt();
		int num2 = 0;
		int num3 = 0;
		int templateId = 201567;
		if (num == 1)
		{
			num2 = packet.ReadInt();
			for (int i = 0; i < num2; i++)
			{
				int slot = packet.ReadInt();
				int templateID = packet.ReadInt();
				UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
				if (petAt != null)
				{
					PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(templateID);
					if (petTemplateInfo != null)
					{
						num3 = num3;
					}
				}
				UpGrade(player, amor, num, num3, null);
				player.PetBag.RemovePet(petAt);
			}
		}
		else
		{
			num2 = packet.ReadInt();
			ItemInfo itemByTemplateID = player.GetItemByTemplateID(templateId);
			if (itemByTemplateID != null)
			{
				int itemCount = player.GetItemCount(templateId);
				if (itemCount < num2)
				{
					num2 = ((itemCount < num2) ? itemCount : num2);
				}
				num3 += itemByTemplateID.Template.Property2 * num2;
				int count = UpGrade(player, amor, num, num3, itemByTemplateID);
				if (num2 == 1)
				{
					player.RemoveTemplate(templateId, num2);
				}
				else
				{
					player.RemoveTemplate(templateId, count);
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.EatPetNotEnoughtCount"));
			}
		}
		player.Out.SendEatPetsInfo(player.PetBag.EatPets);
		player.EquipBag.UpdatePlayerProperties();
		return false;
	}

	private int UpGrade(GamePlayer player, int amor, int type, int totalPoint, ItemInfo eatItem)
	{
		int num = 0;
		int num2 = 0;
		int num3 = PetMoePropertyMgr.FindMaxLevel();
		PetMoePropertyInfo petMoePropertyInfo = null;
		switch (amor)
		{
		case 0:
		{
			num = player.PetBag.EatPets.weaponExp + totalPoint;
			num2 = player.PetBag.EatPets.weaponLevel;
			for (int j = num2; j <= num3; j++)
			{
				petMoePropertyInfo = PetMoePropertyMgr.FindPetMoeProperty(j + 1);
				if (petMoePropertyInfo != null && petMoePropertyInfo.Exp <= num)
				{
					player.PetBag.EatPets.weaponLevel = j + 1;
					num -= petMoePropertyInfo.Exp;
				}
			}
			if (player.PetBag.EatPets.weaponLevel == num3)
			{
				totalPoint = ((num > 0) ? num : totalPoint);
				player.PetBag.EatPets.weaponExp = 0;
			}
			else
			{
				player.PetBag.EatPets.weaponExp = num;
			}
			break;
		}
		case 1:
		{
			num = player.PetBag.EatPets.clothesExp + totalPoint;
			num2 = player.PetBag.EatPets.clothesLevel;
			for (int k = num2; k <= num3; k++)
			{
				petMoePropertyInfo = PetMoePropertyMgr.FindPetMoeProperty(k + 1);
				if (petMoePropertyInfo != null && petMoePropertyInfo.Exp <= num)
				{
					player.PetBag.EatPets.clothesLevel = k + 1;
					num -= petMoePropertyInfo.Exp;
				}
			}
			if (player.PetBag.EatPets.clothesLevel == num3)
			{
				totalPoint = ((num > 0) ? num : totalPoint);
				player.PetBag.EatPets.clothesExp = 0;
			}
			else
			{
				player.PetBag.EatPets.clothesExp = num;
			}
			break;
		}
		case 2:
		{
			num = player.PetBag.EatPets.hatExp + totalPoint;
			num2 = player.PetBag.EatPets.hatLevel;
			for (int i = num2; i <= num3; i++)
			{
				petMoePropertyInfo = PetMoePropertyMgr.FindPetMoeProperty(i + 1);
				if (petMoePropertyInfo != null && petMoePropertyInfo.Exp <= num)
				{
					player.PetBag.EatPets.hatLevel = i + 1;
					num -= petMoePropertyInfo.Exp;
				}
			}
			if (player.PetBag.EatPets.hatLevel == num3)
			{
				totalPoint = ((num > 0) ? num : totalPoint);
				player.PetBag.EatPets.hatExp = 0;
			}
			else
			{
				player.PetBag.EatPets.hatExp = num;
			}
			break;
		}
		}
		if (type == 2 && eatItem != null)
		{
			return totalPoint / eatItem.Template.Property2;
		}
		return 0;
	}
}
