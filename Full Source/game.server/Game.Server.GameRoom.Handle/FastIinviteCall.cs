using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameRoom.Handle;

[GameRoomHandleAttbute(19)]
public class FastIinviteCall : IGameRoomCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		try
		{
			if (Player.CurrentRoom == null)
			{
				return false;
			}
			if (DateTime.Compare(Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
			{
				Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("FastIinviteCall.Msg1"));
				return false;
			}
			ItemInfo itemByTemplateID = Player.PropBag.GetItemByTemplateID(0, 11101);
			if (itemByTemplateID != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(94, Player.PlayerId);
				gSPacketIn.WriteByte(19);
				gSPacketIn.WriteInt(Player.PlayerId);
				gSPacketIn.WriteString(Player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(Player.CurrentRoom.GetNameByMapId());
				gSPacketIn.WriteInt(Player.CurrentRoom.RoomId);
				gSPacketIn.WriteString(Player.CurrentRoom.Password);
				Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer gamePlayer in allPlayers)
				{
					if (gamePlayer != null)
					{
						gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
						gamePlayer.Out.SendTCP(gSPacketIn);
					}
				}
			}
			Player.LastChatTime = DateTime.Now;
		}
		catch
		{
			Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("FastIinviteCall.Msg1"));
			return false;
		}
		return true;
	}
}
