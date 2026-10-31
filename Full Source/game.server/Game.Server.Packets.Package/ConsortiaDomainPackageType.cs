namespace Game.Server.Packets.Package;

internal enum ConsortiaDomainPackageType
{
	RemovePlayer = 1,
	Enter = 2,
	OtherInfo = 3,
	MOVE = 4,
	Fight = 5,
	Active = 7,
	Repair = 8,
	MonsterInfoInFight = 9,
	BuildInfoInFight = 10,
	GetConsortiaInfo = 12,
	KillInfo = 13,
	FightState = 14,
	ActiveSate = 15,
	MonsterInfoSingle = 16,
	buildRepairInfo = 17,
	playerRepairChange = 18,
	ReduceBloodState = 19
}
