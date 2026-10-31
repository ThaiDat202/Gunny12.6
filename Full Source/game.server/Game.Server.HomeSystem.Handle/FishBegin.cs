using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;

namespace Game.Server.HomeSystem.Handle;

[HomeSystemHandleAttbute(18)]
public class FishBegin : IHomeSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		if (Player.Home.Data == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FishBegin.LoadDbFail"));
		}
		else if (Player.Home.Data.remainCount < GameProperties.FishDailyCount)
		{
			Player.Home.Data.remainCount++;
			GSPacketIn gSPacketIn = new GSPacketIn(285);
			gSPacketIn.WriteByte(18);
			int num = 1;
			gSPacketIn.WriteInt(num);
			for (int i = 0; i < num; i++)
			{
				gSPacketIn.WriteInt(i + 1);
			}
			gSPacketIn.WriteInt(Player.Home.Data.remainCount);
			Player.SendTCP(gSPacketIn);
			Player.Home.IsFishing = false;
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FishBegin.EndGame"));
		}
		return false;
	}
}
