using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.MemoryGame.Handle;

[MemoryGameHandleAttbute(11)]
public class GameInfo : IMemoryGameCommandHadler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		bool isOld = true;
		List<MemoryGameInfo> list = JsonConvert.DeserializeObject<List<MemoryGameInfo>>(player.Actives.Info.MemoryGameData);
		if (list == null)
		{
			isOld = false;
			list = player.Actives.CreateMemoryGameData();
		}
		player.Actives.SendMemoryGameData(list, isOld);
		return 1;
	}
}
