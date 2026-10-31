using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(25)]
public class PetFollow : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(25);
		gSPacketIn.WriteBoolean(flag);
		gSPacketIn.WriteInt(num);
		player.SendTCP(gSPacketIn);
		player.PlayerCharacter.PetsID = num;
		player.PetBag.PetFormFollow(num, flag);
		return false;
	}
}
