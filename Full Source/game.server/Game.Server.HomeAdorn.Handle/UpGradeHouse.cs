using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.HomeAdorn.Handle;

[HomeAdornHandleAttbute(10)]
public class UpGradeHouse : IHomeAdornCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		string[] array = GameProperties.HouseUpgradeUse.Split('|');
		if (array.Length < 2)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("HomeHostInfo.LoadDataFail"));
		}
		else
		{
			UserHomeInfo data = Player.Home.Data;
			if (data.houseStage < 2)
			{
				string[] array2 = array[data.houseStage].Split(',');
				if (array2.Length < 5)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("HomeHostInfo.LoadDataFail"));
				}
				else
				{
					int templateId = int.Parse(array2[1]);
					int num = int.Parse(array2[2]);
					int templateId2 = int.Parse(array2[3]);
					int num2 = int.Parse(array2[4]);
					ItemInfo itemByTemplateID = Player.PropBag.GetItemByTemplateID(templateId);
					ItemInfo itemByTemplateID2 = Player.PropBag.GetItemByTemplateID(templateId2);
					if (itemByTemplateID == null || itemByTemplateID2 == null)
					{
						Player.SendMessage(LanguageMgr.GetTranslation("UpGradeHouse.NotEnoughtItem"));
					}
					else if (itemByTemplateID.Count < num || itemByTemplateID2.Count < num2)
					{
						Player.SendMessage(LanguageMgr.GetTranslation("UpGradeHouse.NotEnoughtItem"));
					}
					else
					{
						bool val = false;
						if (Player.PropBag.RemoveTemplate(templateId, num) && Player.PropBag.RemoveTemplate(templateId2, num2))
						{
							val = true;
						}
						data.houseStage = int.Parse(array2[0]);
						GSPacketIn gSPacketIn = new GSPacketIn(292);
						gSPacketIn.WriteByte(10);
						gSPacketIn.WriteBoolean(val);
						Player.SendTCP(gSPacketIn);
					}
				}
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("UpGradeHouse.MaxLevel"));
			}
		}
		return true;
	}
}
