using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(8)]
public class ReleasePet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		int value = Convert.ToInt32(PetMgr.FindConfig("HighRemoveStarCost").Value);
		UsersPetInfo usersPetInfo = player.PetBag.GetPetById(num);
		if (usersPetInfo == null)
		{
			usersPetInfo = player.PetBag.GetPetAt(num);
		}
		if (usersPetInfo.IsSeal)
		{
			if (flag)
			{
				player.SendMessage("该宠物已被封印,无法转化.");
			}
			else
			{
				player.SendMessage("该宠物已被封印,无法放生.");
			}
		}
		if (player.PetBag.RemovePet(usersPetInfo))
		{
			player.EquipBag.UpdatePlayerProperties();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.UpdateUserAdoptPet(usersPetInfo.ID);
			}
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(usersPetInfo.TemplateID);
			if (flag)
			{
				if (flag2)
				{
					player.RemoveGiftToken(value);
				}
				else
				{
					player.RemoveMoney(value);
				}
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(334100), 1, 102);
				ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(11679), 1, 102);
				itemInfo.IsBinds = true;
				itemInfo2.IsBinds = true;
				itemInfo.DefendCompose = usersPetInfo.GP;
				itemInfo.AgilityCompose = usersPetInfo.MaxGP;
				itemInfo.Hole1 = usersPetInfo.breakGrade;
				itemInfo.Hole2 = usersPetInfo.breakAttack;
				itemInfo.Hole3 = usersPetInfo.breakDefence;
				itemInfo.Hole4 = usersPetInfo.breakAgility;
				itemInfo.Hole5 = usersPetInfo.breakLuck;
				itemInfo.Blood = usersPetInfo.breakBlood;
				if (!player.PropBag.AddTemplate(itemInfo, 1))
				{
					player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
					player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				if (!player.PropBag.AddTemplate(itemInfo2, 1))
				{
					player.SendItemToMail(itemInfo2, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
					player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				if (petTemplateInfo.WashGetCount > 0)
				{
					ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(12656), 1, 105);
					if (!player.PropBag.AddTemplate(itemInfo3, petTemplateInfo.WashGetCount))
					{
						player.SendItemToMail(itemInfo3, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
						player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					player.SendMessage("你获得了: " + itemInfo2.Template.Name + "x 1," + itemInfo.Template.Name + "x 1," + itemInfo3.Template.Name + "x " + itemInfo3.Count);
				}
				else
				{
					player.SendMessage("你获得了: " + itemInfo2.Template.Name + "x 1," + itemInfo.Template.Name + "x 1,");
				}
				player.SendMessage("转化成功.");
				player.PetBag.SaveToDatabase(saveAdopt: false);
				return false;
			}
			player.SendMessage("放生成功.");
			player.PetBag.SaveToDatabase(saveAdopt: false);
			return false;
		}
		return false;
	}
}
