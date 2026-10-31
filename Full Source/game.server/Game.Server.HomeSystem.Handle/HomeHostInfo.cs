using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(10)]
public class HomeHostInfo : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		PlayerInfo playerInfo = Player.PlayerCharacter;
		if (num != playerInfo.ID)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null)
			{
				playerInfo = playerById.PlayerCharacter;
			}
			else
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				playerInfo = playerBussiness.GetUserSingleByUserID(num);
			}
		}
		if (playerInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(285);
			gSPacketIn.WriteByte(10);
			gSPacketIn.WriteString(playerInfo.NickName);
			gSPacketIn.WriteInt(playerInfo.typeVIP);
			gSPacketIn.WriteInt(playerInfo.VIPLevel);
			gSPacketIn.WriteString(playerInfo.Style);
			gSPacketIn.WriteString(playerInfo.Colors);
			gSPacketIn.WriteBoolean(playerInfo.Sex);
			gSPacketIn.WriteInt(playerInfo.Hide);
			Player.SendTCP(gSPacketIn);
			gSPacketIn = new GSPacketIn(285);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(playerInfo.ID);
			gSPacketIn.WriteString(playerInfo.NickName);
			gSPacketIn.WriteInt(playerInfo.VIPLevel);
			gSPacketIn.WriteInt(playerInfo.typeVIP);
			gSPacketIn.WriteBoolean(playerInfo.Sex);
			gSPacketIn.WriteString(playerInfo.Style);
			gSPacketIn.WriteString(playerInfo.Colors);
			gSPacketIn.WriteString(playerInfo.Skin);
			gSPacketIn.WriteInt(playerInfo.MountsType);
			gSPacketIn.WriteInt(playerInfo.PetsID);
			gSPacketIn.WriteInt(playerInfo.ConsortiaID);
			gSPacketIn.WriteInt(playerInfo.badgeID);
			gSPacketIn.WriteString(playerInfo.ConsortiaName);
			gSPacketIn.WriteString(playerInfo.Honor);
			gSPacketIn.WriteInt(playerInfo.honorId);
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeHostInfo.LoadDataFail"));
		}
		return false;
	}
}
