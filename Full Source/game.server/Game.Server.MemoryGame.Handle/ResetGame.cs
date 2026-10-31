using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.MemoryGame.Handle;

[MemoryGameHandleAttbute(13)]
public class ResetGame : IMemoryGameCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		bool flag2 = packet.ReadBoolean();
		if (flag)
		{
			List<MemoryGameInfo> list = JsonConvert.DeserializeObject<List<MemoryGameInfo>>(player.Actives.Info.MemoryGameData);
			if (list == null)
			{
				return 1;
			}
			foreach (MemoryGameInfo item in list)
			{
				if (!item.isGet)
				{
					player.SendMessage(LanguageMgr.GetTranslation("MemoryGame.NoEndGame.Msg"));
					return 1;
				}
			}
			list = player.Actives.CreateMemoryGameData();
			player.Actives.SendMemoryGameData(list, isOld: false);
		}
		else if (player.PlayerCharacter.Money >= GameProperties.PairUpResetCost)
		{
			player.RemoveMoney(GameProperties.PairUpResetCost);
			player.Actives.SendMemoryGameData(player.Actives.CreateMemoryGameData(), isOld: false);
		}
		else
		{
			player.SendMessage(LanguageMgr.GetTranslation("UserBuyItemHandler.NoMoney"));
		}
		return 1;
	}
}
