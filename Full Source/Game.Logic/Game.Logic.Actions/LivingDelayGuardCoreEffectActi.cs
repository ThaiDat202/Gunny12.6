using Game.Logic.GuardCoreEffects;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingDelayGuardCoreEffectAction : BaseAction
{
	private AbstractGuardCoreEffect m_effect;

	private Living m_living;

	public LivingDelayGuardCoreEffectAction(Living living, AbstractGuardCoreEffect effect, int delay)
		: base(delay)
	{
		m_effect = effect;
		m_living = living;
	}

	protected override void ExecuteImp(BaseGame game, long tick)
	{
		m_effect.Start(m_living);
		Finish(tick);
	}
}
