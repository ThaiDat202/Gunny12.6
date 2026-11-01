//using System.Collections.Generic;
//using Bussiness.Managers;
//using Game.Base.Packets;
//using SqlDataProvider.Data;

//namespace Game.Server.Packets.Client;

//[PacketHandler(645, "加载积分奖励")]
//public class LoadingScoreHandler : IPacketHandler
//{
//	public int HandlePacket(GameClient client, GSPacketIn packet)
//	{
//		if (client.Player.PlayerCharacter.LoadingScore >= 400)
//		{
//			return 0;
//		}
//		int num = packet.ReadInt();
//		if (num < 20)
//		{
//			return 0;
//		}
//		client.Player.AddLoadingScore(num);
//		List<ItemInfo> list = new List<ItemInfo>();
//		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(11025);
//		int num2 = num / 20;
//		if (itemTemplateInfo != null)
//		{
//			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
//			itemInfo.Count = num2;
//			itemInfo.IsBinds = true;
//			list.Add(itemInfo);
//		}
//		int num3 = 20 - client.Player.PlayerCharacter.LoadingScore / 20;
//		string content = $"你在刚刚的loading等待中，敲打啵咕获得{num}分，奖励{num2}个强化石，今日还可获得{num3}个强化石";
//		WorldEventMgr.SendItemsToMails(list, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, "敲打啵咕赢奖励", content);
//		return 1;
//	}
//}
