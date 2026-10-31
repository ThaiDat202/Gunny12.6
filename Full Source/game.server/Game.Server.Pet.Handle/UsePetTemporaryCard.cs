using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(35)]
public class UsePetTemporaryCard : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadInt();
		int slot = packet.ReadInt();
		PlayerInventory inventory = player.GetInventory(bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (itemAt == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeFail"));
		}
		else
		{
			if (itemAt.Template.CategoryID != 62)
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeFail"));
				return false;
			}
			PetFormDataInfo petFormDataInfo = PetMgr.FindPetFormData(itemAt.TemplateID);
			if (petFormDataInfo != null)
			{
				if (player.PetBag.AddPetForm(petFormDataInfo.TemplateID))
				{
					GSPacketIn gSPacketIn = new GSPacketIn(68);
					gSPacketIn.WriteByte(32);
					gSPacketIn.WriteInt(petFormDataInfo.TemplateID);
					gSPacketIn.WriteBoolean(val: false);
					player.SendTCP(gSPacketIn);
					player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeSuccess", petFormDataInfo.Name));
					inventory.RemoveTemplate(petFormDataInfo.TemplateID, 1);
					player.EquipBag.UpdatePlayerProperties();
				}
				else
				{
					player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeFail"));
				}
			}
			else
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeFail"));
			}
		}
		return false;
	}
}
