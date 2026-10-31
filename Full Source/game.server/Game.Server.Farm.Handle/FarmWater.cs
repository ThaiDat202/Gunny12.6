using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Farm.Handle;

[FarmHandleAttbute(32)]
public class FarmWater : IFarmCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (num < 0 || num > Player.Farm.CurrentFarm.LoveScore)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FarmWater.Msg1"));
		}
		else if (Player.Farm.CurrentFarm.TreeLevel < TreeTemplateMgr.MaxTreeTemplate())
		{
			Player.Farm.CurrentFarm.TreeExp += num;
			TreeTemplateInfo treeTemplateInfo = TreeTemplateMgr.FindNextTreeTemplate(Player.Farm.CurrentFarm.TreeLevel + 1);
			if (Player.Farm.CurrentFarm.TreeExp >= treeTemplateInfo.Exp)
			{
				Player.Farm.CurrentFarm.TreeLevel++;
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("FarmWater.Msg2"));
			}
			Player.Farm.RemoveLoveScore(num);
			GSPacketIn gSPacketIn = new GSPacketIn(81);
			gSPacketIn.WriteByte(32);
			Player.SendTCP(gSPacketIn);
		}
		else
		{
			Player.SendMessage(LanguageMgr.GetTranslation("FarmWater.MaxLevel"));
		}
		return true;
	}
}
