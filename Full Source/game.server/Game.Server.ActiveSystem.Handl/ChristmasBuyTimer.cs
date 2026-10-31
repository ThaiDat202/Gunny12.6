using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(29)]
public class ChristmasBuyTimer : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
		int christmasBuyTimeMoney = GameProperties.ChristmasBuyTimeMoney;
		if (Player.MoneyDirect(christmasBuyTimeMoney))
		{
			int christmasBuyMinute = GameProperties.ChristmasBuyMinute;
			Player.Actives.AddTime(christmasBuyMinute);
			Player.SendMessage(LanguageMgr.GetTranslation("ActiveSystemHandler.Msg2"));
		}
		return true;
	}
}
