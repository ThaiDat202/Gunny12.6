using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(40)]
public class PetPurificar : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		if (DateTime.Compare(player.LastOpenPack.AddMilliseconds(200.0), DateTime.Now) > 0)
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("BeadHandle.Msg6"));
			return false;
		}
		int slot = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		bool flag3 = packet.ReadBoolean();
		bool flag4 = packet.ReadBoolean();
		bool flag5 = packet.ReadBoolean();
		bool flag6 = flag2 || flag3 || flag4 || flag5 || flag;
		UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
		if (petAt == null)
		{
			player.SendMessage("Pet được tìm thấy trong túi.");
			return false;
		}
		int num = 40;
		if (flag2)
		{
			num += 40;
		}
		if (flag3)
		{
			num += 40;
		}
		if (flag4)
		{
			num += 80;
		}
		if (flag5)
		{
			num += 100;
		}
		if (flag)
		{
			num += 40;
		}
		if (flag2 && flag3 && flag4 && flag5 && flag)
		{
			player.SendMessage("Tối đa chỉ được phép khóa bốn thuộc tính!");
			return false;
		}
		if (player.PropBag.GetItemCount(12656) < num)
		{
			player.SendMessage(ItemMgr.FindItemTemplate(12656).Name + "Không đủ số lượng, trùng sinh không thành công!");
			return false;
		}
		PetMgr.purificar(petAt, flag, flag2, flag3, flag4, flag5);
		petAt.BuildProp();
		if (flag6 && GetPetGraded(petAt) > 90)
		{
			GameServer.Instance.LoginServer.SendPacket(WorldMgr.SendSysNotice(eMessageType.ChatNormal, LanguageMgr.GetTranslation("PetPurificar.Tips", player.PlayerCharacter.NickName, petAt.Name)));
		}
		player.PropBag.RemoveTemplate(12656, num);
		player.PropBag.UpdateChangedPlaces();
		player.PetBag.UpdatePet(petAt);
		player.EquipBag.UpdatePlayerProperties();
		player.PetBag.SaveToDatabase(saveAdopt: false);
		player.LastOpenPack = DateTime.Now;
		return true;
	}

	public double GetRandomDouble(Random random, double min, double max)
	{
		return min + random.NextDouble() * (max - min);
	}

	public int GetPetGraded(UsersPetInfo petInfo)
	{
		PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(petInfo.TemplateID);
		int num = (petInfo.BloodGrow - petTemplateInfo.LowBloodGrow) * 100 / (petTemplateInfo.HighBloodGrow - petTemplateInfo.LowBloodGrow);
		int num2 = (petInfo.AttackGrow - petTemplateInfo.LowAttackGrow) * 100 / (petTemplateInfo.HighAttackGrow - petTemplateInfo.LowAttackGrow);
		int num3 = (petInfo.DefenceGrow - petTemplateInfo.LowDefenceGrow) * 100 / (petTemplateInfo.HighDefenceGrow - petTemplateInfo.LowDefenceGrow);
		int num4 = (petInfo.AgilityGrow - petTemplateInfo.LowAgilityGrow) * 100 / (petTemplateInfo.HighAgilityGrow - petTemplateInfo.LowAgilityGrow);
		int num5 = (petInfo.LuckGrow - petTemplateInfo.LowLuckGrow) * 100 / (petTemplateInfo.HighLuckGrow - petTemplateInfo.LowLuckGrow);
		return (num + num2 + num3 + num4 + num5) / 5;
	}
}
