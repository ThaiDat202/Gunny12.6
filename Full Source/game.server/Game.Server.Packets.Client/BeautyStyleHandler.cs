using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(310, "场景用户离开")]
public class BeautyStyleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		string style = packet.ReadString();
		string color = packet.ReadString();
		if (BeautyVoteMgr.Status == BeautyStatus.OPENING)
		{
			UserBeautyVoteInfo player = BeautyVoteMgr.GetPlayer(client.Player.PlayerId);
			if (player == null)
			{
				player = new UserBeautyVoteInfo();
				player.UserID = client.Player.PlayerCharacter.ID;
				player.UserName = client.Player.PlayerCharacter.UserName;
				player.NickName = client.Player.PlayerCharacter.NickName;
				player.Count = 0;
				player.Rank = 0;
				player.Style = style;
				player.Color = color;
				player.TimeCreate = DateTime.Now;
				BeautyVoteMgr.AddPlayer(player);
				GSPacketIn gSPacketIn = new GSPacketIn(310);
				gSPacketIn.WriteBoolean(val: true);
				client.Out.SendTCP(gSPacketIn);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("BeautyVote.Public.Msg.Fail"));
			}
		}
		else
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("BeautyVote.NotOpen.Msg"));
		}
		return 0;
	}
}
