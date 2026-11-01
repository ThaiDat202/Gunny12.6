using Bussiness.Managers;
using EntityDatabase.ServerModels;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(637, "物品注灵")]
public class ItemEnchantSpiritHandler : IPacketHandler
{
    public int HandlePacket(GameClient client, GSPacketIn packet)
    {
        ItemInfo itemAt = client.Player.StoreBag.GetItemAt(1);
        if (itemAt != null && itemAt.IsEnchant())
        {
            TS_UpgradeTemplate upgradeInjetarFusao = ItemMgr.getUpgradeInjetarFusao(itemAt.enchantSpiritLevel + 1);
            if (itemAt.MagicLevel == 0)
            {
                client.Player.SendMessage("Không thể đúc hồn!");
                return 0;
            }
            if (upgradeInjetarFusao != null)
            {
                ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(upgradeInjetarFusao.ItemTempId1);
                ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(upgradeInjetarFusao.ItemTempId2);
                int param = upgradeInjetarFusao.Param1;
                int param2 = upgradeInjetarFusao.Param2;
                int itemCount = client.Player.PropBag.GetItemCount(itemTemplateInfo.TemplateID);
                int itemCount2 = client.Player.PropBag.GetItemCount(itemTemplateInfo2.TemplateID);
                if (itemCount >= param && itemCount2 >= param2)
                {
                    client.Player.PropBag.RemoveTemplate(itemTemplateInfo.TemplateID, param);
                    client.Player.PropBag.RemoveTemplate(itemTemplateInfo2.TemplateID, param2);
                    itemAt.enchantSpiritLevel++;
                    if (!itemAt.IsBinds)
                    {
                        itemAt.IsBinds = true;
                    }
                    client.Player.StoreBag.UpdateItem(itemAt);
                    client.Player.SendMessage("Đúc hồn thành công.");
                    client.Player.EquipBag.UpdatePlayerProperties();
                }
                else
                {
                    client.Player.SendMessage("Không đủ đá đúc hồn.");
                }
            }
            else
            {
                client.Player.SendMessage("Đúc hồn tối đa.");
            }
        }
        else
        {
            client.Player.SendMessage("Vật phẩm này hiện không thể dùng.");
        }
        return 0;
    }
}
