using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Farm.Handle;

[FarmHandleAttbute(25)]
public class FarmCondenser : IFarmCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int templateId = 11957;
		ItemInfo itemByTemplateID = Player.PropBag.GetItemByTemplateID(0, templateId);
		if (itemByTemplateID == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FarmCondenser.Msg1"));
		}
		else
		{
			int itemCount = Player.PropBag.GetItemCount(templateId);
			if (num > itemCount)
			{
				Player.SendMessage(LanguageMgr.GetTranslation("FarmCondenser.Msg1"));
			}
			else
			{
				Player.Farm.CurrentFarm.TreeCostExp += 10 * num;
				TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(Player.Farm.CurrentFarm.TreeLevel);
				if (Player.Farm.CurrentFarm.TreeCostExp >= treeTemplateInfo.CostExp)
				{
					Player.Farm.CurrentFarm.TreeLevel++;
					Player.Farm.CurrentFarm.TreeCostExp = 0;
					treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(Player.Farm.CurrentFarm.TreeLevel);
					Player.Farm.CurrentFarm.TreeExp += treeTemplateInfo.Exp - Player.Farm.CurrentFarm.TreeExp;
				}
				else
				{
					Player.SendMessage(LanguageMgr.GetTranslation("FarmCondenser.Msg2"));
				}
				Player.PropBag.RemoveTemplate(templateId, num);
				GSPacketIn gSPacketIn = new GSPacketIn(81);
				gSPacketIn.WriteByte(25);
				Player.SendTCP(gSPacketIn);
			}
		}
		return true;
	}
}
