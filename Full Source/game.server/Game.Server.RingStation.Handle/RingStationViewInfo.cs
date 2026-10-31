using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.RingStation.Handle;

[RingStationHandleAttbute(1)]
public class RingStationViewInfo : IRingStationCommandHadler
{
	public bool CommandHandler(GamePlayer Player, GSPacketIn packet)
	{
		RingStationMgr.LoadRingStationInfo(Player.PlayerCharacter, (int)Player.BaseAttack, (int)Player.BaseDefence);
		GSPacketIn gSPacketIn = new GSPacketIn(404, Player.PlayerId);
		UserRingStationInfo singleRingStationInfos = RingStationMgr.GetSingleRingStationInfos(Player.PlayerId);
		if (singleRingStationInfos.LastDate.Date < DateTime.Now.Date)
		{
			singleRingStationInfos.LastDate = DateTime.Now;
			singleRingStationInfos.ChallengeNum = RingStationMgr.ConfigInfo.ChallengeNum;
			singleRingStationInfos.buyCount = RingStationMgr.ConfigInfo.buyCount;
			RingStationMgr.UpdateRingStationInfo(singleRingStationInfos);
		}
		RingstationConfigInfo configInfo = RingStationMgr.ConfigInfo;
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(singleRingStationInfos.Rank);
		gSPacketIn.WriteInt(singleRingStationInfos.ChallengeNum);
		gSPacketIn.WriteInt(singleRingStationInfos.buyCount);
		gSPacketIn.WriteInt(configInfo.buyPrice);
		gSPacketIn.WriteDateTime(singleRingStationInfos.ChallengeTime);
		gSPacketIn.WriteInt(configInfo.cdPrice);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(singleRingStationInfos.signMsg);
		gSPacketIn.WriteInt(configInfo.AwardNumByRank(singleRingStationInfos.Rank));
		gSPacketIn.WriteDateTime(configInfo.AwardTime);
		gSPacketIn.WriteString(configInfo.ChampionText);
		gSPacketIn.WriteBoolean(Player.PlayerCharacter.isAttest);
		gSPacketIn.WriteBoolean(singleRingStationInfos.ReardEnable);
		UserRingStationInfo[] ringStationInfos = RingStationMgr.GetRingStationInfos(singleRingStationInfos.UserID, singleRingStationInfos.Rank);
		gSPacketIn.WriteInt(ringStationInfos.Length);
		UserRingStationInfo[] array = ringStationInfos;
		foreach (UserRingStationInfo userRingStationInfo in array)
		{
			PlayerInfo info = userRingStationInfo.Info;
			gSPacketIn.WriteInt(info.ID);
			gSPacketIn.WriteString(info.UserName);
			gSPacketIn.WriteString(info.NickName);
			gSPacketIn.WriteByte(info.typeVIP);
			gSPacketIn.WriteInt(info.VIPLevel);
			gSPacketIn.WriteInt(info.Grade);
			if (GameProperties.VERSION >= 8200)
			{
				gSPacketIn.WriteInt(info.ddtKingGrade);
			}
			gSPacketIn.WriteBoolean(info.Sex);
			gSPacketIn.WriteString(info.Style);
			gSPacketIn.WriteString(info.Colors);
			gSPacketIn.WriteString(info.Skin);
			gSPacketIn.WriteString(info.ConsortiaName);
			gSPacketIn.WriteInt(info.Hide);
			gSPacketIn.WriteInt(info.Offer);
			gSPacketIn.WriteInt(info.Win);
			gSPacketIn.WriteInt(info.Total);
			gSPacketIn.WriteInt(info.Escape);
			gSPacketIn.WriteInt(info.Repute);
			gSPacketIn.WriteInt(info.Nimbus);
			gSPacketIn.WriteInt((info.GP > int.MaxValue) ? int.MaxValue : Convert.ToInt32(info.GP));
			gSPacketIn.WriteInt(info.FightPower);
			gSPacketIn.WriteInt(info.AchievementPoint);
			gSPacketIn.WriteInt(userRingStationInfo.Rank);
			gSPacketIn.WriteBoolean(info.isAttest);
			gSPacketIn.WriteInt(userRingStationInfo.WeaponID);
			gSPacketIn.WriteString(userRingStationInfo.signMsg);
		}
		Player.SendTCP(gSPacketIn);
		return true;
	}
}
