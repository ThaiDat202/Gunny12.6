using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.MoneyTree.Handle;

[MoneyTreeHandleAttbute(1)]
public class InfoOfMoneyTree : IMoneyTreeCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.Home.SendMoneyTreeInfo();
		return false;
	}
}
