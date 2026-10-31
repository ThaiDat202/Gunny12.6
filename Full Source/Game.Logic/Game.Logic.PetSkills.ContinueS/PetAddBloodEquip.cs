using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBloodEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	public PetAddBloodEquip(int count, string elementID)
		: base(ePetEffectType.PetAddBloodEquip, elementID)
	{
		m_count = count;
		switch (elementID)
		{
		case "1358":
		case "1361":
			m_value = 2;
			m_percent = true;
			break;
		case "1366":
		case "1362":
			m_value = 3;
			m_percent = true;
			break;
		case "1359":
		case "1367":
		case "1363":
			m_value = 4;
			m_percent = true;
			break;
		case "1455":
			m_value = 5;
			m_percent = true;
			break;
		case "1360":
			m_value = 6;
			m_percent = true;
			break;
		case "1392":
			m_value = 8;
			m_percent = true;
			break;
		case "1393":
		case "2761":
			m_value = 10;
			m_percent = true;
			break;
		case "2762":
			m_value = 12;
			m_percent = true;
			break;
		case "1456":
			m_value = 15;
			m_percent = true;
			break;
		case "1259":
			m_value = 800;
			break;
		case "1260":
			m_value = 1000;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodEquip) is PetAddBloodEquip petAddBloodEquip)
		{
			petAddBloodEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.PlayerClearBuffSkillPet += player_PlayerClearBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.Game.sendShowPicSkil(player, base.ElementInfo, isActive: false);
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
			return;
		}
		living.SyncAtTime = true;
		if (m_percent)
		{
			living.AddBlood(living.MaxBlood * m_value / 100);
		}
		else
		{
			living.AddBlood(m_value);
		}
		living.SyncAtTime = false;
	}

	private void player_PlayerClearBuffSkillPet(Player player)
	{
		Stop();
	}
}
