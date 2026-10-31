using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(42)]
public class LanternriddlesRankInfo : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GameServer.Instance.LoginServer.SendLightriddleRank(Player.PlayerCharacter.NickName, Player.PlayerCharacter.ID);
		return true;
	}
}
