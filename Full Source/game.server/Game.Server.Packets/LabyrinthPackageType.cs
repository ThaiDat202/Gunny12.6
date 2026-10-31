namespace Game.Server.Packets;

public enum LabyrinthPackageType
{
	DOUBLE_REWARD = 1,
    REQUEST_UPDATE = 2,
    START_SWEEP = 3,
    QUICK_SWEEP = 4,
    STOP_SWEEP = 5,
    RESET_LABYRINTH = 6,
    SWEEP_INFO = 7,
    SWEEP_COMPLETE = 8,
    TRY_AGAIN = 9,
    REQUEST_RANK = 10, // 0x0000000A
}
