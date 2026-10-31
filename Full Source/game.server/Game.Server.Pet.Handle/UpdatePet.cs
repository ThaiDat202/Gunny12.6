using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Helpers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Pet.Handle;

[PetHandleAttbute(1)]
public class UpdatePet : IPetCommandHadler
{
	public bool CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(num);
		UsersPetInfo[] array;
		EatPetsInfo eatPetsInfo;
		if (playerById != null)
		{
			array = playerById.PetBag.GetPets();
			eatPetsInfo = playerById.PetBag.EatPets;
		}
		else
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			array = playerBussiness.GetUserPetSingles(num, player.PlayerCharacter.VIPLevel);
			eatPetsInfo = playerBussiness.GetAllEatPetsByID(num);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].PetEquips = player.PetBag.DeserializePetEquip(array[i].eQPets);
			}
		}
		if (array != null && eatPetsInfo != null)
		{
			if (array.Length > 20)
			{
				IEnumerable<IEnumerable<UsersPetInfo>> enumerable = array.Split(20);
				foreach (IEnumerable<UsersPetInfo> item in enumerable)
				{
					player.Out.SendPetInfo(num, player.ZoneId, item.ToArray(), eatPetsInfo);
				}
			}
			else
			{
				player.Out.SendPetInfo(num, player.ZoneId, array, eatPetsInfo);
			}
		}
		return false;
	}
}
