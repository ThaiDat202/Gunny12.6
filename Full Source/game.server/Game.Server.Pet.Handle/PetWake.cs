using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(32)]
public class PetWake : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		ItemInfo itemByTemplateID = player.PropBag.GetItemByTemplateID(num);
		if (itemByTemplateID == null)
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeFail"));
		}
		else
		{
			PetFormDataInfo petFormDataInfo = PetMgr.FindPetFormData(itemByTemplateID.TemplateID);
			if (petFormDataInfo != null)
			{
				if (player.PetBag.AddPetForm(num))
				{
					GSPacketIn gSPacketIn = new GSPacketIn(68);
					gSPacketIn.WriteByte(32);
					gSPacketIn.WriteInt(num);
					player.SendTCP(gSPacketIn);
					player.SendMessage(LanguageMgr.GetTranslation("PetHandler.WakeSuccess", petFormDataInfo.Name));
					player.PropBag.RemoveTemplate(itemByTemplateID.TemplateID, 1);
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
