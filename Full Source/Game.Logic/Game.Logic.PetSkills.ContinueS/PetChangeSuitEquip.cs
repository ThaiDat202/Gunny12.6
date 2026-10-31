using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ContinueSkills;

public class PetChangeSuitEquip : BasePetEffect
{
	private int m_count;

	private string m_suit;

	public PetChangeSuitEquip(int count, string suit, string elementID)
		: base(ePetEffectType.PetChangeSuitEquip, elementID)
	{
		m_count = count;
		m_suit = suit;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetChangeSuitEquip) is PetChangeSuitEquip petChangeSuitEquip)
		{
			petChangeSuitEquip.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		string[] array = player.PlayerDetail.PlayerCharacter.Style.Split(',');
		array[7] = m_suit;
		string style = string.Join(",", array);
		player.Game.SendTempStyle(player, style, isTemp: true);
		player.BeginSelfTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_BeginSelfTurn;
	}

	private void player_BeginSelfTurn(Living living)
	{
		if (base.ElementInfo.Pic != -1)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
		}
		m_count--;
		if (m_count < 0)
		{
			living.Game.sendShowPicSkil(living, base.ElementInfo, isActive: false);
			living.Game.SendTempStyle(living as Player, "", isTemp: false);
			Stop();
		}
	}
}
