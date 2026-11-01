using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(37)]
public class PetAwaken : IPetCommandHadler
{
	public static Random rand = new Random();

	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(37);
		ItemInfo itemAt = player.StoreBag.GetItemAt(0);
		ItemInfo itemAt2 = player.StoreBag.GetItemAt(1);
		if (itemAt.Count <= 0)
		{
			player.SendMessage("Số lượng đá thức tỉnh không đủ.");
			return false;
		}
		if (itemAt2 == null)
		{
			player.SendMessage("Vui lòng sử dụng đá thức tỉnh.");
			return false;
		}
		if (itemAt2.Template.CategoryID < 50 || itemAt2.Template.CategoryID > 52 || !itemAt2.IsValidItem())
		{
			player.SendMessage("Vật phẩm được sử dụng không đúng.");
			return false;
		}
		int[][] array = new int[3][]
		{
			new int[30] { 30101, 30102, 30103, 30104, 30105, 30106, 30107, 30108, 30109, 30110, 30111, 30112, 30113, 30114, 30115, 30116, 30117, 30118, 30119, 30120, 30121, 30122, 30123, 30124, 30125, 30126, 30127, 30128, 30129, 30130 },
			new int[30] { 31101, 31102, 31103, 31104, 31105, 31106, 31107, 31108, 31109, 31110, 31111, 31112, 31113, 31114, 31115, 31116, 31117, 31118, 31119, 31120, 31121, 31122, 31123, 31124, 31125, 31126, 31127, 31128, 31129, 31130 },
			new int[30] { 32101, 32102, 32103, 32104, 32105, 32106, 32107, 32108, 32109, 32110, 32111, 32112, 32113, 32114, 32115, 32116, 32117, 32118, 32119, 32120, 32121, 32122, 32123, 32124, 32125, 32126, 32127, 32128, 32129, 32130 }
		};
		Random random = new Random();
		player.StoreBag.BeginChanges();
		player.StoreBag.RemoveItemAt(1);
		player.StoreBag.RemoveTemplate(itemAt.TemplateID, 1);
		player.StoreBag.CommitChanges();
		if (random.Next(0, 1000) <= itemAt.Template.Property3)
		{
			int num = 0;
			int num2 = random.Next(array.Length);
			int[] array2 = array[num2];
			int num3 = 0;
			if (itemAt.TemplateID == 12570)
			{
				num3 = array2[random.Next(array2.Length)];
				switch (num3 % 10)
				{
				case 1:
					num = 5;
					break;
				case 2:
					num = 9;
					break;
				case 3:
					num = 6;
					break;
				case 4:
					num = 7;
					break;
				case 5:
					num = 11;
					break;
				case 6:
					num = 10;
					break;
				case 7:
					num = 12;
					break;
				case 8:
					num = 15;
					break;
				case 9:
					num = 27;
					break;
				}
			}
			else
			{
				num = itemAt.Template.Property2;
				num3 = array2[itemAt.Template.Property4];
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(num3);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			itemInfo.IsDirty = true;
			int agilityCompose = random.Next(itemAt.Template.Property5, itemAt.Template.Property6);
			int attackCompose = random.Next(itemAt.Template.Property5, itemAt.Template.Property6);
			int defendCompose = random.Next(itemAt.Template.Property5, itemAt.Template.Property6);
			int luckCompose = random.Next(itemAt.Template.Property5, itemAt.Template.Property6);
			itemInfo.AgilityCompose = agilityCompose;
			itemInfo.AttackCompose = attackCompose;
			itemInfo.DefendCompose = defendCompose;
			itemInfo.LuckCompose = luckCompose;
			itemInfo.IsBinds = true;
			itemInfo.ValidDate = random.Next(5, 15);
			ItemAwakenPet itemAwakenPet = new ItemAwakenPet();
			player.EquipBag.BeginChanges();
			player.EquipBag.AddItem(itemInfo);
			if (itemInfo.ItemID <= 0)
			{
				player.EquipBag.SaveToDatabase();
			}
			player.EquipBag.CommitChanges();
			itemAwakenPet.itemId = itemInfo.ItemID;
			itemAwakenPet.petId = num;
			itemAwakenPet.skillId1 = PetSpecialSkillMgr.GetSkillAwaken(num);
			do
			{
				itemAwakenPet.skillId2 = PetSpecialSkillMgr.GetSkillAwaken(num);
			}
			while (itemAwakenPet.skillId2 == itemAwakenPet.skillId1);
			player.PetBag.ItemAwakens.Add(itemAwakenPet);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(itemAwakenPet.itemId);
			gSPacketIn.WriteInt(itemAwakenPet.skillId1);
			gSPacketIn.WriteInt(itemAwakenPet.skillId2);
			gSPacketIn.WriteInt(itemAwakenPet.petId);
			player.SendMessage("Thức tỉnh thành công.");
			string translation = LanguageMgr.GetTranslation("PetAwaken.Notice", player.PlayerCharacter.NickName, itemInfo.TemplateID);
			WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, player.ZoneId, null);
		}
		else
		{
			player.SendMessage("Thức tỉnh thất bại.");
			gSPacketIn.WriteBoolean(val: false);
		}
		player.SendTCP(gSPacketIn);
		return true;
	}
}
