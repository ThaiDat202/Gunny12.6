using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddTurnEquipEffect : BasePlayerEffect
{
	private int m_count = 0;

	private int m_probability = 0;

	private int m_templateID = 0;

	public AddTurnEquipEffect(int count, int probability, int templateID)
		: base(eEffectType.AddTurnEquipEffect)
	{
		m_count = count;
		m_probability = probability;
		m_templateID = templateID;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) is AddTurnEquipEffect addTurnEquipEffect)
		{
			addTurnEquipEffect.m_probability = ((m_probability > addTurnEquipEffect.m_probability) ? m_probability : addTurnEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
		player.BeginNextTurn += player_BeginNextTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
		player.BeginNextTurn -= player_BeginNextTurn;
	}

	public void player_BeginNextTurn(Living living)
	{
		if (IsTrigger && living is Player)
		{
			int energy = 0;
			switch (m_templateID)
			{
			case 311112:
				energy = 100;
				break;
			case 311129:
				energy = 110;
				break;
			case 311212:
				energy = 120;
				break;
			case 311229:
				energy = 130;
				break;
			case 311312:
				energy = 140;
				break;
			case 311329:
				energy = 150;
				break;
			case 311412:
				energy = 160;
				break;
			case 311429:
				energy = 170;
				break;
			case 311512:
				energy = 180;
				break;
			case 311529:
				energy = 190;
				break;
			case 311545:
				energy = 200;
				break;
			}
			(living as Player).Delay += (living as Player).Delay * m_count / 100;
			(living as Player).IsAddTurn = true;
			(living as Player).Energy = energy;
			IsTrigger = false;
		}
	}

	private void ChangeProperty(Player player)
	{
		if (!player.CurrentBall.IsSpecial() && AbstractEffect.rand.Next(100) < m_probability && player.AttackGemLimit == 0)
		{
			player.AttackGemLimit = 2;
			player.Delay = player.DefaultDelay;
			IsTrigger = true;
			player.EffectTrigger = true;
			player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg"), 9, 0, 1000));
		}
	}
}
