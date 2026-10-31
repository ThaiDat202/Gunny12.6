using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerBank
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private Dictionary<int, UserBankInfo> m_bank;

	private List<UserBankInfo> m_bankRemoved;

	public GamePlayer Player => m_player;

	public PlayerBank(GamePlayer player)
	{
		m_player = player;
		m_bank = new Dictionary<int, UserBankInfo>();
		m_bankRemoved = new List<UserBankInfo>();
	}

	public virtual void LoadFromDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		List<UserBankInfo> allUsersBank = playerBussiness.GetAllUsersBank(Player.PlayerCharacter.ID);
		foreach (UserBankInfo item in allUsersBank)
		{
			AddBank(item);
		}
	}

	public void AddBank(UserBankInfo bank)
	{
		if (bank.BankID == 0)
		{
			AddBankToDb(bank);
		}
		lock (m_bank)
		{
			if (!m_bank.ContainsKey(bank.BankID))
			{
				m_bank.Add(bank.BankID, bank);
			}
		}
	}

	public UserBankInfo GetSingleBank(int bankId)
	{
		lock (m_bank)
		{
			if (m_bank.ContainsKey(bankId))
			{
				return m_bank[bankId];
			}
			return null;
		}
	}

	public List<UserBankInfo> GetAllBank()
	{
		List<UserBankInfo> list = new List<UserBankInfo>();
		lock (m_bank)
		{
			foreach (UserBankInfo value in m_bank.Values)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public bool RemoveBank(UserBankInfo bank)
	{
		bool flag = false;
		lock (m_bank)
		{
			flag = m_bank.Remove(bank.BankID);
		}
		if (flag)
		{
			bank.IsExits = false;
			lock (m_bankRemoved)
			{
				m_bankRemoved.Add(bank);
			}
		}
		return flag;
	}

	public virtual void SaveToDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_bank)
		{
			foreach (UserBankInfo value in m_bank.Values)
			{
				if (value != null && value.IsDirty)
				{
					playerBussiness.AddOrUpdateUserBank(value);
					value.IsDirty = false;
				}
			}
			foreach (UserBankInfo item in m_bankRemoved)
			{
				playerBussiness.AddOrUpdateUserBank(item);
			}
			m_bankRemoved.Clear();
		}
	}

	public virtual void AddBankToDb(UserBankInfo bank)
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		playerBussiness.AddOrUpdateUserBank(bank);
	}
}
