using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetReduceBloodAfterFinishEquip : BasePetEffect
{
	private int m_value;

	private int m_count;

	private bool percent;

	public PetReduceBloodAfterFinishEquip(int count, string elementID)
		: base(ePetEffectType.PetReduceBloodAfterFinishEquip, elementID)
	{
		m_count = count;
		if (elementID == "1745")
		{
			m_value = 20;
			percent = true;
		}
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceBloodAfterFinishEquip) is PetReduceBloodAfterFinishEquip petReduceBloodAfterFinishEquip)
		{
			petReduceBloodAfterFinishEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (base.ElementInfo.Pic != -1)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
		if (m_count <= 0)
		{
			int num = 0;
			num = ((!percent) ? m_value : Convert.ToInt32(living.MaxBlood * m_value / 100));
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			living.SyncAtTime = true;
			if (num >= living.Blood)
			{
				living.RemoveBlood(living.Blood - 1);
			}
			else
			{
				living.RemoveBlood(num);
			}
			living.SyncAtTime = false;
			Stop();
		}
	}
}
