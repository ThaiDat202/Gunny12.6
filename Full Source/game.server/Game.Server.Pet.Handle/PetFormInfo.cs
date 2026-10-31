using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(24)]
public class PetFormInfo : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		player.UpdatePetFrom();
		return false;
	}
}
