using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerDice
{
	public int MAX_LEVEL = 5;

	public int refreshPrice = GameProperties.DiceRefreshPrice;

	public int commonDicePrice = GameProperties.CommonDicePrice;

	public int doubleDicePrice = GameProperties.DoubleDicePrice;

	public int bigDicePrice = GameProperties.BigDicePrice;

	public int smallDicePrice = GameProperties.SmallDicePrice;

	public int[] IntegralPoint = new int[5] { 100, 300, 700, 1500, 3100 };

	protected GamePlayer m_player;

	protected object m_lock = new object();

	private int m_result;

	private List<EventAwardInfo> m_rewardItem;

	private string m_rewardName;

	private Dictionary<int, List<DiceLevelAwardInfo>> m_LevelAward;

	private DiceDataInfo m_diceData;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public int result
	{
		get
		{
			return m_result;
		}
		set
		{
			m_result = value;
		}
	}

	public List<EventAwardInfo> RewardItem
	{
		get
		{
			return m_rewardItem;
		}
		set
		{
			m_rewardItem = value;
		}
	}

	public string RewardName
	{
		get
		{
			return m_rewardName;
		}
		set
		{
			m_rewardName = value;
		}
	}

	public Dictionary<int, List<DiceLevelAwardInfo>> LevelAward
	{
		get
		{
			return m_LevelAward;
		}
		set
		{
			m_LevelAward = value;
		}
	}

	public DiceDataInfo Data
	{
		get
		{
			return m_diceData;
		}
		set
		{
			m_diceData = value;
		}
	}

	public PlayerDice(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_result = 0;
		m_rewardName = "";
	}

	public void LoadFromDatabase()
	{
		if (!IsDiceOpen())
		{
			return;
		}
		if (m_diceData == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			try
			{
				m_diceData = playerBussiness.GetSingleDiceData(Player.PlayerCharacter.ID);
				if (m_diceData == null)
				{
					SetupDiceData();
				}
			}
			finally
			{
			}
		}
		ReceiveLevelAward();
	}

	public bool IsDiceOpen()
	{
		DateTime dateTime = Convert.ToDateTime(GameProperties.DiceBeginTime);
		DateTime dateTime2 = Convert.ToDateTime(GameProperties.DiceEndTime);
		return DateTime.Now.Date < dateTime2.Date;
	}

	public void SendDiceActiveOpen()
	{
		if (IsDiceOpen())
		{
			Player.Out.SendDiceActiveOpen(this);
		}
	}

	public void Reset()
	{
		if (!IsDiceOpen() || m_diceData == null)
		{
			return;
		}
		lock (m_lock)
		{
			m_diceData.LuckIntegral = 0;
			m_diceData.CurrentPosition = -1;
			m_diceData.LuckIntegralLevel = -1;
			m_diceData.UserFirstCell = false;
			m_diceData.FreeCount = 3;
			m_diceData.Level = 0;
		}
	}

	private void SetupDiceData()
	{
		lock (m_lock)
		{
			m_diceData = new DiceDataInfo();
			m_diceData.UserID = Player.PlayerCharacter.ID;
			m_diceData.LuckIntegral = 0;
			m_diceData.CurrentPosition = -1;
			m_diceData.LuckIntegralLevel = -1;
			m_diceData.UserFirstCell = false;
			m_diceData.FreeCount = 3;
			m_diceData.Level = 0;
			m_diceData.AwardArray = "";
		}
	}

	public void ReceiveData()
	{
		if (string.IsNullOrEmpty(m_diceData.AwardArray))
		{
			CreateDiceAward();
			return;
		}
		m_rewardItem = new List<EventAwardInfo>();
		string[] array = m_diceData.AwardArray.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			string[] array3 = text.Split(',');
			EventAwardInfo eventAwardInfo = new EventAwardInfo();
			eventAwardInfo.TemplateID = int.Parse(array3[0]);
			eventAwardInfo.StrengthenLevel = int.Parse(array3[1]);
			eventAwardInfo.Count = int.Parse(array3[2]);
			eventAwardInfo.ValidDate = int.Parse(array3[3]);
			eventAwardInfo.IsBinds = bool.Parse(array3[4]);
			m_rewardItem.Add(eventAwardInfo);
		}
		if (m_rewardItem.Count < 19)
		{
			CreateDiceAward();
		}
	}

	public void ReceiveLevelAward()
	{
		lock (m_lock)
		{
			m_LevelAward = new Dictionary<int, List<DiceLevelAwardInfo>>();
			for (int i = 0; i < MAX_LEVEL; i++)
			{
				List<DiceLevelAwardInfo> value = DiceLevelAwardMgr.FindDiceLevelAward(i + 1);
				if (!m_LevelAward.ContainsKey(i))
				{
					m_LevelAward.Add(i, value);
				}
				else
				{
					m_LevelAward[i] = value;
				}
			}
		}
	}

	public void GetLevelAward()
	{
		StringBuilder stringBuilder = new StringBuilder();
		IList<DiceLevelAwardInfo> list = m_LevelAward[m_diceData.LuckIntegralLevel];
		foreach (DiceLevelAwardInfo item in list)
		{
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
			if (itemTemplateInfo != null)
			{
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item.Count, 103);
				Player.AddTemplate(itemInfo);
				stringBuilder.Append(itemInfo.Template.Name + "x" + itemInfo.Count + "; ");
			}
		}
		if (m_diceData.LuckIntegralLevel > 3)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("PlayerDice.GetLevelAward", stringBuilder.ToString()));
		}
	}

	public void CreateDiceAward()
	{
		lock (m_lock)
		{
			m_rewardItem = new List<EventAwardInfo>();
			Dictionary<int, EventAwardInfo> dictionary = new Dictionary<int, EventAwardInfo>();
			int num = 0;
			while (m_rewardItem.Count < 19)
			{
				List<EventAwardInfo> diceAward = EventAwardMgr.GetDiceAward(eEventType.DICE);
				if (diceAward.Count > 0)
				{
					EventAwardInfo eventAwardInfo = diceAward[0];
					if (!dictionary.Keys.Contains(eventAwardInfo.TemplateID))
					{
						dictionary.Add(eventAwardInfo.TemplateID, eventAwardInfo);
						m_rewardItem.Add(eventAwardInfo);
					}
				}
				num++;
			}
		}
		ConvertAwardArray();
	}

	public void ConvertAwardArray()
	{
		if (m_rewardItem.Count <= 0)
		{
			return;
		}
		string text = "";
		foreach (EventAwardInfo item in m_rewardItem)
		{
			text += $"{item.TemplateID},{item.StrengthenLevel},{item.Count},{item.ValidDate},{item.IsBinds.ToString()}|";
		}
		text = text.Substring(0, text.Length - 1);
		m_diceData.AwardArray = text;
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_diceData != null && m_diceData.IsDirty)
			{
				if (m_diceData.ID > 0)
				{
					playerBussiness.UpdateDiceData(m_diceData);
				}
				else
				{
					playerBussiness.AddDiceData(m_diceData);
				}
			}
		}
	}
}
