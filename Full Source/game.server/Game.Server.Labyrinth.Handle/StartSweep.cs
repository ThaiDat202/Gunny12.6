using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(3)]
public class StartSweep : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.LabyrinthInfo.isSweep)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.StartSweep.AlreadySweeping"));
			Player.Out.SendLabyrinthUpdataInfo(Player);
			return 0;
		}
		int warriorFamRaidDDTPrice = GameProperties.WarriorFamRaidDDTPrice;
		if (Player.PlayerCharacter.DDTMoney >= warriorFamRaidDDTPrice)
		{
			Player.RemoveGiftToken(warriorFamRaidDDTPrice);
			Player.LabyrinthTime = 120;
			LabyrinthMgr.StartSweep(Player);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.StartSweep.GiftTokensNotEnough"));
			Player.Actives.StopCleantOutLabyrinth();
		}
		return 0;
	}
}
