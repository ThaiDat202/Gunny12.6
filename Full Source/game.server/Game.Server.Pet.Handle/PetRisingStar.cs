using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(22)]
public class PetRisingStar : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int templateId = packet.ReadInt();
		int num = packet.ReadInt();
		int slot = packet.ReadInt();
		bool val = false;
		UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
		if (petAt == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.PetNotFound"));
		}
		else
		{
			int num2 = 11162;
			ItemInfo itemInfo = player.GetItemByTemplateID(templateId);
			bool flag = player.Extra.UseDeed(10);
			if (flag)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(num2);
				itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 105);
			}
			if (itemInfo == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.ItemNotFound"));
			}
			else
			{
				PetStarExpInfo petStarExpInfo = PetMgr.FindPetStarExp(petAt.TemplateID);
				if (petStarExpInfo == null)
				{
					player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.UnSupport"));
				}
				else if (itemInfo.TemplateID == num2)
				{
					if (itemInfo.Count < num)
					{
						num = itemInfo.Count;
					}
					int num3 = itemInfo.Template.Property2 * num;
					int num4 = petAt.currentStarExp + num3;
					if (num4 >= petStarExpInfo.Exp)
					{
						int num5 = petStarExpInfo.Exp - petAt.currentStarExp;
						if (num5 < num3 && !flag)
						{
							num = (num3 - num5) / itemInfo.Template.Property2;
						}
						petAt.currentStarExp = 0;
						PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(petStarExpInfo.NewID);
						if (petTemplateInfo != null)
						{
							UsersPetInfo usersPetInfo = PetMgr.CreatePet(petTemplateInfo, petAt.UserID, petAt.Place, petAt.Level, player.PlayerCharacter.VIPLevel);
							petAt.BaseProp = JsonConvert.SerializeObject(usersPetInfo);
							usersPetInfo.Level = petAt.Level;
							usersPetInfo.VIPLevel = player.PlayerCharacter.VIPLevel;
							player.PetBag.UpdateEvolutionPet(usersPetInfo, petAt.Level, player.PetBag.MaxLevelByGrade, player.PlayerCharacter.VIPLevel);
							petAt.TemplateID = usersPetInfo.TemplateID;
							petAt.AttackGrow = usersPetInfo.AttackGrow;
							petAt.DefenceGrow = usersPetInfo.DefenceGrow;
							petAt.AgilityGrow = usersPetInfo.AgilityGrow;
							petAt.LuckGrow = usersPetInfo.LuckGrow;
							petAt.BloodGrow = usersPetInfo.BloodGrow;
							petAt.DamageGrow = usersPetInfo.DamageGrow;
							petAt.GuardGrow = usersPetInfo.GuardGrow;
							petAt.Attack = usersPetInfo.Attack;
							petAt.Defence = usersPetInfo.Defence;
							petAt.Agility = usersPetInfo.Agility;
							petAt.Luck = usersPetInfo.Luck;
							petAt.Blood = usersPetInfo.Blood;
							petAt.Damage = usersPetInfo.Damage;
							petAt.Guard = usersPetInfo.Guard;
							val = true;
						}
					}
					else
					{
						petAt.currentStarExp = num4;
					}
					player.PetBag.UpdatePet(petAt);
					if (!flag)
					{
						player.RemoveCountFromStack(itemInfo, num);
					}
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("PetRisingStar.ItemNotFound"));
				}
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteBoolean(val);
		player.SendTCP(gSPacketIn);
		return false;
	}
}
