using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(142, "场景用户离开")]
public class KingBlessHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int num = packet.ReadInt();
		string text = packet.ReadString();
		bool flag = packet.ReadBoolean();
		if (b == 10)
		{
			if (client.Player.MoneyDirect(GameProperties.DanDanBuffPrice))
			{
				int value = 7;
				string text2 = "";
				client.Player.Extra.Info.DeedRenevalDays(value, b);
				if (string.IsNullOrEmpty(client.Player.Extra.Info.DeedInfo))
				{
					client.Player.Extra.SetupDeed();
					text2 = LanguageMgr.GetTranslation("KingBlessHandler.Msg1");
				}
				else
				{
					text2 = LanguageMgr.GetTranslation("KingBlessHandler.Msg2");
				}
				client.Out.SendMessage(eMessageType.Normal, text2);
				client.Player.Out.SendDeedMain(client.Player.Extra);
			}
		}
		else
		{
			string[] array = GameProperties.KingBuffPrice.Split(',');
			int num2 = int.Parse(array[0]);
			int value2 = 30;
			switch (b)
			{
			case 2:
				num2 = int.Parse(array[1]);
				value2 = 90;
				break;
			case 3:
				num2 = int.Parse(array[2]);
				value2 = 180;
				break;
			}
			string translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg3");
			string message = "";
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (client.Player.MoneyDirect(num2))
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				if (playerById == null)
				{
					PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(num);
					if (userSingleByUserID == null)
					{
						translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg4");
					}
					else
					{
						UsersExtraInfo usersExtraInfo = playerBussiness.GetSingleUsersExtra(num);
						if (usersExtraInfo == null)
						{
							usersExtraInfo = client.Player.Extra.CreateUsersExtra(num);
						}
						int kingBlessIndex = usersExtraInfo.KingBlessIndex;
						if (usersExtraInfo.KingBlessRenevalDays(value2, b))
						{
							if (usersExtraInfo.ID > 0)
							{
								if (kingBlessIndex < b)
								{
									usersExtraInfo.KingBlessInfo = client.Player.Extra.SetupKingBless(isFriend: true, b);
								}
								playerBussiness.UpdateUsersExtra(usersExtraInfo);
								translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg5", userSingleByUserID.NickName);
							}
							else
							{
								usersExtraInfo.KingBlessInfo = client.Player.Extra.SetupKingBless(isFriend: true, b);
								playerBussiness.AddUsersExtra(usersExtraInfo);
								translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg6", userSingleByUserID.NickName);
							}
						}
						else
						{
							client.Player.AddMoney(num2);
							client.Player.LogAddMoney(AddMoneyType.KingBless, AddMoneyType.KingBless_Add, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money);
						}
					}
				}
				else
				{
					int kingBlessIndex2 = playerById.Extra.Info.KingBlessIndex;
					if (client.Player.PlayerCharacter.ID == num)
					{
						if (string.IsNullOrEmpty(playerById.Extra.Info.KingBlessInfo))
						{
							playerById.Extra.Info.KingBlessRenevalDays(value2, b);
							playerById.Extra.CreateSaveLifeBuff();
							playerById.Extra.SetupKingBless(isFriend: false, b);
							translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg7");
						}
						else if (playerById.Extra.Info.KingBlessRenevalDays(value2, b))
						{
							translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg8");
							if (kingBlessIndex2 < b)
							{
								playerById.Extra.Info.KingBlessInfo = playerById.Extra.SetupKingBless(isFriend: true, b);
								playerById.Extra.SetupKingBlessFromData();
							}
						}
						else
						{
							client.Player.AddMoney(num2);
							client.Player.LogAddMoney(AddMoneyType.KingBless, AddMoneyType.KingBless_Add, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money);
						}
					}
					else
					{
						if (string.IsNullOrEmpty(playerById.Extra.Info.KingBlessInfo))
						{
							playerById.Extra.CreateSaveLifeBuff();
							translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg9", playerById.PlayerCharacter.NickName);
							message = LanguageMgr.GetTranslation("KingBlessHandler.Msg10", client.Player.PlayerCharacter.NickName);
							playerById.Extra.Info.KingBlessRenevalDays(value2, b);
							playerById.Extra.Info.KingBlessInfo = playerById.Extra.SetupKingBless(isFriend: true, b);
							playerById.Extra.SetupKingBlessFromData();
						}
						else if (playerById.Extra.Info.KingBlessRenevalDays(value2, b))
						{
							translation = LanguageMgr.GetTranslation("KingBlessHandler.Msg11", playerById.PlayerCharacter.NickName);
							message = LanguageMgr.GetTranslation("KingBlessHandler.Msg12", client.Player.PlayerCharacter.NickName);
							if (kingBlessIndex2 < b)
							{
								playerById.Extra.Info.KingBlessInfo = playerById.Extra.SetupKingBless(isFriend: true, b);
								playerById.Extra.SetupKingBlessFromData();
							}
						}
						else
						{
							client.Player.AddMoney(num2);
							client.Player.LogAddMoney(AddMoneyType.KingBless, AddMoneyType.KingBless_Add, client.Player.PlayerCharacter.ID, num2, client.Player.PlayerCharacter.Money);
						}
						playerById.Out.SendMessage(eMessageType.Normal, message);
					}
				}
				client.Out.SendMessage(eMessageType.Normal, translation);
				client.Player.Out.SendKingBlessMain(client.Player.Extra);
			}
		}
		return 0;
	}
}
