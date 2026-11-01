using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(47)]
public class PetDesselar : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		ItemInfo itemAt = player.EquipBag.GetItemAt(num);
		UsersPetLacrarInfo lacreByPlace = player.PetBag.GetLacreByPlace(num);
		if (itemAt == null)
		{
			player.SendMessage(eMessageType.Normal, "Không có đủ đạo cụ để phong ấn thú cưng!");
			return false;
		}
		if (lacreByPlace == null)
		{
			player.SendMessage(eMessageType.Normal, "Pet này không thể phong ấn!");
			return false;
		}
		UsersPetInfo petById = player.PetBag.GetPetById(lacreByPlace.petId);
		lacreByPlace.isSeal = false;
		if (petById != null)
		{
			petById.IsSeal = false;
		}
		int num2 = int.Parse(GameProperties.SealItems.Split(',')[1]);
		int itemCount = player.PropBag.GetItemCount(12213);
		if (num2 > itemCount)
		{
			player.SendMessage(eMessageType.Normal, "Bạn không có đủ đá giải ấn!");
			return false;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(68);
		gSPacketIn.WriteByte(49);
		gSPacketIn.WriteBoolean(val: true);
		player.Out.SendTCP(gSPacketIn);
		player.Out.SendPetLacrarInfo(player);
		player.PetBag.BeginChanges();
		player.PetBag.OnPlaceChanged(petById.Place);
		player.PetBag.CommitChanges();
		player.EquipBag.UpdatePlayerProperties();
		player.PropBag.BeginChanges();
		player.PropBag.RemoveTemplate(12213, num2);
		player.PropBag.CommitChanges();
		player.SendMessage(eMessageType.Normal, "Giải ấn thành công.");
		return false;
	}
}
