using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Academy.Handle;

[AcademyHandle(9)]
public class ApprenticeRefuse : IAcademyCommandHandler
{
	public int CommandHandler(GamePlayer player, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		AcademyRequestInfo request = AcademyMgr.GetRequest(num, player.PlayerId);
		if (request != null && request.Type == 0)
		{
			AcademyMgr.RemoveRequest(request);
			WorldMgr.GetPlayerById(num)?.Out.SendAcademySystemNotice(LanguageMgr.GetTranslation("ApprenticeRefuse", player.PlayerCharacter.NickName), isAlert: false);
		}
		return 0;
	}
}
