using System.Collections.Generic;
using System.Reflection;
using Game.Server.GameObjects;
using log4net;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerProperty
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected GamePlayer m_player;

	private Dictionary<string, Dictionary<string, int>> m_playerProperty;

	private Dictionary<string, Dictionary<string, int>> m_otherPlayerProperty;

	private int m_loading;

	private int m_totalDamage;

	private int m_totalArmor;

	public GamePlayer Player => m_player;

	public Dictionary<string, Dictionary<string, int>> Current
	{
		get
		{
			return m_playerProperty;
		}
		set
		{
			m_playerProperty = value;
		}
	}

	public Dictionary<string, Dictionary<string, int>> OtherPlayerProperty
	{
		get
		{
			return m_playerProperty;
		}
		set
		{
			m_playerProperty = value;
		}
	}

	public int Loading
	{
		get
		{
			return m_loading;
		}
		set
		{
			m_loading = value;
		}
	}

	public int totalDamage
	{
		get
		{
			return m_totalDamage;
		}
		set
		{
			m_totalDamage = value;
		}
	}

	public int totalArmor
	{
		get
		{
			return m_totalArmor;
		}
		set
		{
			m_totalArmor = value;
		}
	}

	public PlayerProperty(GamePlayer player)
	{
		m_player = player;
		m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
		m_otherPlayerProperty = new Dictionary<string, Dictionary<string, int>>();
		m_loading = 0;
		m_totalDamage = 0;
		m_totalArmor = 0;
		CreateProp(isSelf: true, "Texp", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Card", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Pet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Suit", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Gem", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Bead", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Avatar", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "MagicStone", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Horse", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "HorsePicCherish", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Enchant", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Temple", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "PetAtlas", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Mark", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "SeloPet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "CardAchievement", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "titleAdd", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "honor", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "marKing", 0, 0, 0, 0, 0);
		CreateBaseProp(isSelf: true);
		CreateProp(isSelf: false, "Texp", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Card", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Pet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Suit", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Gem", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Bead", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Avatar", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "MagicStone", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Horse", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "HorsePicCherish", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Enchant", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Temple", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "PetAtlas", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Mark", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "SeloPet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "CardAchievement", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "titleAdd", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "honor", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "marKing", 0, 0, 0, 0, 0);
		CreateBaseProp(isSelf: false);
	}

	public void AddProp(string key, Dictionary<string, int> propAdd)
	{
		if (!m_playerProperty.ContainsKey(key))
		{
			m_playerProperty.Add(key, propAdd);
		}
		else
		{
			m_playerProperty[key] = propAdd;
		}
	}

	public void AddOtherProp(string key, Dictionary<string, int> propAdd)
	{
		if (!m_playerProperty.ContainsKey(key))
		{
			m_otherPlayerProperty.Add(key, propAdd);
		}
		else
		{
			m_otherPlayerProperty[key] = propAdd;
		}
	}

	public void ViewCurrent()
	{
		m_player.Out.SendUpdatePlayerProperty(m_player.PlayerCharacter, this, m_player.PlayerCharacter.CardInfo);
	}

	public void ViewOther(PlayerInfo player)
	{
		m_player.Out.SendOtherPlayerProperty(player, this, m_player.PlayerCharacter.CardInfo);
	}

	public void CreateBaseProp(bool isSelf)
	{
		Dictionary<string, int> propAdd = new Dictionary<string, int>
		{
			{ "MagicStone", 0 },
			{ "Horse", 0 },
			{ "HorsePicCherish", 0 },
			{ "Enchant", 0 },
			{ "Suit", 0 },
			{ "Texp", 0 },
			{ "Card", 0 },
			{ "Mark", 0 },
			{ "SeloPet", 0 },
			{ "CardAchievement", 0 },
			{ "magicHouse", 0 },
			{ "marKing", 0 }
		};
		Dictionary<string, int> propAdd2 = new Dictionary<string, int>
		{
			{ "MagicStone", 0 },
			{ "Horse", 0 },
			{ "HorsePicCherish", 0 },
			{ "Enchant", 0 },
			{ "Suit", 0 },
			{ "Texp", 0 },
			{ "Temple", 0 },
			{ "Card", 0 },
			{ "Mark", 0 },
			{ "SeloPet", 0 },
			{ "CardAchievement", 0 },
			{ "magicHouse", 0 },
			{ "marKing", 0 }
		};
		Dictionary<string, int> propAdd3 = new Dictionary<string, int>
		{
			{ "Bead", 0 },
			{ "Suit", 0 },
			{ "Texp", 0 },
			{ "Avatar", 0 },
			{ "Horse", 0 },
			{ "HorsePicCherish", 0 },
			{ "Mark", 0 },
			{ "SeloPet", 0 },
			{ "CardAchievement", 0 },
			{ "Card", 0 }
		};
		Dictionary<string, int> propAdd4 = new Dictionary<string, int>
		{
			{ "Bead", 0 },
			{ "Suit", 0 },
			{ "Texp", 0 },
			{ "Avatar", 0 },
			{ "Horse", 0 },
			{ "HorsePicCherish", 0 },
			{ "Temple", 0 },
			{ "Pet", 0 },
			{ "Mark", 0 },
			{ "SeloPet", 0 },
			{ "CardAchievement", 0 },
			{ "Card", 0 }
		};
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Emblem", 0);
		dictionary.Add("Elf", 0);
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		dictionary2.Add("Emblem", 0);
		dictionary2.Add("Elf", 0);
		Dictionary<string, int> dictionary3 = new Dictionary<string, int>();
		dictionary3.Add("Emblem", 0);
		dictionary3.Add("Elf", 0);
		Dictionary<string, int> dictionary4 = new Dictionary<string, int>();
		dictionary4.Add("Emblem", 0);
		dictionary4.Add("Elf", 0);
		Dictionary<string, int> dictionary5 = new Dictionary<string, int>();
		dictionary5.Add("Emblem", 0);
		dictionary5.Add("Elf", 0);
		Dictionary<string, int> dictionary6 = new Dictionary<string, int>();
		dictionary6.Add("Emblem", 0);
		dictionary6.Add("Elf", 0);
		Dictionary<string, int> dictionary7 = new Dictionary<string, int>();
		dictionary7.Add("Emblem", 0);
		dictionary7.Add("Elf", 0);
		Dictionary<string, int> dictionary8 = new Dictionary<string, int>();
		dictionary8.Add("Emblem", 0);
		dictionary8.Add("Elf", 0);
		if (isSelf)
		{
			AddProp("Damage", propAdd3);
			AddProp("Armor", propAdd4);
			AddProp("MagicAttack", propAdd);
			AddProp("MagicDefence", propAdd2);
			AddProp("FireAttack", dictionary);
			AddProp("WaterAttack", dictionary2);
			AddProp("WindAttack", dictionary3);
			AddProp("LandAttack", dictionary4);
			AddProp("FireDefence", dictionary5);
			AddProp("WaterDefence", dictionary6);
			AddProp("WindDefence", dictionary7);
			AddProp("LandDefence", dictionary8);
		}
		else
		{
			AddOtherProp("Damage", propAdd3);
			AddOtherProp("Armor", propAdd4);
			AddOtherProp("MagicAttack", propAdd);
			AddOtherProp("MagicDefence", propAdd2);
			AddOtherProp("FireAttack", dictionary);
			AddOtherProp("WaterAttack", dictionary2);
			AddOtherProp("WindAttack", dictionary3);
			AddOtherProp("LandAttack", dictionary4);
			AddOtherProp("FireDefence", dictionary5);
			AddOtherProp("WaterDefence", dictionary6);
			AddOtherProp("WindDefence", dictionary7);
			AddOtherProp("LandDefence", dictionary8);
		}
	}

	public void UpadateBaseProp(bool isSelf, string mainKey, string subKey, double value)
	{
		if (isSelf)
		{
			if (m_playerProperty.ContainsKey(mainKey) && m_playerProperty[mainKey].ContainsKey(subKey))
			{
				m_playerProperty[mainKey][subKey] = (int)value;
			}
		}
		else if (m_otherPlayerProperty.ContainsKey(mainKey) && m_otherPlayerProperty[mainKey].ContainsKey(subKey))
		{
			m_otherPlayerProperty[mainKey][subKey] = (int)value;
		}
	}

	public void CreateProp(bool isSelf, string skey, int attack, int defence, int agility, int lucky, int hp)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Attack", attack);
		dictionary.Add("Defence", defence);
		dictionary.Add("Agility", agility);
		dictionary.Add("Luck", lucky);
		dictionary.Add("HP", hp);
		if (isSelf)
		{
			AddProp(skey, dictionary);
		}
		else
		{
			AddOtherProp(skey, dictionary);
		}
	}
}
