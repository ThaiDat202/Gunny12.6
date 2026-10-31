using System;
using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SimpleBomb : BombObject
{
	private Living m_owner;

	private BaseGame m_game;

	protected Tile m_shape;

	protected int m_radius;

	protected int m_petRadius;

	protected double m_power;

	protected List<BombAction> m_actions;

	protected List<BombAction> m_petActions;

	protected BombType m_type;

	protected bool m_controled;

	protected bool m_invoked;

	private float m_lifeTime;

	private BallInfo m_info;

	private bool m_bombed;

	private bool digMap;

	private int m_angle;

	private bool m_isPlatom;

	private int angle;

	public int KeepLifeCount { get; set; }

	public bool KeepLife { get; set; }

	public bool DigMap => digMap;

	public BallInfo BallInfo => m_info;

	public Living Owner => m_owner;

	public List<BombAction> PetActions => m_petActions;

	public List<BombAction> Actions => m_actions;

	public float LifeTime => m_lifeTime;

	public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled, bool invoked, int angle, bool isPlatom)
		: base(id, info.Mass, info.Weight, info.Wind, info.DragIndex)
	{
		m_owner = owner;
		m_game = game;
		m_info = info;
		m_shape = shape;
		m_type = type;
		m_power = info.Power;
		m_radius = info.Radii;
		m_controled = controled;
		m_invoked = invoked;
		m_bombed = false;
		m_petRadius = 80;
		m_lifeTime = 0f;
		m_angle = Math.Abs(angle);
		m_isPlatom = isPlatom;
		if (m_info.IsSpecial())
		{
			digMap = false;
		}
		else
		{
			digMap = true;
		}
	}

	public override void StartMoving()
	{
		base.StartMoving();
		m_actions = new List<BombAction>();
		m_petActions = new List<BombAction>();
		int lifeTime = m_game.LifeTime;
		while (m_isMoving && m_isLiving)
		{
			m_lifeTime += 0.04f;
			Point point = CompleteNextMovePoint(0.04f);
			MoveTo(point.X, point.Y);
			if (m_isLiving)
			{
				if (Math.Round(m_lifeTime * 100f) % 40.0 == 0.0 && point.Y > 0)
				{
					m_game.AddTempPoint(point.X, point.Y);
				}
				if (base.vY > 0f && m_invoked)
				{
					Living living = m_map.FindinvokedEnemy(m_x, m_y, 150000.0, m_owner);
					if (living != null)
					{
						Point point2;
						if (living is SimpleBoss)
						{
							Rectangle directDemageRect = living.GetDirectDemageRect();
							point2 = new Point(directDemageRect.X - m_x, directDemageRect.Y - m_y);
						}
						else
						{
							point2 = new Point(living.X - m_x, living.Y - m_y);
						}
						point2 = point2.Normalize(1000);
						setSpeedXY(point2.X, point2.Y);
						m_controled = false;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_SPEED, point2.X, point2.Y, 0, 0));
					}
				}
				else if (m_controled && base.vY > 0f)
				{
					Living living2 = m_map.FindNearestEnemy(m_x, m_y, 150.0, m_owner);
					if (living2 != null)
					{
						Point point3;
						if (living2 is SimpleBoss)
						{
							Rectangle directDemageRect2 = living2.GetDirectDemageRect();
							point3 = new Point(directDemageRect2.X - m_x, directDemageRect2.Y - m_y);
						}
						else
						{
							point3 = new Point(living2.X - m_x, living2.Y - m_y);
						}
						point3 = point3.Normalize(1000);
						setSpeedXY(point3.X, point3.Y);
						m_controled = false;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_SPEED, point3.X, point3.Y, 0, 0));
					}
				}
			}
			if (m_bombed)
			{
				m_bombed = false;
				BombImp();
			}
		}
	}

	protected override void CollideObjects(Physics[] list)
	{
		foreach (Physics physics in list)
		{
			physics.CollidedByObject(this);
			m_actions.Add(new BombAction(m_lifeTime, ActionType.PICK, physics.Id, 0, 0, 0));
		}
	}

	protected override void CollideGround()
	{
		base.CollideGround();
		Bomb();
	}

	public void Bomb()
	{
		StopMoving();
		m_isLiving = false;
		m_bombed = true;
		m_owner.Game.SendFightStatus(m_owner, 0);
	}

	private void BombImp()
	{
		Point collidePoint = GetCollidePoint();
		List<Living> list = m_map.FindHitByHitPiont(collidePoint, m_radius);
		foreach (Living item in list)
		{
			if (item.IsNoHole || item.NoHoleTurn)
			{
				item.NoHoleTurn = true;
				if (!m_info.IsSpecial())
				{
					digMap = false;
				}
			}
			item.SyncAtTime = false;
		}
		m_owner.SyncAtTime = false;
		m_owner.LastPoint = collidePoint;
		try
		{
			if (digMap && !m_isPlatom)
			{
				m_map.Dig(m_x, m_y, m_shape, null);
			}
			m_actions.Add(new BombAction(m_lifeTime, ActionType.BOMB, m_x, m_y, digMap ? 1 : 0, 0));
			switch (m_type)
			{
			case BombType.FORZEN:
				foreach (Living item2 in list)
				{
					if (item2.Config.DamageForzen)
					{
						item2.Properties2 = Convert.ToInt32(item2.Properties2) - 1;
						if (Convert.ToInt32(item2.Properties2) <= 0 && item2 is SimpleNpc)
						{
							((SimpleNpc)item2).OnDie();
						}
					}
					if (m_owner is SimpleBoss && new IceFronzeEffect(100).Start(item2))
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, item2.Id, 0, 0, 0));
					}
					else if (m_owner is Player && !(item2 is Player) && item2.Config.DamageForzen)
					{
						item2.Properties2 = (int)item2.Properties2 - 1;
						if ((int)item2.Properties2 <= 0)
						{
							item2.PlayMovie("die", (int)Math.Round((m_lifeTime + 1f) * 1000f), 0);
							item2.Die((int)Math.Round((m_lifeTime + 1f) * 1000f));
						}
						else
						{
							item2.PlayMovie("cry", (int)Math.Round((m_lifeTime + 1f) * 1000f), 0);
						}
					}
					else if (item2 is SimpleNpc || item2 is Player || (item2 is SimpleBoss && item2.Config.IsHelper) || (item2 is SimpleBoss && item2.Config.CanFrost))
					{
						if (item2 is SimpleNpc && item2.Config.CanFrost)
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item2.Id, 0, 0, 4));
							if (item2.CanFrozen)
							{
								item2.IsFrost = true;
							}
						}
						if (item2 is SimpleBoss && item2.Config.CanFrost)
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
							m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_STATE, item2.Id, 0, 0, 0));
						}
						else if (new IceFronzeEffect(3).Start(item2))
						{
							if (item2 is SimpleBoss && !item2.Config.IsHelper)
							{
								m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item2.Id, 0, 0, 3));
							}
							else
							{
								m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, item2.Id, 0, 0, 0));
							}
						}
						else
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
							m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_STATE, item2.Id, 0, 0, 0));
						}
					}
					if (item2 is SimpleBoss || item2 is SimpleNpc)
					{
						Point p = new Point(X, Y);
						double num11 = item2.Distance(p);
						if (num11 < (double)m_radius)
						{
							m_game.AddAction(new LivingAfterShootedFrozen(item2, (int)((m_lifeTime + 1f) * 1000f)));
						}
					}
				}
				break;
			case BombType.FLY:
				if (m_y <= 10 || !(m_lifeTime > 0.04f))
				{
					break;
				}
				if (!m_map.IsEmpty(m_x, m_y))
				{
					PointF point = new PointF(0f - base.vX, 0f - base.vY);
					point = point.Normalize(5f);
					m_x -= (int)point.X;
					m_y -= (int)point.Y;
				}
				m_owner.SetXY(m_x, m_y);
				m_owner.StartMoving();
				m_actions.Add(new BombAction(m_lifeTime, ActionType.TRANSLATE, m_x, m_y, 0, 0));
				m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, m_owner.Id, m_owner.X, m_owner.Y, m_owner.IsLiving ? 1 : 0));
				foreach (Living item3 in list)
				{
					if (item3 is SimpleNpc && item3.Config.IsHelper && collidePoint.X < item3.X + 25 && collidePoint.X > item3.X - 25)
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.WORLDCUP, m_owner.Id, 0, 0, 0));
					}
				}
				(m_owner as Player).OnPlayerAnyShellThrow();
				break;
			case BombType.CURE:
				foreach (Living item4 in list)
				{
					double num4 = 0.0;
					int num5 = FindCureCount(list);
					num4 = ((num5 <= 1) ? 1.0 : ((num5 >= 4) ? 0.4 : ((double)num5 * 0.1)));
					int num6 = 0;
					if (m_info.ID == 10009)
					{
						int num7 = (int)Math.Round(m_lifeTime);
						num6 = m_owner.PetEffects.AddBloodPercent * item4.MaxBlood / 100;
						if (num7 > 1)
						{
							num6 *= num7;
						}
						if (m_owner.Team == item4.Team)
						{
							item4.AddBlood(num6);
							((Player)item4).TotalCure += num6;
							m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item4.Id, item4.Blood, num6, 0));
						}
						else
						{
							item4.AddBlood(-num6, 1);
							m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, item4.Id, num6, 1, item4.Blood));
						}
						continue;
					}
					int num8 = (int)((double)((Player)m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, ((Player)m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * num4);
					if (!item4.ForbidAidBlood)
					{
						num6 = num8;
						num6 += m_owner.FightBuffers.ConsortionAddBloodGunCount;
						num6 += m_owner.PetEffects.IncreaseAngelicPoint;
						num6 += (int)(((Player)m_owner).SubWeaponAddOrReduce / 1000.0 * (double)item4.MaxBlood);
						num6 += num6 * m_owner.AddCureValue / 100;
					}
					else
					{
						num6 = (num8 = 0);
					}
					if (item4 is Player)
					{
						item4.AddBlood(num6);
						((Player)item4).TotalCure += num6;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item4.Id, item4.Blood, num6, 0));
					}
					if ((item4 is SimpleBoss || item4 is SimpleNpc) && item4.Config.IsHelper)
					{
						item4.AddBlood(num6);
						((SimpleBoss)item4).TotalCure += num6;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item4.Id, item4.Blood, num6, 0));
					}
				}
				if (m_info.ID != 10009)
				{
					(m_owner as Player).OnPlayerShootCure();
				}
				(m_owner as Player).OnPlayerAnyShellThrow();
				break;
			case BombType.WORLDCUP:
				switch (m_info.ID)
				{
				case 110:
				{
					bool flag = false;
					foreach (Living item5 in list)
					{
						if (item5 is SimpleNpc && item5.Config.IsGoal)
						{
							((PVEGame)m_game).IsGoal = true;
							m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, item5.Id, 0, 0, item5.Blood));
							flag = true;
							break;
						}
						if (item5 is Player && ((PVEGame)m_game).PassBallActive)
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.WORLDCUP, item5.Id, 0, 0, 0));
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.WORLDCUP, 0, collidePoint.X, collidePoint.X, 0));
					}
					break;
				}
				case 117:
					foreach (Living item6 in list)
					{
						double num9 = 0.0;
						num9 = ((!m_map.FindPlayers(GetCollidePoint(), m_radius)) ? 1.0 : 0.3);
						int num10 = (int)(30.0 * num9);
						if (item6 is Player)
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item6.Id, item6.Blood, num10, 0));
							((Player)item6).TotalCureEnergy += num10;
							item6.AddRemoveEnergy(num10);
						}
					}
					break;
				}
				break;
			case BombType.CATCHINSECT:
				switch (m_info.ID)
				{
				case 128:
					foreach (Living item7 in list)
					{
						if (item7 is SimpleBoss && item7.Config.IsInsectBoss)
						{
							(m_game as PVEGame).OnTakeDamage();
						}
					}
					break;
				case 129:
				{
					int num12 = 0;
					int num13 = 0;
					int num14 = 0;
					int num15 = 0;
					string text = "";
					string text2 = "";
					string text3 = "";
					int num16 = (int)(m_lifeTime * 2500f);
					foreach (Living item8 in list)
					{
						if (item8 is SimpleNpc)
						{
							switch ((item8 as SimpleNpc).NpcInfo.ID)
							{
							case 71006:
								num12 += 10;
								num13++;
								text = (item8 as SimpleNpc).NpcInfo.Name;
								(item8 as SimpleNpc).PlayMovie("dieB", num16, num16);
								break;
							case 71007:
								num12 += 20;
								num14++;
								text2 = (item8 as SimpleNpc).NpcInfo.Name;
								(item8 as SimpleNpc).PlayMovie("dieA", num16, num16);
								break;
							case 71008:
								num12 += 30;
								num15++;
								text3 = (item8 as SimpleNpc).NpcInfo.Name;
								(item8 as SimpleNpc).PlayMovie("dieB", num16, num16);
								break;
							}
							item8.Die();
						}
					}
					if (m_owner is Player)
					{
						string text4 = LanguageMgr.GetTranslation("CatchInsect.AddSmallScore") + " ";
						if (num13 > 0)
						{
							text4 = text4 + num13 + " " + text + " ";
						}
						if (num14 > 0)
						{
							text4 = text4 + num14 + " " + text2 + " ";
						}
						if (num15 > 0)
						{
							text4 = text4 + num15 + " " + text3 + " ";
						}
						if (num13 + num14 + num15 > 0)
						{
							(m_owner as Player).PlayerDetail.SendMessage(text4);
						}
						(m_owner as Player).PlayerDetail.AddSummerScore(num12);
					}
					break;
				}
				}
				break;
			default:
			{
				int num = 0;
				int num2 = 0;
				foreach (Living item9 in list)
				{
					if (m_owner.IsFriendly(item9) || item9.Config.IsDown || (!(item9 is Player) && !item9.Config.CanTakeDamage))
					{
						continue;
					}
					if (m_owner.ClearBuff)
					{
						item9.EffectList.StopAllEffect();
						continue;
					}
					if ((item9 is SimpleBoss || item9 is SimpleNpc) && (item9.Config.IsShield || item9.Config.IsGoal))
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item9.Id, 0, 0, 2));
						continue;
					}
					num = MakeDamage(item9);
					num2 = 0;
					if (num != 0)
					{
						if (m_game.RoomType == eRoomType.FightFootballTime && item9 is SimpleNpc)
						{
							num = GetFightFootballPoint((item9 as SimpleNpc).NpcInfo.ID);
							if (m_owner.Team == 1)
							{
								m_game.blueScore += num;
							}
							else
							{
								m_game.redScore += num;
							}
							m_owner.ScoreArr.Add(num);
						}
						else
						{
							num2 = m_owner.MakeCriticalDamage(item9, num);
						}
						m_owner.OnTakedDamage(m_owner, ref num, ref num2);
						m_owner.OnMakeDamage(m_owner, item9, ref num, ref num2);
						if (item9.TakeDamage(m_owner, ref num, ref num2, "Fire"))
						{
							if ((item9 is SimpleBoss || item9 is SimpleNpc) && item9.Config.KeepLife && item9.Blood == 1)
							{
								m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item9.Id, 0, 0, 2));
							}
							else
							{
								m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, item9.Id, num + num2, (num2 == 0) ? 1 : 2, item9.Blood));
							}
						}
						else
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.UNFORZEN, item9.Id, 0, 0, 0));
						}
						if (m_owner is Player && item9 is SimpleBoss)
						{
							m_owner.TotalDameLiving += num2 + num;
						}
						else if (m_owner is Player && item9 is Player)
						{
							m_owner.TotalDamagePlayer += num2 + num;
						}
						if (item9 is Player)
						{
							int num3 = ((Player)item9).Dander;
							if (m_owner.FightBuffers.ConsortionReduceDander > 0)
							{
								num3 -= num3 * m_owner.FightBuffers.ConsortionReduceDander / 100;
								((Player)item9).Dander = num3;
							}
							m_actions.Add(new BombAction(m_lifeTime, ActionType.DANDER, item9.Id, num3, 0, 0));
						}
						if ((item9 is SimpleBoss || item9 is SimpleNpc) && m_game.RoomType != eRoomType.FightFootballTime && item9.DoAction > -1)
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item9.Id, 0, 0, item9.DoAction));
						}
					}
					else if (item9 is SimpleBoss || item9 is SimpleNpc)
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item9.Id, 0, 0, 0));
					}
					if (item9.IsLiving)
					{
						item9.StartMoving((int)((m_lifeTime + 1f) * 1000f), 12);
						m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, item9.Id, item9.X, item9.Y, item9.IsLiving ? 1 : 0));
						if (m_owner.BombFoul && item9 != m_owner)
						{
							new ContinueReduceBlood(3, 5000, m_owner).Start(item9);
						}
						if (m_owner.TiredShoot && item9 != m_owner)
						{
							new ReduceStrengthEffect(3, 100).Start(item9);
						}
						if (m_owner.LockMove && item9 != m_owner)
						{
							new LockDirectionEffect(2).Start(item9);
						}
					}
				}
				List<Living> list2 = m_map.FindHitByHitPiont(collidePoint, m_petRadius);
				if (m_game.AreaPK() || !(m_owner is Player) || ((Player)m_owner).ShootCount != 1 || m_owner.PetEffects.DamagePercent == 0)
				{
					break;
				}
				if (m_game.RoomType == eRoomType.FightFootballTime)
				{
					list2 = new List<Living>();
				}
				if (list2.Count == 0)
				{
					m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, -1, 0, 0, 0));
					break;
				}
				foreach (Living item10 in list2)
				{
					if (item10.Config.IsDown || item10.Config.IsGoal || item10.Config.IsShield || (item10.Config.KeepLife && item10.Blood == 1) || (!(item10 is Player) && !item10.Config.CanTakeDamage))
					{
						m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, -1, 0, 0, 0));
					}
					else
					{
						if (item10 == m_owner)
						{
							continue;
						}
						num = MakeDamage(item10, isPet: true);
						if (num > 0)
						{
							num = num * m_owner.PetEffects.DamagePercent / 100;
							num2 = 0;
							if (m_owner is Player)
							{
								m_owner.OnTakedPetDamage(m_owner, ref num, ref num2);
							}
							if (item10.PetTakeDamage(m_owner, ref num, ref num2, "PetFire"))
							{
								if (item10 is Player)
								{
									m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, item10.Id, num + num2, ((Player)item10).Dander, item10.Blood));
								}
								else
								{
									m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, item10.Id, num + num2, 0, item10.Blood));
								}
							}
						}
						else
						{
							m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, -1, 0, 0, 0));
						}
					}
				}
				break;
			}
			}
			Die();
		}
		finally
		{
			m_owner.SyncAtTime = true;
			foreach (Living item11 in list)
			{
				item11.SyncAtTime = true;
			}
		}
	}

	public int FindCureCount(List<Living> livs)
	{
		int num = 0;
		foreach (Living liv in livs)
		{
			if (liv is Player || liv.Config.IsHelper)
			{
				num++;
			}
		}
		return num;
	}

	protected int MakeDamage(Living target, bool isPet = false)
	{
		if (target.Config.IsInsectBoss)
		{
			return 1;
		}
		if (target.Config.IsChristmasBoss)
		{
			return 1;
		}
		if (m_game.RoomType == eRoomType.FightFootballTime && target is Player)
		{
			return 1;
		}
		double num = m_owner.BaseDamage;
		double baseGuard = target.BaseGuard;
		baseGuard += target.BaseGuard * (10000.0 * m_owner.SunderArmor / (m_owner.SunderArmor + 2500.0) * 0.75 / 100.0) / 100.0;
		baseGuard -= target.BaseGuard * (10000.0 * target.AvoidInjury / (target.AvoidInjury + 2500.0) * 0.75 / 100.0) / 100.0;
		double num2 = target.Defence;
		double attack = m_owner.Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)target.getHertAddition((target as Player).DeputyWeapon);
			baseGuard += (double)num3;
			num2 += (double)num3;
		}
		if (m_owner.IgnoreArmor)
		{
			baseGuard *= 0.6;
			num2 *= 0.6;
		}
		if (m_owner.AddDamage > 0)
		{
			num += num / 100.0 * (double)m_owner.AddDamage;
		}
		if (m_owner.IgnoreGuard > 0 && !m_owner.IgnoreArmor)
		{
			num2 -= num2 / 100.0 * (double)m_owner.IgnoreGuard;
			baseGuard -= baseGuard / 100.0 * (double)m_owner.IgnoreGuard;
		}
		float currentDamagePlus = m_owner.CurrentDamagePlus;
		float currentShootMinus = m_owner.CurrentShootMinus;
		currentShootMinus += m_owner.CurrentShootMinus * (float)(10000.0 * m_owner.KillPow / (m_owner.KillPow + 2500.0) * 0.75 / 100.0) / 100f;
		currentShootMinus -= m_owner.CurrentShootMinus * (float)(10000.0 * target.WillFight / (target.WillFight + 2500.0) * 0.75 / 100.0) / 100f;
		double num4 = 0.95 * (baseGuard - (double)(3 * m_owner.Grade)) / (500.0 + baseGuard - (double)(3 * m_owner.Grade));
		double num5 = ((num2 - m_owner.Lucky >= 0.0) ? (0.95 * (num2 - m_owner.Lucky) / (600.0 + num2 - m_owner.Lucky)) : 0.0);
		double num6 = (double)m_owner.FightBuffers.WorldBossAddDamage * (1.0 - (baseGuard / 200.0 + num2 * 0.009));
		double num7 = (num6 + num * (1.0 + attack * 0.002) * (1.0 - (num4 + num5 - num4 * num5))) * (double)currentDamagePlus * (double)currentShootMinus;
		Point p = new Point(X, Y);
		double num8 = target.Distance(p);
		if (num8 < (double)m_radius)
		{
			num7 *= 1.0 - num8 / (double)m_radius / 4.0;
			if (m_owner is Player)
			{
				num7 += num7 * (double)(m_owner as Player).PlayerDetail.PlayerCharacter.GoldenAddAttack / 100.0;
			}
			if (m_owner is Player && target is Player && target != m_owner && target != m_owner)
			{
				int num9 = 0;
				if ((m_owner.Direction == 1 && angle >= 91) || (m_owner.Direction == -1 && angle <= 89))
				{
					m_game.AddAction(new FightAchievementAction(m_owner, eAcrobaciaType.ImperadorDejogarDeVolta, m_owner.IdAcrobacias++, 1200));
				}
				if (m_owner.lastShots.ContainsKey(target.Id))
				{
					m_owner.lastShots[target.Id]++;
				}
				else
				{
					m_owner.lastShots.Add(target.Id, 1);
				}
				if (m_owner.Prop1 >= 1 && m_owner.Prop3 >= 1 && m_owner.lastShots.ContainsKey(target.Id) && m_owner.lastShots[target.Id] >= 9)
				{
					m_game.AddAction(new FightAchievementAction(m_owner, eAcrobaciaType.DeusDaPrecisao, m_owner.IdAcrobacias++, 1200));
				}
				foreach (int key in m_owner.lastShots.Keys)
				{
					if (m_owner.lastShots[key] >= 1)
					{
						num9++;
					}
				}
				if (num9 >= 2)
				{
					m_game.AddAction(new FightAchievementAction(m_owner, eAcrobaciaType.MestreDeAcrobacia, m_owner.IdAcrobacias++, 1200));
				}
			}
			if (num7 < 0.0)
			{
				return 1;
			}
		}
		else if (!isPet)
		{
			return 0;
		}
		double num10 = m_owner.MagicAttack * 0.8 - target.MagicDefence * 0.7;
		if (num10 <= 0.0)
		{
			num10 = 0.0;
		}
		double num11 = m_owner.FireAttack * 0.6 - target.FireDefence * 0.3;
		double num12 = m_owner.WaterAttack * 0.6 - target.WaterDefence * 0.3;
		double num13 = m_owner.WindAttack * 0.6 - target.WindDefence * 0.3;
		double num14 = m_owner.LandAttack * 0.6 - target.LandDefence * 0.3;
		double num15 = num11 + num12 + num13 + num14;
		if (num15 <= 0.0)
		{
			num15 = 0.0;
		}
		num7 += num10;
		num7 += num15;
		if (target.KeepLife)
		{
			return 0;
		}
		if (num7 <= 0.0)
		{
			return 1;
		}
		return (int)num7;
	}

	protected int GetFightFootballPoint(int livingID)
	{
		return livingID switch
		{
			10005 => 1, 
			10006 => 2, 
			10007 => 3, 
			10008 => 4, 
			10009 => 5, 
			_ => 1, 
		};
	}

	protected override void FlyoutMap()
	{
		m_actions.Add(new BombAction(m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
		base.FlyoutMap();
	}

	internal void setAngle(int angle)
	{
		this.angle = Math.Abs(Convert.ToInt32(Math.Atan2(Math.Sin((double)angle * Math.PI / 180.0), Math.Cos((double)angle * Math.PI / 180.0)) * 180.0 / Math.PI));
	}
}
