using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.WonderFul;

public class WonderFulActivityManager
{
	private static readonly ILog Log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly object m_lock = new object();

	public static void WonderFulActivityInit(GamePlayer player, int type = 3)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(405, player.PlayerCharacter.ID);
		gSPacketIn.WriteInt(type);
		if (type <= 0)
		{
			return;
		}
		try
		{
			List<GM_Activity> list = GmActivityMgr.GmActivityInfos.FindAll((GM_Activity q) => q.beginTime < DateTime.Now && q.endTime > DateTime.Now);
			gSPacketIn.WriteInt(list.Count);
			foreach (GM_Activity active in list)
			{
				gSPacketIn.WriteString(active.activityId);
				List<GM_Gift> list2 = GmActivityMgr.FindGmGift(active.activityId);
				List<User_GmActivityCondition> list3 = new List<User_GmActivityCondition>();
				List<User_GmActivityReward> list4 = new List<User_GmActivityReward>();
				foreach (GM_Gift gift in list2)
				{
					List<User_GmActivityCondition> list5 = player.GmActivityConditions.FindAll((User_GmActivityCondition q) => q.GiftBagID == gift.giftbagId);
					int day = 0;
					if (list5.Count == 0)
					{
						foreach (User_GmActivityCondition item2 in from conditionInfo in GmActivityMgr.FindGmActiveCondition(gift.giftbagId)
							select new User_GmActivityCondition
							{
								UserID = player.PlayerId,
								ActivityID = active.activityId,
								GiftBagID = conditionInfo.giftbagId,
								StatusID = ((active.activityType == 60) ? day++ : 0),
								StatusValue = 0
							})
						{
							list3.Add(item2);
							player.GmActivityConditions.Add(item2);
						}
					}
					else
					{
						list3.AddRange(list5);
					}
					List<User_GmActivityReward> list6 = player.GmActivityRewards.FindAll((User_GmActivityReward q) => q.GiftBagID == gift.giftbagId);
					if (list6.Count == 0)
					{
						User_GmActivityReward item = new User_GmActivityReward
						{
							UserID = player.PlayerId,
							ActivityID = active.activityId,
							GiftBagID = GmActivityMgr.FindGmActiveReward(gift.giftbagId).First().giftId,
							Times = 0
						};
						list4.Add(item);
						player.GmActivityRewards.Add(item);
					}
					else
					{
						list4.AddRange(list6);
					}
				}
				gSPacketIn.WriteInt(list3.Count);
				foreach (User_GmActivityCondition item3 in list3)
				{
					gSPacketIn.WriteInt(item3.StatusID);
					gSPacketIn.WriteInt(item3.StatusValue);
				}
				gSPacketIn.WriteInt(list4.Count);
				foreach (User_GmActivityReward item4 in list4)
				{
					gSPacketIn.WriteString(item4.GiftBagID);
					gSPacketIn.WriteInt(item4.Times);
					gSPacketIn.WriteInt(GmActivityMgr.FindGmActiveReward(item4.GiftBagID)[0].allGiftGetTimes);
				}
			}
			player.SendTCP(gSPacketIn);
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("WonderFulActivityInit error:", exception);
			}
		}
	}

	public static void RefreshWonderFulActivity(GamePlayer player, string activityId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(405, player.PlayerCharacter.ID);
		gSPacketIn.WriteInt(3);
		List<GM_Activity> list = GmActivityMgr.GmActivityInfos.FindAll((GM_Activity q) => q.beginTime < DateTime.Now && q.endTime > DateTime.Now);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteString(activityId);
		gSPacketIn.WriteInt(player.GmActivityConditions.Count);
		foreach (User_GmActivityCondition gmActivityCondition in player.GmActivityConditions)
		{
			gSPacketIn.WriteInt(gmActivityCondition.StatusID);
			gSPacketIn.WriteInt(gmActivityCondition.StatusValue);
		}
		gSPacketIn.WriteInt(player.GmActivityRewards.Count);
		foreach (User_GmActivityReward gmActivityReward in player.GmActivityRewards)
		{
			gSPacketIn.WriteString(gmActivityReward.GiftBagID);
			gSPacketIn.WriteInt(gmActivityReward.Times);
			gSPacketIn.WriteInt(GmActivityMgr.FindGmActiveReward(gmActivityReward.GiftBagID)[0].allGiftGetTimes);
		}
		player.SendTCP(gSPacketIn);
	}

	public static void SendWonderFulReward(GamePlayer player, GSPacketIn packet)
	{
		try
		{
			lock (m_lock)
			{
				for (int i = 0; i < packet.ReadInt(); i++)
				{
					string activityId = packet.ReadString();
					int num = packet.ReadInt();
					GM_Activity gM_Activity = GmActivityMgr.FindGmActivity(activityId);
					for (int j = 0; j < packet.ReadInt(); j++)
					{
						string[] array = packet.ReadString().Split(',');
						string giftId = array[0];
						int num2 = packet.ReadInt();
						List<ItemInfo> list = new List<ItemInfo>();
						if (gM_Activity == null || gM_Activity.beginTime > DateTime.Now || gM_Activity.endTime < DateTime.Now)
						{
							continue;
						}
						List<GM_Gift> list2 = GmActivityMgr.FindGmGift(activityId);
						if (list2.Count == 0)
						{
							continue;
						}
						List<GM_Active_Reward> list3 = GmActivityMgr.FindGmActiveReward(giftId);
						if (list3.Count == 0)
						{
							continue;
						}
						List<GM_Active_Condition> list4 = GmActivityMgr.FindGmActiveCondition(giftId);
						User_GmActivityReward user_GmActivityReward = player.GmActivityRewards.Find((User_GmActivityReward a) => a.ActivityID == activityId && a.GiftBagID == giftId);
						if (user_GmActivityReward == null)
						{
							continue;
						}
						switch (gM_Activity.activityType)
						{
						case 14:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 0 || user_GmActivityReward.Times > 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition4 = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId);
							if (user_GmActivityCondition4 == null || user_GmActivityCondition4.StatusValue < list4[0].remain1 || user_GmActivityCondition4.StatusValue == 0)
							{
								break;
							}
							foreach (GM_Active_Reward item in list3)
							{
								ItemInfo itemInfo6 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.templateId), 1, 102);
								if (itemInfo6 != null)
								{
									itemInfo6.IsBinds = item.isBind == 1;
									itemInfo6.Count = item.count;
									itemInfo6.ValidDate = item.validDate;
									string[] array7 = item.property.Split(',');
									itemInfo6.StrengthenLevel = int.Parse(array7[0]);
									itemInfo6.AttackCompose = int.Parse(array7[1]);
									itemInfo6.DefendCompose = int.Parse(array7[2]);
									itemInfo6.AgilityCompose = int.Parse(array7[3]);
									itemInfo6.LuckCompose = int.Parse(array7[4]);
									list.Add(itemInfo6);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityReward.Times = 1;
								RefreshWonderFulActivity(player, activityId);
							}
							break;
						}
						case 25:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 0 || user_GmActivityReward.Times > 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId);
							if (user_GmActivityCondition == null || user_GmActivityCondition.StatusValue < list4[0].remain1 || user_GmActivityCondition.StatusValue == 0)
							{
								break;
							}
							foreach (GM_Active_Reward item2 in list3)
							{
								ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.templateId), 1, 102);
								if (itemInfo2 != null)
								{
									itemInfo2.IsBinds = item2.isBind == 1;
									itemInfo2.Count = item2.count;
									itemInfo2.ValidDate = item2.validDate;
									string[] array3 = item2.property.Split(',');
									itemInfo2.StrengthenLevel = int.Parse(array3[0]);
									itemInfo2.AttackCompose = int.Parse(array3[1]);
									itemInfo2.DefendCompose = int.Parse(array3[2]);
									itemInfo2.AgilityCompose = int.Parse(array3[3]);
									itemInfo2.LuckCompose = int.Parse(array3[4]);
									list.Add(itemInfo2);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityReward.Times = 1;
								RefreshWonderFulActivity(player, activityId);
							}
							break;
						}
						case 20:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 0 || user_GmActivityReward.Times > 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition2 = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId);
							if (user_GmActivityCondition2 == null || user_GmActivityCondition2.StatusValue < list4[0].remain1 || user_GmActivityCondition2.StatusValue == 0)
							{
								break;
							}
							foreach (GM_Active_Reward item3 in list3)
							{
								ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item3.templateId), 1, 102);
								if (itemInfo3 != null)
								{
									itemInfo3.IsBinds = item3.isBind == 1;
									itemInfo3.Count = item3.count;
									itemInfo3.ValidDate = item3.validDate;
									string[] array4 = item3.property.Split(',');
									itemInfo3.StrengthenLevel = int.Parse(array4[0]);
									itemInfo3.AttackCompose = int.Parse(array4[1]);
									itemInfo3.DefendCompose = int.Parse(array4[2]);
									itemInfo3.AgilityCompose = int.Parse(array4[3]);
									itemInfo3.LuckCompose = int.Parse(array4[4]);
									list.Add(itemInfo3);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityReward.Times = 1;
								RefreshWonderFulActivity(player, activityId);
							}
							break;
						}
						case 5:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 3 || user_GmActivityReward.Times > 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition5 = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId);
							if (user_GmActivityCondition5 == null)
							{
								break;
							}
							foreach (GM_Active_Reward item4 in list3)
							{
								ItemInfo itemInfo7 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item4.templateId), 1, 102);
								if (itemInfo7 != null)
								{
									itemInfo7.IsBinds = item4.isBind == 1;
									itemInfo7.Count = item4.count;
									itemInfo7.ValidDate = item4.validDate;
									string[] array8 = item4.property.Split(',');
									itemInfo7.StrengthenLevel = int.Parse(array8[0]);
									itemInfo7.AttackCompose = int.Parse(array8[1]);
									itemInfo7.DefendCompose = int.Parse(array8[2]);
									itemInfo7.AgilityCompose = int.Parse(array8[3]);
									itemInfo7.LuckCompose = int.Parse(array8[4]);
									list.Add(itemInfo7);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityReward.Times = 1;
							}
							break;
						}
						case 0:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 6 || user_GmActivityReward.Times > 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition6 = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId);
							if (user_GmActivityCondition6 == null || user_GmActivityCondition6.StatusValue < list4[0].conditionValue)
							{
								break;
							}
							foreach (GM_Active_Reward item5 in list3)
							{
								ItemInfo itemInfo8 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item5.templateId), 1, 102);
								if (itemInfo8 != null)
								{
									itemInfo8.IsBinds = item5.isBind == 1;
									itemInfo8.Count = item5.count;
									itemInfo8.ValidDate = item5.validDate;
									string[] array9 = item5.property.Split(',');
									itemInfo8.StrengthenLevel = int.Parse(array9[0]);
									itemInfo8.AttackCompose = int.Parse(array9[1]);
									itemInfo8.DefendCompose = int.Parse(array9[2]);
									itemInfo8.AgilityCompose = int.Parse(array9[3]);
									itemInfo8.LuckCompose = int.Parse(array9[4]);
									list.Add(itemInfo8);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityReward.Times = 1;
							}
							break;
						}
						case 8:
						{
							if (list4.Count == 0 || gM_Activity.activityChildType != 0)
							{
								break;
							}
							User_GmActivityCondition user_GmActivityCondition3 = player.GmActivityConditions.Find((User_GmActivityCondition a) => a.ActivityID == activityId && a.GiftBagID == giftId);
							if (user_GmActivityCondition3 == null || user_GmActivityCondition3.StatusID < list4.First().conditionValue || user_GmActivityCondition3.StatusValue == 0)
							{
								break;
							}
							foreach (GM_Active_Reward item6 in list3)
							{
								ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item6.templateId), 1, 102);
								if (itemInfo4 != null)
								{
									itemInfo4.IsBinds = item6.isBind == 1;
									itemInfo4.Count = item6.count;
									itemInfo4.ValidDate = item6.validDate;
									string[] array5 = item6.property.Split(',');
									itemInfo4.StrengthenLevel = int.Parse(array5[0]);
									itemInfo4.AttackCompose = int.Parse(array5[1]);
									itemInfo4.DefendCompose = int.Parse(array5[2]);
									itemInfo4.AgilityCompose = int.Parse(array5[3]);
									itemInfo4.LuckCompose = int.Parse(array5[4]);
									list.Add(itemInfo4);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								user_GmActivityCondition3.StatusValue = 0;
							}
							break;
						}
						case 2:
							if (gM_Activity.activityChildType != 0)
							{
								break;
							}
							foreach (GM_Active_Reward item7 in list3.FindAll((GM_Active_Reward r) => r.rewardType == 0))
							{
								if (player.GetItemCount(item7.templateId) < item7.count)
								{
									return;
								}
								player.RemoveTemplate(item7.templateId, item7.count);
							}
							foreach (GM_Active_Reward item8 in list3.FindAll((GM_Active_Reward r) => r.rewardType == 1))
							{
								ItemInfo itemInfo5 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item8.templateId), 1, 102);
								if (itemInfo5 != null)
								{
									itemInfo5.IsBinds = item8.isBind == 1;
									itemInfo5.Count = item8.count;
									itemInfo5.ValidDate = item8.validDate;
									string[] array6 = item8.property.Split(',');
									itemInfo5.StrengthenLevel = int.Parse(array6[0]);
									itemInfo5.AttackCompose = int.Parse(array6[1]);
									itemInfo5.DefendCompose = int.Parse(array6[2]);
									itemInfo5.AgilityCompose = int.Parse(array6[3]);
									itemInfo5.LuckCompose = int.Parse(array6[4]);
									list.Add(itemInfo5);
								}
							}
							if (player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
							{
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.Exchange.Success"));
							}
							break;
						case 19:
						{
							bool flag = int.Parse(array[1]) != -8;
							GM_Active_Condition gM_Active_Condition = list4.Find((GM_Active_Condition c) => c.conditionIndex == 1);
							GM_Active_Condition gM_Active_Condition2 = list4.Find((GM_Active_Condition c) => c.conditionIndex == 2);
							GM_Active_Condition gM_Active_Condition3 = list4.Find((GM_Active_Condition c) => c.conditionIndex == 3);
							GM_Active_Condition gM_Active_Condition4 = list4.Find((GM_Active_Condition c) => c.conditionIndex == 4);
							GM_Active_Condition gM_Active_Condition5 = list4.Find((GM_Active_Condition c) => c.conditionIndex == 5);
							GM_Active_Condition gM_Active_Condition6 = list4.Find((GM_Active_Condition c) => c.conditionIndex == 100);
							int num3 = gM_Active_Condition3.conditionValue * num;
							GM_Active_Reward gM_Active_Reward = list3[0];
							if (flag && gM_Active_Condition2.conditionValue == -9)
							{
								if (player.PlayerCharacter.DDTMoney < num3)
								{
									player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoGiftToken"));
									break;
								}
							}
							else if (player.PlayerCharacter.Money < num3)
							{
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoMoney"));
								break;
							}
							if (gM_Activity.activityChildType == 2)
							{
								if (gM_Active_Condition5.conditionValue > 0)
								{
									if (player.PlayerCharacter.typeVIP == 0)
									{
										player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoVIP"));
										break;
									}
									if (player.PlayerCharacter.VIPLevel < gM_Active_Condition5.conditionValue)
									{
										player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.VIPLimit", gM_Active_Condition5.conditionValue));
										break;
									}
								}
							}
							else if (gM_Activity.activityChildType == 3)
							{
								if (user_GmActivityReward.Times >= gM_Active_Condition.conditionValue)
								{
									player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.IndividualLimit"));
									break;
								}
								if (user_GmActivityReward.Times + num > gM_Active_Condition.conditionValue)
								{
									num = gM_Active_Condition.conditionValue - user_GmActivityReward.Times;
									num3 = gM_Active_Condition3.conditionValue * num;
									if (flag && gM_Active_Condition2.conditionValue == -9)
									{
										if (player.PlayerCharacter.DDTMoney < num3)
										{
											player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoGiftToken"));
											break;
										}
									}
									else if (player.PlayerCharacter.Money < num3)
									{
										player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoMoney"));
										break;
									}
								}
							}
							else if (gM_Activity.activityChildType == 5)
							{
								if (gM_Active_Reward.allGiftGetTimes == gM_Active_Condition6.conditionValue)
								{
									player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.ServerLimit"));
									break;
								}
								if (gM_Active_Reward.allGiftGetTimes + num > gM_Active_Condition6.conditionValue)
								{
									num = gM_Active_Condition6.conditionValue - gM_Active_Reward.allGiftGetTimes;
									num3 = gM_Active_Condition3.conditionValue * num;
									if (flag && gM_Active_Condition2.conditionValue == -9)
									{
										if (player.PlayerCharacter.DDTMoney < num3)
										{
											player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoGiftToken"));
											break;
										}
									}
									else if (player.PlayerCharacter.Money < num3)
									{
										player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.PanicBuying.NoMoney"));
										break;
									}
								}
							}
							for (int k = 0; k < num; k++)
							{
								ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(gM_Active_Reward.templateId), 1, 102);
								if (itemInfo == null)
								{
									break;
								}
								itemInfo.IsBinds = gM_Active_Reward.isBind == 1;
								itemInfo.Count = ((ItemMgr.FindItemTemplate(gM_Active_Reward.templateId).MaxCount == 1) ? gM_Active_Reward.count : (gM_Active_Reward.count * num));
								itemInfo.ValidDate = gM_Active_Reward.validDate;
								string[] array2 = gM_Active_Reward.property.Split(',');
								itemInfo.StrengthenLevel = int.Parse(array2[0]);
								itemInfo.AttackCompose = int.Parse(array2[1]);
								itemInfo.DefendCompose = int.Parse(array2[2]);
								itemInfo.AgilityCompose = int.Parse(array2[3]);
								itemInfo.LuckCompose = int.Parse(array2[4]);
								list.Add(itemInfo);
								if (ItemMgr.FindItemTemplate(gM_Active_Reward.templateId).MaxCount > 1)
								{
									break;
								}
							}
							if (flag && gM_Active_Condition2.conditionValue == -9)
							{
								if (!player.GiftTokenDirect(gM_Active_Condition3.conditionValue))
								{
									break;
								}
								if (!player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
								{
									player.AddGiftToken(gM_Active_Condition3.conditionValue);
									break;
								}
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								if (gM_Activity.activityChildType == 3)
								{
									user_GmActivityReward.Times += num;
								}
								else if (gM_Activity.activityChildType == 4)
								{
									gM_Active_Reward.allGiftGetTimes += num;
								}
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								WonderFulActivityInit(player);
							}
							else
							{
								if (!player.MoneyDirect(gM_Active_Condition3.conditionValue))
								{
									break;
								}
								if (!player.SendItemsToMail(list, LanguageMgr.GetTranslation("WonderFulActivity.Mail.Content"), LanguageMgr.GetTranslation("WonderFulActivity.Mail.Title", gM_Activity.activityName), eMailType.Manage))
								{
									player.AddMoney(gM_Active_Condition3.conditionValue);
									break;
								}
								player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
								if (gM_Activity.activityChildType == 3)
								{
									user_GmActivityReward.Times += num;
								}
								else if (gM_Activity.activityChildType == 4)
								{
									gM_Active_Reward.allGiftGetTimes += num;
								}
								player.SendMessage(LanguageMgr.GetTranslation("WonderfulActivity.GetReward.Success"));
								WonderFulActivityInit(player);
							}
							break;
						}
						}
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (Log.IsErrorEnabled)
			{
				Log.Error("SendWonderFulReward error:", exception);
			}
		}
	}
}
