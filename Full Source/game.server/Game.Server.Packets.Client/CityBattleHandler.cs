using Bussiness;
using Game.Base.Packets;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(368, "城市争霸")]
public class CityBattleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		byte b2 = b;
		byte b3 = b2;
		if (b3 == 7)
		{
			if (!WorldMgr.CheckOpenCityBattle())
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.CityBattle.Insire.OutTime"));
			}
			else
			{
				string[] array = GameProperties.CityOccupationAddPrice.Split('|');
				int num = packet.ReadInt();
				if (num > 0 && num - 1 < array.Length)
				{
					string[] array2 = array[num - 1].Split(',');
					int num2 = int.Parse(array2[1]);
					if (num2 <= client.Player.PlayerCharacter.Money)
					{
						client.Player.RemoveMoney(num2);
						client.Player.AddScoreCityBattle(int.Parse(array2[0]), isSend: true);
						client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.CityBattle.Insire.Success", int.Parse(array2[0])));
					}
				}
			}
		}
		else
		{
			packet.ClientID = client.Player.PlayerId;
			packet.WriteHeader();
			GameServer.Instance.LoginServer.SendPacket(packet);
		}
		return 1;
	}
}
