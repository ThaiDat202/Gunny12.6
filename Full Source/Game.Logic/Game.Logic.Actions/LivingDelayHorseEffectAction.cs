using Game.Logic.HorseEffects;
using Game.Logic.Phy.Object;

namespace Game.Logic.Actions;

public class LivingDelayHorseEffectAction : BaseAction
{
	private AbstractHorseEffect m_effect;

	private Living m_living;

	public LivingDelayHorseEffectAction(Living living, AbstractHorseEffect effect, int delay)
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
