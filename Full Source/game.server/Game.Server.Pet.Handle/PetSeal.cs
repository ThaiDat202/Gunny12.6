using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(46)]
public class PetSeal : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int petId = packet.ReadInt();
		int num = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		int num2 = packet.ReadInt();
		List<int> arrayLockType = new List<int>();
		if (num2 > 0)
		{
			for (int i = 0; i < num2; i++)
			{
				arrayLockType.Add(packet.ReadInt());
			}
		}
		UsersPetInfo petById = player.PetBag.GetPetById(petId);
		ItemInfo itemAt = player.EquipBag.GetItemAt(num);
		UsersPetLacrarInfo lacreByPlace = player.PetBag.GetLacreByPlace(num);
		if (flag)
		{
			arrayLockType.Add(39);
		}
		if (petById == null)
		{
			player.SendMessage(eMessageType.Normal, "Lỗi thông tin Pet");
			return false;
		}
		if (itemAt == null)
		{
			player.SendMessage(eMessageType.Normal, "Vui lòng chọn pet phong ấn trước");
			return false;
		}
		if (lacreByPlace == null)
		{
			player.SendMessage(eMessageType.Normal, "Vui lòng chọn pet phong ấn hoặc không phong ấn trước");
			return false;
		}
		if (petById.IsEquip)
		{
			player.SendMessage(eMessageType.Normal, "Thú cưng được trang bị và không thể phong ấn");
			return false;
		}
		if (!lacreByPlace.isSeal && flag)
		{
			flag = false;
		}
		if (lacreByPlace.isSeal && lacreByPlace.petId != petById.ID)
		{
			player.SendMessage(eMessageType.Normal, "Vui lòng bỏ chặn thú cưng trước");
			return false;
		}
		int num3 = ((!flag) ? int.Parse(GameProperties.SealItems.Split(',')[0]) : int.Parse(GameProperties.SealItems.Split(',')[2]));
		if (num2 >= 1)
		{
			num3 += ((!flag) ? int.Parse(GameProperties.SealLockItems.Split('|')[num2 - 1].Split(',')[1]) : int.Parse(GameProperties.AdvSealLockItems.Split('|')[num2 - 1].Split(',')[1]));
		}
		int itemCount = player.PropBag.GetItemCount(12212);
		if (num3 > itemCount)
		{
			player.SendMessage(eMessageType.Normal, "Không đủ đá phong ấn");
			return false;
		}
		int skillLacrar = PetSpecialSkillMgr.GetSkillLacrar(PetMgr.FindPetTemplate(petById.TemplateID).KindID);
		int[] array = new int[15]
		{
			1, 3, 5, 7, 9, 31, 32, 33, 34, 35,
			36, 37, 39, 101, 102
		};
		Random random = new Random(DateTime.Now.Millisecond);
		petById.IsSeal = true;
		lacreByPlace.petId = petById.ID;
		lacreByPlace.isSeal = true;
		int num4 = petById.BloodGrow + petById.DefenceGrow * 5 + petById.AgilityGrow * 5 + petById.LuckGrow + petById.AttackGrow + petById.breakGrade * 60;
		num4 /= 100;
		if (itemAt.Template.CategoryID != 7)
		{
			array = DeleteArr(39, array);
		}
		if (!flag)
		{
			if (arrayLockType == null)
			{
				lacreByPlace.lockArr.Clear();
			}
			else
			{
				lacreByPlace.lockArr = lacreByPlace.lockArr.Where((StoreFineSealLockInfo o) => arrayLockType.Contains(o.proType)).ToList();
			}
			int count = lacreByPlace.lockArr.Count;
			for (int j = 0; j < 5 - count; j++)
			{
				int proSorteado = 0;
				int num5 = 0;
				int skillId = 0;
				do
				{
					proSorteado = array[random.Next(array.Length)];
				}
				while (lacreByPlace.lockArr.Any((StoreFineSealLockInfo o) => o.proType == proSorteado) || (skillLacrar == 0 && proSorteado == 39));
				switch (proSorteado)
				{
				case 1:
					num5 = (int)((double)num4 * (1.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 3:
					num5 = (int)((double)num4 * (1.4 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 5:
					num5 = (int)((double)num4 * (1.7 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 7:
					num5 = (int)((double)num4 * (1.6 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 9:
					num5 = (int)((double)(int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0)) * 0.1);
					break;
				case 31:
					num5 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 32:
					num5 = (int)((double)num4 * (1.3 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 33:
					num5 = (int)((double)num4 * (1.8 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 34:
					num5 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 35:
					num5 = (int)((double)num4 * (0.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 36:
					num5 = (int)((double)num4 * (0.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 37:
					num5 = (int)((double)num4 * (2.0 + (double)Convert.ToSingle(petById.Level) / 100.0)) * 20;
					break;
				case 39:
					skillId = skillLacrar;
					break;
				case 101:
					num5 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				case 102:
					num5 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
					break;
				}
				int num6 = (int)((double)num5 * 0.1);
				StoreFineSealLockInfo storeFineSealLockInfo = new StoreFineSealLockInfo();
				storeFineSealLockInfo.min = num6;
				storeFineSealLockInfo.max = num5;
				storeFineSealLockInfo.proNum = random.Next(num6, num5 + 1);
				storeFineSealLockInfo.proType = proSorteado;
				storeFineSealLockInfo.skillId = skillId;
				lacreByPlace.lockArr.Add(storeFineSealLockInfo);
			}
		}
		else
		{
			foreach (StoreFineSealLockInfo item in lacreByPlace.lockArr)
			{
				if (arrayLockType == null || !arrayLockType.Contains(item.proType))
				{
					int num7 = 0;
					int skillId2 = 0;
					switch (item.proType)
					{
					case 1:
						num7 = (int)((double)num4 * (1.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 3:
						num7 = (int)((double)num4 * (1.4 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 5:
						num7 = (int)((double)num4 * (1.7 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 7:
						num7 = (int)((double)num4 * (1.6 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 9:
						num7 = (int)((double)(int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0)) * 0.1);
						break;
					case 31:
						num7 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 32:
						num7 = (int)((double)num4 * (1.3 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 33:
						num7 = (int)((double)num4 * (1.8 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 34:
						num7 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 35:
						num7 = (int)((double)num4 * (0.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 36:
						num7 = (int)((double)num4 * (0.5 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 37:
						num7 = (int)((double)num4 * (2.0 + (double)Convert.ToSingle(petById.Level) / 100.0)) * 20;
						break;
					case 39:
						skillId2 = skillLacrar;
						break;
					case 101:
						num7 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					case 102:
						num7 = (int)((double)num4 * (1.0 + (double)Convert.ToSingle(petById.Level) / 100.0));
						break;
					}
					int minValue = (item.min = (int)((double)num7 * 0.1));
					item.max = num7;
					item.skillId = skillId2;
					item.proNum = random.Next(minValue, num7 + 1);
				}
			}
		}
		player.Out.SendPetLacrarInfo(player);
		player.PetBag.BeginChanges();
		player.PetBag.OnPlaceChanged(petById.Place);
		player.PetBag.CommitChanges();
		player.PropBag.BeginChanges();
		player.PropBag.RemoveTemplate(12212, num3);
		player.PropBag.CommitChanges();
		player.EquipBag.UpdatePlayerProperties();
		PlayerBussiness playerBussiness = new PlayerBussiness();
		if (player.PetBag.lacresPet != null)
		{
			playerBussiness.UpdateUserLacresPet(player.PlayerCharacter.ID, player.PetBag.lacresPet);
			Thread.Sleep(2000);
		}
		return false;
	}

	private static int[] DeleteArr(int num, int[] arr)
	{
		List<int> list = new List<int>();
		int[] array = arr;
		foreach (int item in array)
		{
			list.Add(item);
		}
		list.Remove(num);
		arr = new int[list.Count];
		for (int j = 0; j < list.Count; j++)
		{
			arr[j] = list[j];
		}
		return arr;
	}
}
