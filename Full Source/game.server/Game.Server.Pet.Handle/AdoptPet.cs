using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(6)]
public class AdoptPet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = player.PetBag.FindFirstEmptySlot();
		PetInventory petBag = player.PetBag;
		if (num2 == -1)
		{
			player.Out.SendRefreshPet(player, petBag.GetAdoptPet(player.PlayerCharacter.VIPLevel), null, refreshBtn: false);
			player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg15"));
		}
		else
		{
			if (num < 0)
			{
				player.Out.SendRefreshPet(player, petBag.GetAdoptPet(player.PlayerCharacter.VIPLevel), null, refreshBtn: false);
				player.SendMessage(LanguageMgr.GetTranslation("PetHandler.Msg16"));
				return false;
			}
			UsersPetInfo adoptPetAt = petBag.GetAdoptPetAt(num);
			adoptPetAt.VIPLevel = player.PlayerCharacter.VIPLevel;
			adoptPetAt.PetEquips = new List<PetEquipInfo>();
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				if (adoptPetAt.ID > 0)
				{
					playerBussiness.RemoveUserAdoptPet(adoptPetAt.ID);
					adoptPetAt.ID = 0;
				}
			}
			adoptPetAt.PetEquips = new List<PetEquipInfo>();
			adoptPetAt.BaseProp = JsonConvert.SerializeObject(adoptPetAt);
			if (petBag.AddPetTo(adoptPetAt, num2))
			{
				PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(adoptPetAt.TemplateID);
				if (petTemplateInfo.StarLevel > 3)
				{
					string translation = LanguageMgr.GetTranslation("PetHandler.Msg17", player.PlayerCharacter.NickName, petTemplateInfo.Name, petTemplateInfo.StarLevel);
					GSPacketIn packet2 = WorldMgr.SendSysNotice(translation);
					GameServer.Instance.LoginServer.SendPacket(packet2);
				}
				player.OnAdoptPetEvent();
			}
			petBag.SaveToDatabase(saveAdopt: false);
			petBag.RemoveAdoptPet(adoptPetAt);
		}
		return false;
	}
}
