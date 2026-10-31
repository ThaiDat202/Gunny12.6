using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(2)]
public class AddPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int place = packet.ReadInt();
		int bagType = packet.ReadInt();
		int iD = player.PlayerCharacter.ID;
		PetInventory petBag = player.PetBag;
		int num = petBag.FindFirstEmptySlot();
		if (player.PlayerCharacter.Grade < 25)
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg2"));
			return false;
		}
		if (num != -1)
		{
			ItemInfo itemAt = player.GetItemAt((eBageType)bagType, place);
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(itemAt.Template.Property5);
			if (petTemplateInfo == null)
			{
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg4"));
				return false;
			}
			UsersPetInfo usersPetInfo = PetMgr.CreatePet(petTemplateInfo, iD, num, petBag.MaxLevelByGrade, player.PlayerCharacter.VIPLevel);
			usersPetInfo.VIPLevel = player.PlayerCharacter.VIPLevel;
			petBag.AddPetTo(usersPetInfo, num);
			player.RemoveCountFromStack(itemAt, 1);
			if (petTemplateInfo.StarLevel > 4)
			{
				string translation = LanguageMgr.GetTranslation("PetHandler.Msg5", player.PlayerCharacter.NickName, petTemplateInfo.Name, petTemplateInfo.StarLevel);
				GSPacketIn packet2 = WorldMgr.SendSysNotice(translation);
				GameServer.Instance.LoginServer.SendPacket(packet2);
			}
			GSPacketIn gSPacketIn = new GSPacketIn(68);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(usersPetInfo.ID);
			gSPacketIn.WriteBoolean(val: true);
			player.SendTCP(gSPacketIn);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg3"));
		}
		return false;
	}
}
