using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(309, "场景用户离开")]
public class DraftVoteHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		if (num <= 0 || num > 999)
		{
			return 0;
		}
		if (num2 == client.Player.PlayerId)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("BeautyVote.VoteForYou.Msg.Fail"));
			return 0;
		}
		if (BeautyVoteMgr.Status == BeautyStatus.WAITING)
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("BeautyVote.VoteEnd.Msg"));
			return 0;
		}
		bool flag = false;
		int num3 = GameProperties.BeautyVoteCost * num;
		if (client.Player.PlayerCharacter.ticketNum >= num)
		{
			client.Player.PlayerCharacter.ticketNum -= num;
			flag = true;
		}
		else if (client.Player.PlayerCharacter.Money >= num3)
		{
			client.Player.RemoveMoney(num3);
			flag = true;
		}
		if (flag)
		{
			bool val = BeautyVoteMgr.VotePlayer(num2, num);
			GSPacketIn gSPacketIn = new GSPacketIn(309);
			gSPacketIn.WriteBoolean(val);
			client.Out.SendTCP(gSPacketIn);
		}
		else
		{
			client.Player.SendMessage(LanguageMgr.GetTranslation("GoodsPresentHandler.NoMoney"));
		}
		return 1;
	}
}
