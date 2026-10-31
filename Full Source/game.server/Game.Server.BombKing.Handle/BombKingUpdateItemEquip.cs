using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.BombKing.Handle;

[BombKingHandleAttbute(1)]
public class BombKingUpdateItemEquip : IBombKingCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int zoneId = Player.ZoneId;
		int num2 = packet.ReadInt();
		AreaConfigInfo areaConfigInfo = WorldMgr.FindAreaConfig(zoneId);
		if (areaConfigInfo != null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness(areaConfigInfo);
			switch (num2)
			{
			case 0:
			case 1:
			{
				PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(num);
				if (userSingleByUserID == null)
				{
					break;
				}
				userSingleByUserID.ZoneID = Player.ZoneId;
				userSingleByUserID.ZoneName = Player.ZoneName;
				if (num2 == 0)
				{
					List<UsersCardInfo> userCardEuqip = playerBussiness.GetUserCardEuqip(num);
					Player.Out.SendPlayerCardSlot(userSingleByUserID, userCardEuqip);
					Player.Out.SendPlayerCardEquip(userSingleByUserID, userCardEuqip);
					break;
				}
				List<ItemInfo> userEuqip = playerBussiness.GetUserEuqip(num);
				List<ItemInfo> userBeadEuqip = playerBussiness.GetUserBeadEuqip(num);
				userSingleByUserID.Texp = playerBussiness.GetUserTexpInfoSingle(num);
				List<UserGemStone> singleGemstones = playerBussiness.GetSingleGemstones(num);
				List<ItemInfo> userMagicstoneEuqip = playerBussiness.GetUserMagicstoneEuqip(num);
				ExplorerManualInfo explorerInfo = playerBussiness.InitExplorerManualInfo(num);
				List<MarkChipItemInfo> allUsersMark = playerBussiness.GetAllUsersMark(num);
				List<MarkChipItemInfo> list = new List<MarkChipItemInfo>();
				foreach (MarkChipItemInfo item in allUsersMark)
				{
					if (item.Type == 1)
					{
						list.Add(item);
					}
				}
				Player.Out.SendUserEquip(userSingleByUserID, userEuqip, singleGemstones, userBeadEuqip, userMagicstoneEuqip, explorerInfo, list, playerBussiness.GetUserLacresPet(num), Player.EmblemInfos, Player.PlayerCharacter.ElfInfo, Player.PlayerCharacter.AcademyInfo);
				break;
			}
			case 2:
			{
				UsersPetInfo[] userPetSingles = playerBussiness.GetUserPetSingles(num, Player.PlayerCharacter.VIPLevel);
				EatPetsInfo allEatPetsByID = playerBussiness.GetAllEatPetsByID(num);
				if (userPetSingles.Length != 0)
				{
					for (int i = 0; i < userPetSingles.Length; i++)
					{
						userPetSingles[i].PetEquips = Player.PetBag.DeserializePetEquip(userPetSingles[i].eQPets);
					}
					Player.Out.SendPetInfo(num, zoneId, userPetSingles, allEatPetsByID);
				}
				break;
			}
			}
		}
		return false;
	}
}
