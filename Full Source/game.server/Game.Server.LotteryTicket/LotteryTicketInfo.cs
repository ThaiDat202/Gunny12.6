using System;
using System.Collections.Generic;
using ProtoBuf;
using SqlDataProvider.Data;

namespace Game.Server.LotteryTicket;

[ProtoContract]
public class LotteryTicketInfo
{
	[ProtoMember(1)]
	public int PoolCountCurrent;

	[ProtoMember(2)]
	public string PoolWinner;

	[ProtoMember(3)]
	public int FirstCount;

	[ProtoMember(4)]
	public int FirstMoney;

	[ProtoMember(5)]
	public int SecondCount;

	[ProtoMember(6)]
	public int SecondMoney;

	[ProtoMember(7)]
	public int ThirdCount;

	[ProtoMember(8)]
	public int ThirdMoney;

	[ProtoMember(9)]
	public int FourCount;

	[ProtoMember(10)]
	public int FourMoney;

	[ProtoMember(12)]
	public bool CanBuyPool;

	[ProtoMember(13)]
	public DateTime LastTimeSendAward;

	[ProtoMember(14)]
	public string PlayerLastAward;

	[ProtoMember(15)]
	public Dictionary<int, UserLotteryTicketInfo> Players;

	public LotteryTicketStatus Status { get; internal set; }
}
