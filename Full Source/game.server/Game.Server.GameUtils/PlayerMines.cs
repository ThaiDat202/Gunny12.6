using System;
using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerMines
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	private ThreadSafeRandom m_random;

	protected GamePlayer m_player;

	private UserMinesInfo m_data;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public UserMinesInfo Data
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

	public PlayerMines(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		CreateMinesData();
		m_random = new ThreadSafeRandom();
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
			UserMinesInfo usersMines = playerBussiness.GetUsersMines(Player.PlayerCharacter.ID);
			if (usersMines != null)
			{
				lock (m_lock)
				{
					m_data = usersMines;
					return;
				}
			}
		}
		finally
		{
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
			playerBussiness.AddOrUpdateUserMines(Data, isUpdate: false);
		}
		else
		{
			playerBussiness.AddOrUpdateUserMines(Data, isUpdate: true);
		}
	}

	private void CreateMinesData()
	{
		m_data = new UserMinesInfo();
		m_data.LastTimeDig = DateTime.Now;
	}

	public ItemTemplateInfo GetItems(int floor)
	{
		ItemTemplateInfo result = null;
		OnlineArmLevelInfo levelArmWithExp = MinesMgr.GetLevelArmWithExp(m_data.ToolExp, "pickaxe");
		if (levelArmWithExp != null)
		{
			OnlineArmDropInfo armDropRate = MinesMgr.GetArmDropRate(floor, levelArmWithExp.LevelCount);
			if (armDropRate != null)
			{
				int rand = m_random.Next(100);
				result = MinesMgr.GetItemDig(armDropRate, rand);
				AddTimeDig();
			}
		}
		return result;
	}

	public void AddTimeDig()
	{
		int num = m_data.TotalTimesDig / 60;
		m_data.TotalTimesDig += GameProperties.OnlineArmPerTimeDig;
		if (m_data.TotalTimesDig / 60 > num)
		{
			m_player.RemoveMissionEnergy(GameProperties.OnlineArmCostEnergy);
		}
	}

	public void AddExp(int type, int exp)
	{
		switch (type)
		{
		case 0:
			m_data.ToolExp += exp;
			break;
		case 1:
			m_data.HeadExp += exp;
			break;
		case 2:
			m_data.ClothExp += exp;
			break;
		case 3:
			m_data.WeaponExp += exp;
			break;
		case 4:
			m_data.ShieldExp += exp;
			break;
		}
	}
}
