using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeBar.Handle;

[HomeBarHandleAttbute(1)]
public class HomeBarInfo : IHomeBarCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = 0;
		UserHomeInfo data = Player.Home.Data;
		if (num != Player.PlayerId)
		{
			num2 = 1;
		}
		if (data != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(294);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(num2);
			if (num2 == 0)
			{
				gSPacketIn.WriteInt(data.energyCount);
				gSPacketIn.WriteInt(data.drinkCount);
				gSPacketIn.WriteDateTime(data.cdEndTime);
				gSPacketIn.WriteInt(data.pourCount);
			}
			else
			{
				gSPacketIn.WriteBoolean(Player.Home.IsTaste(num));
				gSPacketIn.WriteInt(data.tasteCount);
				gSPacketIn.WriteInt(data.energyCount);
			}
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeBarInfo.LoadDataFail"));
		}
		return true;
	}
}
