using System.Collections.Generic;
using System.Drawing;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace Game.Server.GameServerScript.AI.NPC;

public class ThirdNormalKingFour : ABrain
{
	private bool isBornEffect = false;

	private int m_bloodReduce = 2500;

	private int maxShootPlayer = 2;

	private int npcEnemyId = 3118;

	private SimpleNpc npcEnemy = null;

	private int m_turn = 0;

	private List<PhysicalObj> phyFireObjs = new List<PhysicalObj>();

	private List<Point> pointCreatePhy = new List<Point>();

	private int coldDownAttackNPC = 0;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[2] { "Tiếng gầm của hổ ...！", "Cảm nhận sự đau đớn của cổ họng！ " };

	private static string[] ShootChat = new string[3] { "Lửa địa ngục...", "Tam nhị chân hỏa !", "Đốt ngươi chết luôn" };

	private static string[] CallChat = new string[2] { "Xem đây，<br/>Cây đậu đáng ghét!!", "Biến khỏi đây không ??" };

	private static string[] EnemyNPCChat = new string[3] { "Thật nguy hiểm!!", "Ta không chịu thua ngươi đâu!", "Không bao giờ khuất phục." };

	private static string[] KillAttackChat = new string[3] { "Dám đến gần ta, chết đi...", "Nhìn mặt mà ngu vãi!!!", "Ta dẫm cho nát bét!!!!" };

	private static string[] SealChat = new string[2] { "Chạy đường nào đây?", "Tìm chỗ mà núp đi nhé!!" };

	private static string[] KillPlayerChat = new string[1] { "Lửa bất diệt cháy bừng lên đi!" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
		if (coldDownAttackNPC > 0)
		{
			coldDownAttackNPC--;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		if (!isBornEffect)
		{
			BornEffect();
			isBornEffect = true;
			return;
		}
		bool flag = false;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > base.Body.X - 200 && allFightPlayer.X < base.Body.X + 200)
			{
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(base.Body.X - 200, base.Body.X + 200);
			return;
		}
		if (coldDownAttackNPC <= 0)
		{
			coldDownAttackNPC = 5;
			AttackEnemyNPC();
			return;
		}
		switch (m_turn)
		{
		case 0:
		case 1:
			HalfAttack();
			break;
		case 2:
			PersonAttack();
			break;
		case 3:
			GlobalMoveAttack();
			break;
		case 4:
			PersonAttack();
			m_turn = 0;
			break;
		}
		m_turn++;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void HalfAttack()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 1000, 3000);
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 3000, null);
	}

	private void PersonAttack()
	{
		base.Body.CurrentDamagePlus = 1.2f;
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 1, 500);
		Player player = base.Game.FindRandomPlayer();
		if (player == null)
		{
			return;
		}
		base.Body.ChangeDirection(base.Body.FindDirection(player), 100);
		int num2 = 2000;
		for (int i = 0; i < maxShootPlayer; i++)
		{
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 2.3f, num2))
			{
				base.Body.PlayMovie("aim", num2 - 1000, 0);
				base.Body.PlayMovie("beatA", num2 - 500, 0);
			}
			num2 += 2000;
		}
	}

	private void GlobalMoveAttack()
	{
		base.Body.CurrentDamagePlus = 2f;
		int num = base.Game.Random.Next(0, SealChat.Length);
		((SimpleBoss)base.Body).Say(SealChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 0, 1000);
		base.Body.CallFuction(MoveAllPlayer, 2200);
	}

	private void MoveAllPlayer()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			int x = base.Game.Random.Next(200, 1300);
			pointCreatePhy.Add(new Point(x, 500));
			item.BoltMove(x, 500, 50);
		}
		CreateFlameEffect();
		base.Body.RangeAttacking(base.Body.X - 10000, base.Body.Y + 10000, "cry", 100, null);
	}

	private void AttackEnemyNPC()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		((SimpleBoss)base.Body).Say(CallChat[num], 1, 500);
		base.Body.ChangeDirection(base.Body.FindDirection(npcEnemy), 100);
		if (base.Body.ShootPoint(npcEnemy.X, npcEnemy.Y, 53, 1000, 10000, 1, 2f, 2550))
		{
			base.Body.PlayMovie("aim", 1300, 0);
		}
		base.Body.PlayMovie("beatA", 2000, 0);
		npcEnemy.PlayMovie("die", 2000, 0);
		if (npcEnemy.X <= 700)
		{
			npcEnemy.JumpTo(1225, 563, "born", 5000, 0, 100, null, 1);
			npcEnemy.ChangeDirection(-1, 6000);
			base.Body.ChangeDirection(1, 6000);
		}
		else
		{
			npcEnemy.JumpTo(468, 555, "born", 5000, 0, 100, null, 1);
			npcEnemy.ChangeDirection(1, 6000);
			base.Body.ChangeDirection(-1, 6000);
		}
		((PVEGame)base.Game).SendObjectFocus(npcEnemy, 1, 5000, 0);
		num = base.Game.Random.Next(0, EnemyNPCChat.Length);
		npcEnemy.Say(EnemyNPCChat[num], 1, 6500, 2000);
	}

	private void BornEffect()
	{
		base.Body.Say("Thể xác ốm yếu này, đưa ta mượn tạm xem!", 0, 4000);
		base.Body.MoveTo(761, 583, "walk", 5000, 3);
		base.Body.Say("Lửa địa ngục hãy cháy lên!!!", 0, 6000);
		base.Body.PlayMovie("beatC", 6000, 0);
		Player[] allPlayers = base.Game.GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player != null && player.IsLiving)
			{
				player.AddEffect(new ContinueReduceBloodEffect(200, m_bloodReduce, null), 7800);
			}
		}
		base.Body.CallFuction(BornNPCEnemy, 8300);
	}

	private void BornNPCEnemy()
	{
		LivingConfig livingConfig = ((PVEGame)base.Game).BaseConfig();
		livingConfig.CanTakeDamage = false;
		npcEnemy = ((PVEGame)base.Game).CreateNpc(npcEnemyId, 468, 555, 1, 1, livingConfig);
		npcEnemy.Say("Lửa vĩnh cửu thật khó bị dập tắt. Hãy theo sát tôi, tôi sẽ làm giảm sức mạnh của chúng", 0, 1500, 3000);
		coldDownAttackNPC = 5;
	}

	public void CreateFlameEffect()
	{
		foreach (Point item2 in pointCreatePhy)
		{
			PhysicalObj item = ((PVEGame)base.Game).Createlayer(item2.X, item2.Y, "", "game.assetmap.Flame", "", 1, 1);
			phyFireObjs.Add(item);
		}
		pointCreatePhy = new List<Point>();
		base.Body.CallFuction(Remove, 2000);
	}

	public void Remove()
	{
		foreach (PhysicalObj phyFireObj in phyFireObjs)
		{
			if (phyFireObj != null)
			{
				base.Game.RemovePhysicalObj(phyFireObj, sendToClient: true);
			}
		}
		phyFireObjs = new List<PhysicalObj>();
	}

	public void KillAttack(int fx, int mx)
	{
		base.Body.CurrentDamagePlus = 25f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
	}
}
