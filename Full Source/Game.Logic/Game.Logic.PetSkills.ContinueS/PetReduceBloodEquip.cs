using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceBloodEquip : AbstractPetEffect
{
	private int m_count;

	private int m_value;

	private bool m_percent;

	private Living m_liv;

	public PetReduceBloodEquip(int count, string elementID, Living liv)
		: base(ePetEffectType.PetReduceBloodEquip, elementID)
	{
		m_count = count;
		m_liv = liv;
		switch (elementID)
		{
		case "1150":
			m_value = 1;
			m_percent = true;
			break;
		case "1151":
			m_value = 2;
			m_percent = true;
			break;
		case "1152":
			m_value = 3;
			m_percent = true;
			break;
		case "1210":
			m_value = 500;
			break;
		case "1270":
			m_value = 800;
			break;
		case "1170":
		case "1211":
			m_value = 1000;
			break;
		case "1271":
			m_value = 1300;
			break;
		case "1171":
			m_value = 2000;
			break;
		case "1176":
			m_value = 2;
			m_percent = true;
			break;
		case "1801":
			m_value = 3;
			m_percent = true;
			break;
		case "1177":
		case "1717":
		case "1802":
			m_value = 4;
			m_percent = true;
			break;
		case "1803":
		case "1498":
			m_value = 5;
			m_percent = true;
			break;
		case "1718":
			m_value = 6;
			m_percent = true;
			break;
		case "1719":
			m_value = 8;
			m_percent = true;
			break;
		case "2759":
			m_value = 30;
			m_percent = true;
			break;
		case "2760":
			m_value = 20;
			m_percent = true;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodEquip) is PetReduceBloodEquip petReduceBloodEquip)
		{
			petReduceBloodEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginSelfTurn;
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			return;
		}
		living.SyncAtTime = true;
		if (m_percent)
		{
			living.AddBlood(-(living.MaxBlood * m_value / 100), 1);
		}
		else
		{
			living.AddBlood(-m_value, 1);
		}
		living.SyncAtTime = false;
		if (living.Blood <= 0)
		{
			living.Die();
			if (m_liv != null && m_liv is Player)
			{
				(m_liv as Player).PlayerDetail.OnKillingLiving(m_liv.Game, 2, living.Id, living.IsLiving, m_value);
			}
		}
	}
}
