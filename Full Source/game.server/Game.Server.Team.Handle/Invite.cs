using System.Linq;
using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(3)]
public class Invite : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData playerData = new PlayerData();
		int UserId = packet.ReadInt();
		GamePlayer playerById = WorldMgr.GetPlayerById(UserId);
		if (playerById == null)
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate3"));
		}
		else
		{
			Sys_Users_Team_User sys_Users_Team_User = playerData.Sys_Users_Team_User.Where((Sys_Users_Team_User p) => p.UserID == UserId).FirstOrDefault();
			if (sys_Users_Team_User != null && sys_Users_Team_User.teamID != 0)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate4"));
			}
			else if (playerData.Sys_Users_TeamInviteList.Where((Sys_Users_TeamInviteList p) => p.UserId == UserId).Count() == 0)
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate5"));
				GSPacketIn gSPacketIn = new GSPacketIn(390, player.PlayerId);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(UserId);
				gSPacketIn.WriteInt(player.PlayerCharacter.Team.teamID);
				gSPacketIn.WriteString(player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(player.PlayerCharacter.Team.teamName);
				playerById.Out.SendTCP(gSPacketIn);
			}
			else
			{
				player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("RoomBordenHandler.Translate6"));
			}
		}
		return 0;
	}
}
