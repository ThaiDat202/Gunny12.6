using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.Labyrinth.Handle;

[LabyrinthHandle(6)]
public class ResetLabyrinth : ILabyrinthCommandHandler
{
	public int CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.PlayerCharacter.LabyrinthInfo.nowContinueCount > 0)
		{
			Player.PlayerCharacter.LabyrinthInfo.currentFinishLevel = 1;
			Player.PlayerCharacter.LabyrinthInfo.isPlayGame = false;
			Player.PlayerCharacter.LabyrinthInfo.isSweep = false;
			Player.PlayerCharacter.LabyrinthInfo.nowContinueCount--;
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.ResetLabyrinth.Success"));
			Player.Out.SendLabyrinthUpdataInfo(Player);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("Labyrinth.ResetLabyrinth.Fail"));
		}
		return 0;
	}
}
