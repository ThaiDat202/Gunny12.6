using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(84, "场景用户离开")]
public class ActivityPackageHandler : IPacketHandler
{
	private readonly int[] m_gradeList = new int[9] { 10, 20, 30, 40, 45, 50, 55, 60, 65 };

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int iD = client.Player.PlayerCharacter.ID;
		ActiveSystemInfo info = client.Player.Actives.Info;
		switch (num)
		{
		case 1:
		{
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			if (DateTime.Compare(client.Player.LastOpenGrowthPackage.AddMilliseconds(500.0), DateTime.Now) > 0)
			{
				client.Player.Out.SendGrowthPackageUpadte(client.Player.PlayerCharacter.ID, info.AvailTime);
				return 0;
			}
			int availTime = info.AvailTime;
			int promotePackagePrice = GameProperties.PromotePackagePrice;
			string translation = LanguageMgr.GetTranslation("UserBuyItemHandler.Success");
			List<ItemInfo> list9 = new List<ItemInfo>();
			int num9 = (client.Player.PlayerCharacter.Sex ? 1 : 2);
			int num10 = packet.ReadInt();
			switch (num10)
			{
			case 1:
				if (!client.Player.MoneyDirect(promotePackagePrice))
				{
					break;
				}
				if (CanGetAward(client.Player.PlayerCharacter.Grade, availTime))
				{
					List<ActivitySystemItemInfo> growthPackage2 = ActiveMgr.GetGrowthPackage(availTime);
					foreach (ActivitySystemItemInfo item in growthPackage2)
					{
						ItemTemplateInfo itemTemplateInfo6 = ItemMgr.FindItemTemplate(item.TemplateID);
						if (itemTemplateInfo6 != null && (itemTemplateInfo6.NeedSex == 0 || itemTemplateInfo6.NeedSex == num9))
						{
							ItemInfo itemInfo6 = ItemInfo.CreateFromTemplate(itemTemplateInfo6, item.Count, 102);
							itemInfo6.Count = item.Count;
							itemInfo6.IsBinds = item.IsBind;
							itemInfo6.ValidDate = item.ValidDate;
							itemInfo6.StrengthenLevel = item.StrengthLevel;
							itemInfo6.AttackCompose = item.AttackCompose;
							itemInfo6.DefendCompose = item.DefendCompose;
							itemInfo6.AgilityCompose = item.AgilityCompose;
							itemInfo6.LuckCompose = item.LuckCompose;
							list9.Add(itemInfo6);
						}
					}
					break;
				}
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ActivityPackageHandler.Msg1"));
				return 0;
			case 2:
				if (CanGetAward(client.Player.PlayerCharacter.Grade, availTime))
				{
					translation = LanguageMgr.GetTranslation("ActivityPackageHandler.Msg3");
					List<ActivitySystemItemInfo> growthPackage = ActiveMgr.GetGrowthPackage(availTime);
					foreach (ActivitySystemItemInfo item2 in growthPackage)
					{
						ItemTemplateInfo itemTemplateInfo5 = ItemMgr.FindItemTemplate(item2.TemplateID);
						if (itemTemplateInfo5 != null && (itemTemplateInfo5.NeedSex == 0 || itemTemplateInfo5.NeedSex == num9))
						{
							ItemInfo itemInfo5 = ItemInfo.CreateFromTemplate(itemTemplateInfo5, item2.Count, 102);
							itemInfo5.Count = item2.Count;
							itemInfo5.IsBinds = item2.IsBind;
							itemInfo5.ValidDate = item2.ValidDate;
							itemInfo5.StrengthenLevel = item2.StrengthLevel;
							itemInfo5.AttackCompose = item2.AttackCompose;
							itemInfo5.DefendCompose = item2.DefendCompose;
							itemInfo5.AgilityCompose = item2.AgilityCompose;
							itemInfo5.LuckCompose = item2.LuckCompose;
							list9.Add(itemInfo5);
						}
					}
					break;
				}
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ActivityPackageHandler.Msg1"));
				return 0;
			default:
				Console.WriteLine("GrowthPackageType.{0}", (GrowthPackageType)num10);
				break;
			}
			if (list9.Count > 0)
			{
				info.AvailTime++;
				WorldEventMgr.SendItemsToMails(list9, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, $"Qùa trưởng thành cấp {m_gradeList[availTime]} ");
			}
			else
			{
				translation = LanguageMgr.GetTranslation("ActivityPackageHandler.Msg2");
			}
			client.Player.Out.SendGrowthPackageUpadte(client.Player.PlayerCharacter.ID, info.AvailTime);
			client.Out.SendMessage(eMessageType.Normal, translation);
			client.Player.LastOpenGrowthPackage = DateTime.Now;
			break;
		}
		case 2:
		{
			UserChickActiveInfo chickActiveData = client.Player.Actives.GetChickActiveData();
			int[] array2 = new int[10] { 1, 2, 4, 8, 16, 32, 64, 128, 256, 512 };
			int[] array3 = new int[10] { 5, 10, 25, 30, 40, 45, 48, 50, 55, 60 };
			int num5 = packet.ReadInt();
			GSPacketIn gSPacketIn = new GSPacketIn(84, client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteInt(2);
			switch (num5)
			{
			case 3:
				if (chickActiveData != null)
				{
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteInt(chickActiveData.IsKeyOpened);
					gSPacketIn.WriteInt(1);
					gSPacketIn.WriteDateTime(chickActiveData.KeyOpenedTime);
					gSPacketIn.WriteInt(chickActiveData.KeyOpenedType);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Monday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Tuesday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Wednesday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Thursday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Friday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != DayOfWeek.Saturday) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.EveryDay.Day >= DateTime.Now.Day || DateTime.Now.DayOfWeek != 0) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
					gSPacketIn.WriteInt((chickActiveData.AfterThreeDays.Day >= DateTime.Now.Day || !chickActiveData.OnThreeDay(DateTime.Now)) ? 1 : 0);
					gSPacketIn.WriteInt((!(chickActiveData.Weekly < chickActiveData.StartOfWeek(DateTime.Now, DayOfWeek.Saturday)) || DateTime.Now.DayOfWeek != DayOfWeek.Saturday) ? 1 : 0);
					gSPacketIn.WriteInt(chickActiveData.CurrentLvAward);
					client.Player.SendTCP(gSPacketIn);
				}
				break;
			case 1:
			{
				string text = packet.ReadString();
				if (text.Length != 6 || chickActiveData.IsKeyOpened != 0)
				{
					break;
				}
				using (new PlayerBussiness())
				{
					switch (123456)
					{
					case 0:
						chickActiveData.Active((client.Player.PlayerCharacter.Grade <= 15) ? 1 : 2);
						client.Player.Actives.SaveChickActiveData(chickActiveData);
						client.Player.Actives.SendUpdateChickActivation();
						client.Player.SendMessage(LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.Success"));
						break;
					case 1:
						client.Player.SendMessage(LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.NotExit"));
						break;
					case 2:
						client.Player.SendMessage(LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.IsUsed"));
						break;
					}
				}
				break;
			}
			case 2:
			{
				int num6 = packet.ReadInt();
				int num7 = packet.ReadInt();
				if (chickActiveData.IsKeyOpened == 1 && chickActiveData.KeyOpenedTime.AddDays(60.0).Date >= DateTime.Now.Date)
				{
					List<ActivitySystemItemInfo> list3 = null;
					int num8 = (client.Player.PlayerCharacter.Sex ? 1 : 2);
					string title = "";
					string message = "";
					if (num6 <= 7 && chickActiveData.EveryDay.Day < DateTime.Now.Day)
					{
						chickActiveData.EveryDay = DateTime.Now;
						list3 = ActiveMgr.FindChickActivePakage((chickActiveData.KeyOpenedType != 2) ? 1 : 101);
						title = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.DaylyAward.Title");
						message = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.DaylyAward.Msg");
					}
					else if (num6 <= 10 && chickActiveData.AfterThreeDays.Day < DateTime.Now.Day && chickActiveData.OnThreeDay(DateTime.Now))
					{
						chickActiveData.AfterThreeDays = DateTime.Now;
						list3 = ActiveMgr.FindChickActivePakage((chickActiveData.KeyOpenedType == 2) ? 102 : 2);
						title = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.WeekenAward.Title");
						message = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.WeekenAward.Msg");
					}
					else if (num6 == 11 && chickActiveData.Weekly < chickActiveData.StartOfWeek(DateTime.Now, DayOfWeek.Saturday) && DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
					{
						chickActiveData.Weekly = chickActiveData.StartOfWeek(DateTime.Now, DayOfWeek.Saturday);
						list3 = ActiveMgr.FindChickActivePakage((chickActiveData.KeyOpenedType == 2) ? 103 : 3);
						title = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.WeeklyAward.Title");
						message = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.WeeklyAward.Msg");
					}
					else if (num6 == 12 && array2[num7 - 1] > chickActiveData.CurrentLvAward && client.Player.PlayerCharacter.Grade >= array3[num7 - 1] && chickActiveData.KeyOpenedType == 1)
					{
						int[] array4 = new int[10] { 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10000, 10009, 10010 };
						chickActiveData.CurrentLvAward += array2[num7 - 1];
						list3 = ActiveMgr.FindChickActivePakage(array4[num7 - 1]);
						title = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.LevelAward.Title", array3[num7 - 1]);
						message = LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.LevelAward.Msg", array3[num7 - 1]);
					}
					if (list3 != null)
					{
						List<ItemInfo> list4 = new List<ItemInfo>();
						foreach (ActivitySystemItemInfo item3 in list3)
						{
							ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(item3.TemplateID);
							if (itemTemplateInfo2 != null && (itemTemplateInfo2.NeedSex == 0 || itemTemplateInfo2.NeedSex == num8))
							{
								ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo2, item3.Count, 102);
								itemInfo2.Count = item3.Count;
								itemInfo2.IsBinds = item3.IsBind;
								itemInfo2.ValidDate = item3.ValidDate;
								itemInfo2.StrengthenLevel = item3.StrengthLevel;
								itemInfo2.AttackCompose = item3.AttackCompose;
								itemInfo2.DefendCompose = item3.DefendCompose;
								itemInfo2.AgilityCompose = item3.AgilityCompose;
								itemInfo2.LuckCompose = item3.LuckCompose;
								list4.Add(itemInfo2);
							}
						}
						if (list4.Count > 0)
						{
							WorldEventMgr.SendItemsToMails(list4, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, title);
							client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
						}
						client.Player.Actives.SaveChickActiveData(chickActiveData);
						client.Player.Actives.SendUpdateChickActivation();
						client.Out.SendMessage(eMessageType.Normal, message);
					}
					else
					{
						client.Player.SendMessage(LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.NotFoundAward"));
					}
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("ActivityPackageHandler.ChickActivation.NotActive"));
				}
				break;
			}
			default:
				Console.WriteLine("ChickActivationType.{0}", (ChickActivationType)num5);
				break;
			}
			break;
		}
		case 4:
			if (client.Player.MagicHouseHandler != null)
			{
				client.Player.MagicHouseHandler.ProcessData(client.Player, packet);
			}
			break;
		case 6:
			if (client.Player.ZodiacHandler != null)
			{
				client.Player.ZodiacHandler.ProcessData(client.Player, packet);
			}
			break;
		case 7:
			if (client.Player.AnotherDimensionHandler != null)
			{
				client.Player.AnotherDimensionHandler.ProcessData(client.Player, packet);
			}
			break;
		case 11:
			switch (packet.ReadInt())
			{
			case 1:
			{
				bool isLoginDevice = packet.ReadBoolean();
				client.Player.IsLoginDevice = isLoginDevice;
				client.Out.SendLoginDeviceCheck(client.Player, client.Player.IsLoginDevice);
				break;
			}
			case 2:
			{
				if (client.Player.Extra.Info.loginDeviceDownAward)
				{
					break;
				}
				List<ActivitySystemItemInfo> list7 = ActiveMgr.FindLoginDevicePackage(1);
				if (list7.Count <= 0)
				{
					break;
				}
				client.Player.Extra.Info.loginDeviceDownAward = true;
				List<ItemInfo> list8 = new List<ItemInfo>();
				foreach (ActivitySystemItemInfo item4 in list7)
				{
					ItemTemplateInfo itemTemplateInfo4 = ItemMgr.FindItemTemplate(item4.TemplateID);
					if (itemTemplateInfo4 != null)
					{
						ItemInfo itemInfo4 = ItemInfo.CreateFromTemplate(itemTemplateInfo4, item4.Count, 102);
						itemInfo4.Count = item4.Count;
						itemInfo4.IsBinds = item4.IsBind;
						itemInfo4.ValidDate = item4.ValidDate;
						itemInfo4.StrengthenLevel = item4.StrengthLevel;
						itemInfo4.AttackCompose = item4.AttackCompose;
						itemInfo4.DefendCompose = item4.DefendCompose;
						itemInfo4.AgilityCompose = item4.AgilityCompose;
						itemInfo4.LuckCompose = item4.LuckCompose;
						list8.Add(itemInfo4);
					}
				}
				if (list8.Count > 0)
				{
					WorldEventMgr.SendItemsToMails(list8, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, LanguageMgr.GetTranslation("GameServer.LoginDevice.AwardTitleMail"));
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Out.SendLoginDeviceCheck(client.Player, client.Player.IsLoginDevice);
				break;
			}
			case 3:
			{
				if (client.Player.Extra.Info.loginDeviceDailyAward)
				{
					break;
				}
				List<ActivitySystemItemInfo> list5 = ActiveMgr.FindLoginDevicePackage(2);
				if (list5.Count <= 0)
				{
					break;
				}
				client.Player.Extra.Info.loginDeviceDailyAward = true;
				List<ItemInfo> list6 = new List<ItemInfo>();
				foreach (ActivitySystemItemInfo item5 in list5)
				{
					ItemTemplateInfo itemTemplateInfo3 = ItemMgr.FindItemTemplate(item5.TemplateID);
					if (itemTemplateInfo3 != null)
					{
						ItemInfo itemInfo3 = ItemInfo.CreateFromTemplate(itemTemplateInfo3, item5.Count, 102);
						itemInfo3.Count = item5.Count;
						itemInfo3.IsBinds = item5.IsBind;
						itemInfo3.ValidDate = item5.ValidDate;
						itemInfo3.StrengthenLevel = item5.StrengthLevel;
						itemInfo3.AttackCompose = item5.AttackCompose;
						itemInfo3.DefendCompose = item5.DefendCompose;
						itemInfo3.AgilityCompose = item5.AgilityCompose;
						itemInfo3.LuckCompose = item5.LuckCompose;
						list6.Add(itemInfo3);
					}
				}
				if (list6.Count > 0)
				{
					WorldEventMgr.SendItemsToMails(list6, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, LanguageMgr.GetTranslation("GameServer.LoginDevice.AwardTitleMail"));
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Out.SendLoginDeviceCheck(client.Player, client.Player.IsLoginDevice);
				break;
			}
			}
			break;
		case 12:
		{
			packet.ReadInt();
			int num3 = packet.ReadInt();
			List<ActivitySystemItemInfo> list = ActiveMgr.FindSignBuffPackage(num3);
			if (list.Count <= 0)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("SignBuffFightPower.NoAward"));
				return 0;
			}
			client.Player.Actives.SynFightPowerSign();
			List<int> prop = GameProperties.getProp(GameProperties.FightPowerSignData);
			int[] array = JsonConvert.DeserializeObject<int[]>(client.Player.Actives.Info.fightPowerSignData);
			int num4 = prop.IndexOf(num3 * 10000);
			if (num4 != -1 && num4 < array.Length && array[num4] == 1)
			{
				array[num4] = 2;
				client.Player.Actives.Info.fightPowerSignData = JsonConvert.SerializeObject(array);
				List<ItemInfo> list2 = new List<ItemInfo>();
				foreach (ActivitySystemItemInfo item6 in list)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item6.TemplateID);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item6.Count, 102);
						itemInfo.Count = item6.Count;
						itemInfo.IsBinds = item6.IsBind;
						itemInfo.ValidDate = item6.ValidDate;
						itemInfo.StrengthenLevel = item6.StrengthLevel;
						itemInfo.AttackCompose = item6.AttackCompose;
						itemInfo.DefendCompose = item6.DefendCompose;
						itemInfo.AgilityCompose = item6.AgilityCompose;
						itemInfo.LuckCompose = item6.LuckCompose;
						itemInfo.canRenew = item6.canRenew;
						itemInfo.canTransfer = item6.canTransfer;
						list2.Add(itemInfo);
					}
				}
				if (list2.Count > 0)
				{
					WorldEventMgr.SendItemsToMails(list2, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.ZoneId, LanguageMgr.GetTranslation("SignBuffFightPower.Mail.Title", prop[num4]));
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				client.Out.SendSignBuffInfo(client.Player.Actives.Info);
			}
			else
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("SignBuffFightPower.Error"));
			}
			break;
		}
		case 21:
			switch (packet.ReadInt())
			{
			case 2:
			{
				bool flag2 = false;
				if (client.Player.Extra.PvePowerBuff.refreshCount <= 1)
				{
					flag2 = true;
				}
				else if (client.Player.RemoveMoney(GameProperties.BlessBuffRefreshPrice) > 0)
				{
					flag2 = true;
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ActivityPackage.PvePowerBuff.Msg1"));
				}
				if (flag2)
				{
					client.Player.Extra.PvePowerBuff.refreshCount++;
					client.Player.Extra.RefershPvePowerData();
					client.Out.SendPvePowerBuffRefresh(client.Player.Extra.PvePowerBuff);
				}
				break;
			}
			case 3:
			{
				int num2 = packet.ReadInt();
				bool flag = false;
				if (client.Player.Extra.PvePowerBuff.ListIndexBuff.Count <= num2)
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ActivityPackage.PvePowerBuff.Msg2"));
					return 0;
				}
				if (client.Player.Extra.PvePowerBuff.getBuffCount == 0)
				{
					flag = true;
				}
				else if (client.Player.RemoveMoney(GameProperties.BlessBuffAddBuffPrice) > 0)
				{
					flag = true;
				}
				else
				{
					client.Player.SendMessage(LanguageMgr.GetTranslation("GameServer.ActivityPackage.PvePowerBuff.Msg1"));
				}
				if (flag)
				{
					client.Player.Extra.PvePowerBuff.getBuffCount++;
					client.Player.Extra.PvePowerBuff.getBuffIndex = num2;
					client.Player.Extra.PvePowerBuff.getBuffDate = DateTime.Now;
					UserPvePowerBuffTempInfo userPvePowerBuffTempInfo = client.Player.Extra.PvePowerBuff.ListIndexBuff[num2];
					client.Player.Extra.PvePowerBuff.getBuffAtk = userPvePowerBuffTempInfo.Attack;
					client.Player.Extra.PvePowerBuff.getBuffDef = userPvePowerBuffTempInfo.Defence;
					client.Player.Extra.PvePowerBuff.getBuffAgl = userPvePowerBuffTempInfo.Agility;
					client.Player.Extra.PvePowerBuff.getBuffLuck = userPvePowerBuffTempInfo.Luck;
					client.Player.Extra.PvePowerBuff.getBuffDmg = userPvePowerBuffTempInfo.Damage;
					client.Player.Extra.PvePowerBuff.getBuffAr = userPvePowerBuffTempInfo.Guard;
					client.Player.Extra.PvePowerBuff.getBuffHp = userPvePowerBuffTempInfo.HP;
					client.Player.Extra.PvePowerBuff.getBuffMAtk = userPvePowerBuffTempInfo.MagicAttack;
					client.Player.Extra.PvePowerBuff.getBuffMDef = userPvePowerBuffTempInfo.MagicDefence;
					client.Out.SendPvePowerGetBuff(client.Player.Extra.PvePowerBuff);
				}
				break;
			}
			}
			break;
		default:
			Console.WriteLine("ACTIVITY_PACKAGE not found Cmd {0}", num);
			break;
		}
		return 0;
	}

	private bool CanGetAward(int grace, int index)
	{
		switch (index)
		{
		case 0:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 1:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 2:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 3:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 4:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 5:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 6:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 7:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		case 8:
			if (m_gradeList[index] <= grace)
			{
				return true;
			}
			break;
		}
		return false;
	}
}
