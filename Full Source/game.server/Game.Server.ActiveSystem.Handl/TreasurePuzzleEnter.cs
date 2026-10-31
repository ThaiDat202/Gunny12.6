using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(103)]
public class TreasurePuzzleEnter : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Actives.LoadTreasurePuzzleFromDatabase())
		{
			TreasurePuzzleDataInfo[] treasurepuzzle = Player.Actives.Treasurepuzzle;
			GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
			gSPacketIn.WriteByte(103);
			gSPacketIn.WriteInt(treasurepuzzle.Length);
			TreasurePuzzleDataInfo[] array = treasurepuzzle;
			foreach (TreasurePuzzleDataInfo treasurePuzzleDataInfo in array)
			{
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.PuzzleID);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole1Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole1Have);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole2Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole2Have);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole3Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole3Have);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole4Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole4Have);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole5Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole5Have);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole6Need);
				gSPacketIn.WriteInt(treasurePuzzleDataInfo.hole6Have);
				gSPacketIn.WriteBoolean(treasurePuzzleDataInfo.IsGetAward);
			}
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.LoadDataFail"));
		}
		return true;
	}
}
