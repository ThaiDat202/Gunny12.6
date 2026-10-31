using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(4)]
public class InviteRepeal : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		PlayerData playerData = new PlayerData();
		Sys_Users_TeamInviteList sys_Users_TeamInviteList = playerData.Sys_Users_TeamInviteList.Find(num);
		if (sys_Users_TeamInviteList != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(num);
			player.Out.SendTCP(gSPacketIn);
			player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate7"));
			playerData.Sys_Users_TeamInviteList.Remove(sys_Users_TeamInviteList);
			playerData.SaveChanges();
		}
		return 0;
	}
}
