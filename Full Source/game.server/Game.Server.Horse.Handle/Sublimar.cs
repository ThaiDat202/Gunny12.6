using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Horse.Handle;

[HorseHandleAttbute(32)]
public class Sublimar : IHorseCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		if (Player.Horse.horsePicCherish.ContainsKey(num))
		{
			MountDrawDataInfo mountDrawDataInfo = Player.Horse.horsePicCherish[num];
			if (mountDrawDataInfo.qiLingGrade < ItemMgr.maxGradeSublimarImagemMontaria())
			{
				TS_UpgradeTemplate upgradeSublimarImagemMontaria = ItemMgr.getUpgradeSublimarImagemMontaria(mountDrawDataInfo.qiLingGrade);
				int itemCount = Player.PropBag.GetItemCount(upgradeSublimarImagemMontaria.ItemTempId1);
				if (itemCount >= upgradeSublimarImagemMontaria.Param1)
				{
					Player.PropBag.BeginChanges();
					Player.PropBag.RemoveTemplate(upgradeSublimarImagemMontaria.ItemTempId1, upgradeSublimarImagemMontaria.Param1);
					Player.PropBag.UpdateChangedPlaces();
					Player.PropBag.CommitChanges();
					mountDrawDataInfo.qiLingGrade++;
					Player.Horse.SaveToDatabase();
					GSPacketIn gSPacketIn = new GSPacketIn(260);
					gSPacketIn.WriteByte(32);
					gSPacketIn.WriteInt(num);
					packet.WriteInt(mountDrawDataInfo.qiLingGrade);
					packet.WriteInt(0);
					Player.SendTCP(packet);
					Player.EquipBag.UpdatePlayerProperties();
					return true;
				}
				Player.SendMessage("Não possui " + ItemMgr.FindItemTemplate(upgradeSublimarImagemMontaria.ItemTempId1).Name);
				return false;
			}
			Player.SendMessage("VAI HACKEAR A MÃE");
			return false;
		}
		Player.SendMessage("Imagem de montaria não ativa");
		return false;
	}
}
