using Bussiness;
using Game.Base.Packets;
using Game.Server.Statics;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(193, "更新拍卖")]
public class AuctionUpdateHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		bool val = false;
		int num3 = GameProperties.LimitLevel(1);
		if (client.Player.PlayerCharacter.Grade < num3)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("AuctionUpdateHandler.Msg", num3));
			return 0;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(193, client.Player.PlayerCharacter.ID);
		string text = "AuctionUpdateHandler.Fail";
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			AuctionInfo auctionSingle = playerBussiness.GetAuctionSingle(num);
			if (auctionSingle == null)
			{
				text = "AuctionUpdateHandler.Msg1";
			}
			else if (auctionSingle.PayType == 0 && num2 > client.Player.PlayerCharacter.Gold)
			{
				text = "AuctionUpdateHandler.Msg2";
			}
			else if (auctionSingle.PayType == 1 && num2 > client.Player.PlayerCharacter.Money)
			{
				text = "AuctionUpdateHandler.Msg3";
			}
			else if (auctionSingle.BuyerID == 0 && auctionSingle.Price > num2)
			{
				text = "AuctionUpdateHandler.Msg4";
			}
			else if (auctionSingle.BuyerID != 0 && auctionSingle.Price + auctionSingle.Rise > num2 && (auctionSingle.Mouthful == 0 || auctionSingle.Mouthful > num2))
			{
				text = "AuctionUpdateHandler.Msg5";
			}
			else
			{
				string text2 = "Hien tai co: ";
				int buyerID = auctionSingle.BuyerID;
				auctionSingle.BuyerID = client.Player.PlayerCharacter.ID;
				auctionSingle.BuyerName = client.Player.PlayerCharacter.NickName;
				auctionSingle.Price = num2;
				if (auctionSingle.Mouthful != 0 && num2 >= auctionSingle.Mouthful)
				{
					auctionSingle.Price = auctionSingle.Mouthful;
					auctionSingle.IsExist = false;
				}
				if (auctionSingle.PayType == 0)
				{
					client.Player.RemoveGold(auctionSingle.Price);
				}
				else
				{
					text2 = text2 + client.Player.PlayerCharacter.Money + "xu";
					if (client.Player.RemoveMoney(auctionSingle.Price) == auctionSingle.Price)
					{
						text2 = text2 + ", sau khi khau tru con: " + client.Player.PlayerCharacter.Money + "xu";
						if (playerBussiness.UpdateAuction(auctionSingle))
						{
							LogMgr.LogMoneyAdd(LogMoneyType.Auction, LogMoneyType.Auction_Update, client.Player.PlayerCharacter.ID, auctionSingle.Price, 0, 0, 0, 0, 0, $"AuctionUpdate type:{auctionSingle.PayType}, AuctioneerID:{auctionSingle.AuctioneerID}, BuyerID:{auctionSingle.BuyerID}, {text2}", 0, 0);
							client.Player.UpdatePlayer();
						}
					}
					else
					{
						text = "AuctionUpdateHandler.Msg3";
						LogMgr.LogMoneyAdd(LogMoneyType.Auction, LogMoneyType.Auction_Update, client.Player.PlayerCharacter.ID, auctionSingle.Price, 0, 0, 0, 0, 0, "Nghi van hack", 0, 0);
					}
				}
				if (auctionSingle.IsExist)
				{
					text = "AuctionUpdateHandler.Msg6";
				}
				else
				{
					text = "AuctionUpdateHandler.Msg7";
					client.Out.SendMailResponse(auctionSingle.AuctioneerID, eMailRespose.Receiver);
					client.Out.SendMailResponse(auctionSingle.BuyerID, eMailRespose.Receiver);
				}
				if (buyerID != 0)
				{
					client.Out.SendMailResponse(buyerID, eMailRespose.Receiver);
				}
				val = true;
			}
			client.Out.SendAuctionRefresh(auctionSingle, num, auctionSingle?.IsExist ?? false, null);
			if (text != "")
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(text));
			}
		}
		gSPacketIn.WriteBoolean(val);
		gSPacketIn.WriteInt(num);
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
