using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.ActiveSystem.Handle;

[ActiveSystemHandleAttbute(105)]
public class TreasurepuzzleGetReward : IActiveSystemCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		int num = packet.ReadInt() - 1;
		TreasurePuzzleDataInfo[] treasurepuzzle = Player.Actives.Treasurepuzzle;
		if (num < 0 || num > treasurepuzzle.Length)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.GetAwardFail"));
		}
		else
		{
			TreasurePuzzleDataInfo treasurePuzzleDataInfo = treasurepuzzle[num];
			if (treasurePuzzleDataInfo != null)
			{
				if (!treasurePuzzleDataInfo.canGetReward())
				{
					Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.PiceNotEnought"));
					return false;
				}
				if (treasurePuzzleDataInfo.canGetReward() && treasurePuzzleDataInfo.IsGetAward)
				{
					Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.GetComplete"));
					return false;
				}
				List<TreasurePuzzleRewardInfo> list = ActiveSystermAwardMgr.FindTreasurePuzzleReward(treasurePuzzleDataInfo.PuzzleID);
				List<ItemInfo> list2 = new List<ItemInfo>();
				foreach (TreasurePuzzleRewardInfo item in list)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
						itemInfo.Count = item.Count;
						itemInfo.IsBinds = item.IsBinds;
						itemInfo.StrengthenLevel = item.StrengthenLevel;
						itemInfo.AttackCompose = item.AttackCompose;
						itemInfo.DefendCompose = item.DefendCompose;
						itemInfo.AgilityCompose = item.AgilityCompose;
						itemInfo.LuckCompose = item.LuckCompose;
						list2.Add(itemInfo);
					}
				}
				if (list2.Count > 0)
				{
					Player.Actives.Treasurepuzzle[num].IsGetAward = true;
					Player.Actives.TreasurepuzzleGetAward(num);
					treasurepuzzle = Player.Actives.Treasurepuzzle;
					WorldEventMgr.SendItemsToMails(list2, Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName, Player.ZoneId, LanguageMgr.GetTranslation("TreasurePuzzleEnter.Title"));
					GSPacketIn gSPacketIn = new GSPacketIn(145, Player.PlayerCharacter.ID);
					gSPacketIn.WriteByte(108);
					gSPacketIn.WriteInt(treasurepuzzle.Length);
					TreasurePuzzleDataInfo[] array = treasurepuzzle;
					foreach (TreasurePuzzleDataInfo treasurePuzzleDataInfo2 in array)
					{
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.PuzzleID);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole1Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole1Have);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole2Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole2Have);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole3Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole3Have);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole4Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole4Have);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole5Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole5Have);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole6Need);
						gSPacketIn.WriteInt(treasurePuzzleDataInfo2.hole6Have);
						gSPacketIn.WriteBoolean(treasurePuzzleDataInfo2.IsGetAward);
					}
					Player.SendTCP(gSPacketIn);
					Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.GetAwardTrue"));
					Player.Actives.SaveTreasurepuzzleDatabase();
				}
				else
				{
					Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.GetAwardFail2"));
				}
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("TreasurePuzzleEnter.GetAwardFail"));
			}
		}
		return true;
	}
}
