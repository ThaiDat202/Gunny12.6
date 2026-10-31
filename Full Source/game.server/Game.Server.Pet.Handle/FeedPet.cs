using System;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(4)]
public class FeedPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int place = packet.ReadInt();
		eBageType bagType = (eBageType)packet.ReadInt();
		int slot = packet.ReadInt();
		ItemInfo itemAt = player.GetItemAt(bagType, place);
		if (itemAt == null)
		{
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("PetHandler.Msg9"));
			return false;
		}
		int num = Convert.ToInt32(PetMgr.FindConfig("MaxHunger").Value);
		UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
		int num2 = itemAt.Count;
		int property = itemAt.Template.Property2;
		int property2 = itemAt.Template.Property1;
		int num3 = num2 * property2;
		int num4 = num3 + petAt.Hunger;
		int num5 = num2 * property;
		string text = "";
		if (itemAt.TemplateID == 334100)
		{
			num5 = itemAt.DefendCompose;
			if (petAt.breakGrade < itemAt.Hole1)
			{
				petAt.breakGrade = itemAt.Hole1;
			}
			if (petAt.breakBlood < itemAt.Blood)
			{
				petAt.breakBlood = itemAt.Blood;
			}
			if (petAt.breakAttack < itemAt.Hole2)
			{
				petAt.breakAttack = itemAt.Hole2;
			}
			if (petAt.breakDefence < itemAt.Hole3)
			{
				petAt.breakDefence = itemAt.Hole3;
			}
			if (petAt.breakAgility < itemAt.Hole4)
			{
				petAt.breakAgility = itemAt.Hole4;
			}
			if (petAt.breakLuck < itemAt.Hole5)
			{
				petAt.breakLuck = itemAt.Hole5;
			}
		}
		int num6 = petAt.MaxLevel();
		if (petAt.Level < num6)
		{
			num5 += petAt.GP;
			int level = petAt.Level;
			int level2 = PetMgr.GetLevel(num5, num6);
			int gP = PetMgr.GetGP(level2 + 1, num6);
			int gP2 = PetMgr.GetGP(num6, num6);
			int num7 = num5;
			if (num5 > gP2)
			{
				num5 -= gP2;
				if (num5 >= property && property != 0)
				{
					num2 -= (int)Math.Ceiling((double)num5 / (double)property);
				}
			}
			petAt.GP = ((num7 >= gP2) ? gP2 : num7);
			petAt.Level = level2;
			petAt.MaxGP = ((gP == 0) ? gP2 : gP);
			petAt.Hunger = ((num4 > num) ? num : num4);
			if (level < level2)
			{
				player.PetBag.UpdateEvolutionPet(petAt, level2, num6, player.PlayerCharacter.VIPLevel);
				text = LanguageMgr.GetTranslation("FeedPet.Success", petAt.Name, level2);
			}
			if (itemAt.TemplateID == 334100)
			{
				player.StoreBag.RemoveItem(itemAt);
			}
			else
			{
				player.StoreBag.RemoveCountFromStack(itemAt, num2);
				player.OnUsingItem(itemAt.TemplateID, num2);
			}
			petAt.Hunger = num4;
			if (petAt.Hunger > num)
			{
				petAt.Hunger = num;
			}
			player.PetBag.UpdatePet(petAt);
			player.PetBag.SaveToDatabase(saveAdopt: false);
			player.EquipBag.UpdatePlayerProperties();
		}
		else if (petAt.Hunger < num)
		{
			petAt.Hunger = 10000;
			if (petAt.Hunger > num)
			{
				petAt.Hunger = num;
			}
			player.StoreBag.RemoveCountFromStack(itemAt, num2);
			text = LanguageMgr.GetTranslation("PetHandler.Msg10", num3);
			player.PetBag.UpdatePet(petAt);
			player.PetBag.SaveToDatabase(saveAdopt: false);
			player.EquipBag.UpdatePlayerProperties();
		}
		else
		{
			text = LanguageMgr.GetTranslation("PetHandler.Msg11");
		}
		if (!string.IsNullOrEmpty(text))
		{
			player.SendMessage(text);
		}
		return false;
	}
}
