using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(17)]
public class FightPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool isEquip = packet.ReadBoolean();
		UsersPetInfo petAt = player.PetBag.GetPetAt(num);
		if (petAt == null)
		{
			return false;
		}
		if (player.PetBag.EquipPet(num, isEquip))
		{
			player.EquipBag.UpdatePlayerProperties();
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg22"));
		}
		return false;
	}
}
