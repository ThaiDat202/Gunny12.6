using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Academy.Handle;

[AcademyHandle(8)]
public class MasterRefuse : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		AcademyRequestInfo request = AcademyMgr.GetRequest(num, player.PlayerId);
		if (request != null && request.Type == 1)
		{
			AcademyMgr.RemoveRequest(request);
			WorldMgr.GetPlayerById(num)?.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("MasterRefuse", player.PlayerCharacter.NickName), isAlert: false);
		}
		return 0;
	}
}
