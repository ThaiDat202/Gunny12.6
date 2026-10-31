using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(22)]
public class FishPlayerInfo : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Home.Data == null)
		{
			Player.Home.LoadFromDatabase(Player.PlayerId);
		}
		else
		{
			Player.Home.SendUpdateFishPlayerInfo();
		}
		return false;
	}
}
