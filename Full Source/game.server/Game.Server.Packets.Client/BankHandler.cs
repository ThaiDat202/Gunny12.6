using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(389, "user ac action")]
public class BankHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadInt())
		{
		case 1:
		{
			List<UserBankInfo> allBank2 = client.Player.Bank.GetAllBank();
			GSPacketIn gSPacketIn2 = new GSPacketIn(389);
			gSPacketIn2.WriteInt(1);
			gSPacketIn2.WriteInt(allBank2.Count);
			foreach (UserBankInfo item in allBank2)
			{
				gSPacketIn2.WriteInt(item.BankID);
				gSPacketIn2.WriteInt(item.TempID);
				gSPacketIn2.WriteInt(item.UserID);
				gSPacketIn2.WriteInt(item.Amount);
				gSPacketIn2.WriteDateTime(item.BeginTime);
			}
			client.SendTCP(gSPacketIn2);
			break;
		}
		case 2:
		{
			int id = packet.ReadInt();
			int num5 = packet.ReadInt();
			BankTemplateInfo bankTemplate3 = BankMgr.GetBankTemplate(id);
			if (bankTemplate3 != null)
			{
				if (num5 < bankTemplate3.MinAmount || num5 % bankTemplate3.Multiple > 0 || num5 > 1000000000)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg4"));
				}
				else if (client.Player.RemoveMoney(num5) > 0)
				{
					UserBankInfo userBankInfo = new UserBankInfo();
					userBankInfo.TempID = bankTemplate3.ID;
					userBankInfo.UserID = client.Player.PlayerId;
					userBankInfo.Amount = num5;
					userBankInfo.BeginTime = DateTime.Now;
					userBankInfo.IsExits = true;
					client.Player.Bank.AddBank(userBankInfo);
					GSPacketIn gSPacketIn = new GSPacketIn(389);
					gSPacketIn.WriteInt(2);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(userBankInfo.BankID);
					gSPacketIn.WriteInt(userBankInfo.TempID);
					gSPacketIn.WriteInt(userBankInfo.UserID);
					gSPacketIn.WriteInt(userBankInfo.Amount);
					gSPacketIn.WriteDateTime(userBankInfo.BeginTime);
					client.SendTCP(gSPacketIn);
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg2"));
				}
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg1"));
			}
			break;
		}
		case 3:
		{
			int bankId = packet.ReadInt();
			int num3 = packet.ReadInt();
			UserBankInfo singleBank = client.Player.Bank.GetSingleBank(bankId);
			if (singleBank != null)
			{
				BankTemplateInfo bankTemplate2 = BankMgr.GetBankTemplate(singleBank.TempID);
				if (bankTemplate2 == null)
				{
					break;
				}
				if (num3 <= 0 || num3 % bankTemplate2.Multiple > 0 || num3 > singleBank.Amount || num3 > 100000000)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg4"));
					break;
				}
				singleBank.Amount -= num3;
				client.Player.AddMoney(num3);
				int num4 = 0;
				num4 = ((!singleBank.IsAchieve(bankTemplate2.DeadLine)) ? (BankMgr.GetProfitMoney(singleBank, isAll: false, num3, isAchieve: false) * (100 - bankTemplate2.Consume) / 100) : BankMgr.GetProfitMoney(singleBank, isAll: false, num3, isAchieve: true));
				if (singleBank.Amount <= 0)
				{
					client.Player.Bank.RemoveBank(singleBank);
				}
				if (num4 > 0)
				{
					client.Player.AddGiftToken(num4);
				}
				client.Out.SendUserBankUpdate(singleBank);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg3"));
			}
			break;
		}
		case 4:
		{
			List<UserBankInfo> allBank = client.Player.Bank.GetAllBank();
			int num = 0;
			int num2 = 0;
			foreach (UserBankInfo item2 in allBank)
			{
				BankTemplateInfo bankTemplate = BankMgr.GetBankTemplate(item2.TempID);
				if (bankTemplate != null && bankTemplate.DeadLine > 0 && item2.IsAchieve(bankTemplate.DeadLine))
				{
					num = item2.Amount;
					num2 = BankMgr.GetProfitMoney(item2, isAll: true, 0, isAchieve: true);
					item2.Amount = 0;
					client.Out.SendUserBankUpdate(item2);
					client.Player.Bank.RemoveBank(item2);
				}
			}
			if (num > 0 || num2 > 0)
			{
				client.Player.AddMoney(num);
				client.Player.AddGiftToken(num2);
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.Bank.Msg5", num, num2));
			}
			break;
		}
		}
		return 1;
	}
}
