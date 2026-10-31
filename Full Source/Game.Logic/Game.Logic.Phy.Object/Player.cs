using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using EntityDatabase.PlayerModels;
using game.logic.Game.Logic.PetSkills;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.ElementSkills;
using Game.Logic.ElfSkills.PassiveSkills;
using Game.Logic.GuardCoreEffect.EquipEffect;
using Game.Logic.HorseEffects;
using Game.Logic.MarkSkills;
using Game.Logic.PetSealSkills;
using Game.Logic.PetSealSkills.PassiveSkills;
using Game.Logic.PetSkills.ActiveSkills;
using Game.Logic.PetSkills.PassiveSkills;
using Game.Logic.Phy.Maths;
using Game.Logic.Spells;
using Newtonsoft.Json;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class Player : TurnedLiving
{
	public delegate void PlayerUsePetMPEventHandle(Player player, int value);

	private IGamePlayer m_player;

	private UsersPetInfo m_pet;

	private ItemInfo m_weapon;

	private ItemInfo m_DeputyWeapon;

	private ItemInfo m_Healstone;

	private int m_mainBallId;

	private int m_spBallId;

	private int m_AddWoundBallId;

	private int m_MultiBallId;

	private PetFightPropertyInfo _petFightProperty;

	private BallInfo m_currentBall;

	private int m_energy;

	private List<int> m_prop;

	public Point TargetPoint;

	public int GainGP;

	public int GainOffer;

	public bool LockDirection;

	public int TotalCure;

	private bool _canGetProp;

	private List<int> propsBloqueados;

	public int TotalAllHurt;

	public int TotalAllHitTargetCount;

	public int TotalAllShootCount;

	public int TotalAllKill;

	public int TotalAllExperience;

	public int TotalAllScore;

	public int TotalAllCure;

	public int CanTakeOut;

	public bool FinishTakeCard;

	public bool HasPaymentTakeCard;

	public int BossCardCount;

	public bool Ready;

	public int TotalCureEnergy;

	public bool LimitEnergy;

	public bool CanUsePetSkill;

	public int MaxSoulPropUsedCount = 2;

	public int SoulPropCount;

	public bool IsShadown;

	private Dictionary<int, PetSkillInfo> _petSkillCd;

	private Dictionary<int, MountSkillTemplateInfo> _horseSkillCd;

	private int[] m_horseSkillEquip;

	private BatleConfigInfo m_batleConfig;

	private List<int> banSkills;

	public int ratioPower;

	private int m_loadingProcess;

	private int m_shootCount;

	private int m_ballCount;

	private int m_changeSpecialball;

	private ArrayList m_tempBoxes = new ArrayList();

	private int[] buffs;

	private int deputyWeaponResCount = 0;

	private double subWeaponAddOrReduce = 0.0;

	private static readonly int CARRY_TEMPLATE_ID = 10016;

	private int _flyCoolDown;

	public bool movingenergy;

	public Dictionary<int, PetSkillInfo> PetSkillCD => _petSkillCd;

	public Dictionary<int, MountSkillTemplateInfo> HorseSkillCd => _horseSkillCd;

	public bool IsBlackFlame { get; set; }

	public int[] HorseSkillEquip => m_horseSkillEquip;

	public BatleConfigInfo BatleConfig => m_batleConfig;

	public int PowerRatio
	{
		get
		{
			return ratioPower;
		}
		set
		{
			ratioPower = value;
		}
	}

	public IGamePlayer PlayerDetail => m_player;

	public UsersPetInfo Pet => m_pet;

	public ItemInfo Weapon => m_weapon;

	public ItemInfo DeputyWeapon => m_DeputyWeapon;

	public List<int> Prop
	{
		get
		{
			return m_prop;
		}
		set
		{
			m_prop = value;
		}
	}

	public bool CanGetProp
	{
		get
		{
			return _canGetProp;
		}
		set
		{
			if (_canGetProp != value)
			{
				_canGetProp = value;
			}
		}
	}

	public int LoadingProcess
	{
		get
		{
			return m_loadingProcess;
		}
		set
		{
			if (m_loadingProcess != value)
			{
				m_loadingProcess = value;
				if (m_loadingProcess >= 100)
				{
					OnLoadingCompleted();
				}
			}
		}
	}

	public int Energy
	{
		get
		{
			return m_energy;
		}
		set
		{
			m_energy = value;
		}
	}

	public BallInfo CurrentBall => m_currentBall;

	public bool IsSpecialSkill => m_currentBall.ID == m_spBallId;

	public int ChangeSpecialBall
	{
		get
		{
			return m_changeSpecialball;
		}
		set
		{
			m_changeSpecialball = value;
		}
	}

	public int ShootCount
	{
		get
		{
			return m_shootCount;
		}
		set
		{
			if (m_shootCount != value)
			{
				m_shootCount = value;
				m_game.SendGameUpdateShootCount(this);
			}
		}
	}

	public int BallCount
	{
		get
		{
			return m_ballCount;
		}
		set
		{
			if (m_ballCount != value)
			{
				m_ballCount = value;
			}
		}
	}

	public int flyCount => _flyCoolDown;

	public int deputyWeaponCount => deputyWeaponResCount;

	public double SubWeaponAddOrReduce => subWeaponAddOrReduce;

	public bool jaUsouSkill { get; set; }

	public PlayerEventHandle PlayerGameStart { get; internal set; }

	public event PlayerUsePetMPEventHandle PlayerUsePetMP;

	public event PlayerEventHandle PlayerSkip;

	public event PlayerEventHandle BeforePlayerShoot;

	public event PlayerEventHandle PlayerAfterBuffSkillPet;

	public event PlayerEventHandle PlayerBuffSkill;

	public event PlayerEventHandle PlayerAnyShellThrow;

	public event PlayerEventHandle PlayerShootCure;

	public event PlayerEventHandle PlayerBeginMoving;

	public event PlayerEventHandle AfterPlayerShooted;

	public event PlayerEventHandle PlayerCompleteShoot;

	public event PlayerEventHandle LoadingCompleted;

	public event PlayerEventHandle PlayerShoot;

	public event PlayerSecondWeaponEventHandle PlayerUseSecondWeapon;

	public event PlayerEventHandle PlayerCure;

	public event PlayerEventHandle PlayerGuard;

	public event PlayerEventHandle PlayerBuffSkillPet;

	public event PlayerEventHandle PlayerClearBuffSkillPet;

	public event PlayerEventHandle PlayerBuffSkillHorse;

	public event PlayerEventHandle CollidByObject;

	public event PlayerEventHandle PlayerUseDander;

	public Player(IGamePlayer player, int id, BaseGame game, int team, int maxBlood)
		: base(id, game, team, "", "", maxBlood, 0, 1)
	{
		m_rect = new Rectangle(-15, -20, 30, 30);
		_petSkillCd = new Dictionary<int, PetSkillInfo>();
		_horseSkillCd = new Dictionary<int, MountSkillTemplateInfo>();
		m_player = player;
		m_player.GamePlayerId = id;
		_canGetProp = true;
		Grade = player.PlayerCharacter.Grade;
		TotalAllHurt = 0;
		TotalAllHitTargetCount = 0;
		TotalAllShootCount = 0;
		TotalAllKill = 0;
		TotalAllExperience = 0;
		TotalAllScore = 0;
		TotalAllCure = 0;
		m_loadingProcess = 0;
		ChangeSpecialBall = 0;
		EquipGhostReduceCritDamage = 0;
		m_prop = new List<int>();
		base.VaneOpen = player.PlayerCharacter.IsWeakGuildFinish(9);
		CanUsePetSkill = true;
		m_weapon = m_player.MainWeapon;
		m_DeputyWeapon = m_player.SecondWeapon;
		m_Healstone = m_player.Healstone;
		EffectBadges = false;
		m_pet = player.Pet;
		m_batleConfig = m_player.BatleConfig;
		m_horseSkillEquip = player.HorseSkillEquip;
		propsBloqueados = new List<int>();
		banSkills = new List<int>();
		base.CanAddBlood = true;
		if (game == null)
		{
			return;
		}
		InitEqupedEffect(m_player.EquipEffect);
		InitFightBuffer(player.FightBuffs);
		InitHourseSkillEffect();
		InitPetSealSkills();
		InitElementSkills();
		InitElfSkills();
		InitEquipGhostSkills();
		InitMarkSkills();
		InitTeamSkills();
		if (m_pet != null)
		{
			InitPetSkillEffect();
			_petFightProperty = PetMgr.FindFightProperty(player.PlayerCharacter.evolutionGrade);
		}
		if (PlayerDetail.PlayerCharacter.SecondPro != null)
		{
			Crit += PlayerDetail.PlayerCharacter.SecondPro.Crit;
			KillPow += PlayerDetail.PlayerCharacter.SecondPro.Kill;
			Guard += PlayerDetail.PlayerCharacter.SecondPro.Guard;
			Speed += PlayerDetail.PlayerCharacter.SecondPro.Speed;
			Tenacity += PlayerDetail.PlayerCharacter.SecondPro.Tenacity;
			WillFight += PlayerDetail.PlayerCharacter.SecondPro.WillFight;
			AvoidInjury += PlayerDetail.PlayerCharacter.SecondPro.AvoidInjury;
			SunderArmor += PlayerDetail.PlayerCharacter.SecondPro.SunderArmor;
			ViolenceInjury += PlayerDetail.PlayerCharacter.SecondPro.ViolenceInjury;
		}
		if (PlayerDetail.PlayerCharacter.ElementPro != null)
		{
			FireAttack += PlayerDetail.PlayerCharacter.ElementPro.FireAttack;
			FireDefence += PlayerDetail.PlayerCharacter.ElementPro.FireDefence;
			WaterAttack += PlayerDetail.PlayerCharacter.ElementPro.WaterAttack;
			WaterDefence += PlayerDetail.PlayerCharacter.ElementPro.WaterDefence;
			WindAttack += PlayerDetail.PlayerCharacter.ElementPro.WindAttack;
			WindDefence += PlayerDetail.PlayerCharacter.ElementPro.WindDefence;
			LandAttack += PlayerDetail.PlayerCharacter.ElementPro.LandAttack;
			LandDefence += PlayerDetail.PlayerCharacter.ElementPro.LandDefence;
		}
		if (m_DeputyWeapon != null)
		{
			deputyWeaponResCount = m_DeputyWeapon.StrengthenLevel + 1;
			SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo = SubWeaponEvolutionTemplateMgr.FindSubWeaponEvolutionByGp(m_DeputyWeapon.curExp);
			if (subWeaponEvolutionTemplateInfo != null)
			{
				subWeaponAddOrReduce = ((m_DeputyWeapon.Template.Property3 == 31) ? subWeaponEvolutionTemplateInfo.ReduceDamage : subWeaponEvolutionTemplateInfo.AddBlood);
			}
		}
		else
		{
			deputyWeaponResCount = 1;
		}
		if (m_weapon != null)
		{
			BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
			if (m_weapon.GoldValidDate())
			{
				ballConfigInfo = BallConfigMgr.FindBall(m_weapon.GoldEquip.TemplateID);
			}
			if (ballConfigInfo != null)
			{
				m_mainBallId = ballConfigInfo.Common;
				m_spBallId = ballConfigInfo.Special;
				m_AddWoundBallId = ballConfigInfo.CommonAddWound;
				m_MultiBallId = ballConfigInfo.CommonMultiBall;
			}
		}
	}

	private void InitTeamSkills()
	{
		int teamGrade = m_player.PlayerCharacter.Team.teamGrade;
		List<int> list = new List<int>();
		if (teamGrade == 0)
		{
			return;
		}
		switch (teamGrade)
		{
		case 1:
			list.Add(483);
			list.Add(490);
			break;
		case 2:
			list.Add(484);
			list.Add(491);
			break;
		case 3:
			list.Add(485);
			list.Add(492);
			break;
		case 4:
			list.Add(486);
			list.Add(493);
			break;
		case 5:
			list.Add(487);
			list.Add(494);
			break;
		case 6:
			list.Add(488);
			list.Add(495);
			break;
		case 7:
			list.Add(489);
			list.Add(496);
			break;
		}
		if (list == null)
		{
			return;
		}
		foreach (int item in list)
		{
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(item);
			if (petSkillInfo == null)
			{
				continue;
			}
			Console.WriteLine($"InitTeamSkills, UserID: {m_player.PlayerCharacter.ID}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
			string[] array = petSkillInfo.ElementIDs.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				switch (text)
				{
				case "2028":
				case "2029":
				case "2030":
				case "2031":
				case "2032":
				case "2033":
				case "2034":
					new TeamSkill_AddDamage(text).Start(this);
					break;
				case "2035":
				case "2036":
				case "2037":
				case "2038":
				case "2039":
				case "2040":
				case "2041":
					new ShowPic(text).Start(this);
					break;
				}
			}
		}
	}

	private void InitMarkSkills()
	{
		List<int> markSkill = MarkSkillMgr.GetMarkSkill(m_player.PlayerCharacter.ID);
		if (markSkill == null)
		{
			return;
		}
		foreach (int item in markSkill)
		{
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(item);
			if (petSkillInfo == null)
			{
				continue;
			}
			Console.WriteLine($"InitMarkSkills, UserID: {m_player.PlayerCharacter.ID}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
			string[] array = petSkillInfo.ElementIDs.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				switch (text)
				{
				case "1633":
					new AddBaseGuardAfterKilled(text).Start(this);
					break;
				case "1635":
					new ReduceBaseDamageOnMakeDamage(text).Start(this);
					break;
				case "1636":
					new AddMagicAttackOnMakeDamage(text).Start(this);
					break;
				case "1638":
					new AddDamageOnMakeDamage(text).Start(this);
					break;
				case "1692":
				case "1706":
				case "1822":
				case "1845":
				case "2598":
					new ShowPic(text).Start(this);
					break;
				}
			}
		}
	}

	private void GetMarkSkillValue(int HP, double Damage, ref int HP_, ref double Damage_)
	{
		List<int> markSkill = MarkSkillMgr.GetMarkSkill(m_player.PlayerCharacter.ID);
		if (markSkill == null)
		{
			return;
		}
		foreach (int item in markSkill)
		{
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(item);
			if (petSkillInfo == null)
			{
				continue;
			}
			string[] array = petSkillInfo.ElementIDs.Split(',');
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				switch (array2[i])
				{
				case "1692":
				case "1845":
					HP_ += HP * 7 / 100;
					Damage_ += Damage * 7.0 / 100.0;
					break;
				case "1706":
					Damage_ += Damage * 6.0 / 100.0;
					AddBloodValue += 4;
					break;
				case "1822":
					Damage_ += Damage * 5.0 / 100.0;
					AddCureValue += 3;
					break;
				case "2598":
					Damage_ += Damage * 4.0 / 100.0;
					AddCureValue += 4;
					break;
				}
			}
		}
	}

	private void InitEquipGhostSkills()
	{
		Dictionary<string, UserEquipGhostInfo> dictionary = JsonConvert.DeserializeObject<Dictionary<string, UserEquipGhostInfo>>(PlayerDetail.PlayerCharacter.GhostEquipList) ?? new Dictionary<string, UserEquipGhostInfo>();
		if (dictionary == null)
		{
			return;
		}
		SpiritSkillMgr.Init();
		foreach (UserEquipGhostInfo value in dictionary.Values)
		{
			int spiritSkill = SpiritSkillMgr.GetSpiritSkill(value.BagType, value.Place, value.Level);
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(spiritSkill);
			if (petSkillInfo == null)
			{
				continue;
			}
			Console.WriteLine($"InitEquipGhostSkills, UserID: {m_player.PlayerCharacter.ID}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
			string[] array = petSkillInfo.ElementIDs.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				switch (text)
				{
				case "2101":
				case "2102":
				case "2103":
				case "2104":
				case "2105":
				case "2106":
				case "2107":
				case "2108":
				case "2109":
				case "2110":
					new EquipGhostSkill_AddDanderDamage(text).Start(this);
					break;
				case "2201":
					EquipGhostReduceCritDamage = 5;
					break;
				case "2202":
					EquipGhostReduceCritDamage = 10;
					break;
				case "2203":
					EquipGhostReduceCritDamage = 15;
					break;
				case "2204":
					EquipGhostReduceCritDamage = 20;
					break;
				case "2205":
					EquipGhostReduceCritDamage = 25;
					break;
				case "2206":
					EquipGhostReduceCritDamage = 30;
					break;
				case "2207":
					EquipGhostReduceCritDamage = 35;
					break;
				case "2208":
					EquipGhostReduceCritDamage = 40;
					break;
				case "2209":
					EquipGhostReduceCritDamage = 45;
					break;
				case "2210":
					EquipGhostReduceCritDamage = 50;
					break;
				case "2401":
				case "2402":
				case "2403":
				case "2404":
				case "2405":
				case "2406":
				case "2407":
				case "2408":
				case "2409":
				case "2410":
					new EquipGhostSkill_AddMagicDefence(text).Start(this);
					break;
				}
			}
		}
	}

	public override void Reset()
	{
		if (m_game.RoomType == eRoomType.Dungeon || m_game.RoomType == eRoomType.SpecialActivityDungeon || m_game.RoomType == eRoomType.SurvivalMode || m_game.RoomType == eRoomType.Element_PVE)
		{
			m_game.Cards = new int[21];
		}
		else
		{
			m_game.Cards = new int[9];
		}
		base.Dander = ((m_game.RoomType == eRoomType.ConsortiaBattle && m_player.PlayerCharacter.ActivePowFirstGame) ? 200 : 0);
		base.PetMP = 10;
		base.psychic = 0;
		base.IsLiving = true;
		FinishTakeCard = false;
		m_Healstone = m_player.Healstone;
		m_changeSpecialball = 0;
		m_DeputyWeapon = m_player.SecondWeapon;
		if (m_DeputyWeapon != null)
		{
			deputyWeaponResCount = m_DeputyWeapon.StrengthenLevel + 1;
			SubWeaponEvolutionTemplateInfo subWeaponEvolutionTemplateInfo = SubWeaponEvolutionTemplateMgr.FindSubWeaponEvolutionByGp(m_DeputyWeapon.curExp);
			if (subWeaponEvolutionTemplateInfo != null)
			{
				subWeaponAddOrReduce = ((m_DeputyWeapon.Template.Property3 == 31) ? subWeaponEvolutionTemplateInfo.ReduceDamage : subWeaponEvolutionTemplateInfo.AddBlood);
			}
		}
		else
		{
			deputyWeaponResCount = 1;
		}
		m_weapon = m_player.MainWeapon;
		BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
		if (ballConfigInfo != null)
		{
			m_mainBallId = ballConfigInfo.Common;
			m_spBallId = ballConfigInfo.Special;
			m_AddWoundBallId = ballConfigInfo.CommonAddWound;
			m_MultiBallId = ballConfigInfo.CommonMultiBall;
		}
		else
		{
			Console.WriteLine("systerm do not suport this weapon ID {0}, enter db and check dbo.BallConfig", m_weapon.TemplateID);
		}
		m_maxBlood = m_player.PlayerCharacter.hp;
		BaseDamage = m_player.BaseAttack;
		BaseGuard = m_player.BaseDefence;
		Attack = m_player.PlayerCharacter.Attack;
		Defence = m_player.PlayerCharacter.Defence;
		Agility = m_player.PlayerCharacter.Agility;
		Lucky = m_player.PlayerCharacter.Luck;
		MagicAttack = m_player.PlayerCharacter.MagicAttack;
		MagicDefence = m_player.PlayerCharacter.MagicDefence;
		IsAddTurn = false;
		BaseDamage += m_player.PlayerCharacter.DameAddPlus + base.FightBuffers.WorldBossAttrack_MoneyBuff;
		if (base.FightBuffers.ConsortionAddDamage > 0)
		{
			BaseDamage += base.FightBuffers.ConsortionAddDamage;
		}
		if (m_game.RoomType == eRoomType.ConsortiaBattle)
		{
			Attack += Attack / 100.0 * (double)m_player.PlayerCharacter.AttPlusGuildBattle;
			Agility += Agility / 100.0 * (double)m_player.PlayerCharacter.AgiPlusGuildBattle;
		}
		BaseGuard += m_player.PlayerCharacter.GuardAddPlus;
		Attack += m_player.PlayerCharacter.AttackAddPlus;
		Defence += m_player.PlayerCharacter.DefendAddPlus;
		Agility += m_player.PlayerCharacter.AgiAddPlus;
		Lucky += m_player.PlayerCharacter.LuckAddPlus;
		if (_petFightProperty != null)
		{
			Attack += _petFightProperty.Attack;
			Defence += _petFightProperty.Defence;
			Agility += _petFightProperty.Agility;
			Lucky += _petFightProperty.Lucky;
			m_maxBlood += _petFightProperty.Blood;
		}
		Attack += m_player.PlayerCharacter.StrengthEnchance;
		Defence += m_player.PlayerCharacter.StrengthEnchance;
		Agility += m_player.PlayerCharacter.StrengthEnchance;
		Lucky += m_player.PlayerCharacter.StrengthEnchance;
		if (base.FightBuffers.ConsortionAddMaxBlood > 0)
		{
			m_maxBlood += m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood / 100;
		}
		m_maxBlood += m_player.PlayerCharacter.HpAddPlus + base.FightBuffers.WorldBossHP + base.FightBuffers.WorldBossHP_MoneyBuff;
		if (m_player.PlayerCharacter.ringFlag)
		{
			m_maxBlood += m_player.PlayerCharacter.loveBuffValue;
		}
		m_maxBlood += base.ExtendMaxBlood + ((base.PetEffects != null) ? base.PetEffects.AddMaxBloodValue : 0);
		int teamGrade = m_player.PlayerCharacter.Team.teamGrade;
		if (teamGrade != 0 && base.Game is PVPGame)
		{
			int num = 0;
			switch (teamGrade)
			{
			case 1:
				num = 5;
				break;
			case 2:
				num = 6;
				break;
			case 3:
				num = 7;
				break;
			case 4:
				num = 8;
				break;
			case 5:
				num = 9;
				break;
			case 6:
				num = 10;
				break;
			case 7:
				num = 12;
				break;
			}
			m_maxBlood += m_maxBlood * num / 100;
		}
		int HP_ = 0;
		double Damage_ = 0.0;
		GetMarkSkillValue(m_maxBlood, BaseDamage, ref HP_, ref Damage_);
		m_maxBlood += HP_;
		BaseDamage += Damage_;
		if (base.FightBuffers.ConsortionAddProperty > 0)
		{
			Attack += base.FightBuffers.ConsortionAddProperty;
			Defence += base.FightBuffers.ConsortionAddProperty;
			Agility += base.FightBuffers.ConsortionAddProperty;
			Lucky += base.FightBuffers.ConsortionAddProperty;
		}
		if (m_game.RoomType == eRoomType.Element_PVE)
		{
			m_maxBlood = 30000;
			Attack = 4000.0;
			Defence = 4000.0;
			Agility = 4000.0;
			Lucky = 4000.0;
			MagicAttack = 0.0;
			MagicDefence = 0.0;
			BaseDamage = 1000.0;
			BaseGuard = 800.0;
		}
		if (m_game.RoomType == eRoomType.TEAM_ROOM)
		{
			m_maxBlood = 30000 + m_maxBlood * 8 / 100;
			Attack = 2000.0 + Attack * 0.15;
			Defence = 2000.0 + Defence * 0.15;
			Agility = 2000.0 + Agility * 0.15;
			Lucky = 2000.0 + Lucky * 0.15;
			BaseDamage = 2000.0 + BaseDamage * 0.1;
			BaseGuard = 2000.0 + BaseGuard * 0.1;
			MagicAttack = 0.0;
			MagicDefence = 0.0;
			FireAttack *= 0.1;
			FireDefence *= 0.1;
			WaterAttack *= 0.1;
			WaterDefence *= 0.1;
			LandAttack *= 0.1;
			LandDefence *= 0.1;
			WindAttack *= 0.1;
			WindDefence *= 0.1;
			PlayerInfo playerCharacter = m_player.PlayerCharacter;
			_ = playerCharacter.EmblemInfoAttackSKill_Value;
			playerCharacter.EmblemInfoAttackSKill_Value = 0;
			PlayerInfo playerCharacter2 = m_player.PlayerCharacter;
			_ = playerCharacter2.EmblemInfoDefendSKill_Value;
			playerCharacter2.EmblemInfoDefendSKill_Value = 0;
		}
		if (m_player.PvePowerBuff != null && m_player.PvePowerBuff.IsActive() && (m_game.RoomType != eRoomType.Freedom || GameProperties.batismoNoDesafio))
		{
			Attack += Math.Floor((double)m_player.PvePowerBuff.getBuffAtk * 0.5 / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			Defence += Math.Floor((double)m_player.PvePowerBuff.getBuffDef * 0.5 / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			Agility += Math.Floor((double)m_player.PvePowerBuff.getBuffAgl / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			Lucky += Math.Floor((double)m_player.PvePowerBuff.getBuffLuck / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			BaseDamage += Math.Floor((double)m_player.PvePowerBuff.getBuffDmg * 0.5 / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			BaseGuard += Math.Floor((double)m_player.PvePowerBuff.getBuffAr / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			m_maxBlood += (int)Math.Floor((double)m_player.PvePowerBuff.getBuffHp / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			MagicAttack += Math.Floor((double)m_player.PvePowerBuff.getBuffMAtk * 0.5 / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			MagicDefence += Math.Floor((double)m_player.PvePowerBuff.getBuffMDef / 1000.0 * (double)GameProperties.BlessBuffAddRate);
			m_player.SendMessage("您的洗礼效果已经在战斗中生效.");
		}
		m_energy = (int)Agility / 30 + 240;
		if (base.FightBuffers.ConsortionAddEnergy > 0)
		{
			m_energy += base.FightBuffers.ConsortionAddEnergy;
		}
		ratioPower = 100 - base.FightBuffers.ConsortionReduceEnergyUse;
		m_currentBall = BallMgr.FindBall(m_mainBallId);
		m_shootCount = 1;
		m_ballCount = 1;
		CurrentIsHitTarget = false;
		LimitEnergy = false;
		TotalCure = 0;
		TotalHitTargetCount = 0;
		TotalHurt = 0;
		TotalKill = 0;
		TotalShootCount = 0;
		LockDirection = false;
		GainGP = 0;
		GainOffer = 0;
		Ready = false;
		m_delay = GetTurnDelay();
		LoadingProcess = 0;
		SoulPropCount = 0;
		base.OldAttack = Attack;
		base.OldDefence = Defence;
		base.OldAgility = Agility;
		base.OldLucky = Lucky;
		AddBloodValue = 0;
		AddCureValue = 0;
		base.OldMagicAttack = MagicAttack;
		base.OldMagicDefende = MagicDefence;
		base.OldBaseDamage = BaseDamage;
		base.OldBaseGuard = BaseGuard;
		base.CanAddBlood = true;
		CanFrozen = true;
		m_prop.Clear();
		PlayerDetail.ClearTempBag();
		ResetSkillCd();
		base.Reset();
	}

	public void ResetSkillCd()
	{
		if (m_pet != null)
		{
			string[] array = m_pet.SkillEquip.Split('|');
			foreach (string text in array)
			{
				int key = int.Parse(text.Split(',')[0]);
				if (_petSkillCd.ContainsKey(key))
				{
					_petSkillCd[key].Turn = 0;
				}
			}
		}
		int[] horseSkillEquip = HorseSkillEquip;
		foreach (int key2 in horseSkillEquip)
		{
			if (_horseSkillCd.ContainsKey(key2))
			{
				_horseSkillCd[key2].Turn = 0;
				_horseSkillCd[key2].Count = 0;
			}
		}
	}

	public void InitGuardCoreEffect()
	{
		GuardCoreTemplateInfo guardCoreTemplateInfo = GuardCoreMgr.FindGuardCoreTemplate(m_player.PlayerCharacter.guardCoreID);
		if (guardCoreTemplateInfo != null && base.Game.GuardCoreEnable())
		{
			switch (guardCoreTemplateInfo.ID)
			{
			case 20101:
			case 20102:
				new ResistanceSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20201:
			case 20202:
				new MagicSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20301:
			case 20302:
				new HealthSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20401:
			case 20402:
				new LuckSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20501:
			case 20502:
			case 20503:
			case 20504:
				new RestorationSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20601:
			case 20602:
				new SteelSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20701:
			case 20702:
				new KingofSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			case 20801:
			case 20802:
			case 20803:
			case 20804:
				new WeakSoulEffect(guardCoreTemplateInfo).Start(this);
				break;
			}
		}
	}

	public void InitHourseSkillEffect()
	{
		MountSkillTemplateInfo mountSkillTemplateInfo = null;
		int[] horseSkillEquip = HorseSkillEquip;
		foreach (int num in horseSkillEquip)
		{
			mountSkillTemplateInfo = MountMgr.GetMountSkillTemplate(num);
			if (mountSkillTemplateInfo == null)
			{
				continue;
			}
			MountSkillTemplateInfo mountSkillTemplateInfo2 = new MountSkillTemplateInfo();
			mountSkillTemplateInfo2.Clone(mountSkillTemplateInfo);
			if (!_horseSkillCd.ContainsKey(num))
			{
				_horseSkillCd.Add(num, mountSkillTemplateInfo2);
			}
			string[] array = mountSkillTemplateInfo2.ElementIDs.Split(',');
			int probability = mountSkillTemplateInfo2.Probability;
			int delay = mountSkillTemplateInfo2.Delay;
			int gameType = mountSkillTemplateInfo2.GameType;
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					switch (text)
					{
					case "10101":
					case "10102":
					case "10103":
					case "10104":
					case "10105":
					case "10106":
					case "10107":
					case "10108":
					case "10109":
					case "10110":
						new HorseBuffHPEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10201":
					case "10202":
					case "10203":
					case "10204":
					case "10205":
					case "10206":
					case "10207":
					case "10208":
					case "10209":
					case "10210":
						new HorseBuffHPTeamEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10401":
					case "10402":
					case "10403":
					case "10404":
					case "10405":
						new HorseBuffDanderEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "11401":
					case "11402":
					case "11403":
					case "11404":
					case "11405":
						new HorseAddDamageNextDanderEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10301":
					case "10302":
					case "10303":
					case "10304":
					case "10305":
					case "10306":
					case "10307":
					case "10308":
					case "10309":
					case "10310":
						new HorseRemoveGuardEnemyEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10501":
					case "10502":
					case "10503":
					case "10504":
					case "10505":
						new HorseReviveTeamEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10601":
					case "10602":
					case "10603":
					case "10604":
					case "10605":
					case "10606":
					case "10607":
					case "10608":
					case "10609":
					case "10610":
						new HorseRemoveRandomEffectEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10701":
					case "10702":
					case "10703":
					case "10704":
					case "10705":
						new HorseFrostEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10801":
					case "10802":
					case "10803":
					case "10804":
					case "10805":
						new HorseControlBallEquipEffect(0, probability, gameType, num, delay, text).Start(this);
						break;
					case "10901":
					case "10902":
					case "10903":
					case "10904":
					case "10905":
						new HorseHideEquipEffect(3, probability, gameType, num, delay, text).Start(this);
						break;
					case "11001":
					case "11002":
					case "11003":
					case "11004":
					case "11005":
						new HorseHideTeamEquipEffect(2, probability, gameType, num, delay, text).Start(this);
						break;
					case "11101":
					case "11102":
					case "11103":
					case "11104":
					case "11105":
					case "11106":
					case "11107":
					case "11108":
					case "11109":
					case "11110":
						new HorseAddDamageEquipEffect(2, probability, gameType, num, delay, text).Start(this);
						break;
					case "11201":
					case "11202":
					case "11203":
					case "11204":
					case "11205":
						new HorseNoHoleEquipEffect(1, probability, gameType, num, delay, text).Start(this);
						break;
					case "11301":
					case "11302":
					case "11303":
					case "11304":
					case "11305":
					case "11306":
					case "11307":
					case "11308":
					case "11309":
					case "11310":
						new HorseReduceDamageEquipEffect(2, probability, gameType, num, delay, text).Start(this);
						break;
					}
				}
			}
		}
	}

	public void InitFightBuffer(List<BufferInfo> buffers)
	{
		foreach (BufferInfo buffer in buffers)
		{
			switch (buffer.Type)
			{
			case 400:
				base.FightBuffers.WorldBossHP = buffer.Value;
				break;
			case 401:
				base.FightBuffers.WorldBossAttrack = buffer.Value;
				break;
			case 402:
				base.FightBuffers.WorldBossHP_MoneyBuff = buffer.Value;
				break;
			case 403:
				base.FightBuffers.WorldBossAttrack_MoneyBuff = buffer.Value;
				break;
			case 404:
				base.FightBuffers.WorldBossMetalSlug = buffer.Value;
				break;
			case 405:
				base.FightBuffers.WorldBossAncientBlessings = buffer.Value;
				break;
			case 406:
				base.FightBuffers.WorldBossAddDamage = buffer.Value;
				break;
			case 101:
				base.FightBuffers.ConsortionAddBloodGunCount = buffer.Value;
				break;
			case 102:
				base.FightBuffers.ConsortionAddDamage = buffer.Value;
				break;
			case 103:
				base.FightBuffers.ConsortionAddCritical = buffer.Value;
				break;
			case 104:
				base.FightBuffers.ConsortionAddMaxBlood = buffer.Value;
				break;
			case 105:
				base.FightBuffers.ConsortionAddProperty = buffer.Value;
				break;
			case 106:
				base.FightBuffers.ConsortionReduceEnergyUse = buffer.Value;
				break;
			case 107:
				base.FightBuffers.ConsortionAddEnergy = buffer.Value;
				break;
			case 108:
				base.FightBuffers.ConsortionAddEffectTurn = buffer.Value;
				break;
			case 109:
				base.FightBuffers.ConsortionAddOfferRate = buffer.Value;
				break;
			case 110:
				base.FightBuffers.ConsortionAddPercentGoldOrGP = buffer.Value;
				break;
			case 111:
				base.FightBuffers.ConsortionAddSpellCount = buffer.Value;
				break;
			case 112:
				base.FightBuffers.ConsortionReduceDander = buffer.Value;
				break;
			default:
				Console.WriteLine($"Not Found FightBuff Type {buffer.Type} Value {buffer.Value}");
				break;
			}
		}
	}

	public void InitElfSkills()
	{
		if (m_player.PlayerCharacter.ElfInfo == null)
		{
			return;
		}
		Sys_Users_Elf sys_Users_Elf = m_player.PlayerCharacter.ElfInfo.FirstOrDefault((Sys_Users_Elf e) => e.IsFight);
		if (sys_Users_Elf == null || string.IsNullOrEmpty(sys_Users_Elf.SkillList))
		{
			return;
		}
		foreach (int item in sys_Users_Elf.SkillList.Split(',').Select(int.Parse))
		{
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(item);
			if (petSkillInfo == null)
			{
				continue;
			}
			Console.WriteLine($"InitElfSkills, UserID: {m_player.GamePlayerId}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
			string[] array = petSkillInfo.ElementIDs.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				switch (text)
				{
				case "2766":
				case "2767":
				case "2768":
				case "2790":
				case "2791":
				case "2792":
					new ElfAddFireAttack_Passive(text).Start(this);
					break;
				case "2769":
				case "2770":
				case "2771":
				case "2793":
				case "2794":
				case "2795":
					new ElfAddWaterAttack_Passive(text).Start(this);
					break;
				case "2772":
				case "2773":
				case "2774":
				case "2796":
				case "2797":
				case "2798":
					new ElfAddWindAttack_Passive(text).Start(this);
					break;
				case "2775":
				case "2776":
				case "2777":
				case "2799":
				case "2800":
				case "2801":
					new ElfAddLandAttack_Passive(text).Start(this);
					break;
				case "2778":
				case "2779":
				case "2780":
				case "2802":
				case "2803":
				case "2804":
					new ElfAddFireDefence_Passive(text).Start(this);
					break;
				case "2781":
				case "2782":
				case "2783":
				case "2805":
				case "2806":
				case "2807":
					new ElfAddWaterDefence_Passive(text).Start(this);
					break;
				case "2784":
				case "2785":
				case "2786":
				case "2808":
				case "2809":
				case "2810":
					new ElfAddWindDefence_Passive(text).Start(this);
					break;
				case "2787":
				case "2788":
				case "2789":
				case "2811":
				case "2812":
				case "2813":
					new ElfAddLandDefence_Passive(text).Start(this);
					break;
				case "2814":
				case "2815":
				case "2816":
				case "2817":
				case "2818":
				case "2819":
					new ElfAddElementSkillDamage_Passive(text, 817, isRate: false).Start(this);
					break;
				case "2820":
				case "2821":
				case "2822":
					new ElfAddElementSkillDamage_Passive(text, 817, isRate: true).Start(this);
					break;
				case "2829":
				case "2830":
				case "2831":
				case "2832":
				case "2833":
				case "2834":
					new ElfAddElementSkillDamage_Passive(text, 818, isRate: false).Start(this);
					break;
				case "2835":
				case "2836":
				case "2837":
					new ElfAddElementSkillDamage_Passive(text, 818, isRate: true).Start(this);
					break;
				case "2844":
				case "2845":
				case "2846":
				case "2847":
				case "2848":
				case "2849":
					new ElfAddElementSkillDamage_Passive(text, 819, isRate: false).Start(this);
					break;
				case "2850":
				case "2851":
				case "2852":
					new ElfAddElementSkillDamage_Passive(text, 819, isRate: true).Start(this);
					break;
				case "2856":
				case "2857":
				case "2858":
				case "2859":
				case "2860":
				case "2861":
					new ElfAddElementSkillDamage_Passive(text, 820, isRate: false).Start(this);
					break;
				default:
					Console.WriteLine($"Not Found ElfSkill ElementID: {text}");
					break;
				}
			}
		}
	}

	public void InitElementSkills()
	{
		if (m_player.PlayerCharacter.EmblemInfoAttackSKill == 0)
		{
			return;
		}
		PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(m_player.PlayerCharacter.EmblemInfoAttackSKill);
		if (petSkillInfo == null)
		{
			return;
		}
		Console.WriteLine($"InitElementSkills, UserID: {m_player.GamePlayerId}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
		string[] array = petSkillInfo.ElementIDs.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			switch (text)
			{
			case "2634":
				new ElementFlamingGrilling(m_player.PlayerCharacter.EmblemInfoAttackSKill, m_player.PlayerCharacter.EmblemInfoAttackSKill_Value, m_player.PlayerCharacter.EmblemInfoAttackSKill_Rate).Start(this);
				break;
			case "2635":
				new ElementForceFreeze(m_player.PlayerCharacter.EmblemInfoAttackSKill, m_player.PlayerCharacter.EmblemInfoAttackSKill_Value).Start(this);
				break;
			case "2636":
				new ElementLightningRadiation(m_player.PlayerCharacter.EmblemInfoAttackSKill, m_player.PlayerCharacter.EmblemInfoAttackSKill_Value, m_player.PlayerCharacter.EmblemInfoAttackSKill_Rate).Start(this);
				break;
			case "2637":
				new ElementPowerOfTheEarth().Start(this);
				break;
			case "2641":
				new ElementEarthSpirit().Start(this);
				break;
			default:
				Console.WriteLine($"Not Found ElementSkill ElementID: {text}");
				break;
			}
		}
	}

	public void InitPetSealSkills()
	{
		if (m_player.PlayerCharacter.CurrentSealSkill == 0)
		{
			return;
		}
		PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(m_player.PlayerCharacter.CurrentSealSkill);
		if (petSkillInfo == null)
		{
			return;
		}
		Console.WriteLine($"InitPetSealSkills, UserID: {m_player.GamePlayerId}, SkillId:{petSkillInfo.ID}, SkillName:{petSkillInfo.Name}, SkillElementIDs: {petSkillInfo.ElementIDs}");
		string[] array = petSkillInfo.ElementIDs.Split(',');
		string[] array2 = array;
		foreach (string text in array2)
		{
			switch (text)
			{
			case "2418":
			case "2419":
			case "2420":
				new AddBaseDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2421":
				new Guide_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2422":
			case "2423":
			case "2424":
				new AddBaseGuard_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2425":
			case "2426":
			case "2427":
				new ReduceDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2567":
			case "2568":
			case "2569":
				new BackDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2440":
			case "2442":
			case "2444":
				new AddDefence_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2441":
			case "2443":
			case "2445":
				new AddBaseGuard_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2446":
			case "2447":
			case "2448":
				new AddAttackForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2449":
			case "2450":
			case "2451":
				new AddDefenceForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2452":
			case "2453":
			case "2454":
				new AddAgilityForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2455":
			case "2456":
			case "2457":
				new AddLuckForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2461":
			case "2462":
			case "2463":
				new AddBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2467":
			case "2468":
			case "2469":
				new ReduceTargetBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2470":
			case "2471":
			case "2472":
				new AddBaseDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2473":
			case "2474":
			case "2475":
				new AddAgility_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2476":
			case "2477":
			case "2478":
				new AddCritRate_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2479":
			case "2480":
			case "2481":
				new AddAttackForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2482":
			case "2483":
			case "2484":
			case "2485":
			case "2486":
			case "2487":
				new CorrosionTarget_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2488":
			case "2489":
			case "2490":
				new AddMaxBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2491":
			case "2492":
			case "2493":
				new ReduceTargetMp_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2497":
			case "2498":
			case "2499":
				new AddCritRate_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2500":
			case "2502":
			case "2504":
				new AddDefence_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2501":
			case "2503":
			case "2505":
				new AddBaseGuard_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2506":
			case "2507":
			case "2508":
				new AddBaseDamageForTeam_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2509":
				new SealOffTargetPetSkill_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2510":
				new RecoverBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2512":
				new MoveProhibition_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2513":
			case "2514":
			case "2515":
				new AddViolenceInjury_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2516":
			case "2517":
			case "2518":
				new PetReduceAllTargetMagicDefence_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2519":
			case "2520":
			case "2521":
			case "2535":
			case "2536":
			case "2537":
				new AddBaseDamageWithBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2522":
			case "2526":
			case "2530":
			case "2538":
			case "2542":
			case "2546":
				new ReduceDefenceBeforeMakeDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2523":
			case "2527":
			case "2531":
			case "2539":
			case "2543":
			case "2547":
				new ReduceBaseGuradBeforeMakeDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2524":
				new LockDirection_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2607":
				new ShowPic(text).Start(this);
				break;
			case "2551":
			case "2552":
			case "2553":
				new AddSpeed_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: false).Start(this);
				break;
			case "2554":
				new Blind_Seal(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2555":
				new ProhibitionAddBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2556":
			case "2557":
			case "2558":
				new AddTurnMp_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text).Start(this);
				break;
			case "2608":
			case "2609":
			case "2610":
				new AddBaseDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2611":
			case "2612":
			case "2613":
				new AddMagicAttack_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2618":
			case "2620":
			case "2622":
				new AddMaxBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2710":
			case "2711":
			case "2712":
				new AddMaxBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "3142":
			case "3143":
			case "3144":
				new AddMaxBlood_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "2707":
			case "2708":
			case "2709":
				new ReduceDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			case "3163":
			case "3164":
			case "3165":
				new ReduceDamage_SealPassive(m_player.PlayerCharacter.CurrentSealSkill, text, isPercent: true).Start(this);
				break;
			default:
				Console.WriteLine($"Not Found PetSealSkill ElementID: {text}");
				break;
			case "2458":
			case "2459":
			case "2460":
			case "2464":
			case "2465":
			case "2466":
				break;
			}
		}
	}

	public void InitPetSkillEffect()
	{
		string[] array = m_pet.SkillEquip.Split('|');
		PetSkillInfo petSkillInfo = null;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = int.Parse(text.Split(',')[0]);
			petSkillInfo = PetMgr.FindPetSkill(num);
			if (petSkillInfo == null)
			{
				continue;
			}
			string[] array3 = petSkillInfo.ElementIDs.Split(',');
			int coldDown = petSkillInfo.ColdDown;
			int probability = petSkillInfo.Probability;
			int delay = petSkillInfo.Delay;
			int gameType = petSkillInfo.GameType;
			if (!_petSkillCd.ContainsKey(num))
			{
				_petSkillCd.Add(num, petSkillInfo);
			}
			Console.WriteLine($"InitPetSkillEffect, m_pet.UserID: {m_pet.UserID}, Name: {m_pet.Name},skillInfo.Name: {petSkillInfo.Name}, skillInfo.ElementIDs: {petSkillInfo.ElementIDs}");
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				if (!string.IsNullOrEmpty(text2))
				{
					switch (text2)
					{
					case "1122":
					case "1123":
					case "1124":
					case "1139":
					case "1149":
					case "1325":
					case "1326":
					case "1327":
					case "1542":
					case "1543":
					case "1544":
					case "1578":
					case "1579":
					case "1580":
					case "1639":
					case "1640":
					case "1641":
					case "2570":
					case "2571":
					case "2572":
					case "2642":
					case "2643":
					case "2644":
					case "2749":
					case "2750":
					case "2751":
					case "2953":
					case "2954":
					case "2955":
					case "3112":
					case "3113":
					case "3114":
					case "3166":
					case "3167":
					case "3168":
					case "3208":
					case "3209":
					case "3210":
					case "3550":
					case "3551":
					case "3552":
					case "3568":
					case "3569":
					case "3570":
					case "3806":
					case "3807":
					case "3808":
					case "1616":
					case "1506":
					case "1532":
					case "1494":
					case "1481":
					case "1473":
					case "1462":
					case "3522":
						new PetAfterPlayerShooted_Passive(probability, text2).Start(this);
						break;
					case "1039":
					case "1133":
					case "1134":
					case "1328":
					case "1329":
					case "1330":
					case "1418":
					case "1419":
					case "1420":
					case "1548":
					case "1549":
					case "1550":
					case "1584":
					case "1585":
					case "1586":
					case "1645":
					case "1646":
					case "1647":
					case "2573":
					case "2574":
					case "2575":
					case "2646":
					case "2649":
					case "2651":
					case "2752":
					case "2753":
					case "2754":
					case "2958":
					case "2959":
					case "2960":
					case "3170":
					case "3172":
					case "3173":
					case "3212":
					case "3214":
					case "3215":
					case "3553":
					case "3554":
					case "3555":
					case "3571":
					case "3572":
					case "3573":
					case "3132":
					case "3133":
					case "3134":
					case "3810":
					case "3811":
					case "3812":
						new PetMakeDamagePercent(probability, num, text2).Start(this);
						break;
					case "1138":
					case "1551":
					case "3115":
					case "3169":
					case "2647":
					case "3211":
					case "3813":
						new PetCritRate100(probability, num, text2).Start(this);
						break;
					case "1038":
						new PetFatalEffect(1, probability, num, delay, text2).Start(this);
						break;
					case "1492":
						new PetFatalEffect(2, probability, num, delay, text2).Start(this);
						break;
					case "1287":
						new PetDisibleActiveSkill(2, probability, num, text2).Start(this);
						break;
					case "1533":
						new PetDisibleActiveSkill(3, probability, num, text2).Start(this);
						break;
					case "2956":
						new PetAddTurn(120, probability, num, text2).Start(this);
						break;
					case "1082":
						new PetAlwayNoHoleBeforeMove(probability, num, text2).Start(this);
						break;
					case "1348":
						new PetClearV3Battery(probability, num, text2).Start(this);
						break;
					case "1188":
					case "1520":
						new PetClearHellIce(probability, num, text2).Start(this);
						break;
					case "1338":
						new PetClearSpotShoot(probability, num, text2).Start(this);
						break;
					case "1017":
						new PetStopMoving(2, probability, num, text2).Start(this);
						break;
					case "1021":
						new PetNoHole(2, probability, num, text2).Start(this);
						break;
					case "1280":
						new PetHide(2, probability, num, text2).Start(this);
						break;
					case "1288":
						new PetMoveToRandomMapPoint(probability, num, text2).Start(this);
						break;
					case "1161":
					case "1162":
					case "2668":
					case "2669":
					case "3217":
					case "3227":
						new PetDamageAllEnemy(probability, num, text2).Start(this);
						break;
					case "1323":
					case "1324":
						new PetSpikeEnemy(probability, num, text2).Start(this);
						break;
					case "1222":
					case "1503":
					case "3159":
						new PetStopTrageMoving(2, probability, num, text2).Start(this);
						break;
					case "2670":
						new PetNoFly(2, probability, num, text2).Start(this);
						break;
					case "1436":
					case "1437":
						new PetHavocInHeaven(probability, num, text2).Start(this);
						break;
					case "1426":
						new PetSkipAllTrageTurn(1, probability, num, text2).Start(this);
						break;
					case "1432":
						new PetLookedHidePlayers(2, probability, num, text2).Start(this);
						break;
					case "1433":
					case "1511":
						new PetAddAllTrageTakeDamage(2, probability, num, text2).Start(this);
						break;
					case "1174":
					case "1175":
					case "1469":
						new PetShowEffect(3, probability, num, text2).Start(this);
						break;
					case "1377":
					case "1378":
					case "1379":
						new PetAddCritDamage(2, probability, num, text2).Start(this);
						break;
					case "1500":
						new PetAddCritDamage(3, probability, num, text2).Start(this);
						break;
					case "3177":
						new PetAddCritDamageForTeam(2, probability, num, text2).Start(this);
						break;
					case "1439":
					case "1440":
					case "1441":
					case "1522":
					case "1523":
					case "1524":
						new PetAddBaseGuard(2, probability, num, text2).Start(this);
						break;
					case "1178":
					case "1179":
					case "1180":
					case "1482":
						new PetAddAttack(2, probability, num, text2).Start(this);
						break;
					case "1194":
					case "1195":
						new PetAddAttack(1, probability, num, text2).Start(this);
						break;
					case "1489":
						new PetAddAttack(2, probability, num, text2).Start(this);
						break;
					case "1421":
					case "1507":
						new PetAddCritRate(2, probability, num, text2).Start(this);
						break;
					case "1198":
					case "1199":
						new PetAddCritRate(1, probability, num, text2).Start(this);
						break;
					case "1491":
						new PetAddCritRate(2, probability, num, text2).Start(this);
						break;
					case "1212":
					case "1213":
					case "1499":
						new PetAddCritRate(3, probability, num, text2).Start(this);
						break;
					case "1386":
					case "1387":
						new PetShootedAddTakeDamageForTeam(2, probability, num, text2).Start(this);
						break;
					case "1153":
					case "1154":
					case "1465":
						new PetAddBaseDamage(3, probability, num, text2).Start(this);
						break;
					case "1196":
					case "1197":
						new PetAddBaseDamage(1, probability, num, text2).Start(this);
						break;
					case "1490":
					case "1483":
						new PetAddBaseDamage(2, probability, num, text2).Start(this);
						break;
					case "1155":
					case "1156":
					case "1466":
						new PetReduceBaseGuard(3, probability, num, text2).Start(this);
						break;
					case "1181":
					case "1182":
					case "1183":
					case "1257":
					case "1258":
					case "1484":
						new PetAddBaseGuardForTeam(2, probability, num, text2).Start(this);
						break;
					case "1403":
					case "1404":
					case "3581":
						new PetAddAgilityForTeam(3, probability, num, text2).Start(this);
						break;
					case "2755":
					case "1467":
						new PetAddMaxBlood(3, probability, num, text2).Start(this);
						break;
					case "3175":
					case "3176":
						new PetAddMaxBlood(2, probability, num, text2).Start(this);
						break;
					case "1342":
					case "1343":
					case "1184":
					case "1185":
					case "1485":
						new PetAlwayAddBaseDamageBeforeMove(probability, num, text2).Start(this);
						break;
					case "1344":
					case "1345":
						new PetAlwayAddLuckyBeforeMove(probability, num, text2).Start(this);
						break;
					case "1399":
					case "1400":
						new PetAddAttackForTeam(3, probability, num, text2).Start(this);
						break;
					case "1401":
					case "1402":
						new PetAddDefenceForTeam(3, probability, num, text2).Start(this);
						break;
					case "1405":
					case "1406":
						new PetAddLuckyForTeam(3, probability, num, text2).Start(this);
						break;
					case "1407":
					case "1408":
						new PetAddMagicAttackForTeam(3, probability, num, text2).Start(this);
						break;
					case "1409":
					case "1410":
						new PetAddMagicDefenceForTeam(3, probability, num, text2).Start(this);
						break;
					case "1336":
					case "1337":
						new PetAddBaseGuardBeforeMove(probability, num, text2).Start(this);
						break;
					case "1331":
					case "1332":
					case "1333":
						new PetAddDefence(2, probability, num, text2).Start(this);
						break;
					case "1201":
					case "1202":
					case "1203":
					case "1495":
						new PetReduceTargetBaseDamage(3, probability, num, text2).Start(this);
						break;
					case "1545":
					case "1546":
					case "1547":
					case "1581":
					case "1582":
					case "1583":
					case "1651":
					case "1652":
						new PetReduceTargetBaseDamage(2, probability, num, text2).Start(this);
						break;
					case "1254":
					case "1255":
					case "1256":
						new PetReduceTargetAttack(3, probability, num, text2).Start(this);
						break;
					case "1642":
					case "1643":
					case "1644":
						new PetBurningTargetEnergy(3, probability, num, text2).Start(this);
						break;
					case "3580":
						new PetReduceAgility(3, probability, num, text2).Start(this);
						break;
					case "2757":
						new PetReduceTargetMaxBlood(3, probability, num, text2).Start(this);
						break;
					case "1346":
					case "1347":
						new PetAlwayReduceBaseGuardBeforeMove(probability, num, text2).Start(this);
						break;
					case "1334":
					case "1335":
						new PetReduceAttack(2, probability, num, text2).Start(this);
						break;
					case "1220":
					case "1221":
					case "1501":
						new PetReduceAllTargetBaseGuard(2, probability, num, text2).Start(this);
						break;
					case "1502":
						new PetReduceAllTargetMagicAttack(2, probability, num, text2).Start(this);
						break;
					case "1204":
					case "1205":
					case "1496":
						new PetReduceAllTargetAttack(2, probability, num, text2).Start(this);
						break;
					case "1206":
					case "1207":
					case "1497":
						new PetReduceAllTargetDefence(2, probability, num, text2).Start(this);
						break;
					case "1117":
						new PetImmuneDamage(1, probability, num, text2).Start(this);
						break;
					case "1259":
					case "1260":
					case "1358":
						new PetAddBlood(2, probability, num, text2).Start(this);
						break;
					case "1455":
					case "1456":
						new PetAddBlood(1, probability, num, text2).Start(this);
						break;
					case "1364":
						new PetAddBloodForTeamNow(probability, num, text2).Start(this);
						break;
					case "1365":
						new PetAddBloodForTeamNow(probability, num, text2).Start(this);
						break;
					case "1392":
					case "1393":
						new PetAddBloodForTeam(1, probability, num, text2).Start(this);
						break;
					case "1359":
					case "1360":
						new PetAddBloodForTeam(2, probability, num, text2).Start(this);
						break;
					case "1366":
					case "1361":
					case "1362":
						new PetAddBloodForTeam(3, probability, num, text2).Start(this);
						break;
					case "1367":
					case "1363":
						new PetAddBloodForTeam(4, probability, num, text2).Start(this);
						break;
					case "1150":
					case "1151":
					case "1152":
					case "1270":
					case "1271":
						new PetBurningTargetBlood(3, probability, num, text2).Start(this);
						break;
					case "1170":
					case "1171":
					case "1176":
					case "1177":
						new PetBurningAllTargetBlood(3, probability, num, text2).Start(this);
						break;
					case "1186":
					case "1187":
					case "1486":
						new PetAlwayReduceBloodBeforeMove(probability, num, text2).Start(this);
						break;
					case "1210":
					case "1211":
					case "1498":
						new PetReduceBlood(2, probability, num, text2).Start(this);
						break;
					case "1172":
					case "1173":
					case "1468":
						new PetAddBloodAfterKilled(3, probability, num, text2).Start(this);
						break;
					case "1310":
					case "1311":
						new PetSuckBlood(probability, num, text2).Start(this);
						break;
					case "1264":
					case "1265":
						new PetAddBaseDamageForTeam_Passive(probability, text2).Start(this);
						break;
					case "1261":
					case "1262":
						new PetAddLucky_Passive(probability, text2).Start(this);
						break;
					case "1165":
					case "1166":
					case "2977":
					case "1464":
						new PetAddBaseDamage_Passive(probability, text2).Start(this);
						break;
					case "1214":
					case "1215":
					case "1449":
					case "1450":
					case "2978":
					case "1528":
					case "1529":
					case "1517":
						new PetAddBaseGuard_Passive(probability, text2).Start(this);
						break;
					case "2979":
						new PetAddMagicAttack_Passive(probability, text2).Start(this);
						break;
					case "2980":
						new PetAddMagicDefence_Passive(probability, text2).Start(this);
						break;
					case "1355":
					case "1357":
						new PetAddBaseDamageAfterKilled_Passive(probability, text2).Start(this);
						break;
					case "1192":
					case "1193":
					case "1488":
						new PetAddAttackForTeam_Passive(probability, text2).Start(this);
						break;
					case "1422":
					case "1424":
						new PetAddMagicAttackForTeam_Passive(probability, text2).Start(this);
						break;
					case "1395":
					case "1396":
						new PetAddAngelicPoint_Passive(probability, text2).Start(this);
						break;
					case "1190":
					case "1191":
					case "3128":
					case "1487":
						new PetAddAgility_Passive(probability, text2).Start(this);
						break;
					case "1216":
					case "1217":
					case "1218":
					case "1219":
					case "1518":
						new PetAddMaxBlood_Passive(probability, text2).Start(this);
						break;
					case "1451":
					case "1452":
					case "1530":
					case "1531":
						new PetAddDefence_Passive(probability, text2).Start(this);
						break;
					case "1163":
					case "1463":
						new PetAddAttack_Passive(probability, text2).Start(this);
						break;
					case "1438":
					case "1515":
						new PetAddMagicPointAfterUseProp_Passive(probability, text2).Start(this);
						break;
					case "1300":
					case "1301":
					case "1534":
						new PetAddMagicPointAfterTrageUseSkills_Passive(probability, text2).Start(this);
						break;
					case "1423":
					case "1425":
						new PetReduceAllTargetMagicDefence_Passive(probability, text2).Start(this);
						break;
					case "1352":
						new PetReduceBloodBeginSelfTurn_Passive(probability, text2).Start(this);
						break;
					case "1266":
					case "1267":
						new PetReduceTargetMP(probability, num, text2).Start(this);
						break;
					case "1268":
					case "1269":
						new PetAddMPForTeam(probability, num, text2).Start(this);
						break;
					case "1411":
					case "1412":
						new PetAddMPOnGameStart_Passive(probability, text2).Start(this);
						break;
					case "1376":
						new PetAddMPOnGreenSproutSkill_Passive(probability, text2).Start(this);
						break;
					case "1200":
						new PetAddMPForTeamBeginSelfTurn_Passive(probability, text2).Start(this);
						break;
					case "1493":
						new PetAddBloodBeginSelfTurn_Passive(probability, text2).Start(this);
						break;
					case "1351":
						new PetAddMPBeginSelfTurn_Passive(4, probability, text2).Start(this);
						break;
					case "3567":
						new PetAddMPBeginSelfTurn_Passive(1, probability, text2).Start(this);
						break;
					case "1223":
					case "1263":
					case "1457":
					case "3822":
						new PetAddMPAfterKilledByLiving_Passive(probability, text2).Start(this);
						break;
					case "1208":
					case "1209":
					case "1427":
						new PetReduceAllTargetMP(probability, num, text2).Start(this);
						break;
					case "1306":
					case "1307":
					case "1540":
						new PetReduceAllTargetMPAndAddSelfMD_Passive(probability, text2).Start(this);
						break;
					case "1380":
					case "1381":
						new PetReduceTakeDamageForTeam(2, probability, num, text2).Start(this);
						break;
					case "1278":
					case "1279":
					case "1535":
						new PetReduceCritDamageForTeam(probability, text2).Start(this);
						break;
					case "1032":
					case "1033":
					case "1034":
					case "1232":
					case "1233":
					case "1234":
					case "1442":
					case "1443":
					case "1444":
					case "1445":
					case "1446":
					case "1525":
					case "1526":
					case "1527":
					case "1619":
						new PetReduceTakeDamage(1, probability, num, text2).Start(this);
						break;
					case "1281":
					case "1282":
					case "1283":
						new PetReduceTakeDamage(2, probability, num, text2).Start(this);
						break;
					case "1537":
						new PetReduceTakeDamage(3, probability, num, text2).Start(this);
						break;
					case "1428":
					case "1429":
					case "3174":
					case "3584":
					case "1509":
						new PetReduceTakeDamage(-1, probability, num, text2).Start(this);
						break;
					case "3223":
					case "3490":
						new PetReduceTakeDamage_Passive(probability, text2).Start(this);
						break;
					case "3180":
					case "3181":
					case "3224":
					case "3491":
						new PetReduceTakeCritDamage(-1, probability, num, text2).Start(this);
						break;
					case "1430":
					case "1431":
					case "1510":
						new PetReduceTargetCritRate(-1, probability, num, text2).Start(this);
						break;
					case "1649":
					case "1650":
						new PetAddSpeedByBloodPercent_Passive(probability, text2).Start(this);
						break;
					case "1662":
					case "2893":
						new PetAddMPByBloodPercent_Passive(probability, text2).Start(this);
						break;
					case "1322":
						new PetAddMPByBlood_Passive(probability, text2).Start(this);
						break;
					case "1368":
					case "1369":
						new PetAddBloodAfterKilledForTeam_Passive(probability, text2).Start(this);
						break;
					case "1435":
					case "1513":
						new PetKeepPlayerAlive_Passive(3, probability, text2).Start(this);
						break;
					case "1626":
					case "1778":
						new PetKeepPlayerAlive_Passive(1, probability, text2).Start(this);
						break;
					case "3492":
						new PetAddMaxBlood_Passive(probability, text2).Start(this);
						new PetKeepPlayerAlive_Passive(1, probability, text2).Start(this);
						break;
					case "1397":
					case "1398":
						new PetAddBloodAfterPlayerUseSecondWeapon_Passive(probability, text2).Start(this);
						break;
					case "1372":
					case "1373":
						new PetAddBloodAfterShootAngel_Passive(probability, text2).Start(this);
						break;
					case "1340":
					case "1341":
						new PetReduceEnemyBlood_Passive(probability, text2).Start(this);
						break;
					case "1067":
					case "1068":
					case "1228":
					case "1229":
						new PetReflectionDamage(2, probability, num, text2).Start(this);
						break;
					case "1136":
					case "1230":
						new PetReflectionDamageEffect(2, probability, num, text2).Start(this);
						break;
					case "1137":
						new PetActiveReflectionDamage_Passive(probability, text2).Start(this);
						break;
					case "1459":
					case "1460":
						new PetReflectionDamage_Passive(probability, text2).Start(this);
						break;
					case "3565":
						new PetAddTeamMPAfterUsePetMP_Passive(probability, text2).Start(this);
						break;
					case "3582":
						new PetNoTeammatesToAddMaxBlood_Passive(probability, text2).Start(this);
						break;
					case "1552":
					case "1553":
					case "1588":
					case "1589":
					case "20001":
						new PetDefensivePosture_CritDamage(2, probability, num, text2).Start(this);
						break;
					case "1554":
					case "1555":
					case "1590":
					case "1591":
					case "20002":
						new PetDefensivePosture_Damage(2, probability, num, text2).Start(this);
						break;
					case "1617":
						new PetDefensivePosture_AddDander(2, probability, num, text2).Start(this);
						break;
					case "1556":
						new PetAddTurn(120, probability, num, text2).Start(this);
						break;
					case "1592":
						new PetAddTurn(80, probability, num, text2).Start(this);
						break;
					case "1632":
						new PetAddTurn(180, probability, num, text2).Start(this);
						break;
					case "1577":
					case "1613":
					case "1618":
						new PetAttackPosture_ReduceDander(probability, num, text2).Start(this);
						new PetAttackPosture_AddAgilityAndSpeed(probability, num, text2).Start(this);
						break;
					case "1558":
					case "1614":
					case "1594":
					case "1615":
					case "1559":
						new PetStealthPosture_HidePassive(probability, text2).Start(this);
						break;
					case "1560":
						new PetClonePosture(1, probability, num, delay, text2).Start(this);
						break;
					case "1562":
					case "1598":
						new PetControlPosture_LockDirection(2, probability, num, text2).Start(this);
						break;
					case "1564":
					case "1565":
					case "1600":
					case "1601":
					case "1627":
						new PetViolentPosture_ReduceEnemyDefence(1, probability, num, text2).Start(this);
						break;
					case "1566":
					case "1567":
					case "1602":
					case "1603":
					case "1628":
						new PetViolentPosture_ReduceEnemyBaseGuard(1, probability, num, text2).Start(this);
						break;
					case "1572":
					case "1573":
					case "1608":
					case "1609":
					case "1631":
						new PetViolentPosture_ReduceBlood(probability, num, text2).Start(this);
						break;
					case "1622":
						new PetViolentPosture_AddDamageOnHide(1, probability, num, text2).Start(this);
						break;
					case "1574":
					case "1610":
						new PetKungFuSpecialization(probability, num, text2).Start(this);
						break;
					case "1575":
					case "1611":
					case "1624":
						new PetKungFuSpecialization_AddMPPassive(probability, text2).Start(this);
						break;
					case "1576":
					case "1612":
					case "1625":
						new PetKungFuSpecialization_AddBaseDamageByBloodPassive(probability, text2).Start(this);
						break;
					case "2645":
					case "2648":
					case "2650":
						new PetReduceTakeDamageForRandPlayer(3, probability, num, text2).Start(this);
						break;
					case "2652":
					case "2657":
					case "2661":
						new PetAddBaseGuard(3, probability, num, text2).Start(this);
						break;
					case "2667":
						new PetReduceCritRate(3, probability, num, text2).Start(this);
						break;
					case "1655":
					case "1656":
						new PetReduceTakeDamage_Passive(probability, text2).Start(this);
						break;
					case "1659":
					case "1660":
						new PetAddTakeDamage(1, probability, num, text2).Start(this);
						break;
					case "1661":
						new PetShadowOfTheDemon(probability, num, text2).Start(this);
						break;
					case "2580":
					case "2581":
						new PetPowerOfBlood_Passive(probability, text2).Start(this);
						break;
					case "2586":
					case "2587":
						new PetSoulDefense_Passive(probability, text2).Start(this);
						break;
					case "2759":
					case "2760":
						new PetReduceBlood(1, probability, num, text2).Start(this);
						break;
					case "2761":
					case "2762":
						new PetAddBlood(3, probability, num, text2).Start(this);
						break;
					case "2976":
						new PetUseMPToAddBlood(probability, text2).Start(this);
						break;
					case "2981":
						new PetPhoenixNirvana_Passive(2, probability, text2).Start(this);
						break;
					case "3171":
						new PetClearTargetDander(2, probability, text2).Start(this);
						break;
					case "3213":
						new PetIceFronzeTarget(2, num, text2).Start(this);
						break;
					case "3225":
						new PetIceFronzeTarget(3, num, text2).Start(this);
						break;
					case "1714":
					case "1715":
					case "1716":
					case "1792":
					case "1793":
					case "1794":
						new PetShareTakeDamageToTeam(2, probability, num, text2).Start(this);
						break;
					case "1752":
					case "1753":
					case "1754":
					case "1772":
						new PetReduceTakeDamageForTeamWithFire(2, probability, num, text2).Start(this);
						break;
					case "1717":
					case "1718":
					case "1719":
					case "1801":
					case "1802":
					case "1803":
						new PetReduceEnemyBlood(3, probability, num, text2).Start(this);
						break;
					case "1758":
					case "1773":
						new PetProhibitionAddBloodWithFire(3, num, text2).Start(this);
						break;
					case "1767":
					case "1768":
					case "1769":
					case "1774":
					case "1804":
					case "1805":
					case "1806":
						new PetReduceTakeDamage_Passive(probability, text2).Start(this);
						break;
					case "1723":
					case "1724":
					case "1725":
					case "1807":
					case "1808":
					case "1809":
					case "1775":
						new PetAddDamageOnPlayerAboveOrBellowOfMe_Passive(probability, text2).Start(this);
						break;
					case "1776":
						new PetReduceDamageOnPlayerAboveOrBellowOfMe_Passive(probability, text2).Start(this);
						break;
					case "1820":
					case "1834":
						new ShowPic(text2).Start(this);
						break;
					case "1726":
					case "1727":
					case "1810":
					case "1811":
						new PetChangeDamageWind(num, text2).Start(this);
						break;
					case "1777":
						new PetAddBaseDamageAndMagicAttackWind(num, text2).Start(this);
						break;
					case "1729":
						new PetChangeWindDirection(num, text2).Start(this);
						break;
					case "1730":
						new PetAddShootCount(num, text2).Start(this);
						break;
					case "1741":
						new PetReduceEnergy(3, probability, num, text2).Start(this);
						break;
					case "1789":
						new PetAddMpBeginSelfTurn(3, probability, num, text2).Start(this);
						break;
					case "1743":
						new PetAddMagicAttack(2, probability, num, text2).Start(this);
						break;
					case "1744":
						new PetAddBaseDamage(2, probability, num, text2).Start(this);
						break;
					case "1745":
						new PetReduceBloodAfterFinish(2, probability, num, text2).Start(this);
						break;
					case "1750":
						new PetShowFireOfResentment(2, num, text2).Start(this);
						break;
					case "1751":
						new PetShowFireOfHatred(2, num, text2).Start(this);
						break;
					case "1748":
					case "1815":
					case "1781":
						new PetAddMpAfterCount_Passive(text2).Start(this);
						break;
					case "1765":
					case "1816":
					case "1784":
						new PetAddMpAfterUseSkillCount_Passive(text2).Start(this);
						break;
					case "1770":
						new PetChangeSuit(2, num, text2).Start(this);
						break;
					case "1731":
						new PetIceImmunity(2, probability, num, text2).Start(this);
						break;
					case "1749":
						new PetBlockUseTrident(2, probability, num, text2).Start(this);
						break;
					case "1732":
						new PetReduceSpeed(2, probability, num, text2).Start(this);
						break;
					case "1735":
						new PetAddMaxBlood(2, probability, num, text2).Start(this);
						break;
					case "1738":
					case "1779":
						new PetAddBloodWithFire(num, text2).Start(this);
						break;
					case "1790":
						new PetBlackFlameBomb(2, probability, num, text2).Start(this);
						break;
					case "1835":
						new PetChangeSuit(3, num, text2).Start(this);
						break;
					case "1836":
						new PetIceImmunity(3, probability, num, text2).Start(this);
						break;
					case "1837":
						new PetBlockUseTrident(3, probability, num, text2).Start(this);
						break;
					case "1838":
						new PetReduceSpeed(3, probability, num, text2).Start(this);
						break;
					case "1839":
						new PetAddMaxBlood(3, probability, num, text2).Start(this);
						break;
					case "1840":
						new PetBlackFlameBomb(3, probability, num, text2).Start(this);
						break;
					case "3219":
						new PetGetshield_Passive(2, text2).Start(this);
						break;
					case "3230":
						new PetGetshield_Passive(3, text2).Start(this);
						break;
					case "3220":
						new PetReduceTakeDamageOnGameStarted_Passive(2, text2).Start(this);
						break;
					case "3218":
					case "3229":
						new PetAddBaseDamageOnPlayerBuffSkill_Passive(text2).Start(this);
						break;


                        //Phượng Hoàng Lửa Test.
                        case "2961": // Xoáy Lửa
                            new AE2961(coldDown, probability, gameType, num, delay, text2).Start((Living)this);
                            break;
                        case "2971": // Dẫn Đường
                            new CASE1038(coldDown, probability, gameType, num, delay, text2).Start((Living)this);
                            break;
                        case "2973": // Miễn Kháng
                            new CASE1021(coldDown, probability, gameType, num, delay, text2).Start((Living)this);
                            break;


                        default:
						Console.WriteLine($"Not Found PetSkill ElementID: {text2}, Pet name: {m_pet.Name}");
						break;
					case "2653":
					case "2658":
					case "2662":
					case "2654":
					case "1841":
						break;
					}
				}
			}
		}
	}

	public void InitEqupedEffect(List<ItemInfo> equpedEffect)
	{
		base.EffectList.StopAllEffect();
		foreach (ItemInfo item in equpedEffect)
		{
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
			string[] array = runeTemplateInfo.Attribute1.Split('|');
			string[] array2 = runeTemplateInfo.Attribute2.Split('|');
			string[] array3 = runeTemplateInfo.Attribute3.Split('|');
			if (item.Hole1 > runeTemplateInfo.BaseLevel)
			{
				if (array.Length > 1)
				{
					num = 1;
				}
				if (array2.Length > 1)
				{
					num2 = 1;
				}
				if (array3.Length > 1)
				{
					num3 = 1;
				}
			}
			int num4 = runeTemplateInfo.Type1;
			int num5 = Convert.ToInt32(array[num]);
			int probability = runeTemplateInfo.Rate1;
			if (num4 == 39)
			{
				ReduceCritFisrtGem = num5;
				ReduceCritSecondGem = num5;
				int type = runeTemplateInfo.Type2;
				if (DefenFisrtGem == 0)
				{
					DefenFisrtGem = type;
				}
				else
				{
					DefenSecondGem = type;
				}
			}
			if (num4 == 37 || num4 == 39)
			{
				num4 = runeTemplateInfo.Type2;
				num5 = Convert.ToInt32(array2[num2]);
				probability = runeTemplateInfo.Rate2;
			}
			if (runeTemplateInfo.Type3 == 0)
			{
				switch (num4)
				{
				case 1:
					new AddAttackEffect(num5, probability).Start(this);
					break;
				case 2:
					new AddDefenceEffect(num5, probability, num4).Start(this);
					break;
				case 3:
					new AddAgilityEffect(num5, probability).Start(this);
					break;
				case 4:
					new AddLuckyEffect(num5, probability).Start(this);
					break;
				case 5:
					new AddDamageEffect(num5, probability).Start(this);
					break;
				case 6:
					new ReduceDamageEffect(num5, probability, num4).Start(this);
					break;
				case 7:
					new AddBloodEffect(num5, probability).Start(this);
					break;
				case 8:
					new FatalEffect(num5, probability).Start(this);
					break;
				case 9:
					new IceFronzeEquipEffect(num5, probability).Start(this);
					break;
				case 10:
					new NoHoleEquipEffect(num5, probability, num4).Start(this);
					break;
				case 11:
					new AtomBombEquipEffect(num5, probability).Start(this);
					break;
				case 12:
					new ArmorPiercerEquipEffect(num5, probability).Start(this);
					break;
				case 13:
					new AvoidDamageEffect(num5, probability, num4).Start(this);
					break;
				case 14:
					new MakeCriticalEffect(num5, probability).Start(this);
					break;
				case 15:
					new AssimilateDamageEffect(num5, probability, num4).Start(this);
					break;
				case 16:
					new AssimilateBloodEffect(num5, probability).Start(this);
					break;
				case 17:
					new SealEquipEffect(num5, probability).Start(this);
					break;
				case 18:
					new AddTurnEquipEffect(num5, probability, runeTemplateInfo.TemplateID).Start(this);
					break;
				case 19:
					new AddDanderEquipEffect(num5, probability, num4).Start(this);
					break;
				case 20:
					new ReflexDamageEquipEffect(num5, probability).Start(this);
					break;
				case 21:
					new ReduceStrengthEquipEffect(num5, probability).Start(this);
					break;
				case 22:
					new ContinueReduceBloodEquipEffect(num5, probability).Start(this);
					break;
				case 23:
					new LockDirectionEquipEffect(num5, probability).Start(this);
					break;
				case 24:
					new AddBombEquipEffect(num5, probability).Start(this);
					break;
				case 25:
					new ContinueReduceDamageEquipEffect(num5, probability).Start(this);
					break;
				case 26:
					new RecoverBloodEffect(num5, probability, num4).Start(this);
					break;
				default:
					Console.WriteLine("Not Found Effect: " + num4);
					break;
				}
				continue;
			}
			switch (runeTemplateInfo.Type1 - 30)
			{
			case 1:
				new AddAttackEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 2:
				new AddDefenceEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 3:
				new AddAgilityEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 4:
				new AddLuckyEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 5:
				new AddDamageEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 6:
				new ReduceDamageEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 7:
				new AddBloodEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 8:
				new FatalEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 9:
				new IceFronzeEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 10:
				new NoHoleEquipEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 11:
				new AtomBombEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 12:
				new ArmorPiercerEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 13:
				new AvoidDamageEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 14:
				new MakeCriticalEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 15:
				new AssimilateDamageEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 16:
				new AssimilateBloodEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 17:
				new SealEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 18:
				new AddTurnEquipEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.TemplateID).Start(this);
				break;
			case 19:
				new AddDanderEquipEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			case 20:
				new ReflexDamageEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 21:
				new ReduceStrengthEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 22:
				new ContinueReduceBloodEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 23:
				new LockDirectionEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 24:
				new AddBombEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 25:
				new ContinueReduceDamageEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 26:
				new RecoverBloodEffect(Convert.ToInt32(array[num]), probability, num4).Start(this);
				break;
			default:
				Console.WriteLine("Not Found Effect: " + num4);
				break;
			}
			switch (runeTemplateInfo.Type1 - 30)
			{
			case 1:
				new AddAttackEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 2:
				new AddDefenceEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 3:
				new AddAgilityEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 4:
				new AddLuckyEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 5:
				new AddDamageEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 6:
				new ReduceDamageEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 7:
				new AddBloodEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 8:
				new FatalEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 9:
				new IceFronzeEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 10:
				new NoHoleEquipEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 11:
				new AtomBombEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 12:
				new ArmorPiercerEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 13:
				new AvoidDamageEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 14:
				new MakeCriticalEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 15:
				new AssimilateDamageEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 16:
				new AssimilateBloodEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 17:
				new SealEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 18:
				new AddTurnEquipEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.TemplateID).Start(this);
				break;
			case 19:
				new AddDanderEquipEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			case 20:
				new ReflexDamageEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 21:
				new ReduceStrengthEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 22:
				new ContinueReduceBloodEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 23:
				new LockDirectionEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 24:
				new AddBombEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 25:
				new ContinueReduceDamageEquipEffect(Convert.ToInt32(array[num]), probability).Start(this);
				break;
			case 26:
				new RecoverBloodEffect(Convert.ToInt32(array[num]), probability, runeTemplateInfo.Type1 - 30).Start(this);
				break;
			default:
				Console.WriteLine("Not Found Effect: " + (runeTemplateInfo.Type1 - 30));
				break;
			}
			switch (runeTemplateInfo.Type2 - 30)
			{
			case 1:
				new AddAttackEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 2:
				new AddDefenceEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 3:
				new AddAgilityEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 4:
				new AddLuckyEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 5:
				new AddDamageEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 6:
				new ReduceDamageEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 7:
				new AddBloodEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 8:
				new FatalEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 9:
				new IceFronzeEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 10:
				new NoHoleEquipEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 11:
				new AtomBombEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 12:
				new ArmorPiercerEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 13:
				new AvoidDamageEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 14:
				new MakeCriticalEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 15:
				new AssimilateDamageEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 16:
				new AssimilateBloodEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 17:
				new SealEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 18:
				new AddTurnEquipEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.TemplateID).Start(this);
				break;
			case 19:
				new AddDanderEquipEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			case 20:
				new ReflexDamageEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 21:
				new ReduceStrengthEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 22:
				new ContinueReduceBloodEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 23:
				new LockDirectionEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 24:
				new AddBombEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 25:
				new ContinueReduceDamageEquipEffect(Convert.ToInt32(array2[num2]), probability).Start(this);
				break;
			case 26:
				new RecoverBloodEffect(Convert.ToInt32(array2[num2]), probability, runeTemplateInfo.Type2 - 30).Start(this);
				break;
			default:
				Console.WriteLine("Not Found Effect: " + (runeTemplateInfo.Type2 - 30));
				break;
			}
			switch (runeTemplateInfo.Type3 - 30)
			{
			case 1:
				new AddAttackEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 2:
				new AddDefenceEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 3:
				new AddAgilityEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 4:
				new AddLuckyEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 5:
				new AddDamageEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 6:
				new ReduceDamageEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 7:
				new AddBloodEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 8:
				new FatalEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 9:
				new IceFronzeEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 10:
				new NoHoleEquipEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 11:
				new AtomBombEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 12:
				new ArmorPiercerEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 13:
				new AvoidDamageEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 14:
				new MakeCriticalEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 15:
				new AssimilateDamageEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 16:
				new AssimilateBloodEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 17:
				new SealEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 18:
				new AddTurnEquipEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.TemplateID).Start(this);
				break;
			case 19:
				new AddDanderEquipEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			case 20:
				new ReflexDamageEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 21:
				new ReduceStrengthEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 22:
				new ContinueReduceBloodEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 23:
				new LockDirectionEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 24:
				new AddBombEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 25:
				new ContinueReduceDamageEquipEffect(Convert.ToInt32(array3[num3]), probability).Start(this);
				break;
			case 26:
				new RecoverBloodEffect(Convert.ToInt32(array3[num3]), probability, runeTemplateInfo.Type3 - 30).Start(this);
				break;
			default:
				Console.WriteLine("Not Found Effect: " + (runeTemplateInfo.Type3 - 30));
				break;
			}
		}
	}

	public virtual int AddEnegy(int value)
	{
		m_energy = value;
		return value;
	}

	public bool ReduceEnergy(int value)
	{
		if (value > m_energy)
		{
			value = m_energy;
		}
		m_energy -= value;
		return true;
	}

	public bool AddEnergy(int value)
	{
		m_energy += value;
		return true;
	}

	public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		bool flag = false;
		if (base.Game.RoomType != eRoomType.SurvivalMode && (source == this || source.Team == base.Team) && damageAmount + criticalAmount >= m_blood)
		{
			damageAmount = m_blood - 1;
			criticalAmount = 0;
		}
		flag = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
		if (base.IsLiving)
		{
			AddDander(damageAmount * 60 / base.MaxBlood + m_game.Random.Next(5));
		}
		return flag;
	}

	public void UseSpecialSkill()
	{
		if (base.PetEffects.CurrentUseSkill != 0)
		{
			m_game.SendGameUpdateDander(this);
		}
		else if (base.Dander >= 200)
		{
			SetBall(m_spBallId, special: true);
			m_ballCount = m_currentBall.Amount;
			SetDander(0);
			OnPlayerUseDander();
		}
	}

	public void SetBall(int ballId)
	{
		SetBall(ballId, special: false);
	}

	public void SetBall(int ballId, bool special)
	{
		if (ballId != m_currentBall.ID)
		{
			if (BallMgr.FindBall(ballId) != null)
			{
				m_currentBall = BallMgr.FindBall(ballId);
			}
			m_game.SendGameUpdateBall(this, special);
		}
	}

	public void MarkMeHide(bool isMark)
	{
		m_game.SendMarkMeHideInfo(this, base.Id, isMark);
	}

	public void SetCurrentWeapon(ItemInfo item)
	{
		m_weapon = item;
		BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
		if (m_weapon.GoldValidDate())
		{
			ballConfigInfo = BallConfigMgr.FindBall(m_weapon.GoldEquip.TemplateID);
		}
		if (ChangeSpecialBall > 0)
		{
			ballConfigInfo = BallConfigMgr.FindBall(70396);
		}
		m_mainBallId = ballConfigInfo.Common;
		m_spBallId = ballConfigInfo.Special;
		m_AddWoundBallId = ballConfigInfo.CommonAddWound;
		m_MultiBallId = ballConfigInfo.CommonMultiBall;
		SetBall(m_mainBallId);
	}

	public void CalculatePlayerOffer(Player player)
	{
		if (m_game.RoomType == eRoomType.Match && (m_game.GameType == eGameType.Guild || m_game.GameType == eGameType.Free) && !player.IsLiving)
		{
			int num = ((base.Game.GameType == eGameType.Guild) ? 50 : ((PlayerDetail.PlayerCharacter.ConsortiaID == 0 || player.PlayerDetail.PlayerCharacter.ConsortiaID == 0) ? 10 : 30));
			if (num > player.PlayerDetail.PlayerCharacter.Offer)
			{
				num = player.PlayerDetail.PlayerCharacter.Offer;
			}
			if (num > 0)
			{
				GainOffer += num;
			}
		}
	}

	public override void StartMoving()
	{
		if (m_map != null)
		{
			Point point = m_map.FindYLineNotEmptyPointDown(m_x, m_y);
			if (point.IsEmpty && m_map.Ground != null)
			{
				m_y = m_map.Ground.Height;
			}
			else
			{
				m_x = point.X;
				m_y = point.Y;
			}
			if (point.IsEmpty)
			{
				m_syncAtTime = false;
				Die();
			}
		}
	}

	public void BanSkill(int id)
	{
		if (!banSkills.Contains(id))
		{
			banSkills.Add(id);
		}
	}

	public void RemoveSkill(int id)
	{
		banSkills.Remove(id);
	}

	public override void StartMoving(int delay, int speed)
	{
		if (m_map != null)
		{
			Point point = m_map.FindYLineNotEmptyPointDown(m_x, m_y);
			if (point.IsEmpty)
			{
				m_y = m_map.Ground.Height;
			}
			else
			{
				m_x = point.X;
				m_y = point.Y;
			}
			base.StartMoving(delay, speed);
			if (point.IsEmpty)
			{
				m_syncAtTime = false;
				Die();
			}
		}
	}

	public void StartRotate(int rotation, int speed, string endPlay, int delay)
	{
		m_game.AddAction(new LivingRotateTurnAction(this, rotation, speed, endPlay, delay));
	}

	public void StartSpeedMult(int x, int y)
	{
		StartSpeedMult(x, y, 3000);
	}

	public void StartSpeedMult(int x, int y, int delay)
	{
		Point point = new Point(x - X, y - Y);
		m_game.AddAction(new PlayerSpeedMultAction(this, new Point(X + point.X, Y + point.Y), delay));
	}

	public void StartGhostMoving()
	{
		if (!TargetPoint.IsEmpty)
		{
			Point point = new Point(TargetPoint.X - X, TargetPoint.Y - Y);
			Point targetPoint = TargetPoint;
			if (point.Length() > 160.0)
			{
				point.Normalize(160);
			}
			m_game.AddAction(new GhostMoveAction(this, new Point(X + point.X, Y + point.Y)));
		}
	}

	public override void SetXY(int x, int y)
	{
		if (m_x == x && m_y == y)
		{
			return;
		}
		int value = (int)((double)Math.Abs(x - m_x) * ((double)PowerRatio / 100.0));
		m_x = x;
		m_y = y;
		if (walkenergy0)
		{
			PowerRatio = 0;
		}
		if (base.IsLiving)
		{
			if (m_game.IsPVE())
			{
				((PVEGame)m_game).OnMoving();
				if (((PVEGame)m_game).IsWorldCup() && ((PVEGame)m_game).SessionId == 0)
				{
					value = 0;
				}
			}
			if (!LimitEnergy)
			{
				ReduceEnergy(value);
				OnPlayerMoving();
			}
		}
		else
		{
			if (m_map == null)
			{
				return;
			}
			Rectangle rect = m_rect;
			rect.Offset(m_x, m_y);
			Physics[] array = m_map.FindPhysicalObjects(rect, this);
			Physics[] array2 = array;
			foreach (Physics physics in array2)
			{
				if (physics is SimpleBox)
				{
					SimpleBox box = physics as SimpleBox;
					PickBox(box);
				}
			}
		}
	}

	public override void Die()
	{
		if (base.IsLiving)
		{
			m_y -= 70;
			base.Die();
		}
	}

	public override void PickBox(SimpleBox box)
	{
		m_tempBoxes.Add(box);
		base.PickBox(box);
	}

	public void OpenBox(int boxId)
	{
		SimpleBox simpleBox = null;
		foreach (SimpleBox tempBox in m_tempBoxes)
		{
			if (tempBox.Id == boxId)
			{
				simpleBox = tempBox;
				break;
			}
		}
		if (simpleBox == null || simpleBox.Item == null)
		{
			return;
		}
		ItemInfo item = simpleBox.Item;
		switch (item.TemplateID)
		{
		case -100:
			m_player.AddGold(item.Count);
			break;
		case -200:
			m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, m_player.PlayerCharacter.ID, item.Count, m_player.PlayerCharacter.Money);
			m_player.AddMoney(item.Count);
			break;
		case -1100:
			m_player.AddGiftToken(item.Count);
			break;
		case -300:
			m_player.AddMedal(item.Count);
			break;
		default:
			if (item.Template.CategoryID != 10)
			{
				m_player.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
			}
			break;
		}
		m_tempBoxes.Remove(simpleBox);
	}

	public override void PrepareNewTurn()
	{
		if (CurrentIsHitTarget)
		{
			TotalHitTargetCount++;
		}
		m_energy = (int)Agility / 30 + 240;
		if (base.FightBuffers.ConsortionAddEnergy > 0)
		{
			m_energy += base.FightBuffers.ConsortionAddEnergy;
		}
		if (LimitEnergy)
		{
			m_energy = TotalCureEnergy;
		}
		if (movingenergy)
		{
			PowerRatio = 0;
		}
		base.PetEffects.CurrentUseSkill = 0;
		base.PetEffects.Delay = 500;
		base.SpecialSkillDelay = 0;
		m_shootCount = 1;
		m_ballCount = 1;
		EffectTrigger = false;
		PetEffectTrigger = false;
		base.PetEffects.DisibleActiveSkill = false;
		SetCurrentWeapon(PlayerDetail.MainWeapon);
		if (m_currentBall.ID != m_mainBallId)
		{
			m_currentBall = BallMgr.FindBall(m_mainBallId);
		}
		if (m_game.RoomType == eRoomType.FightFootballTime)
		{
			m_currentBall = BallMgr.FindBall(24);
		}
		if (!base.IsLiving)
		{
			SoulPropCount = 0;
			StartGhostMoving();
			TargetPoint = Point.Empty;
		}
		base.PrepareNewTurn();
	}

	public override void PrepareSelfTurn()
	{
		DefaultDelay = m_delay;
		_flyCoolDown--;
		base.PetEffects.BallType = 0;
		if (base.BlockTurn)
		{
			AddDelay(GetTurnDelay());
		}
		if (base.IsFrost)
		{
			AddDelay(500);
			AddPetMP(10);
		}
		m_game.SendRoundOneEnd(this);
		if (m_pet != null)
		{
			foreach (int key in _petSkillCd.Keys)
			{
				if (_petSkillCd[key].Turn > 0)
				{
					_petSkillCd[key].Turn--;
				}
			}
		}
		foreach (int key2 in _horseSkillCd.Keys)
		{
			if (_horseSkillCd[key2].Turn > 0)
			{
				_horseSkillCd[key2].Turn--;
			}
		}
		m_game.SendSkillInfoInit(this, PetSkillCD, HorseSkillCd);
		jaUsouSkill = false;
		base.PrepareSelfTurn();
	}

	public override void CollidedByObject(Physics phy)
	{
		base.CollidedByObject(phy);
		if (phy is SimpleBomb)
		{
			OnCollidedByObject();
		}
	}

	public override void StartAttacking()
	{
		if (!base.IsAttacking)
		{
			if (m_Healstone != null && m_blood < m_maxBlood && m_player.RemoveHealstone())
			{
				AddBlood(m_Healstone.Template.Property2);
			}
			AddDelay(GetTurnDelay());
			base.StartAttacking();
		}
	}

	private int GetTurnDelay()
	{
		double num = 1600.0;
		double num2 = Attack / num;
		double num3 = Agility / num;
		double num4 = Agility / (10.0 + num3);
		double num5 = Attack / (10.0 + num2);
		return (int)(num - 1200.0 * num4 / (num4 + 1200.0) + num5 / 10.0);
	}

	public override void Skip(int spendTime)
	{
		if (base.IsAttacking)
		{
			base.Game.SendSkipNext(this);
			m_prop.Clear();
			AddDelay(100);
			AddDander(40);
			AddPetMP(10);
			OnPlayerSkip();
			base.Skip(spendTime);
		}
	}

	public void SkipAttack()
	{
		base.Game.SendSkipNext(this);
		m_prop.Clear();
		AddDelay(100);
		base.Skip(1000);
	}

	public void PrepareShoot(byte speedTime)
	{
		int turnWaitTime = m_game.GetTurnWaitTime();
		int value = ((speedTime > turnWaitTime) ? turnWaitTime : speedTime);
		AddDelay(value);
		TotalShootCount++;
		PassBallFail = false;
	}

	public bool CanUseSkill(int energy)
	{
		return m_energy >= energy && (base.IsAttacking || (!base.IsLiving && base.Team == m_game.CurrentLiving.Team));
	}

	public bool UseHorseSkill(MountSkillTemplateInfo skillInfo)
	{
		if (CanUseSkill(skillInfo.CostEnergy))
		{
			m_energy -= skillInfo.CostEnergy;
			AddDelay(skillInfo.Delay);
			base.PetEffects.CurrentHorseSkill = skillInfo.ID;
			OnPlayerBuffSkillHorse();
			return true;
		}
		return false;
	}

	public void PetUseKill(int skillId, int type)
	{
		if (banSkills.Contains(skillId))
		{
			m_player.SendMessage(LanguageMgr.GetTranslation("Player.UseSkill.Disable"));
		}
		else if (type == 2 && HorseSkillCd.ContainsKey(skillId))
		{
			MountSkillTemplateInfo mountSkillTemplateInfo = _horseSkillCd[skillId];
			if (mountSkillTemplateInfo.Turn > 0)
			{
				m_player.SendMessage(LanguageMgr.GetTranslation("Player.HorseUseSkill.CD"));
			}
			else if (mountSkillTemplateInfo.Count < mountSkillTemplateInfo.UseCount)
			{
				if (UseHorseSkill(mountSkillTemplateInfo))
				{
					mountSkillTemplateInfo.Count++;
					mountSkillTemplateInfo.Turn = mountSkillTemplateInfo.ColdDown;
					int count = mountSkillTemplateInfo.UseCount - mountSkillTemplateInfo.Count;
					m_game.SendPetUseKill(this, skillId, isUse: true, type, count);
					OnPlayerBuffSkill();
				}
				else
				{
					m_player.SendMessage(LanguageMgr.GetTranslation("Player.HorseUseSkill.NotEnoughEnegy"));
				}
			}
			else
			{
				m_player.SendMessage(LanguageMgr.GetTranslation("Player.HorseUseSkill.UseCount.Empty"));
			}
		}
		else
		{
			if (!CanUsePetSkill || base.PetEffects.DisibleActiveSkill || jaUsouSkill)
			{
				return;
			}
			PetSkillInfo petSkillInfo = _petSkillCd[skillId];
			if (base.PetMP > 0 && base.PetMP >= petSkillInfo.CostMP)
			{
				if (petSkillInfo.NewBallID != -1)
				{
					base.PetEffects.Delay += petSkillInfo.Delay;
					SetBall(petSkillInfo.NewBallID);
				}
				base.PetMP -= petSkillInfo.CostMP;
				AddDelay(petSkillInfo.Delay);
				OnPlayerUsePetMP(petSkillInfo.CostMP);
				base.PetEffects.CurrentUseSkill = skillId;
				base.PetEffects.BallType = petSkillInfo.BallType;
				OnPlayerBuffSkillPet();
				m_game.SendPetUseKill(this, type, 0);
				petSkillInfo.Turn = petSkillInfo.ColdDown;
				OnPlayerAfterBuffSkillPet();
				OnPlayerBuffSkill();
			}
			else
			{
				m_player.SendMessage(LanguageMgr.GetTranslation("Player.Msg1"));
			}
		}
	}

	public virtual void Revive(int blood, int x, int y)
	{
		Revive();
		base.Blood = blood;
		m_game.SendLivingRevive(this, blood, x, y);
	}

	public bool Shoot(int x, int y, int force, int angle)
	{
		if (m_shootCount > 0)
		{
			EffectTrigger = false;
			OnPlayerShoot();
			int num = m_currentBall.ID;
			if (!IsSpecialSkill && !IsBlackFlame)
			{
				buffs = new int[9];
				using (List<int>.Enumerator enumerator = Prop.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						switch (enumerator.Current)
						{
						case 10001:
							buffs[1]++;
							break;
						case 10004:
							buffs[4]++;
							break;
						}
					}
				}
				if (buffs[1] >= 2)
				{
					num = m_MultiBallId;
				}
				else if (buffs[4] >= 2)
				{
					num = m_AddWoundBallId;
				}
			}
			OnBeforePlayerShoot();
			if (IsSpecialSkill)
			{
				ControlBall = false;
				base.SpecialSkillDelay = 2000;
			}
			OnPlayerAnyShellThrow();
			int embleType = ((PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill == 817) ? 1 : ((PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill == 818) ? 4 : ((PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill == 819) ? 2 : ((PlayerDetail.PlayerCharacter.EmblemInfoAttackSKill == 820) ? 3 : 0))));
			if (ShootImp(num, x, y, force, angle, m_ballCount, ShootCount, embleType))
			{
				if (num == 4)
				{
					m_game.AddAction(new FightAchievementAction(this, eAcrobaciaType.ExplosaoNucleardoSuperHomem, IdAcrobacias++, 1200));
				}
				m_shootCount--;
				if (m_shootCount <= 0 || !base.IsLiving)
				{
					StopAttacking();
					AddDelay(m_currentBall.Delay + m_weapon.Template.Property8);
					AddDander(20);
					AddPetMP(10);
					m_prop.Clear();
					if (CanGetProp)
					{
						List<ItemInfo> info = null;
						if (DropInventory.FireDrop(m_game.RoomType, ref info) && info != null)
						{
							foreach (ItemInfo item in info)
							{
								if (item != null && base.VaneOpen && item.Template.CategoryID != 10)
								{
									PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eGameView.dungeonTypeGet);
								}
							}
						}
					}
					OnPlayerCompleteShoot();
				}
				OnAfterPlayerShoot();
				return true;
			}
		}
		return false;
	}

	public void unlockProp(int templateid)
	{
		propsBloqueados.Remove(templateid);
	}

	public void lockProp(int templateid)
	{
		if (!propsBloqueados.Contains(templateid))
		{
			propsBloqueados.Add(templateid);
		}
	}

	public bool CanUseItem(ItemTemplateInfo item)
	{
		if (item == null)
		{
			return false;
		}
		if (PlayerDetail.PlayerCharacter.IsAutoBot)
		{
			return true;
		}
		if (!base.IsLiving)
		{
			return base.psychic >= item.Property7 && SoulPropCount < MaxSoulPropUsedCount;
		}
		if (propsBloqueados.Contains(item.TemplateID))
		{
			return false;
		}
		return m_energy >= item.Property4 && (base.IsAttacking || (!base.IsLiving && base.Team == m_game.CurrentLiving.Team));
	}

	public bool UseItem(ItemTemplateInfo item)
	{
		if (CanUseItem(item))
		{
			if (base.IsLiving)
			{
				m_energy -= item.Property4;
				if (LimitEnergy)
				{
					TotalCureEnergy -= item.Property4;
				}
				AddDelay(item.Property5);
			}
			else
			{
				base.psychic -= item.Property7;
				if (base.Game.CurrentLiving != null)
				{
					base.Game.CurrentLiving.AddDelay(item.Property5);
				}
				SoulPropCount++;
			}
			m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID, this);
			SpellMgr.ExecuteSpell(m_game, this, item);
			Prop.Add(item.TemplateID);
			if (item.Property6 == 1 && base.IsAttacking)
			{
				StopAttacking();
				m_game.CheckState(0);
			}
			OnBeginUseProp();
			return true;
		}
		return false;
	}

	public void UseFlySkill()
	{
		if (_flyCoolDown <= 0)
		{
			m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
			SetBall(3);
			_flyCoolDown = 2;
			m_energy -= 150;
		}
	}

	public void UseSecondWeapon()
	{
		if (m_DeputyWeapon != null && CanUseItem(m_DeputyWeapon.Template))
		{
			if (m_DeputyWeapon.Template.Property3 == 31)
			{
				int num = (int)getHertAddition(m_DeputyWeapon);
				new AddGuardEquipEffect(subWeaponAddOrReduce, 1).Start(this);
				OnPlayerGuard();
			}
			else
			{
				SetCurrentWeapon(m_DeputyWeapon);
				OnPlayerCure();
				ShootCount = 1;
			}
			m_energy -= m_DeputyWeapon.Template.Property4;
			AddDelay(m_DeputyWeapon.Template.Property5);
			m_game.SendPlayerUseProp(this, -2, -2, m_DeputyWeapon.Template.TemplateID);
			if (deputyWeaponResCount > 0)
			{
				deputyWeaponResCount--;
				m_game.SendUseDeputyWeapon(this, deputyWeaponResCount);
			}
			OnPlayerUseSecondWeapon(m_DeputyWeapon.Template.Property3);
		}
	}

	public bool IsCure()
	{
		if (CurrentBall == null)
		{
			return false;
		}
		return CurrentBall.IsSpecial();
	}

	public void DeadLink()
	{
		if (base.IsLiving)
		{
			Die();
		}
	}

	public bool CheckShootPoint(int x, int y)
	{
		int num = Math.Abs(X - x);
		if (num > 100)
		{
			string userName = m_player.PlayerCharacter.UserName;
			string nickName = m_player.PlayerCharacter.NickName;
			Console.WriteLine("Shoot fail point: {0}", num);
			return false;
		}
		return true;
	}

	public void OnPlayerUsePetMP(int value)
	{
		if (this.PlayerUsePetMP != null)
		{
			this.PlayerUsePetMP(this, value);
		}
	}

	public void OnPlayerSkip()
	{
		this.PlayerSkip?.Invoke(this);
	}

	protected void OnBeforePlayerShoot()
	{
		if (this.BeforePlayerShoot != null)
		{
			this.BeforePlayerShoot(this);
		}
	}

	public void OnPlayerAfterBuffSkillPet()
	{
		this.PlayerAfterBuffSkillPet?.Invoke(this);
	}

	public void OnPlayerBuffSkill()
	{
		this.PlayerBuffSkill?.Invoke(this);
	}

	public void OnPlayerAnyShellThrow()
	{
		if (this.PlayerAnyShellThrow != null)
		{
			this.PlayerAnyShellThrow(this);
		}
	}

	public void OnPlayerShootCure()
	{
		if (this.PlayerShootCure != null)
		{
			this.PlayerShootCure(this);
		}
	}

	public void OnPlayerMoving()
	{
		if (this.PlayerBeginMoving != null)
		{
			this.PlayerBeginMoving(this);
		}
	}

	protected void OnAfterPlayerShoot()
	{
		if (this.AfterPlayerShooted != null)
		{
			this.AfterPlayerShooted(this);
		}
	}

	protected void OnPlayerCompleteShoot()
	{
		if (this.PlayerCompleteShoot != null)
		{
			this.PlayerCompleteShoot(this);
		}
	}

	protected void OnLoadingCompleted()
	{
		if (this.LoadingCompleted != null)
		{
			this.LoadingCompleted(this);
		}
	}

	public void OnPlayerShoot()
	{
		if (this.PlayerShoot != null)
		{
			this.PlayerShoot(this);
		}
	}

	public void OnPlayerUseSecondWeapon(int type)
	{
		if (this.PlayerUseSecondWeapon != null)
		{
			this.PlayerUseSecondWeapon(this, type);
		}
	}

	public void OnPlayerGuard()
	{
		if (this.PlayerGuard != null)
		{
			this.PlayerGuard(this);
		}
	}

	public void OnPlayerCure()
	{
		if (this.PlayerCure != null)
		{
			this.PlayerCure(this);
		}
	}

	public void OnPlayerBuffSkillPet()
	{
		jaUsouSkill = true;
		if (this.PlayerBuffSkillPet != null)
		{
			this.PlayerBuffSkillPet(this);
		}
	}

	public void OnPlayerClearBuffSkillPet()
	{
		if (this.PlayerClearBuffSkillPet != null)
		{
			this.PlayerClearBuffSkillPet(this);
		}
	}

	public void OnPlayerBuffSkillHorse()
	{
		if (this.PlayerBuffSkillHorse != null)
		{
			this.PlayerBuffSkillHorse(this);
		}
	}

	protected void OnCollidedByObject()
	{
		if (this.CollidByObject != null)
		{
			this.CollidByObject(this);
		}
	}

	public void OnPlayerUseDander()
	{
		if (this.PlayerUseDander != null)
		{
			this.PlayerUseDander(this);
		}
	}

	public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
	{
		base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
		if (target is Player)
		{
			m_player.OnKillingLiving(m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);
			return;
		}
		int id = 0;
		if (target is SimpleBoss)
		{
			SimpleBoss simpleBoss = target as SimpleBoss;
			id = simpleBoss.NpcInfo.ID;
		}
		if (target is SimpleNpc)
		{
			SimpleNpc simpleNpc = target as SimpleNpc;
			id = simpleNpc.NpcInfo.ID;
		}
		m_player.OnKillingLiving(m_game, 2, id, target.IsLiving, damageAmount + criticalAmount);
	}

	public void SendPicturePlayer(int type, bool state, int count)
	{
		if (m_syncAtTime)
		{
			m_game.updatePlayerBuff(this, type, state, count);
		}
	}
}
