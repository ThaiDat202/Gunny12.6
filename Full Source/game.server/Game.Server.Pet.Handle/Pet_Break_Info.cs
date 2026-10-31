using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(34)]
public class Pet_Break_Info : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int id = packet.ReadInt();
		PetBreakInfo petBreakInfo = PetBreakMgr.FindPetBreak(id);
		if (petBreakInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(68);
			gSPacketIn.WriteByte(34);
			gSPacketIn.WriteInt(petBreakInfo.targetGrade);
			gSPacketIn.WriteInt(petBreakInfo.stoneNumber);
			gSPacketIn.WriteInt(petBreakInfo.breakGrade1);
			gSPacketIn.WriteInt(petBreakInfo.star1);
			gSPacketIn.WriteInt(petBreakInfo.breakGrade2);
			gSPacketIn.WriteInt(petBreakInfo.star2);
			player.SendTCP(gSPacketIn);
		}
		return false;
	}
}
