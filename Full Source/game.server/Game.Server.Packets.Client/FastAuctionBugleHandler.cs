using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(327, "场景用户离开")]
public class FastAuctionBugleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int auctionID = packet.ReadInt();
		if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("SmallBugleHandler.Msg"));
			return 1;
		}
		string text = "FastAuctionBugleHandler.Fail";
		if (client.Player.PropBag.GetItemByTemplateID(11101) != null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			AuctionInfo auctionSingle = playerBussiness.GetAuctionSingle(auctionID);
			if (auctionSingle == null)
			{
				text = "FastAuctionBugleHandler.Msg1";
			}
			else
			{
				GSPacketIn gSPacketIn = new GSPacketIn(327);
				gSPacketIn.WriteInt(auctionSingle.AuctioneerID);
				gSPacketIn.WriteString(auctionSingle.AuctioneerName);
				gSPacketIn.WriteInt(auctionSingle.AuctionID);
				gSPacketIn.WriteInt(auctionSingle.TemplateID);
				gSPacketIn.WriteInt(auctionSingle.goodsCount);
				gSPacketIn.WriteInt(auctionSingle.Mouthful);
				gSPacketIn.WriteInt(auctionSingle.PayType);
				gSPacketIn.WriteInt(auctionSingle.Price);
				gSPacketIn.WriteInt(auctionSingle.Rise);
				gSPacketIn.WriteInt(auctionSingle.ValidDate);
				client.Player.PropBag.RemoveTemplate(11101, 1);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer gamePlayer in allPlayers)
				{
					gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
					gamePlayer.Out.SendTCP(gSPacketIn);
				}
				text = "";
				client.Player.LastChatTime = DateTime.Now;
			}
		}
		if (text != "")
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(text));
		}
		return 0;
	}
}
