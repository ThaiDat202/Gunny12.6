using System.Collections.Generic;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SkillBomb : BombObject
{
	private Living m_owner;

	private BaseGame m_game;

	protected List<BombAction> m_actions;

	protected Tile m_shape;

	private float m_lifeTime;

	private BallInfo m_info;

	public SkillBomb(int id, Living owner, BaseGame game, BallInfo info, Tile shape, int angle)
		: base(id, info.Mass, info.Weight, info.Wind, info.DragIndex)
	{
		m_owner = owner;
		m_game = game;
		m_info = info;
		m_shape = shape;
	}
}
