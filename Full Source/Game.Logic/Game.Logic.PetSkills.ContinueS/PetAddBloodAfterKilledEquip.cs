using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetAddBloodAfterKilledEquip : BasePetEffect
{
	private int m_count;

	private int m_value;

	private string m_elementID;

	public PetAddBloodAfterKilledEquip(int count, string elementID)
		: base(ePetEffectType.PetAddBloodAfterKilledEquip, elementID)
	{
		m_elementID = elementID;
		m_count = count;
		switch (elementID)
		{
		case "1172":
			m_value = 2;
			break;
		case "1173":
		case "1468":
			m_value = 4;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddBloodAfterKilledEquip) is PetAddBloodAfterKilledEquip petAddBloodAfterKilledEquip)
		{
			petAddBloodAfterKilledEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_BeginSelfTurn;
		player.AfterKilledByLiving += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
	}

	private void player_BeginSelfTurn(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		int num = 50;
		if (m_elementID == "1468")
		{
			num = 100;
		}
		if (target != living && rand.Next(100) < num)
		{
			int value = living.MaxBlood * m_value / 100;
			living.SyncAtTime = true;
			living.AddBlood(value);
			living.SyncAtTime = false;
		}
	}
}
