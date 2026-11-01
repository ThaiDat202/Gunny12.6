using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bussiness;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.GuardCoreEffects;
using Game.Logic.HorseEffects;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class Living : Physics
{
	protected BaseGame m_game;

	protected int m_maxBlood;

	protected int m_blood;

	private int m_team;

	private string m_name;

	private string m_action;

	private string m_modelId;

	private Rectangle m_demageRect;

	private int m_state;

	private int m_doAction;

	public int m_direction;

	private eLivingType m_type;

	public int IdAcrobacias;

	public bool IsAddTurn;

	private Random rand;

	public double BaseDamage = 10.0;

	public double BaseGuard = 10.0;

	public double Defence = 10.0;

	public double Attack = 10.0;

	public double Agility = 10.0;

	public double Lucky = 10.0;

	public double MagicAttack = 10.0;

	public double MagicDefence = 10.0;

	public double Crit = 0.0;

	public double Tenacity = 0.0;

	public double SunderArmor = 0.0;

	public double AvoidInjury = 0.0;

	public double KillPow = 0.0;

	public double WillFight = 0.0;

	public double ViolenceInjury = 0.0;

	public double Guard = 0.0;

	public double Speed = 0.0;

	public double FireAttack = 0.0;

	public double FireDefence = 0.0;

	public double WaterAttack = 0.0;

	public double WaterDefence = 0.0;

	public double LandAttack = 0.0;

	public double LandDefence = 0.0;

	public double WindAttack = 0.0;

	public double WindDefence = 0.0;

	public double LighAttack = 0.0;

	public double LighDefence = 0.0;

	public double DrakAttack = 0.0;

	public double DrakDefence = 0.0;

	public int Prop1;

	public int Prop2;

	public int Prop3;

	public int Prop4;

	public int Grade = 1;

	public int Experience = 10;

	public float CurrentDamagePlus;

	public float CurrentShootMinus;

	public bool IgnoreArmor;

	public int IgnoreGuard;

	public int AddDamage;

	public bool AddArmor;

	public bool ControlBall;

	public bool Invoked;

	public bool ForbidAidBlood;

	public bool walkenergy0;

	public bool EffectBadges;

	public int ReduceCritFisrtGem;

	public int ReduceCritSecondGem;

	public int EquipGhostReduceCritDamage;

	public int DefenFisrtGem;

	public int DefenSecondGem;

	public int DefendActiveGem;

	public int AttackGemLimit;

	public bool NoHoleTurn;

	public int AddBloodValue;

	public int AddCureValue;

	public bool CurrentIsHitTarget;

	public int FlyingPartical;

	public int TurnNum;

	public int TotalHurt;

	public int TotalDameLiving;

	public int TotalDamagePlayer;

	public int TotalHitTargetCount;

	public int TotalShootCount;

	public int TotalKill;

	public int MaxBeatDis;

	public int EffectsCount;

	public int ShootMovieDelay;

	public List<int> ScoreArr;

	public Point LastPoint;

	public bool PassBallFail;

	private GuardCoreEffectList m_guardCoreEffectList;

	private HorseEffectList m_horseEffectList;

	private PetEffectList m_petEffectList;

	private EffectList m_effectList;

	public bool EffectTrigger;

	public bool PetEffectTrigger;

	protected bool m_syncAtTime;

	public Dictionary<int, int> lastShots;

	public int totalCritical;

	public int totalShotTurn;

	private PetEffectInfo m_petEffects;

	private bool m_vaneOpen;

	private FightBufferInfo m_fightBufferInfo;

	public int countBoom;

	public bool TiredShoot;

	public bool ClearBuff;

	public bool LockMove;

	public bool BombFoul;

	public bool CanFrozen;

	private int m_targetId;

	private LivingConfig m_config;

	private int m_FallCount;

	private int m_FindCount;

	private bool m_isAttacking;

	protected static int MOVE_SPEED = 2;

	protected static int GHOST_MOVE_SPEED = 8;

	protected static int StepX = 1;

	protected static int StepY = 3;

	private int m_specialSkillDelay;

	public int LastLifeTimeShoot = 0;

	private bool m_isFrost;

	private bool m_isHide;

	private bool m_isNoHole;

	private bool m_isfire;

	private bool m_isSeal;

	private bool m_isBlockTurn;

	private bool m_isHoldBall;

	private int m_extendMaxBlood;

	public bool IsNPC;

	public double OldAttack { get; set; }

	public double OldDefence { get; set; }

	public double OldAgility { get; set; }

	public double OldLucky { get; set; }

	public double OldMagicAttack { get; set; }

	public double OldMagicDefende { get; set; }

	public double OldBaseDamage { get; set; }

	public double OldBaseGuard { get; set; }

	public bool FlagUsePetSkill { get; set; }

	public bool CanAddBlood { get; set; }

	public int FireX { get; set; }

	public int FireY { get; set; }

	public int targetId
	{
		get
		{
			return m_targetId;
		}
		set
		{
			m_targetId = value;
		}
	}

	public LivingConfig Config
	{
		get
		{
			return m_config;
		}
		set
		{
			m_config = value;
		}
	}

	public PetEffectInfo PetEffects
	{
		get
		{
			return m_petEffects;
		}
		set
		{
			m_petEffects = value;
		}
	}

	public FightBufferInfo FightBuffers
	{
		get
		{
			return m_fightBufferInfo;
		}
		set
		{
			m_fightBufferInfo = value;
		}
	}

	public bool VaneOpen
	{
		get
		{
			return m_vaneOpen;
		}
		set
		{
			m_vaneOpen = value;
		}
	}

	public string ActionStr
	{
		get
		{
			return m_action;
		}
		set
		{
			m_action = value;
		}
	}

	public BaseGame Game => m_game;

	public string Name => m_name;

	public string ModelId => m_modelId;

	public int Team => m_team;

	public bool SyncAtTime
	{
		get
		{
			return m_syncAtTime;
		}
		set
		{
			m_syncAtTime = value;
		}
	}

	public int FallCount
	{
		get
		{
			return m_FallCount;
		}
		set
		{
			m_FallCount = value;
		}
	}

	public int FindCount
	{
		get
		{
			return m_FindCount;
		}
		set
		{
			m_FindCount = value;
		}
	}

	public int Direction
	{
		get
		{
			return m_direction;
		}
		set
		{
			if (m_direction != value)
			{
				m_direction = value;
				SetRect(-m_rect.X - m_rect.Width, m_rect.Y, m_rect.Width, m_rect.Height);
				SetRectBomb(-m_rectBomb.X - m_rectBomb.Width, m_rectBomb.Y, m_rectBomb.Width, m_rectBomb.Height);
				SetRelateDemagemRect(-m_demageRect.X - m_demageRect.Width, m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateDirection(this);
				}
			}
		}
	}

	public eLivingType Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public bool IsSay { get; set; }

	public EffectList EffectList => m_effectList;

	public PetEffectList PetEffectList => m_petEffectList;

	public HorseEffectList HorseEffectList => m_horseEffectList;

	public GuardCoreEffectList GuardCoreEffectList => m_guardCoreEffectList;

	public bool IsAttacking => m_isAttacking;

	public int SpecialSkillDelay
	{
		get
		{
			return m_specialSkillDelay;
		}
		set
		{
			m_specialSkillDelay = value;
		}
	}

	public bool BlockTurn
	{
		get
		{
			return m_isBlockTurn;
		}
		set
		{
			m_isBlockTurn = value;
		}
	}

	public bool IsHoldBall => m_isHoldBall;

	public bool IsFrost
	{
		get
		{
			return m_isFrost;
		}
		set
		{
			if (m_isFrost != value)
			{
				m_isFrost = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateFrozenState(this);
				}
			}
		}
	}

	public bool IsNoHole
	{
		get
		{
			return m_isNoHole;
		}
		set
		{
			if (m_isNoHole != value)
			{
				m_isNoHole = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateNoHoleState(this);
				}
			}
		}
	}

	public bool Isfire
	{
		get
		{
			return m_isfire;
		}
		set
		{
			if (m_isfire != value)
			{
				m_isfire = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdatefireState(this);
				}
			}
		}
	}

	public bool IsHide
	{
		get
		{
			return m_isHide;
		}
		set
		{
			if (m_isHide != value)
			{
				m_isHide = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateHideState(this);
				}
			}
		}
	}

	public int State
	{
		get
		{
			return m_state;
		}
		set
		{
			if (m_state != value)
			{
				m_state = value;
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateAngryState(this);
				}
			}
		}
	}

	public int DoAction
	{
		get
		{
			return m_doAction;
		}
		set
		{
			if (m_doAction != value)
			{
				m_doAction = value;
			}
		}
	}

	public int MaxBlood
	{
		get
		{
			return m_maxBlood;
		}
		set
		{
			m_maxBlood = value;
		}
	}

	public int ExtendMaxBlood
	{
		get
		{
			return m_extendMaxBlood;
		}
		set
		{
			m_extendMaxBlood = value;
		}
	}

	public int Blood
	{
		get
		{
			return m_blood;
		}
		set
		{
			m_blood = value;
		}
	}

	public int KeepLifeCount { get; set; }

	public bool KeepLife { get; set; }

	public event LivingEventHandle PropUseItem;

	public event LivingEventHandle Died;

	public event LivingMakeDamageEventHandle BeforeMakeDamage;

	public event LivingMakeDamageEventHandle AfterMakeDamage;

	public event LivingTakedDamageEventHandle BeforeTakeDamage;

	public event LivingTakedDamageEventHandle TakePlayerDamage;

	public event LivingTakedDamageEventHandle OnMakeDamageEvent;

	public event LivingEventHandle BeginNextTurn;

	public event LivingEventHandle BeginSelfTurn;

	public event LivingEventHandle BeginAttacking;

	public event LivingEventHandle BeginAttacked;

	public event LivingEventHandle EndAttacking;

	public event LivingEventHandle BeginUseProp;

	public event LivingEventHandle GameStarted;

	public event KillLivingEventHanlde AfterKillingLiving;

	public event KillLivingEventHanlde AfterKilledByLiving;

	public event LivingTakedDamageEventHandle TakePetDamage;

	public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
		: base(id)
	{
		m_vaneOpen = false;
		m_action = "";
		m_game = game;
		m_team = team;
		m_name = name;
		m_modelId = modelId;
		m_maxBlood = maxBlood;
		m_direction = direction;
		m_state = 0;
		m_doAction = -1;
		MaxBeatDis = 100;
		AddArmor = false;
		ReduceCritFisrtGem = 0;
		ReduceCritSecondGem = 0;
		EquipGhostReduceCritDamage = 0;
		DefenFisrtGem = 0;
		DefenSecondGem = 0;
		DefendActiveGem = 0;
		AttackGemLimit = 0;
		m_effectList = new EffectList(this, immunity);
		m_petEffectList = new PetEffectList(this, immunity);
		m_horseEffectList = new HorseEffectList(this, immunity);
		m_fightBufferInfo = new FightBufferInfo();
		m_guardCoreEffectList = new GuardCoreEffectList(this, immunity);
		m_config = new LivingConfig();
		m_syncAtTime = true;
		m_type = eLivingType.Living;
		rand = new Random();
		ScoreArr = new List<int>();
		m_petEffects = new PetEffectInfo();
		lastShots = new Dictionary<int, int>();
	}

	public void SetRelateDemagemRect(int x, int y, int width, int height)
	{
		m_demageRect.X = x;
		m_demageRect.Y = y;
		m_demageRect.Width = width;
		m_demageRect.Height = height;
	}

	public void DemagemRect(int x, int y, int width, int height)
	{
		m_demageRect.Width = width;
		m_demageRect.Height = height;
	}

	public Point GetShootPoint()
	{
		if (this is SimpleBoss || this is SimpleNpc)
		{
			return (m_direction > 0) ? new Point(X - FireX, Y + FireY) : new Point(X + FireX, Y + FireY);
		}
		return (m_direction > 0) ? new Point(X - m_rect.X + 5, Y + m_rect.Y - 5) : new Point(X + m_rect.X - 5, Y + m_rect.Y - 5);
	}

	public Rectangle GetDirectDemageRect()
	{
		return (m_direction > 0) ? new Rectangle(X - m_demageRect.X, Y + m_demageRect.Y, m_demageRect.Width, m_demageRect.Height) : new Rectangle(X + m_demageRect.X, Y + m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
	}

	public List<Rectangle> GetDirectBoudRect()
	{
		List<Rectangle> list = new List<Rectangle>();
		list.Add((m_direction > 0) ? new Rectangle(X - base.Bound.X, Y + base.Bound.Y, base.Bound.Width, base.Bound.Height) : new Rectangle(X + base.Bound.X, Y + base.Bound.Y, base.Bound.Width, base.Bound.Height));
		list.Add((m_direction > 0) ? new Rectangle(X - base.Bound1.X, Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height) : new Rectangle(X + base.Bound1.X, Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height));
		return list;
	}

	public double Distance(Point p)
	{
		List<double> list = new List<double>();
		Rectangle directDemageRect = GetDirectDemageRect();
		for (int i = directDemageRect.X; i <= directDemageRect.X + directDemageRect.Width; i += 10)
		{
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y - p.Y) * (directDemageRect.Y - p.Y)));
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y + directDemageRect.Height - p.Y) * (directDemageRect.Y + directDemageRect.Height - p.Y)));
		}
		for (int j = directDemageRect.Y; j <= directDemageRect.Y + directDemageRect.Height; j += 10)
		{
			list.Add(Math.Sqrt((directDemageRect.X - p.X) * (directDemageRect.X - p.X) + (j - p.Y) * (j - p.Y)));
			list.Add(Math.Sqrt((directDemageRect.X + directDemageRect.Width - p.X) * (directDemageRect.X + directDemageRect.Width - p.X) + (j - p.Y) * (j - p.Y)));
		}
		return list.Min();
	}

	public double BoundDistance(Point p)
	{
		List<double> list = new List<double>();
		foreach (Rectangle item in GetDirectBoudRect())
		{
			for (int i = item.X; i <= item.X + item.Width; i += 10)
			{
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y - p.Y) * (item.Y - p.Y)));
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y + item.Height - p.Y) * (item.Y + item.Height - p.Y)));
			}
			for (int j = item.Y; j <= item.Y + item.Height; j += 10)
			{
				list.Add(Math.Sqrt((item.X - p.X) * (item.X - p.X) + (j - p.Y) * (j - p.Y)));
				list.Add(Math.Sqrt((item.X + item.Width - p.X) * (item.X + item.Width - p.X) + (j - p.Y) * (j - p.Y)));
			}
		}
		return list.Min();
	}

	public virtual void Reset()
	{
		m_blood = m_maxBlood;
		m_isFrost = false;
		m_isHide = false;
		m_isNoHole = false;
		m_isLiving = true;
		m_isBlockTurn = false;
		KeepLifeCount = 0;
		TurnNum = 0;
		TotalHurt = 0;
		TotalKill = 0;
		TotalShootCount = 0;
		TotalHitTargetCount = 0;
		if (this is Player && m_game.RoomType == eRoomType.ConsortiaBattle)
		{
			PlayerInfo playerCharacter = (this as Player).PlayerDetail.PlayerCharacter;
			if (playerCharacter.ReduceStartBlood > 0 && playerCharacter.ReduceStartBlood < m_maxBlood)
			{
				m_blood = playerCharacter.ReduceStartBlood;
				m_maxBlood = playerCharacter.hp;
			}
		}
	}

	public virtual void PickBox(SimpleBox box)
	{
		if (box.Type > 1 && this is Player)
		{
			box.Die();
			if ((this as Player).psychic < (this as Player).MaxPsychic)
			{
				(this as Player).psychic += ((box.Type == 2) ? 10 : 20);
			}
			return;
		}
		box.UserID = base.Id;
		box.Die();
		if (m_syncAtTime)
		{
			m_game.SendGamePickBox(this, box.Id, 0, "");
		}
		if (base.IsLiving && this is Player)
		{
			(this as Player).OpenBox(box.Id);
		}
	}

	public virtual void PickBall(Ball ball)
	{
		ball.Die();
		string currentAction = ball.CurrentAction;
		ball.PlayMovie(ball.ActionMapping[currentAction], 1000, 0);
	}

	public override void PrepareNewTurn()
	{
		IgnoreGuard = 0;
		ShootMovieDelay = 0;
		CurrentDamagePlus = 1f;
		CurrentShootMinus = 1f;
		IgnoreArmor = false;
		ControlBall = false;
		NoHoleTurn = false;
		CurrentIsHitTarget = false;
		totalCritical = 0;
		totalShotTurn = 0;
		IdAcrobacias = 1;
		Prop1 = 0;
		Prop2 = 0;
		Prop3 = 0;
		Prop4 = 0;
		lastShots.Clear();
		OnBeginNewTurn();
		BombFoul = false;
		TiredShoot = false;
		ClearBuff = false;
		LockMove = false;
	}

	public virtual void PrepareSelfTurn()
	{
		PrepareAttackGemLilit();
		PrepareDefendGem();
		OnBeginSelfTurn();
	}

	public void StartAttacked()
	{
		OnStartAttacked();
	}

	public void PrepareAttackGemLilit()
	{
		if (AttackGemLimit > 0)
		{
			AttackGemLimit--;
		}
	}

	public void OnPropUseItem()
	{
		this.PropUseItem?.Invoke(this);
	}

	public void PrepareDefendGem()
	{
		if (DefenFisrtGem > 0 && DefenSecondGem > 0)
		{
			int[] array = new int[2] { DefenFisrtGem, DefenSecondGem };
			int num = rand.Next(array.Length);
			DefendActiveGem = array[num];
		}
		else
		{
			DefendActiveGem = DefenFisrtGem;
		}
	}

	public virtual void StartAttacking()
	{
		if (!m_isAttacking)
		{
			m_isAttacking = true;
			OnStartAttacking();
		}
	}

	public virtual void StopAttacking()
	{
		if (m_isAttacking)
		{
			m_isAttacking = false;
			OnStopAttacking();
		}
	}

	public virtual void BeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
	}

	public virtual void OnAfterTakedBomb()
	{
	}

	public virtual void OnAfterTakedFrozen()
	{
	}

	public override void CollidedByObject(Physics phy)
	{
		if (phy is SimpleBomb)
		{
			((SimpleBomb)phy).Bomb();
		}
	}

	public void SetXY(int x, int y, int delay)
	{
		m_game.AddAction(new LivingDirectSetXYAction(this, x, y, delay));
	}

	public void AddEffect(AbstractEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
	}

	public void AddPetEffect(AbstractPetEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayPetEffectAction(this, effect, delay));
	}

	public void AddHorseEffect(AbstractHorseEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayHorseEffectAction(this, effect, delay));
	}

	public void AddGuardCoreEffect(AbstractGuardCoreEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayGuardCoreEffectAction(this, effect, delay));
	}

	public void Say(string msg, int type, int delay, int finishTime)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
	}

	public void Say(string msg, int type, int delay)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, 1000));
	}

	public bool MoveTo(int x, int y, string action, int delay, int speed)
	{
		return MoveTo(x, y, action, delay, "", speed, null);
	}

	public bool MoveTo(int x, int y, string action, int delay, int speed, LivingCallBack callback)
	{
		return MoveTo(x, y, action, delay, "", speed, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
	{
		return MoveTo(x, y, action, delay, sAction, speed, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
	{
		if (m_x == x && m_y == y)
		{
			return false;
		}
		if (x < 0 || x > m_map.Bound.Width)
		{
			return false;
		}
		List<Point> list = new List<Point>();
		int x2 = m_x;
		int y2 = m_y;
		int num = ((x > x2) ? 1 : (-1));
		Point point = new Point(x2, y2);
		if (Config.IsFly)
		{
			Point point2 = new Point(x - point.X, y - point.Y);
			while (point2.Length() > (double)speed)
			{
				point2 = point2.Normalize(speed);
				point = new Point(point.X + point2.X, point.Y + point2.Y);
				point2 = new Point(x - point.X, y - point.Y);
				if (point != Point.Empty)
				{
					list.Add(point);
					continue;
				}
				list.Add(new Point(x, y));
				break;
			}
		}
		else
		{
			while ((x - x2) * num > 0)
			{
				point = m_map.FindNextWalkPointDown(x2, y2, num, speed * StepX, speed * StepY);
				if (point != Point.Empty)
				{
					list.Add(point);
					x2 = point.X;
					y2 = point.Y;
					continue;
				}
				break;
			}
		}
		if (list.Count > 0)
		{
			m_game.AddAction(new LivingMoveToAction(this, list, action, delay, speed, sAction, callback, delayCallback));
			return true;
		}
		return false;
	}

	public override void StartMoving()
	{
		StartMoving(0, 30);
	}

	public virtual void StartMoving(int delay, int speed)
	{
		if (!Config.IsFly)
		{
			if (m_map.IsEmpty(X, Y))
			{
				FallFrom(X, Y, null, delay, 0, speed);
			}
			base.StartMoving();
		}
	}

	public Point StartFalling(bool direct)
	{
		return StartFalling(direct, 0, MOVE_SPEED * 10);
	}

	public virtual Point StartFalling(bool direct, int delay, int speed)
	{
		if (m_map == null)
		{
			return Point.Empty;
		}
		Point point = m_map.FindYLineNotEmptyPointDown(X, Y);
		if (point == Point.Empty)
		{
			point = new Point(X, m_game.Map.Bound.Height + 1);
		}
		if (point.Y == Y)
		{
			return Point.Empty;
		}
		if (direct && this is Player)
		{
			SetXY(point);
			if (m_map.IsOutMap(point.X, point.Y))
			{
				Die();
				if (Game.CurrentLiving != this && Game.CurrentLiving is Player && this is Player && Team != Game.CurrentLiving.Team)
				{
					Player player = Game.CurrentLiving as Player;
					player.PlayerDetail.OnKillingLiving(m_game, 1, base.Id, base.IsLiving, 0);
					TurnedLiving currentLiving = Game.CurrentLiving;
					currentLiving.TotalKill++;
					player.CalculatePlayerOffer(this as Player);
				}
			}
		}
		else if (Config != null && !Config.IsFly)
		{
			m_game.AddAction(new LivingFallingAction(this, point.X, point.Y, speed, null, delay, 0, null));
		}
		return point;
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed)
	{
		return FallFrom(x, y, action, delay, type, speed, null);
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point point = m_map.FindYLineNotEmptyPointDown(x, y);
		if (point == Point.Empty)
		{
			point = new Point(x, m_game.Map.Bound.Height + 1);
		}
		if (Y < point.Y)
		{
			m_game.AddAction(new LivingFallingAction(this, point.X, point.Y, speed, action, delay, type, callback));
			return true;
		}
		return false;
	}

	public bool FallFromTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		m_game.AddAction(new LivingFallingAction(this, x, y, speed, action, delay, type, callback));
		return true;
	}

	public bool JumpTo(int x, int y, string action, int delay, int type)
	{
		return JumpTo(x, y, action, delay, type, 20, null, 0);
	}

	public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback)
	{
		return JumpTo(x, y, ation, delay, type, 20, callback, 0);
	}

	public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		return JumpTo(x, y, action, delay, type, speed, callback, 0);
	}

	public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback, int value)
	{
		Point point = m_map.FindYLineNotEmptyPointDown(x, y);
		if (point.Y < Y || value == 1)
		{
			m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
			return true;
		}
		return false;
	}

	public bool JumpToSpeed(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point point = m_map.FindYLineNotEmptyPointDown(x, y);
		int num = 200;
		num = point.Y;
		m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
		return true;
	}

	public void ChangeDirection(int direction, int delay)
	{
		if (delay > 0)
		{
			m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
		}
		else
		{
			Direction = direction;
		}
	}

	public void ChangeDirection(Living obj, int delay)
	{
		int direction = FindDirection(obj);
		if (delay > 0)
		{
			m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
		}
		else
		{
			Direction = direction;
		}
	}

	public int FindDirection(Living obj)
	{
		if (obj.X > X)
		{
			return 1;
		}
		return -1;
	}

	public double getHertAddition(ItemInfo item)
	{
		if (item == null)
		{
			return 0.0;
		}
		double num = item.Template.Property7;
		double y = item.StrengthenLevel;
		double a = num * Math.Pow(1.1, y) - num;
		return Math.Round(a) + num;
	}

	protected int MakeDamage(Living target)
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
		double num = BaseDamage;
		double num2 = target.BaseGuard;
		double num3 = target.Defence;
		double attack = Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num4 = (int)target.getHertAddition((target as Player).DeputyWeapon);
			num2 += (double)num4;
			num3 += (double)num4;
		}
		if (IgnoreArmor)
		{
			num2 *= 0.6;
			num3 *= 0.6;
		}
		if (AddDamage > 0)
		{
			num += num / 100.0 * (double)AddDamage;
		}
		if (IgnoreGuard > 0 && !IgnoreArmor)
		{
			num3 -= num3 / 100.0 * (double)IgnoreGuard;
			num2 -= num2 / 100.0 * (double)IgnoreGuard;
		}
		float currentDamagePlus = CurrentDamagePlus;
		float currentShootMinus = CurrentShootMinus;
		double num5 = 0.95 * (num2 - (double)(3 * Grade)) / (500.0 + num2 - (double)(3 * Grade));
		double num6 = ((num3 - Lucky >= 0.0) ? (0.95 * (num3 - Lucky) / (600.0 + num3 - Lucky)) : 0.0);
		double num7 = (double)FightBuffers.WorldBossAddDamage * (1.0 - (num2 / 200.0 + num3 * 0.003));
		double num8 = (num7 + num * (1.0 + attack * 0.001) * (1.0 - (num5 + num6 - num5 * num6))) * (double)currentDamagePlus * (double)currentShootMinus;
		double num9 = MagicAttack * 0.8 - target.MagicDefence * 0.7;
		double num10 = FireAttack * 0.6 - target.FireDefence * 0.3;
		double num11 = WaterAttack * 0.6 - target.WaterDefence * 0.3;
		double num12 = WindAttack * 0.6 - target.WindDefence * 0.3;
		double num13 = LandAttack * 0.6 - target.LandDefence * 0.3;
		double num14 = num10 + num11 + num12 + num13;
		num8 += num9;
		num8 += num14;
		Point point = new Point(X, Y);
		if (num8 < 0.0)
		{
			return 1;
		}
		if (target.KeepLife)
		{
			return 0;
		}
		return (int)num8;
	}

	public int MakeCriticalDamage(Living target, int baseDamage)
	{
		double lucky = Lucky;
		double lucky2 = target.Lucky;
		double num = lucky * 25.0 / (800.0 + lucky);
		num += 10000.0 * ((lucky / (lucky + 500.0) * 1500.0 + Crit) / (lucky / (lucky + 500.0) * 1500.0 + Crit + 2500.0) * 0.75 - lucky / (lucky + 500.0) * 1500.0 / (lucky / (lucky + 500.0) * 1500.0 + 2500.0) * 0.75) / 100.0 / 100.0;
		num -= 10000.0 * ((lucky2 / (lucky2 + 500.0) * 1500.0 + target.Tenacity) / (lucky2 / (lucky2 + 500.0) * 1500.0 + target.Tenacity + 2500.0) * 0.75 - lucky2 / (lucky2 + 500.0) * 1500.0 / (lucky2 / (lucky2 + 500.0) * 1500.0 + 2500.0) * 0.75) / 100.0 / 100.0;
		num += (double)PetEffects.CritRate;
		if (num >= (double)m_game.Random.Next(100))
		{
			int num2 = target.ReduceCritFisrtGem + target.ReduceCritSecondGem + target.PetEffects.ReduceCritValue;
			double num3 = lucky * 0.00015;
			if (num3 >= 2.0)
			{
				num3 = 1.5;
			}
			if (num3 >= 2.5)
			{
				num3 = 2.0;
			}
			if (num3 >= 3.0)
			{
				num3 = 2.5;
			}
			int num4 = (int)((double)baseDamage * num3);
			num4 = num4 * (100 - num2) / 100;
			if (FightBuffers.ConsortionAddCritical > 0)
			{
				num4 += FightBuffers.ConsortionAddCritical;
			}
			if (lucky < 3400.0)
			{
				num4 += (int)(10000.0 * ((lucky * 0.5 / (1.0 - lucky * 0.00015) + ViolenceInjury) / (lucky * 0.5 / (1.0 - lucky * 0.00015) + ViolenceInjury + 3300.0) - lucky * 0.5 / (1.0 - lucky * 0.00015) / (lucky * 0.5 / (1.0 - lucky * 0.00015) + 3300.0)) / 100.0 / 100.0);
				num4 -= (int)(10000.0 * ((lucky2 * 0.5 / (1.0 - lucky2 * 0.00015) + target.Guard) / (lucky2 * 0.5 / (1.0 - lucky2 * 0.00015) + target.Guard + 3300.0) - lucky2 * 0.5 / (1.0 - lucky2 * 0.00015) / (lucky2 * 0.5 / (1.0 - lucky2 * 0.00015) + 3300.0)) / 100.0) / 100;
			}
			else
			{
				num4 += (int)(10000.0 * ((lucky / (lucky + 500.0) * 4000.0 + ViolenceInjury) / (lucky / (lucky + 500.0) * 4000.0 + ViolenceInjury + 3300.0) - lucky / (lucky + 500.0) * 4000.0 / (lucky / (lucky + 500.0) * 4000.0 + 3300.0)) / 100.0) / 100;
				num4 -= (int)(10000.0 * ((lucky2 / (lucky2 + 500.0) * 4000.0 + target.Guard) / (lucky2 / (lucky2 + 500.0) * 4000.0 + target.Guard + 3300.0) - lucky2 / (lucky2 + 500.0) * 4000.0 / (lucky2 / (lucky2 + 500.0) * 4000.0 + 3300.0)) / 100.0) / 100;
			}
			num4 -= num4 * target.EquipGhostReduceCritDamage / 100;
			return (num4 < 0) ? 1 : num4;
		}
		return 0;
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay)
	{
		return Beat(target, action, demageAmount, criticalAmount, delay, 1, 1);
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect)
	{
		return Beat(target, action, demageAmount, criticalAmount, delay, livingCount, attackEffect, "0");
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect, string deadEffect)
	{
		return Beat(target, action, demageAmount, criticalAmount, delay, livingCount, attackEffect, deadEffect, 1);
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect, string deadEffect, int type)
	{
		if (target == null || !target.IsLiving)
		{
			return false;
		}
		demageAmount = MakeDamage(target);
		OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
		StartAttacked();
		int num = (int)target.Distance(X, Y);
		if (num <= MaxBeatDis)
		{
			if (X - target.X > 0)
			{
				Direction = -1;
			}
			else
			{
				Direction = 1;
			}
			m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, livingCount, attackEffect, deadEffect, type));
			return true;
		}
		return false;
	}

	public void BeatDirect(Living target, string action, int delay, int livingCount, int attackEffect)
	{
		int num = MakeDamage(target);
		int criticalAmount = MakeCriticalDamage(target, num);
		m_game.AddAction(new LivingBeatAction(this, target, num, criticalAmount, action, delay, livingCount, attackEffect, "", 1));
	}

	public bool RangeAttacking(int fx, int tx, string action, int delay, List<Player> players)
	{
		return RangeAttacking(fx, tx, action, delay, removeFrost: true, directDamage: false, players);
	}

	public bool RangeAttacking(int fx, int tx, string action, int delay, bool directDamage)
	{
		return RangeAttacking(fx, tx, action, delay, removeFrost: true, directDamage, null);
	}

	public bool RangeAttacking(int fx, int tx, string action, int delay, bool removeFrost, List<Player> players)
	{
		return RangeAttacking(fx, tx, action, delay, removeFrost, directDamage: false, players);
	}

	public bool RangeAttacking(int fx, int tx, string action, int delay, bool removeFrost, bool directDamage, List<Player> players)
	{
		if (base.IsLiving)
		{
			m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, removeFrost, directDamage, players));
			return true;
		}
		return false;
	}

	public bool RangeAttackingNPC(string action, int delay, List<Living> livings)
	{
		if (base.IsLiving && livings != null && livings.Count > 0)
		{
			m_game.AddAction(new LivingRangeAttackingNPCAction(this, action, delay, livings));
			return true;
		}
		return false;
	}

	public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
	{
		if (minTime >= maxTime)
		{
			return;
		}
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		if (m_game == null || ballInfo == null)
		{
			return;
		}
		Map map = m_game.Map;
		Point shootPoint = GetShootPoint();
		float num = x - shootPoint.X;
		float num2 = y - shootPoint.Y;
		float af = map.airResistance * (float)ballInfo.DragIndex;
		float f = map.gravity * (float)ballInfo.Weight * (float)ballInfo.Mass;
		float f2 = map.wind * (float)ballInfo.Wind * map.windRate;
		float m = ballInfo.Mass;
		for (float num3 = time; num3 <= 4f; num3 += 0.6f)
		{
			double num4 = ComputeVx(num, m, af, f2, num3);
			double num5 = ComputeVy(num2, m, af, f, num3);
			if (!(num5 < 0.0) || !(num4 * (double)m_direction > 0.0))
			{
				continue;
			}
			double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
			if (num6 < 2000.0)
			{
				force = (int)num6;
				angle = (int)(Math.Atan(num5 / num4) / Math.PI * 180.0);
				if (num4 < 0.0)
				{
					angle += 180;
				}
				break;
			}
		}
		x = shootPoint.X;
		y = shootPoint.Y;
	}

	public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
	{
		return ShootPoint(x, y, 0, 0, bombId, minTime, maxTime, bombCount, time, delay, null);
	}

	public bool ShootPoint(int x, int y, int force, int angle, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
	{
		return ShootPoint(x, y, force, angle, bombId, minTime, maxTime, bombCount, time, delay, null);
	}

	public bool ShootPoint(int x, int y, int force, int angle, int bombId, int minTime, int maxTime, int bombCount, float time, int delay, LivingCallBack callBack)
	{
		m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, minTime, maxTime, time, delay, callBack));
		return true;
	}

	public bool IsFriendly(Living living)
	{
		if (Game.RoomType == eRoomType.SurvivalMode)
		{
			return false;
		}
		if (living is Player)
		{
			return false;
		}
		if (living.Team == Team)
		{
			return true;
		}
		return false;
	}

	public bool Shoot(int bombId, int x, int y, int force, int angle, int bombCount, int delay)
	{
		m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, delay, 0, 0f, 0, null));
		return true;
	}

	public static double ComputeVx(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.7;
	}

	public static double ComputeVy(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
	}

	public static double ComputDX(double v, float m, float af, float f, float dt)
	{
		return v * (double)dt + ((double)f - (double)af * v) / (double)m * (double)dt * (double)dt;
	}

    public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount, int shootCount, int embleType)
	{
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		Tile shape = BallMgr.FindTile(bombId);
		BombType ballType = BallMgr.GetBallType(bombId);
		int num = (int)(m_map.wind * 10f);
		if (ballInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, base.Id);
			gSPacketIn.Parameter1 = base.Id;
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteBoolean(num > 0);
			gSPacketIn.WriteByte(m_game.GetVane(num, 1));
			gSPacketIn.WriteByte(m_game.GetVane(num, 2));
			gSPacketIn.WriteByte(m_game.GetVane(num, 3));
			gSPacketIn.WriteInt((int)m_map.windRate);
			gSPacketIn.WriteInt((int)m_map.windRate * 100);
			gSPacketIn.WriteInt(bombCount);
			float num2 = 0f;
			int num3 = 0;
			int num4 = 0;
			SimpleBomb simpleBomb = null;
			for (int i = 0; i < bombCount; i++)
			{
				double num5 = 1.0;
				int num6 = 0;
				switch (i)
				{
				case 1:
					num5 = 0.9;
					num6 = -5;
					break;
				case 2:
					num5 = 1.1;
					num6 = 5;
					break;
				}
				num3 = (int)((double)force * num5 * Math.Cos((double)(angle + num6) / 180.0 * Math.PI));
				num4 = (int)((double)force * num5 * Math.Sin((double)(angle + num6) / 180.0 * Math.PI));
				SimpleBomb simpleBomb2 = new SimpleBomb(m_game.PhysicalId++, ballType, this, m_game, ballInfo, shape, ControlBall, Invoked, angle, isPlatom: false);
				simpleBomb2.SetXY(x, y);
				simpleBomb2.setSpeedXY(num3, num4);
				simpleBomb2.setAngle(angle);
				totalShotTurn = ((totalShotTurn == 0) ? (bombCount * shootCount) : totalShotTurn);
				m_map.AddPhysical(simpleBomb2);
				simpleBomb2.StartMoving();
				if (i == 0)
				{
					simpleBomb = simpleBomb2;
				}
				gSPacketIn.WriteInt(bombCount);
				gSPacketIn.WriteInt(shootCount);
				gSPacketIn.WriteBoolean(simpleBomb2.DigMap);
				gSPacketIn.WriteInt(simpleBomb2.Id);
				if (GameProperties.VERSION >= 8100)
				{
					gSPacketIn.WriteInt(-1);
				}
				gSPacketIn.WriteInt(x);
				gSPacketIn.WriteInt(y);
				gSPacketIn.WriteInt(num3);
				gSPacketIn.WriteInt(num4);
				gSPacketIn.WriteInt(simpleBomb2.BallInfo.ID);
				if (FlyingPartical != 0)
				{
					gSPacketIn.WriteString(FlyingPartical.ToString());
				}
				else
				{
					gSPacketIn.WriteString(ballInfo.FlyingPartical);
				}
				gSPacketIn.WriteInt(simpleBomb2.BallInfo.Radii * 1000 / bombCount);
				gSPacketIn.WriteInt((int)simpleBomb2.BallInfo.Power * 1000);
				if (GameProperties.VERSION >= 11000)
				{
					gSPacketIn.WriteInt(embleType);
				}
				gSPacketIn.WriteInt(simpleBomb2.Actions.Count);
				foreach (BombAction action in simpleBomb2.Actions)
				{
					gSPacketIn.WriteInt(action.TimeInt);
					gSPacketIn.WriteInt(action.Type);
					if (GameProperties.VERSION >= 8100 && action.Type == 27)
					{
						gSPacketIn.WriteInt(action.Param1);
						gSPacketIn.WriteInt(action.Param2);
					}
					else
					{
						gSPacketIn.WriteInt(action.Param1);
						gSPacketIn.WriteInt(action.Param2);
						gSPacketIn.WriteInt(action.Param3);
						gSPacketIn.WriteInt(action.Param4);
					}
					if (action.Type == 33)
					{
						if (action.Param2 > 0 && action.Param3 > 0)
						{
							PassBallFail = true;
							m_game.TakePassBall(-1);
						}
						else
						{
							m_game.TakePassBall(action.Param1);
						}
					}
				}
				num2 = Math.Max(num2, simpleBomb2.LifeTime);
			}
			int count = simpleBomb.PetActions.Count;
			if (count > 0)
			{
				if (simpleBomb.PetActions[0].Param1 == -1)
				{
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(count);
					foreach (BombAction petAction in simpleBomb.PetActions)
					{
						gSPacketIn.WriteInt(petAction.Param1);
						gSPacketIn.WriteInt(petAction.Param2);
						gSPacketIn.WriteInt(petAction.Param4);
						gSPacketIn.WriteInt(petAction.Param3);
					}
				}
				gSPacketIn.WriteInt(1);
			}
			else
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
			if (GameProperties.VERSION < 8100)
			{
				gSPacketIn.WriteBoolean(val: false);
				if (m_game.RoomType == eRoomType.FightFootballTime)
				{
					gSPacketIn.WriteInt(m_game.redScore);
					gSPacketIn.WriteInt(m_game.blueScore);
					gSPacketIn.WriteInt((this as Player).PlayerDetail.PlayerCharacter.ID);
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(ScoreArr.Count);
					for (int j = 0; j < ScoreArr.Count; j++)
					{
						gSPacketIn.WriteInt(0);
						gSPacketIn.WriteInt(ScoreArr[j]);
					}
				}
			}
			m_game.SendToAll(gSPacketIn);
			LastLifeTimeShoot = (int)((num2 + 2f + (float)(bombCount / 3)) * 1000f) + PetEffects.Delay + SpecialSkillDelay;
			m_game.WaitTime(LastLifeTimeShoot);
			if (m_game.IsPVE())
			{
				((PVEGame)m_game).OnShooted();
			}
			return true;
		}
		return false;
	}

	public void PlayMovie(string action, int delay, int MovieTime)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack call)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public void BoltMove(int x, int y, int delay)
	{
		m_game.AddAction(new LivingBoltMoveAction(this, x, y, delay));
	}

	public void SetNiutou(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 33, state);
		}
	}

	public void SetIndian(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 34, state);
		}
	}

	public void SetTargeting(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 7, state);
		}
	}

	public void HoldingBall(bool state)
	{
		if (m_isHoldBall != state)
		{
			m_isHoldBall = state;
			if (m_syncAtTime)
			{
				m_game.SendPlayerPicture(this, 99, state);
			}
		}
	}

	public void SetSeal(bool state)
	{
		if (m_isSeal != state)
		{
			m_isSeal = state;
		}
		m_game.SendGamePlayerProperty(this, "silenceMany", state.ToString());
	}

	public void AddRemoveEnergy(int value)
	{
		m_game.SendCurrentPlayerProperty(this, "energy", value.ToString());
	}

	public void NoFly(bool value)
	{
		m_game.SendCurrentPlayerProperty(this, "nofly", value.ToString());
	}

	public void SetHidden(bool state)
	{
		m_game.SendGamePlayerProperty(this, "visible", state.ToString());
	}

	public void SpeedMultX(int value)
	{
		m_game.SendGamePlayerProperty(this, "speedX", value.ToString());
	}

	public void SpeedMultY(int value)
	{
		m_game.SendGamePlayerProperty(this, "speedY", value.ToString());
	}

	public void SetOffsetX(int value)
	{
		m_game.SendGamePlayerProperty(this, "offsetX", value.ToString());
	}

	public void SetOffsetY(int value)
	{
		m_game.SendGamePlayerProperty(this, "offsetY", value.ToString());
	}

	public void OnSmallMap(bool state)
	{
		m_game.SendGamePlayerProperty(this, "onSmallMap", state.ToString());
	}

	public void SetThumbnailBoss(bool state)
	{
		m_game.SendGamePlayerProperty(this, "hideBossThumbnail", state.ToString());
	}

	public void ShowImprisonment(bool state)
	{
		m_game.SendGamePlayerProperty(this, "showImprisonment", state.ToString());
	}

	public void ShowProvoke1(bool state)
	{
		m_game.SendGamePlayerProperty(this, "showProvoke1", state.ToString());
	}

	public void ShowProvoke2(bool state)
	{
		m_game.SendGamePlayerProperty(this, "showProvoke2", state.ToString());
	}

	public void ShowProvoke3(bool state)
	{
		m_game.SendGamePlayerProperty(this, "locksecondWeapon", state.ToString());
	}

	public void ShowBlind(bool state)
	{
		m_game.SendGamePlayerProperty(this, "showBlind", state.ToString());
	}

	public void SetVisible(bool state)
	{
		m_game.SendGamePlayerProperty(this, "visible", state.ToString());
	}

	public bool GetSealState()
	{
		return m_isSeal;
	}

	public void Seal(Player player, int type, int delay)
	{
		m_game.AddAction(new LivingSealAction(this, player, type, delay));
	}

	public virtual int AddMaxBlood(int value)
	{
		MaxBlood += value;
		m_game.SendGameUpdateMaxBlood(this);
		return value;
	}

	public virtual int AddBlood(int value, int type, int emabletype = 0)
	{
		if (CanAddBlood || value < 0)
		{
			m_blood += value;
			if (m_blood > m_maxBlood)
			{
				m_blood = m_maxBlood;
			}
			if (m_syncAtTime)
			{
				m_game.SendGameUpdateHealth(this, type, value, emabletype);
			}
		}
		return value;
	}

	public virtual int AddBlood(int value)
	{
		if (value > 0 && ForbidAidBlood)
		{
			value = 0;
		}
		return AddBlood(value, 0);
	}

	public virtual int AddBlood(int value, int type)
	{
		if (value > 0 && ForbidAidBlood)
		{
			value = 0;
		}
		if (CanAddBlood)
		{
			if (value > 0)
			{
				value += value * AddBloodValue / 100;
			}
			if (m_blood + value >= m_maxBlood)
			{
				m_blood = m_maxBlood;
			}
			else
			{
				m_blood += value;
			}
			if (m_syncAtTime)
			{
				m_game.SendGameUpdateHealth(this, type, value);
			}
		}
		return value;
	}

	public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if (Config.IsHelper && (this is SimpleNpc || this is SimpleBoss) && source is Player)
		{
			return false;
		}
		if (Game.RoomType == eRoomType.SurvivalMode && targetId != source.Id)
		{
			return false;
		}
		bool result = false;
		if (!IsFrost && m_blood > 0)
		{
			if (source != this || source.Team == Team)
			{
				OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
				StartAttacked();
			}
			int num = ((damageAmount + criticalAmount >= 0) ? (damageAmount + criticalAmount) : 0);
			if (this is Player && num != 0)
			{
				PlayerInfo playerCharacter = (this as Player).PlayerDetail.PlayerCharacter;
				int num2 = playerCharacter.ReduceDamePlus + playerCharacter.GoldenReduceDamage;
				int num3 = num * num2 / 100;
				num -= num3;
				if (source is Player && (source as Player).IsSpecialSkill && (this as Player).BatleConfig.PetFormReduceDamage != 0)
				{
					num -= num * (this as Player).BatleConfig.PetFormReduceDamage / 100;
				}
			}
			if (KeepLife)
			{
				num = 0;
			}
			m_blood -= ((num >= 0) ? num : 0);
			int num4 = m_maxBlood * 30 / 100;
			if (this is Player && m_blood > 0 && m_blood < num4 && (this as Player).PlayerDetail.UseKingBlessHelpStraw(m_game.RoomType))
			{
				(this as Player).PlayerDetail.SendMessage(LanguageMgr.GetTranslation("Living.Msg1", (this as Player).PlayerDetail.PlayerCharacter.NickName, num4));
				AddBlood(num4);
			}
			OnAfterTakedDamage(source, damageAmount, criticalAmount);
			if (m_blood <= 0 && Config.KeepLife)
			{
				m_blood = 1;
				m_game.SendGameUpdateHealth(this, 1, 1);
			}
			if (m_syncAtTime)
			{
				if (this is SimpleBoss && ((SimpleBoss)this).NpcInfo.ID == 0)
				{
					m_game.SendGameUpdateHealth(this, 6, num);
				}
				else
				{
					m_game.SendGameUpdateHealth(this, 1, num);
				}
			}
			if (source.Prop4 >= 2)
			{
				if (criticalAmount > 0 && source != this)
				{
					m_game.AddAction(new FightAchievementAction(source, eAcrobaciaType.PeritoEmGolpes, source.IdAcrobacias++, 1200));
					source.totalCritical++;
				}
				if (source.totalCritical >= source.totalShotTurn)
				{
					m_game.AddAction(new FightAchievementAction(source, eAcrobaciaType.ExplosaoDePersonagem, source.IdAcrobacias++, 1200));
				}
			}
			if (m_blood <= 0 && m_game.RoomType != eRoomType.FightFootballTime)
			{
				if (this is SimpleBoss simpleBoss && this is SimpleBoss && simpleBoss.NpcInfo.ID == 71009)
				{
					PlayMovie("dieA", 0, 0);
					Die(1500);
				}
				else
				{
					Die();
				}
			}
			if (m_blood <= 0 && m_game.RoomType != eRoomType.FightFootballTime)
			{
				Die();
			}
			source.OnAfterKillingLiving(this, damageAmount, criticalAmount);
			result = true;
		}
		else
		{
			IsFrost = false;
			EffectBadges = false;
		}
		EffectList.StopEffect(typeof(IceFronzeEffect));
		EffectList.StopEffect(typeof(HideEffect));
		EffectList.StopEffect(typeof(NoHole_Active));
		return result;
	}

	public void SetIceFronze(Living living)
	{
		new IceFronzeEffect(3).Start(this);
		BeginNextTurn -= SetIceFronze;
	}

	public virtual bool PetTakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if (Config.IsHelper)
		{
			return false;
		}
		if (source.Team == m_team)
		{
			return false;
		}
		if (Game.RoomType == eRoomType.SurvivalMode && targetId != source.Id)
		{
			return false;
		}
		bool result = false;
		if (m_blood > 0)
		{
			m_blood -= damageAmount + criticalAmount;
			OnAfterTakedDamage(source, damageAmount, criticalAmount);
			if (m_blood <= 0)
			{
				if (Config.KeepLife)
				{
					m_blood = 1;
				}
				else
				{
					Die();
				}
			}
			result = true;
		}
		return result;
	}

	public virtual void Die(int delay)
	{
		if (base.IsLiving && m_game != null)
		{
			m_game.AddAction(new LivingDieAction(this, delay));
		}
	}

	public override void Die()
	{
		if (this is Player)
		{
			if (m_game is PVPGame)
			{
				(m_game as PVPGame).SendTempStyle(eRoomType.Exploration);
			}
			if (m_game is PVEGame)
			{
				(m_game as PVEGame).SendTempStyle(0);
			}
		}
		if (m_blood > 0)
		{
			m_blood = 0;
			m_doAction = -1;
			if (m_syncAtTime)
			{
				m_game.SendGameUpdateHealth(this, 6, 0);
			}
		}
		if (base.IsLiving)
		{
			if (IsAttacking)
			{
				StopAttacking();
			}
			base.Die();
			OnDied();
			m_game.CheckState(0);
		}
	}

	public override void Revive()
	{
		base.Revive();
	}

	public void OnBeforeMakeDamage(Living target)
	{
		this.BeforeMakeDamage?.Invoke(this, target);
	}

	public void OnAfterMakeDamage(Living target)
	{
		this.AfterMakeDamage?.Invoke(this, target);
	}

	public void OnTakedPetDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.TakePetDamage != null)
		{
			this.TakePetDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	protected void OnBeginUseProp()
	{
		if (this.BeginUseProp != null)
		{
			this.BeginUseProp(this);
		}
	}

	public void OnGameStarted()
	{
		if (this.GameStarted != null)
		{
			this.GameStarted(this);
		}
	}

	protected void OnDied()
	{
		if (this.Died != null)
		{
			this.Died(this);
		}
		if (this is Player && Game is PVEGame)
		{
			((PVEGame)Game).DoOther();
		}
		if ((this is SimpleBoss || this is SimpleNpc) && Game is PVEGame)
		{
			((PVEGame)Game).OnDied();
		}
	}

	protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.BeforeTakeDamage != null)
		{
			this.BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (this.TakePlayerDamage != null)
		{
			this.TakePlayerDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	public void OnMakeDamage(Living source, Living target, ref int damageAmount, ref int criticalAmount)
	{
		if (this.OnMakeDamageEvent != null)
		{
			this.OnMakeDamageEvent(target, source, ref damageAmount, ref criticalAmount);
		}
	}

	protected void OnBeginNewTurn()
	{
		if (this.BeginNextTurn != null)
		{
			this.BeginNextTurn(this);
		}
	}

	protected void OnBeginSelfTurn()
	{
		if (this.BeginSelfTurn != null)
		{
			this.BeginSelfTurn(this);
		}
	}

	protected void OnStartAttacked()
	{
		if (this.BeginAttacked != null)
		{
			this.BeginAttacked(this);
		}
	}

	protected void OnStartAttacking()
	{
		if (this.BeginAttacking != null)
		{
			this.BeginAttacking(this);
		}
	}

	protected void OnStopAttacking()
	{
		if (this.EndAttacking != null)
		{
			this.EndAttacking(this);
		}
	}

	public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
	{
		if ((target.Team != Team && Game.RoomType != eRoomType.SurvivalMode) || (Game.RoomType == eRoomType.SurvivalMode && targetId == target.Id))
		{
			CurrentIsHitTarget = true;
			TotalHurt += damageAmount + criticalAmount;
			if (!target.IsLiving)
			{
				TotalKill++;
			}
			m_game.CurrentTurnTotalDamage = damageAmount + criticalAmount;
			m_game.TotalHurt += damageAmount + criticalAmount;
		}
		if (this.AfterKillingLiving != null)
		{
			this.AfterKillingLiving(this, target, damageAmount, criticalAmount);
		}
	}

	public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount)
	{
		if (this.AfterKilledByLiving != null)
		{
			this.AfterKilledByLiving(this, target, damageAmount, criticalAmount);
		}
	}

	public void CallFuction(LivingCallBack func, int delay)
	{
		if (m_game != null)
		{
			m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
		}
	}

	public virtual int RemoveBlood(int value)
	{
		return AddBlood(-value, 1);
	}

	public virtual int RemoveMaxBlood(int value)
	{
		return AddMaxBlood(-value);
	}

	public int MakeDamage1(Living living0)
	{
		return MakeDamage(living0);
	}

	public virtual int ReduceBlood(int value, int emabletype = 0)
	{
		m_blood -= value;
		if (m_blood < 0)
		{
			m_blood = 0;
			Die();
		}
		m_game.UpdateBlood(this, 7, value, emabletype);
		return value;
	}
}
