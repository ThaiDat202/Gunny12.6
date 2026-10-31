using System;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerKingGrade
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private KingGradeDataInfo m_data;

	private bool m_saveToDb;

	private int minKingGrade = 40;

	private int MaxKingGrade = 30;

	private int baseUsableNum = 10;

	public GamePlayer Player => m_player;

	public KingGradeDataInfo Data
	{
		get
		{
			return m_data;
		}
		set
		{
			m_data = value;
		}
	}

	public PlayerKingGrade(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_data = new KingGradeDataInfo();
		m_saveToDb = saveTodb;
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		try
		{
			KingGradeDataInfo sigleKingGradeData = playerBussiness.GetSigleKingGradeData(Player.PlayerCharacter.ID);
			if (sigleKingGradeData != null)
			{
				lock (m_lock)
				{
					m_data = sigleKingGradeData;
					return;
				}
			}
		}
		finally
		{
			Player.PlayerCharacter.ddtKingGrade = Data.ddtKingGrade;
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		if (Data.UserID == 0)
		{
			lock (m_lock)
			{
				m_data.UserID = Player.PlayerCharacter.ID;
			}
			playerBussiness.AddKingGradeData(Data);
		}
		else
		{
			playerBussiness.UpdateKingGradeData(Data);
		}
	}

	public int PlusProperty(eKingGrade type, int value, ref int currentVal)
	{
		MaxLevelTemplateInfo maxLevelTemplateInfo = null;
		switch (type)
		{
		case eKingGrade.Attack:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.AttackLevel)?.Attack ?? 0;
			break;
		case eKingGrade.Defence:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.DefenceLevel)?.Attack ?? 0;
			break;
		case eKingGrade.Agility:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.AgilityLevel)?.Attack ?? 0;
			break;
		case eKingGrade.Lucky:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.LuckyLevel)?.Attack ?? 0;
			break;
		case eKingGrade.MagicAttack:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.MagicAttackLevel)?.Attack ?? 0;
			break;
		case eKingGrade.MagicDefence:
			currentVal = MaxLevelTemplateMgr.FindMaxLevelTemplate(Data.MagicDefenceLevel)?.Attack ?? 0;
			break;
		}
		return currentVal * value / 10 / 100;
	}

	public void AddGP(long exp)
	{
		bool flag = false;
		lock (m_lock)
		{
			long num = m_data.currentExp + exp;
			if (num > int.MaxValue)
			{
				m_data.currentExp = int.MaxValue;
			}
			else
			{
				m_data.currentExp += Convert.ToInt32(exp);
			}
			if (m_data.currentExp >= GetNextGP() && m_data.ddtKingGrade < MaxKingGrade)
			{
				while (m_data.currentExp >= GetNextGP() && m_data.ddtKingGrade < MaxKingGrade)
				{
					m_data.currentExp -= GetNextGP();
					long num2 = baseUsableNum + (int)Math.Round((double)(m_data.ddtKingGrade / 10));
					if (num2 > int.MaxValue)
					{
						m_data.usableNum = int.MaxValue;
					}
					else
					{
						m_data.usableNum += Convert.ToInt32(num2);
					}
					m_data.ddtKingGrade++;
				}
				flag = true;
			}
			else if (m_data.ddtKingGrade == MaxKingGrade)
			{
				long num3 = m_data.usableNum + baseUsableNum + (int)Math.Round((double)(m_data.ddtKingGrade / 10));
				if (num3 > int.MaxValue)
				{
					m_data.usableNum = int.MaxValue;
				}
				else
				{
					m_data.usableNum += Convert.ToInt32(num3);
				}
				flag = true;
			}
		}
		if (flag)
		{
			Player.PlayerCharacter.ddtKingGrade = Data.ddtKingGrade;
			Player.SendUpdatePublicPlayer();
		}
	}

	public int[] GetAllLevel()
	{
		return new int[6] { Data.AttackLevel, Data.DefenceLevel, Data.AgilityLevel, Data.LuckyLevel, Data.MagicAttackLevel, Data.MagicDefenceLevel };
	}

	public int GetNextGP()
	{
		return LevelMgr.FindLevel(70).GP + LevelMgr.FindLevel(minKingGrade + Data.ddtKingGrade).GP;
	}

	public bool GradeUp(int index)
	{
		bool result = false;
		int num = 0;
		switch (index)
		{
		case 0:
			num = Data.AttackLevel;
			break;
		case 1:
			num = Data.DefenceLevel;
			break;
		case 2:
			num = Data.AgilityLevel;
			break;
		case 3:
			num = Data.LuckyLevel;
			break;
		case 4:
			num = Data.MagicAttackLevel;
			break;
		case 5:
			num = Data.MagicDefenceLevel;
			break;
		}
		MaxLevelTemplateInfo maxLevelTemplateInfo = MaxLevelTemplateMgr.FindMaxLevelTemplate(num + 1);
		if (maxLevelTemplateInfo == null)
		{
			if (num >= MaxLevelTemplateMgr.MaxLevel())
			{
				Player.SendMessage(LanguageMgr.GetTranslation("DDTKingGradeUp.Maxlevel"));
			}
			else
			{
				Player.SendMessage(LanguageMgr.GetTranslation("DDTKingGradeUp.DataError"));
			}
		}
		else if (Data.usableNum < maxLevelTemplateInfo.Cost)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DDTKingGradeUp.CostNotEnought"));
		}
		else
		{
			lock (m_lock)
			{
				switch (index)
				{
				case 0:
					m_data.AttackLevel = maxLevelTemplateInfo.Level;
					break;
				case 1:
					m_data.DefenceLevel = maxLevelTemplateInfo.Level;
					break;
				case 2:
					m_data.AgilityLevel = maxLevelTemplateInfo.Level;
					break;
				case 3:
					m_data.LuckyLevel = maxLevelTemplateInfo.Level;
					break;
				case 4:
					m_data.MagicAttackLevel = maxLevelTemplateInfo.Level;
					break;
				case 5:
					m_data.MagicDefenceLevel = maxLevelTemplateInfo.Level;
					break;
				}
				m_data.usableNum -= maxLevelTemplateInfo.Cost;
			}
			Player.EquipBag.UpdatePlayerProperties();
			UpdateKingInfo();
			result = true;
		}
		return result;
	}

	public void Reset(bool allReset, int index)
	{
		int num = 0;
		MaxLevelTemplateInfo maxLevelTemplateInfo = null;
		int num2 = 0;
		lock (m_lock)
		{
			if (allReset)
			{
				for (num2 = 0; num2 < m_data.AttackLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				for (num2 = 0; num2 < m_data.DefenceLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				for (num2 = 0; num2 < m_data.AgilityLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				for (num2 = 0; num2 < m_data.LuckyLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				for (num2 = 0; num2 < m_data.MagicAttackLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				for (num2 = 0; num2 < m_data.MagicDefenceLevel; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
				m_data.AttackLevel = 0;
				m_data.DefenceLevel = 0;
				m_data.AgilityLevel = 0;
				m_data.LuckyLevel = 0;
				m_data.MagicAttackLevel = 0;
				m_data.MagicDefenceLevel = 0;
			}
			else
			{
				int num3 = 0;
				switch (index)
				{
				case 0:
					num3 = m_data.AttackLevel;
					m_data.AttackLevel = 0;
					break;
				case 1:
					num3 = m_data.DefenceLevel;
					m_data.DefenceLevel = 0;
					break;
				case 2:
					num3 = m_data.AgilityLevel;
					m_data.AgilityLevel = 0;
					break;
				case 3:
					num3 = m_data.LuckyLevel;
					m_data.LuckyLevel = 0;
					break;
				case 4:
					num3 = m_data.MagicAttackLevel;
					m_data.MagicAttackLevel = 0;
					break;
				case 5:
					num3 = m_data.MagicDefenceLevel;
					m_data.MagicDefenceLevel = 0;
					break;
				}
				for (num2 = 0; num2 < num3; num2++)
				{
					num += MaxLevelTemplateMgr.FindMaxLevelTemplate(num2 + 1)?.Cost ?? 0;
				}
			}
			m_data.usableNum += num;
		}
		if (num == 0)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("DDTKingGradeUp.NọtHaveProp"));
			return;
		}
		Player.SendMessage(LanguageMgr.GetTranslation("DDTKingGradeUp.Success"));
		Player.EquipBag.UpdatePlayerProperties();
		UpdateKingInfo();
	}

	public void UpdateKingInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(370, Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(Data.usableNum);
		gSPacketIn.WriteLong(Data.currentExp);
		gSPacketIn.WriteLong(GetNextGP());
		int[] allLevel = GetAllLevel();
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < allLevel.Length; i++)
		{
			num = 0;
			num2 = 0;
			MaxLevelTemplateInfo maxLevelTemplateInfo = MaxLevelTemplateMgr.FindMaxLevelTemplate(allLevel[i]);
			if (maxLevelTemplateInfo == null)
			{
				gSPacketIn.WriteInt(num2);
				gSPacketIn.WriteInt(num);
				continue;
			}
			num2 = maxLevelTemplateInfo.Cost;
			switch (i)
			{
			case 0:
				num = maxLevelTemplateInfo.Attack;
				break;
			case 1:
				num = maxLevelTemplateInfo.Defence;
				break;
			case 2:
				num = maxLevelTemplateInfo.Agility;
				break;
			case 3:
				num = maxLevelTemplateInfo.Lucky;
				break;
			case 4:
				num = maxLevelTemplateInfo.MagicAttack;
				break;
			case 5:
				num = maxLevelTemplateInfo.MagicDefence;
				break;
			}
			gSPacketIn.WriteInt(num2);
			gSPacketIn.WriteInt(num);
		}
		Player.SendTCP(gSPacketIn);
	}
}
