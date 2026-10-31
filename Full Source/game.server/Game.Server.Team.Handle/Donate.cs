using Bussiness;
using EntityDatabase.PlayerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Team.Handle;

[TeamHandle(24)]
public class Donate : ITeamCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		PlayerData db = new PlayerData();
		int num = packet.ReadInt();
		if (player.PlayerCharacter.Money < num)
		{
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Donate.Money"));
			return 0;
		}
		player.RemoveMoney(num);
		int score = num / GameProperties.MoneyRichesOffer;
		int AddMoney = 0;
		TeamMgr.AddScore(db, player, score, 2, LanguageMgr.GetTranslation("TeamHandler.Donate.Translate"), ref AddMoney);
		if (AddMoney > 0)
		{
			player.AddMoney(AddMoney);
			player.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("TeamHandler.Donate.Translate1", AddMoney));
		}
		TeamMgr.Out.GetActiveList(db, player);
		return 0;
	}
}
