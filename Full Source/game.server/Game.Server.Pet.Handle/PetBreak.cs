using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(31)]
public class PetBreak : IPetCommandHadler
{
	private Random random = new Random();

	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		bool flag = packet.ReadBoolean();
		UsersPetInfo petAt = player.PetBag.GetPetAt(slot);
		List<UsersPetInfo> list = new List<UsersPetInfo>();
		int num = petAt.breakGrade + 1;
		PetBreakInfo petBreakInfo = PetBreakMgr.FindPetBreak(num);
		int num2 = 0;
		bool val = false;
		int templateId = 11167;
		int templateId2 = 11168;
		int itemCount = player.PropBag.GetItemCount(templateId);
		ItemInfo itemByTemplateID = player.PropBag.GetItemByTemplateID(templateId2);
		if (petBreakInfo == null)
		{
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("PetHandler.PetBreak.ErrorData", num));
			return false;
		}
		if (itemCount == 0)
		{
			player.Out.SendMessage(eMessageType.Normal, ItemMgr.FindItemTemplate(templateId).Name + "Không đủ số lượng.");
			return false;
		}
		if (itemCount >= petBreakInfo.stoneNumber)
		{
			num2 = ((petBreakInfo.breakGrade2 == 0) ? 1 : 2);
			for (int i = 0; i < num2; i++)
			{
				UsersPetInfo petAt2 = player.PetBag.GetPetAt(packet.ReadInt());
				if (petAt2 != null)
				{
					list.Add(petAt2);
				}
			}
			if (flag && itemByTemplateID == null)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("PetHandler.ProtectItem.ErrorData"));
				return false;
			}
			if (flag && itemByTemplateID.Count < num2)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("PetHandler.ProtectItem.NotEnought", itemByTemplateID.Template.Name, num2));
				return false;
			}
			if (num2 != 0 && list.Count == num2)
			{
				if (random.Next(100) < 5)
				{
					val = true;
					petAt.breakGrade++;
					player.PetBag.UpdatePet(petAt, petAt.Place);
				}
				if (flag)
				{
					player.PropBag.RemoveTemplate(templateId2, num2);
				}
				else
				{
					foreach (UsersPetInfo item in list)
					{
						player.PetBag.RemovePet(item);
					}
				}
				player.PropBag.RemoveTemplate(templateId, petBreakInfo.stoneNumber);
			}
		}
		else
		{
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(ItemMgr.FindItemTemplate(templateId).Name) + "数量不足.");
		}
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(31);
		gSPacketIn.WriteBoolean(val);
		player.SendTCP(gSPacketIn);
		return false;
	}
}
