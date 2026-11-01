using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class SixNormalSecondNpc : ABrain
{
	private int m_attackTurn = 0;

	private int m_stepMove = 0;

	private List<Point> m_point = new List<Point>
	{
		new Point(620, 1080),
		new Point(620, 980),
		new Point(720, 980),
		new Point(820, 980),
		new Point(920, 980),
		new Point(1020, 980),
		new Point(1120, 980),
		new Point(1220, 980),
		new Point(1320, 980),
		new Point(1420, 980),
		new Point(1520, 980),
		new Point(1620, 980),
		new Point(1620, 830),
		new Point(1520, 830),
		new Point(1420, 830),
		new Point(1320, 830),
		new Point(1220, 830),
		new Point(1120, 830),
		new Point(1020, 830),
		new Point(920, 830),
		new Point(820, 830),
		new Point(720, 830),
		new Point(620, 830),
		new Point(620, 680),
		new Point(720, 680),
		new Point(820, 680),
		new Point(920, 680),
		new Point(1020, 680),
		new Point(1120, 680),
		new Point(1220, 680),
		new Point(1320, 680),
		new Point(1420, 680),
		new Point(1520, 680),
		new Point(1620, 680),
		new Point(1620, 530),
		new Point(1520, 530),
		new Point(1420, 530),
		new Point(1320, 530),
		new Point(1220, 530),
		new Point(1120, 530),
		new Point(1020, 530),
		new Point(920, 530),
		new Point(820, 530),
		new Point(720, 530),
		new Point(620, 530),
		new Point(620, 380),
		new Point(720, 380),
		new Point(820, 380),
		new Point(920, 380),
		new Point(1020, 380),
		new Point(1120, 380),
		new Point(1220, 380),
		new Point(1320, 380),
		new Point(1420, 380),
		new Point(1520, 380),
		new Point(1620, 380),
		new Point(1620, 260)
	};

	private List<Point> m_pointAward = new List<Point>
	{
		new Point(700, 260),
		new Point(800, 260),
		new Point(900, 260),
		new Point(1000, 260),
		new Point(1100, 260),
		new Point(1200, 260),
		new Point(1300, 260),
		new Point(1400, 260),
		new Point(1400, 260),
		new Point(1500, 260)
	};

	private int m_countStepMove = 0;

	private bool m_firstMove = true;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (base.Body.Blood > 1)
		{
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "standB", "stand");
			base.Body.PlayMovie("stand", 0, 0);
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
		m_stepMove = 1;
		m_countStepMove = base.Body.Config.MaxStepMove;
		base.Body.Config.CompleteStep = false;
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_attackTurn++;
		if (m_firstMove)
		{
			m_countStepMove = base.Body.Config.FirstStepMove;
		}
		m_firstMove = false;
		if (base.Body.Blood > 1)
		{
			MovePlace();
		}
	}

	private void MovePlace()
	{
		if (m_stepMove >= m_point.Count)
		{
			if ((base.Game as PVEGame).CountMosterPlace < m_pointAward.Count && !base.Body.Config.CompleteStep)
			{
				Point point = m_pointAward[(base.Game as PVEGame).CountMosterPlace];
				(base.Game as PVEGame).CountMosterPlace++;
				base.Body.MoveTo(point.X, point.Y, "flyLR", 0, "happy", 8, null);
				base.Body.Config.CompleteStep = true;
			}
			return;
		}
		if (m_stepMove == m_point.Count - 1)
		{
			m_countStepMove++;
		}
		Point point2 = m_point[m_stepMove];
		string action = "walk";
		if (point2.X == base.Body.X && (point2.Y == 920 || point2.Y == 760))
		{
			action = "flyUp";
		}
		else if (point2.X >= 620)
		{
			action = "flyLR";
		}
		m_stepMove++;
		if (m_stepMove <= m_countStepMove && m_stepMove <= m_point.Count)
		{
			base.Body.MoveTo(point2.X, point2.Y, action, 0, 8, MovePlace);
			return;
		}
		m_countStepMove = m_stepMove + base.Body.Config.MaxStepMove;
		base.Body.MoveTo(point2.X, point2.Y, action, 0, 8);
	}

	public override void OnDie()
	{
		base.OnDie();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public override void OnBeforeTakedDamage(Living source, ref int damage, ref int crit)
	{
		if (damage + crit >= base.Body.Blood)
		{
			damage = base.Body.Blood - 1;
			crit = 0;
			((PVEGame)base.Game).SendLivingActionMapping(base.Body, "stand", "standB");
			base.Body.PlayMovie("standB", 0, 0);
		}
		base.OnBeforeTakedDamage(source, ref damage, ref crit);
	}
}
