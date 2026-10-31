using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(256, "LEAGUE_GETAWARD")]
public class DailyLeagueGetRewardHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = 0;
		int num3 = 0;
		if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(0.5), DateTime.Now) > 0)
		{
			return 0;
		}
		string translateId = "DailyLeagueGetReward.Successfull";
		ProduceBussiness produceBussiness = new ProduceBussiness();
		DailyLeagueAwardList[] allDailyLeagueAwardList = produceBussiness.GetAllDailyLeagueAwardList();
		DailyLeagueAwardItems[] allDailyLeagueAwardItems = produceBussiness.GetAllDailyLeagueAwardItems();
		List<ItemInfo> list = new List<ItemInfo>();
		DailyLeagueAwardList[] array = allDailyLeagueAwardList;
		foreach (DailyLeagueAwardList dailyLeagueAwardList in array)
		{
			if (dailyLeagueAwardList.Class == num)
			{
				num2 = dailyLeagueAwardList.Score;
				num3 = dailyLeagueAwardList.Grade;
			}
		}
		int num4 = (1 << num) - 1;
		DailyLeagueAwardItems[] array2 = allDailyLeagueAwardItems;
		foreach (DailyLeagueAwardItems dailyLeagueAwardItems in array2)
		{
			if (dailyLeagueAwardItems.Class == num)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(dailyLeagueAwardItems.TemplateID), 1, 104);
				itemInfo.StrengthenLevel = dailyLeagueAwardItems.StrengthLevel;
				itemInfo.AttackCompose = dailyLeagueAwardItems.AttackCompose;
				itemInfo.DefendCompose = dailyLeagueAwardItems.DefendCompose;
				itemInfo.AgilityCompose = dailyLeagueAwardItems.AgilityCompose;
				itemInfo.LuckCompose = dailyLeagueAwardItems.LuckCompose;
				itemInfo.IsBinds = dailyLeagueAwardItems.IsBind;
				itemInfo.ValidDate = dailyLeagueAwardItems.ValidDate;
				itemInfo.Count = dailyLeagueAwardItems.Count;
				list.Add(itemInfo);
			}
		}
		if (list != null)
		{
			if (client.Player.PlayerCharacter.Grade >= num3)
			{
				if (client.Player.MatchInfo.weeklyScore >= num2)
				{
					client.Player.RefreshLeagueGetReward(num4, num2);
					Console.WriteLine($"awardGot: {num4}");
					client.Player.SendItemsToMail(list, LanguageMgr.GetTranslation("DailyLeagueGetReward.Content"), LanguageMgr.GetTranslation("Game.Server.LeagueReward.Title"), eMailType.Manage);
					return 1;
				}
				return 0;
			}
			return 0;
		}
		client.Player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
		client.Player.LastOpenCard = DateTime.Now;
		return 1;
	}
}
