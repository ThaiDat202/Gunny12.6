using System;
using System.Collections;
using System.Reflection;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.RingStation.Action;
using Game.Server.RingStation.RoomGamePkg;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.RingStation;

public class VirtualGamePlayer
{
	private bool _canUserProp = true;

	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public BaseRoomRingStation CurRoom;

	private ArrayList m_actions = new ArrayList();

	private long m_passTick;

	private RoomGame seflRoom = new RoomGame();

	private int _ID;

	private long _fightPower;

	public int Agility { get; set; }

	public double AntiAddictionRate { get; set; }

	public int Speed { get; set; }

	public int virtualyuansu { get; set; }

	public int MagicAttack { get; set; }

	public int MagicDefence { get; set; }

	public double FireAttack { get; set; }

	public double FireDefence { get; set; }

	public double WaterAttack { get; set; }

	public double WaterDefence { get; set; }

	public double LandAttack { get; set; }

	public double LandDefence { get; set; }

	public double WindAttack { get; set; }

	public double WindDefence { get; set; }

	public int Attack { get; set; }

	public float AuncherExperienceRate { get; set; }

	public float AuncherOfferRate { get; set; }

	public float AuncherRichesRate { get; set; }

	public int badgeID { get; set; }

	public double BaseAgility { get; set; }

	public double BaseAttack { get; set; }

	public double BaseBlood { get; set; }

	public double BaseDefence { get; set; }

	public bool CanUserProp
	{
		get
		{
			return _canUserProp;
		}
		set
		{
			_canUserProp = value;
		}
	}

	public string Colors { get; set; }

	public int ConsortiaID { get; set; }

	public int ConsortiaLevel { get; set; }

	public string ConsortiaName { get; set; }

	public int ConsortiaRepute { get; set; }

	public int Dander { get; set; }

	public int Defence { get; set; }

	public int Direction { get; set; }

	public long FightPower
	{
		get
		{
			if (_fightPower >= int.MaxValue)
			{
				return 2147483647L;
			}
			return _fightPower;
		}
		set
		{
			_fightPower = value;
		}
	}

	public bool FirtDirection { get; set; }

	public int GamePlayerId
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
		}
	}

	public float GMExperienceRate { get; set; }

	public float GMOfferRate { get; set; }

	public float GMRichesRate { get; set; }

	public int GP { get; set; }

	public double GPAddPlus { get; set; }

	public int Grade { get; set; }

	public int Healstone { get; set; }

	public int HealstoneCount { get; set; }

	public int Hide { get; set; }

	public string Honor { get; set; }

	public int hp { get; set; }

	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
		}
	}

	public int LastAngle { get; set; }

	public int LastForce { get; set; }

	public int LastX { get; set; }

	public int LastY { get; set; }

	public int Luck { get; set; }

	public int LX { get; set; }

	public int LY { get; set; }

	public string NickName { get; set; }

	public int Nimbus { get; set; }

	public int Offer { get; set; }

	public double OfferAddPlus { get; set; }

	public int Repute { get; set; }

	public int SecondWeapon { get; set; }

	public bool Sex { get; set; }

	public int ShootCount { get; set; }

	public string Skin { get; set; }

	public string Style { get; set; }

	public int StrengthLevel { get; set; }

	public int Team { get; set; }

	public int TemplateID { get; set; }

	public int Total { get; set; }

	public byte typeVIP { get; set; }

	public int VIPLevel { get; set; }

	public string WeaklessGuildProgressStr { get; set; }

	public int Win { get; set; }

	public int X { get; set; }

	public int Y { get; set; }

	public bool IsNoHole { get; internal set; }

	public void AddAction(IAction action)
	{
		lock (m_actions)
		{
			m_actions.Add(action);
		}
	}

	public void AddAction(ArrayList action)
	{
		lock (m_actions)
		{
			m_actions.AddRange(action);
		}
	}

	public void AddTurn(GSPacketIn pkg)
	{
		if (pkg.Parameter1 == GamePlayerId)
		{
			m_actions.Add(new PlayerShotAction(LastX, LastY - 25, LastForce, LastAngle, 0));
		}
	}

	public static double ComputeVx(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.8;
	}

	public static double ComputeVy(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
	}

	public void NextTurn(GSPacketIn pkg)
	{
		SendSelfTurn(pkg.Parameter1 == GamePlayerId, useBuff: true);
	}

	public void Pause(int time)
	{
		m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
	}

	internal void ProcessPacket(GSPacketIn pkg)
	{
		if (seflRoom != null)
		{
			seflRoom.ProcessData(this, pkg);
		}
	}

	public void Resume()
	{
		m_passTick = 0L;
	}

	public void SendCreateGame(GSPacketIn pkg)
	{
		ShootCount = 100;
		FirtDirection = true;
		Direction = -1;
	}

	private void SendDirection(int Direction)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(Direction);
		SendTCP(gSPacketIn);
	}

	public void SendGameCMDShoot(int x, int y, int force, int angle)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		gSPacketIn.WriteInt(force);
		gSPacketIn.WriteInt(angle);
		SendTCP(gSPacketIn);
	}

	public void sendGameCMDStunt(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(15);
		gSPacketIn.WriteInt(type);
		SendTCP(gSPacketIn);
	}

	public void SendLoadingComplete(int state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = GamePlayerId;
		gSPacketIn.WriteByte(16);
		gSPacketIn.WriteInt(state);
		SendTCP(gSPacketIn);
	}

	private void SendSelfTurn(bool fire)
	{
		SendSelfTurn(fire, useBuff: false);
	}

	private void SendSelfTurn(bool fire, bool useBuff)
	{
		if (fire)
		{
			FindTarget();
		}
	}

	public void FindTarget()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(143);
		SendTCP(gSPacketIn);
	}

	public void SendShootTag(bool b, int time)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(96);
		gSPacketIn.WriteBoolean(b);
		gSPacketIn.WriteByte((byte)time);
		SendTCP(gSPacketIn);
	}

	private void SendSkipNext()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(12);
		gSPacketIn.WriteByte(100);
		SendTCP(gSPacketIn);
	}

	internal void SendTCP(GSPacketIn pkg)
	{
		CurRoom.SendTCP(pkg);
	}

	public void SendUseProp(int templateId)
	{
		SendUseProp(templateId, 0, 0, 0, 0);
	}

	public void SendUseProp(int templateId, int x, int y, int force, int angle)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91)
		{
			Parameter1 = GamePlayerId
		};
		gSPacketIn.WriteByte(32);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(-1);
		gSPacketIn.WriteInt(templateId);
		SendTCP(gSPacketIn);
		if (templateId == 10001 || templateId == 10002)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
			for (int i = 0; i < itemTemplateInfo.Property2; i++)
			{
				SendGameCMDShoot(x, y, force, angle);
			}
		}
	}

	public void Update(long tick)
	{
		if (m_passTick >= tick)
		{
			return;
		}
		ArrayList arrayList;
		lock (m_actions)
		{
			arrayList = (ArrayList)m_actions.Clone();
			m_actions.Clear();
		}
		if (arrayList == null || seflRoom == null || arrayList.Count <= 0)
		{
			return;
		}
		ArrayList arrayList2 = new ArrayList();
		foreach (IAction item in arrayList)
		{
			try
			{
				item.Execute(this, tick);
				if (!item.IsFinished(tick))
				{
					arrayList2.Add(item);
				}
			}
			catch (Exception exception)
			{
				log.Error("Bot action error:", exception);
			}
		}
		AddAction(arrayList2);
	}
}
