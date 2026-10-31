using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.CardNewSystem.Handle;

[CardNewSystemHandle(9)]
public class CardReplace : ICardNewSystemCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		Player.PlayerCharacter.CardInfo.Attack = Player.CardBag.TempCardnew.Attack;
		Player.PlayerCharacter.CardInfo.Defence = Player.CardBag.TempCardnew.Defence;
		Player.PlayerCharacter.CardInfo.Agility = Player.CardBag.TempCardnew.Agility;
		Player.PlayerCharacter.CardInfo.Lucky = Player.CardBag.TempCardnew.Lucky;
		Player.CardBag.TempCardnew = new Sys_Users_CardInfo();
		Player.UpdateCardInfo();
		CardNewSystemMgr.Out.SendCardInfo(Player);
		return 0;
	}
}
