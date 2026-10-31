using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class RecoverBloodEffect : BasePlayerEffect
{
	private int m_type = 0;

	private int m_count = 0;

	private int m_probability = 0;

	public RecoverBloodEffect(int count, int probability, int type)
		: base(eEffectType.RecoverBloodEffect)
	{
		m_count = count;
		m_probability = probability;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (!(living.EffectList.GetOfType(eEffectType.RecoverBloodEffect) is RecoverBloodEffect recoverBloodEffect))
		{
			return base.Start(living);
		}
		m_probability = ((m_probability > recoverBloodEffect.m_probability) ? m_probability : recoverBloodEffect.m_probability);
		return true;
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += ChangeProperty;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= ChangeProperty;
	}

	public void ChangeProperty(Living living, Living target, int damageAmount, int criticalAmount)
	{
		m_count = (int)((double)living.MaxBlood * 0.1);
		if (living.IsLiving)
		{
			IsTrigger = false;
			if (AbstractEffect.rand.Next(100) < m_probability && living.DefendActiveGem == m_type)
			{
				IsTrigger = true;
				living.EffectTrigger = true;
				living.SyncAtTime = true;
				living.AddBlood(m_count);
				living.SyncAtTime = false;
				living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
				living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("RecoverBloodEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
