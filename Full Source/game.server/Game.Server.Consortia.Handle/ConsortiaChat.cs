using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;

namespace Game.Server.Consortia.Handle;

[ConsortiaHandleAttbute(20)]
public class ConsortiaChat : IConsortiaCommandHadler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		if (Player.PlayerCharacter.IsBanChat)
		{
			Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
			return 1;
		}
		packet.ClientID = Player.PlayerCharacter.ID;
		byte b = packet.ReadByte();
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		packet.WriteInt(Player.PlayerCharacter.ConsortiaID);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.ConsortiaID == Player.PlayerCharacter.ConsortiaID)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
		GameServer.Instance.LoginServer.SendPacket(packet);
		return 0;
	}
}
