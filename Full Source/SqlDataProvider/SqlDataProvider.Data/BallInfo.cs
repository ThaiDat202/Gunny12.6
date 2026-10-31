namespace SqlDataProvider.Data;

public class BallInfo
{
	public int ID { get; set; }

	public string Name { get; set; }

	public string Crater { get; set; }

	public int AttackResponse { get; set; }

	public double Power { get; set; }

	public int Radii { get; set; }

	public int Amount { get; set; }

	public string FlyingPartical { get; set; }

	public string BombPartical { get; set; }

	public bool IsSpin { get; set; }

	public int Mass { get; set; }

	public double SpinVA { get; set; }

	public int SpinV { get; set; }

	public int Wind { get; set; }

	public int DragIndex { get; set; }

	public int Weight { get; set; }

	public bool Shake { get; set; }

	public int Delay { get; set; }

	public string ShootSound { get; set; }

	public string BombSound { get; set; }

	public int ActionType { get; set; }

	public bool HasTunnel { get; set; }

	public int BombType { get; set; }

	public bool IsSpecial()
	{
		switch (ID)
		{
		case 1:
		case 3:
		case 5:
		case 16:
		case 51:
		case 59:
		case 61:
		case 64:
		case 86:
		case 96:
		case 97:
		case 98:
		case 110:
		case 117:
		case 180:
		case 186:
		case 187:
		case 298:
		case 10001:
		case 10002:
		case 10003:
		case 10004:
		case 10005:
		case 10006:
		case 10007:
		case 10008:
		case 10009:
		case 10010:
		case 10011:
		case 10012:
		case 10013:
		case 10014:
		case 10015:
		case 10016:
		case 10017:
		case 10018:
		case 10019:
		case 10020:
		case 10021:
		case 10022:
		case 10023:
		case 11392:
		case 11398:
		case 11403:
		case 11412:
		case 11416:
		case 11417:
		case 11423:
		case 11431:
		case 11432:
		case 11439:
		case 11448:
		case 11458:
		case 11459:
			return true;
		default:
			return false;
		}
	}
}
