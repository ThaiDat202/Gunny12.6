using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(4)]
public class HomeBarTaste : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int hostId = packet.ReadInt();
		UserHomeInfo data = Player.Home.Data;
		if (data != null)
		{
			if (data.tasteCount > 0)
			{
				if (!Player.Home.IsTaste(hostId))
				{
					data.tasteCount--;
					Player.Home.AddTaste(hostId);
					Player.AddHomeBarEnergy(GameProperties.HomeBarTastePower, hostId, 1);
				}
				else
				{
					Player.SendMessage(LanguageMgr.GetTranslation("HomeBarTaste.TasteExit"));
				}
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("HomeBarTaste.NotEnoughtTaste"));
			}
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeBarInfo.LoadDataFail"));
		}
		return true;
	}
}
