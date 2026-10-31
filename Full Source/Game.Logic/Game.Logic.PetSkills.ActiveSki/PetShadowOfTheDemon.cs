using System.Collections.Generic;
using System.Drawing;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetSkills.ActiveSkills;

public class PetShadowOfTheDemon : BasePetEffect
{
	private int m_probability = 0;

	private int m_currentId;

	public PetShadowOfTheDemon(int probability, int skillId, string elementID)
		: base(ePetEffectType.PetShadowOfTheDemon, elementID)
	{
		m_probability = ((probability == -1) ? 10000 : probability);
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetShadowOfTheDemon) is PetShadowOfTheDemon petShadowOfTheDemon)
		{
			petShadowOfTheDemon.m_probability = ((m_probability > petShadowOfTheDemon.m_probability) ? m_probability : petShadowOfTheDemon.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_PlayerBuffSkillPet;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_PlayerBuffSkillPet;
	}

	private void player_PlayerBuffSkillPet(Player player)
	{
		if (player.PetEffects.CurrentUseSkill != m_currentId || !(player.Game is PVPGame))
		{
			return;
		}
		List<Point> list = new List<Point>();
		List<Player> allEnemyPlayers = player.Game.GetAllEnemyPlayers(player);
		int num = 0;
		while (num < 8)
		{
			list.Add(player.Game.GetPlayerPoint(m_living.Team));
		}
		foreach (Player item in allEnemyPlayers)
		{
			list.Add(new Point(item.X, item.Y));
		}
		List<Player> allTeamPlayers = player.Game.GetAllTeamPlayers(player);
		foreach (Player item2 in allTeamPlayers)
		{
			list.Add(new Point(item2.X, item2.Y));
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (Player item3 in allEnemyPlayers)
		{
			int index = rand.Next(list.Count);
			item3.SetXY(list[index]);
			item3.StartMoving();
		}
		foreach (Player item4 in allTeamPlayers)
		{
			int index = rand.Next(list.Count);
			item4.SetXY(list[index]);
			item4.StartMoving();
		}
	}
}
