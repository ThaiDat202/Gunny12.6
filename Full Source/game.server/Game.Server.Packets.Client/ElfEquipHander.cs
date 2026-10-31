using Game.Base.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.Server.Packets.Client
{
    [PacketHandler(707, "?????")]
    internal class ElfEquipHander : IPacketHandler
    {
        public int HandlePacket(GameClient client, GSPacketIn packet)
        {
            client.Player.ElfHandler.ProcessData(client.Player, packet);
            return 1;
        }
    }
}
