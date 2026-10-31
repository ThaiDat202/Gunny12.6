using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(7)]
public class EquipSkillPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int place = packet.ReadInt();
		int killId = packet.ReadInt();
		int skillPlace = packet.ReadInt();
		string msg = "";
		player.PetBag.EquipSkillPet(place, killId, skillPlace, ref msg);
		if (msg != "")
		{
			player.SendMessage(LanguageMgr.GetTranslation(msg));
		}
		return false;
	}
}
