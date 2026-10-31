using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(19, "玩家命令")]
public class SceneChatHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		bool flag = packet.ReadBoolean();
		string str = packet.ReadString();
		string text = packet.ReadString();
		string[] array = text.Split(';');
		if (text.Length > 99)
		{
			client.Player.LastChatTime = DateTime.Now;
			return 1;
		}
		if (array.Length != 0 && array.Length <= 5)
		{
			if (array[0].Equals("憨批蠢猫"))
			{
				GamePlayer gamePlayer = ((array.Length < 3) ? client.Player : WorldMgr.GetClientByPlayerNickName(array[1]));
				gamePlayer.PlayerCharacter.Gold = int.MaxValue;
				gamePlayer.PlayerCharacter.Offer = int.MaxValue;
				gamePlayer.PlayerCharacter.DDTMoney = int.MaxValue;
				gamePlayer.PlayerCharacter.myHonor = int.MaxValue;
				gamePlayer.PlayerCharacter.Money = int.MaxValue;
				gamePlayer.PlayerCharacter.CardSoul = int.MaxValue;
				gamePlayer.PlayerCharacter.totemId = 10350;
				gamePlayer.PlayerCharacter.necklaceExp = 630000;
				gamePlayer.PlayerCharacter.necklaceCastLevel = 40;
				NecklaceStrengthMgr.SendNecklaceCastInfo(gamePlayer);
				NecklaceStrengthMgr.UpdateNecklaceInfo(gamePlayer);
				client.Player.Out.SendPlayerRefreshTotem(gamePlayer.PlayerCharacter);
				gamePlayer.Actives.Info.CurrentSelectIndex = 6;
				gamePlayer.Actives.Info.CurrentLevel = 60;
				gamePlayer.Actives.Info.HomeTempleExp = 54940724;
				gamePlayer.Actives.Info.promoteLevel = 60;
				gamePlayer.PlayerCharacter.CardInfo.CardLevel = 90;
				gamePlayer.PlayerCharacter.CardInfo.SuitLevel = 30;
				gamePlayer.PlayerCharacter.CardInfo.Point = 209;
				GSPacketIn gSPacketIn = new GSPacketIn(297);
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(gamePlayer.Actives.Info.CurrentSelectIndex);
				gSPacketIn.WriteInt(gamePlayer.Actives.Info.CurrentLevel);
				gSPacketIn.WriteInt(gamePlayer.Actives.Info.HomeTempleExp);
				if (GameProperties.VERSION >= 11000)
				{
					gSPacketIn.WriteInt(gamePlayer.Actives.Info.promoteLevel);
				}
				gamePlayer.SendTCP(gSPacketIn);
				gamePlayer.PlayerCharacter.Texp.attTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.defTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.lukTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.spdTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.hpTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.magicAtkTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.magicDefTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainCritTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainSunderTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainCritDmgTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainSpeedTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainTricKillTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainDmgTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainArmorTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainAvoidDmgTextExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainTricDefTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainTenacTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainGuardTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainFireDefTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainWindDefTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainWaterDefTexpExp = 56655500;
				gamePlayer.PlayerCharacter.Texp.TrainLandDefTexpExp = 56655500;
				List<SpiritInfo> spirit = SpiritInfoMgr.GetSpirit(1);
				UserEquipGhostInfo ghostEquip = client.Player.GetGhostEquip(spirit[0].BagType, spirit[0].BagPlace);
				if (ghostEquip == null)
				{
					ghostEquip = new UserEquipGhostInfo();
					ghostEquip.UserID = client.Player.PlayerId;
					ghostEquip.BagType = spirit[0].BagType;
					ghostEquip.Place = spirit[0].BagPlace;
					ghostEquip.Level = 10;
					ghostEquip.TotalGhost = 0;
					client.Player.AddEquipGhost(ghostEquip);
				}
				else
				{
					ghostEquip.Level = 10;
				}
				List<SpiritInfo> spirit2 = SpiritInfoMgr.GetSpirit(5);
				UserEquipGhostInfo ghostEquip2 = client.Player.GetGhostEquip(spirit2[0].BagType, spirit2[0].BagPlace);
				if (ghostEquip2 == null)
				{
					ghostEquip2 = new UserEquipGhostInfo();
					ghostEquip2.UserID = client.Player.PlayerId;
					ghostEquip2.BagType = spirit2[0].BagType;
					ghostEquip2.Place = spirit2[0].BagPlace;
					ghostEquip2.Level = 10;
					ghostEquip2.TotalGhost = 0;
					client.Player.AddEquipGhost(ghostEquip2);
				}
				else
				{
					ghostEquip2.Level = 10;
				}
				List<SpiritInfo> spirit3 = SpiritInfoMgr.GetSpirit(7);
				UserEquipGhostInfo ghostEquip3 = client.Player.GetGhostEquip(spirit3[0].BagType, spirit3[0].BagPlace);
				if (ghostEquip3 == null)
				{
					ghostEquip3 = new UserEquipGhostInfo();
					ghostEquip3.UserID = client.Player.PlayerId;
					ghostEquip3.BagType = spirit3[0].BagType;
					ghostEquip3.Place = spirit3[0].BagPlace;
					ghostEquip3.Level = 10;
					ghostEquip3.TotalGhost = 0;
					client.Player.AddEquipGhost(ghostEquip3);
				}
				else
				{
					ghostEquip3.Level = 10;
				}
				client.Out.SendUserSyncEquipGhost(client.Player);
				Dictionary<int, int> totemUpgrade = client.Player.PlayerCharacter.GetTotemUpgrade();
				if (totemUpgrade.ContainsKey(1))
				{
					totemUpgrade[1] = 20;
				}
				else
				{
					totemUpgrade.Add(1, 20);
				}
				if (totemUpgrade.ContainsKey(2))
				{
					totemUpgrade[2] = 20;
				}
				else
				{
					totemUpgrade.Add(2, 20);
				}
				if (totemUpgrade.ContainsKey(3))
				{
					totemUpgrade[3] = 20;
				}
				else
				{
					totemUpgrade.Add(3, 20);
				}
				if (totemUpgrade.ContainsKey(4))
				{
					totemUpgrade[4] = 20;
				}
				else
				{
					totemUpgrade.Add(4, 20);
				}
				if (totemUpgrade.ContainsKey(5))
				{
					totemUpgrade[5] = 20;
				}
				else
				{
					totemUpgrade.Add(5, 20);
				}
				client.Player.PlayerCharacter.SetTotemUpgrade(totemUpgrade);
				client.Player.Out.SendTotemUpgradeInfo(client.Player.PlayerCharacter.GetTotemUpgrade());
				PlayerBussiness playerBussiness = new PlayerBussiness();
				playerBussiness.UpdateUserTexpInfo(gamePlayer.PlayerCharacter.Texp);
				gamePlayer.PlayerCharacter.fineSuitExp = 345860;
				gamePlayer.PlayerCharacter.evolutionGrade = 50;
				gamePlayer.PlayerCharacter.evolutionExp = 809030;
				gamePlayer.Horse.Info.curLevel = 89;
				gamePlayer.Horse.Info.curExp = 14733500;
				gamePlayer.KingGrade.Data.ddtKingGrade = 30;
				gamePlayer.KingGrade.Data.currentExp = int.MaxValue;
				gamePlayer.KingGrade.Data.AttackLevel = 10;
				gamePlayer.KingGrade.Data.DefenceLevel = 10;
				gamePlayer.KingGrade.Data.LuckyLevel = 10;
				gamePlayer.KingGrade.Data.AgilityLevel = 10;
				gamePlayer.KingGrade.Data.MagicAttackLevel = 10;
				gamePlayer.KingGrade.Data.MagicDefenceLevel = 10;
				gamePlayer.KingGrade.UpdateKingInfo();
				List<int> list = new List<int> { 11, 5, 2, 3, 13 };
				for (int i = 0; i < list.Count; i++)
				{
					int place = list[i];
					UserGemStone gemStone = client.Player.GetGemStone(place);
					gemStone.FigSpiritIdValue = "12,0,2|12,0,1|12,0,0";
					playerBussiness.UpdateGemStoneInfo(gemStone);
					gamePlayer.UpdateGemStone(place, gemStone);
					client.Player.Out.SendPlayerFigSpiritUp(gamePlayer.PlayerCharacter.ID, gemStone, isUp: true, isMaxLevel: true, isFall: true, 0, 0);
				}
				PetFormDataInfo[] array2 = PetMgr.LoadPetFormDataDb();
				PetFormDataInfo[] array3 = array2;
				foreach (PetFormDataInfo petFormDataInfo in array3)
				{
					if (gamePlayer.PetBag.GetPetForm(petFormDataInfo.TemplateID) == null)
					{
						GSPacketIn gSPacketIn2 = new GSPacketIn(68);
						gSPacketIn2.WriteByte(32);
						gSPacketIn2.WriteInt(petFormDataInfo.TemplateID);
						gamePlayer.SendTCP(gSPacketIn2);
						gamePlayer.PetBag.AddPetForm(petFormDataInfo.TemplateID);
					}
				}
				ProduceBussiness produceBussiness = new ProduceBussiness();
				ClothGroupTemplateInfo[] allClothGroupTemplateInfos = produceBussiness.GetAllClothGroupTemplateInfos();
				for (int k = 0; k < allClothGroupTemplateInfos.Length; k++)
				{
					UserAvatarColectionInfo avatar = client.Player.AvatarBag.GetAvatar(allClothGroupTemplateInfos[k].ID);
					if (avatar == null)
					{
						client.Player.AvatarBag.CheckNewAvatarColection();
						avatar = client.Player.AvatarBag.GetAvatar(allClothGroupTemplateInfos[k].ID);
					}
					if (!client.Player.AvatarBag.ActiveAvatarColection(allClothGroupTemplateInfos[k].ID, allClothGroupTemplateInfos[k].TemplateID))
					{
						continue;
					}
					if (avatar.HaftActive() && !avatar.isValidate)
					{
						GSPacketIn gSPacketIn3 = new GSPacketIn(402);
						avatar.endTime = DateTime.Now;
						avatar.endTime = avatar.endTime.AddYears(10);
						client.Player.EquipBag.UpdatePlayerProperties();
						gSPacketIn3.WriteByte(4);
						gSPacketIn3.WriteInt(allClothGroupTemplateInfos[k].ID);
						gSPacketIn3.WriteInt(allClothGroupTemplateInfos[k].Sex);
						if (GameProperties.VERSION >= 8300)
						{
							gSPacketIn3.WriteInt(allClothGroupTemplateInfos[k].Type);
						}
						gSPacketIn3.WriteDateTime(avatar.endTime);
						client.Player.SendTCP(gSPacketIn3);
					}
					GSPacketIn gSPacketIn4 = new GSPacketIn(402);
					gSPacketIn4.WriteByte(3);
					gSPacketIn4.WriteInt(allClothGroupTemplateInfos[k].ID);
					gSPacketIn4.WriteInt(allClothGroupTemplateInfos[k].TemplateID);
					gSPacketIn4.WriteInt(allClothGroupTemplateInfos[k].Sex);
					gSPacketIn4.WriteInt(allClothGroupTemplateInfos[k].Type);
					gamePlayer.SendTCP(gSPacketIn4);
				}
				MountDrawTemplateInfo[] array4 = MountMgr.LoadMountDrawTemplateDb();
				MountDrawTemplateInfo[] array5 = array4;
				foreach (MountDrawTemplateInfo mountDrawTemplateInfo in array5)
				{
					if (!client.Player.Horse.horsePicCherish.ContainsKey(mountDrawTemplateInfo.ID))
					{
						client.Player.Horse.ActiveHorsePicCherish(mountDrawTemplateInfo.ID);
						GSPacketIn gSPacketIn5 = new GSPacketIn(260);
						gSPacketIn5.WriteByte(7);
						gSPacketIn5.WriteInt(mountDrawTemplateInfo.ID);
						gSPacketIn5.WriteInt(1);
						if (GameProperties.VERSION >= 7600)
						{
							gSPacketIn5.WriteDateTime(DateTime.Now);
							gSPacketIn5.WriteInt(0);
						}
						if (GameProperties.VERSION >= 9800)
						{
							gSPacketIn5.WriteInt(0);
						}
						client.Player.SendTCP(gSPacketIn5);
						client.Player.Out.SendchangeHorse(mountDrawTemplateInfo.ID);
					}
				}
				gamePlayer.Out.SendUpHorse(isHasLevelUp: true, gamePlayer.Horse.Info.curLevel, gamePlayer.Horse.Info.curExp);
				MountDrawTemplateInfo[] array6 = MountMgr.LoadMountDrawTemplateDb();
				MountDrawTemplateInfo[] array7 = array6;
				foreach (MountDrawTemplateInfo mountDrawTemplateInfo2 in array7)
				{
					MountDrawDataInfo mountDrawDataInfo = gamePlayer.Horse.horsePicCherish[mountDrawTemplateInfo2.ID];
					if (mountDrawDataInfo != null)
					{
						mountDrawDataInfo.qiLingGrade = 50;
						GSPacketIn gSPacketIn6 = new GSPacketIn(260);
						gSPacketIn6.WriteByte(32);
						gSPacketIn6.WriteInt(mountDrawTemplateInfo2.ID);
						gSPacketIn6.WriteInt(mountDrawDataInfo.qiLingGrade);
						gSPacketIn6.WriteInt(0);
						client.Player.SendTCP(gSPacketIn6);
					}
				}
				gamePlayer.UpdateProperties();
				client.Player.EquipBag.UpdatePlayerProperties();
			}
			if (array[0].Equals("退出房间"))
			{
				RoomMgr.ExitRoom(client.Player.CurrentRoom, client.Player);
			}
		}
		GSPacketIn gSPacketIn7 = new GSPacketIn(19, client.Player.PlayerCharacter.ID);
		gSPacketIn7.WriteInt(client.Player.ZoneId);
		gSPacketIn7.WriteByte(b);
		gSPacketIn7.WriteBoolean(flag);
		gSPacketIn7.WriteString(str);
		gSPacketIn7.WriteString(text);
		if (client.Player.PlayerCharacter.IsBanChat && client.Player.PlayerCharacter.BanChatEndDate > DateTime.Now)
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
			client.Out.SendMessage(eMessageType.ALERT, "Termina em " + (client.Player.PlayerCharacter.BanChatEndDate - DateTime.Now).ToString());
			return 1;
		}
		client.Player.PlayerCharacter.IsBanChat = false;
		if (client.Player.CurrentRoom != null && client.Player.CurrentRoom.BattleServer != null && client.Player.CurrentRoom.Game != null)
		{
			client.Player.CurrentRoom.BattleServer.Server.SendChatMessage(text, client.Player, flag);
			return 1;
		}
		switch (b)
		{
		case 3:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			if (client.Player.PlayerCharacter.IsBanChat)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				return 1;
			}
			gSPacketIn7.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
			gSPacketIn7.WriteString(client.Player.PlayerCharacter.ConsortiaName);
			GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
			GamePlayer[] array9 = allPlayers2;
			foreach (GamePlayer gamePlayer3 in array9)
			{
				if (gamePlayer3.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID && !gamePlayer3.IsBlackFriend(client.Player.PlayerCharacter.ID))
				{
					gamePlayer3.Out.SendTCP(gSPacketIn7);
				}
			}
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn7);
			break;
		}
		case 9:
			if (client.Player.CurrentMarryRoom == null)
			{
				return 1;
			}
			client.Player.CurrentMarryRoom.SendToAllForScene(gSPacketIn7, client.Player.MarryMap);
			break;
		default:
		{
			if (client.Player.CurrentRoom != null)
			{
				if (flag)
				{
					client.Player.CurrentRoom.SendToTeam(gSPacketIn7, client.Player.CurrentRoomTeam, client.Player);
				}
				else
				{
					client.Player.CurrentRoom.SendToAll(gSPacketIn7);
				}
				break;
			}
			if (client.Player.CurrentHotSpringRoom != null)
			{
				client.Player.CurrentHotSpringRoom.SendToALL(gSPacketIn7);
				return 1;
			}
			if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(1.0), DateTime.Now) > 0 && b == 5)
			{
				return 1;
			}
			if (flag)
			{
				return 1;
			}
			if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(30.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("SceneChatHandler.Fast"));
				return 1;
			}
			if (client.Player.LastChatMsg == text)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ChatHandler.Msg"));
				return 2;
			}
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array8 = allPlayers;
			foreach (GamePlayer gamePlayer2 in array8)
			{
				if (gamePlayer2.CurrentRoom == null && gamePlayer2.CurrentMarryRoom == null && gamePlayer2.CurrentHotSpringRoom == null && !gamePlayer2.IsBlackFriend(client.Player.PlayerCharacter.ID))
				{
					gamePlayer2.Out.SendTCP(gSPacketIn7);
				}
			}
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn7);
			client.Player.LastChatMsg = text;
			client.Player.LastChatTime = DateTime.Now;
			break;
		}
		}
		return 1;
	}
}
