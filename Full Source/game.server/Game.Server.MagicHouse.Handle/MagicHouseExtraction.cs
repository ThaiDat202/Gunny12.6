using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.MagicHouse.Handle;

[MagicHouseHandleAttbute(6)]
public class MagicHouseExtraction : IMagicHouseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int templateId = packet.ReadInt();
		int num = packet.ReadInt();
		ItemInfo itemByTemplateID = Player.StoreBag.GetItemByTemplateID(templateId);
		if (itemByTemplateID == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseExtraction.Empty"));
		}
		else
		{
			MagicFusionDataInfo magicFusionData = MagicFusionDataMgr.GetMagicFusionData(itemByTemplateID.TemplateID);
			bool flag = false;
			if (magicFusionData != null && magicFusionData.Type == 2)
			{
				flag = Player.StoreBag.RemoveTemplate(itemByTemplateID.TemplateID, itemByTemplateID.Count);
				if (flag)
				{
					Player.MagicHouse.Info.essence += magicFusionData.GetKeys * itemByTemplateID.Count;
					Player.MagicHouse.UpdateMessage();
					Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseExtraction.Success"));
				}
			}
			if (!flag)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("MagicHouseExtraction.WrongItem"));
			}
			GSPacketIn gSPacketIn = new GSPacketIn(84, Player.PlayerId);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(6);
			gSPacketIn.WriteBoolean(flag);
			Player.SendTCP(gSPacketIn);
		}
		return false;
	}
}
