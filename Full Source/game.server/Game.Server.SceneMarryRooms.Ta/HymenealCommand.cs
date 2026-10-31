using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.SceneMarryRooms.TankHandle;

[MarryCommandAttbute(2)]
public class HymenealCommand : IMarryCommandHandler
{
	public bool HandleCommand(TankMarryLogicProcessor process, GamePlayer player, GSPacketIn packet)
	{
		if (player.CurrentMarryRoom == null || player.CurrentMarryRoom.RoomState != eRoomState.FREE)
		{
			return false;
		}
		if (player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && player.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
		{
			return false;
		}
		int pRICE_PROPOSE = GameProperties.PRICE_PROPOSE;
		if (player.CurrentMarryRoom.Info.IsHymeneal && player.PlayerCharacter.Money < pRICE_PROPOSE)
		{
			player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("UserFirecrackersCommand.MoneyNotEnough"));
			return false;
		}
		GamePlayer playerByUserID = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.GroomID);
		if (playerByUserID == null)
		{
			player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoGroom"));
			return false;
		}
		GamePlayer playerByUserID2 = player.CurrentMarryRoom.GetPlayerByUserID(player.CurrentMarryRoom.Info.BrideID);
		if (playerByUserID2 == null)
		{
			player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("HymenealCommand.NoBride"));
			return false;
		}
		bool flag = false;
		GSPacketIn gSPacketIn = packet.Clone();
		int num = packet.ReadInt();
		if (1 == num)
		{
			player.CurrentMarryRoom.RoomState = eRoomState.FREE;
		}
		else
		{
			player.CurrentMarryRoom.RoomState = eRoomState.Hymeneal;
			player.CurrentMarryRoom.BeginTimerForHymeneal(170000);
			int seniorType = player.CurrentMarryRoom.Info.SeniorType;
			string[] array = GameProperties.SENIOR_MARRY_SUIT.Split(',');
			string[] array2 = GameProperties.SENIOR_MARRY_GIFT.Split(',');
			int templateId = 9022;
			if (array.Length != 3)
			{
				array = new string[3] { "13705", "13706", "46060" };
			}
			if (array2.Length != 3)
			{
				array2 = new string[3] { "1120324", "1120325", "1120326" };
			}
			switch (seniorType)
			{
			case 1:
				templateId = int.Parse(array2[0]);
				break;
			case 2:
				templateId = int.Parse(array2[1]);
				break;
			case 3:
				templateId = int.Parse(array2[2]);
				break;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(templateId);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(int.Parse(array[0]));
			ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(int.Parse(array[1]));
			ItemTemplateInfo goods2 = ItemMgr.FindItemTemplate(int.Parse(array[2]));
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 112);
			itemInfo.IsBinds = true;
			ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(playerByUserID.PlayerCharacter.Sex ? itemTemplateInfo2 : itemTemplateInfo, 1, 112);
			itemInfo2.IsBinds = true;
			itemInfo2.ValidDate = 7;
			ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(goods2, 1, 112);
			itemInfo3.IsBinds = true;
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				itemInfo.UserID = 0;
				playerBussiness.AddGoods(itemInfo);
				if (seniorType > 0)
				{
					playerBussiness.AddGoods(itemInfo2);
					if (seniorType == 4)
					{
						playerBussiness.AddGoods(itemInfo3);
					}
				}
				string translation = LanguageMgr.GetTranslation("HymenealCommand.Content", playerByUserID2.PlayerCharacter.NickName);
				MailInfo mailInfo = new MailInfo();
				mailInfo.Annex1 = itemInfo.ItemID.ToString();
				if (seniorType > 0)
				{
					mailInfo.Annex2 = itemInfo2.ItemID.ToString();
					if (seniorType == 4)
					{
						mailInfo.Annex3 = itemInfo3.ItemID.ToString();
					}
				}
				mailInfo.Content = translation;
				mailInfo.Gold = 0;
				mailInfo.IsExist = true;
				mailInfo.Money = 0;
				mailInfo.Receiver = playerByUserID.PlayerCharacter.NickName;
				mailInfo.ReceiverID = playerByUserID.PlayerCharacter.ID;
				mailInfo.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender");
				mailInfo.SenderID = 0;
				mailInfo.Title = LanguageMgr.GetTranslation("HymenealCommand.Title");
				mailInfo.Type = 14;
				if (playerBussiness.SendMail(mailInfo, player.ZoneId))
				{
					flag = true;
				}
				player.Out.SendMailResponse(mailInfo.ReceiverID, eMailRespose.Receiver);
			}
			ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(goods, 1, 112);
			itemInfo4.IsBinds = true;
			ItemInfo itemInfo5 = ItemInfo.CreateFromTemplate(playerByUserID2.PlayerCharacter.Sex ? itemTemplateInfo2 : itemTemplateInfo, 1, 112);
			itemInfo5.IsBinds = true;
			itemInfo5.ValidDate = 7;
			ItemInfo itemInfo6 = ItemInfo.CreateFromTemplate(goods2, 1, 112);
			itemInfo6.IsBinds = true;
			using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
			{
				itemInfo4.UserID = 0;
				playerBussiness2.AddGoods(itemInfo4);
				if (seniorType > 0)
				{
					playerBussiness2.AddGoods(itemInfo5);
					if (seniorType == 4)
					{
						playerBussiness2.AddGoods(itemInfo6);
					}
				}
				string translation2 = LanguageMgr.GetTranslation("HymenealCommand.Content", playerByUserID.PlayerCharacter.NickName);
				MailInfo mailInfo2 = new MailInfo();
				mailInfo2.Annex1 = itemInfo4.ItemID.ToString();
				if (seniorType > 0)
				{
					mailInfo2.Annex2 = itemInfo5.ItemID.ToString();
					if (seniorType == 4)
					{
						mailInfo2.Annex3 = itemInfo6.ItemID.ToString();
					}
				}
				mailInfo2.Content = translation2;
				mailInfo2.Gold = 0;
				mailInfo2.IsExist = true;
				mailInfo2.Money = 0;
				mailInfo2.Receiver = playerByUserID2.PlayerCharacter.NickName;
				mailInfo2.ReceiverID = playerByUserID2.PlayerCharacter.ID;
				mailInfo2.Sender = LanguageMgr.GetTranslation("HymenealCommand.Sender");
				mailInfo2.SenderID = 0;
				mailInfo2.Title = LanguageMgr.GetTranslation("HymenealCommand.Title");
				mailInfo2.Type = 14;
				if (playerBussiness2.SendMail(mailInfo2, player.ZoneId))
				{
					flag = true;
				}
				player.Out.SendMailResponse(mailInfo2.ReceiverID, eMailRespose.Receiver);
			}
			player.CurrentMarryRoom.Info.IsHymeneal = true;
			using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
			{
				playerBussiness3.UpdateMarryRoomInfo(player.CurrentMarryRoom.Info);
				playerBussiness3.UpdatePlayerGotRingProp(playerByUserID.PlayerCharacter.ID, playerByUserID2.PlayerCharacter.ID);
				playerByUserID.LoadMarryProp();
				playerByUserID2.LoadMarryProp();
			}
			player.RemoveMoney(pRICE_PROPOSE);
			CountBussiness.InsertSystemPayCount(player.PlayerCharacter.ID, pRICE_PROPOSE, 0, 0, 1);
			gSPacketIn.WriteInt(player.CurrentMarryRoom.Info.ID);
			gSPacketIn.WriteBoolean(flag);
			player.CurrentMarryRoom.SendToAll(gSPacketIn);
			if (flag)
			{
				string translation3 = LanguageMgr.GetTranslation("HymenealCommand.Succeed", playerByUserID.PlayerCharacter.NickName, playerByUserID2.PlayerCharacter.NickName);
				GSPacketIn packet2 = player.Out.SendMessage(eMessageType.ChatNormal, translation3);
				player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet2, player);
			}
		}
		return true;
	}
}
