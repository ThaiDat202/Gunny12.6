using Game.Logic.Actions;
using Game.Logic.ElementSkills.ContinueSkills;
using Game.Logic.PetSkills;
using Game.Logic.Phy.Object;

namespace Game.Logic.ElementSkills;

public class ElementForceFreeze : BasePetEffect
{
	private int m_value;

	private int m_skillid;

	public ElementForceFreeze(int skillID, int value)
		: base(ePetEffectType.ElementForceFreeze, "")
	{
		m_value = value;
		m_skillid = skillID;
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKillingLiving += player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (!target.EffectBadges && living != target && living.Game is PVPGame && rand.Next(10000) <= 2000)
		{
			target.EffectBadges = true;
			living.Game.AddAction(new LivingSayAction(living, "极寒之力", 9, 0, 1000));
			living.Game.SendEquipEffect(living, "触发<极寒之力>目标无法攻击或移动2回合");
			new ElementForceFreezeEquip(m_skillid, m_value).Start(target);
		}
	}

	public override bool Start(Living living)
	{
		return base.Start(living);
	}
}
