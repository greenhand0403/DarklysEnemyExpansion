using Terraria;
using Terraria.ID;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace DarklysEnemyExpansion.Content.Items.Accessories
{
    /// <summary>
    /// 腐化立方，增加7点防御，4%减伤2%移速，腐化环境+4%伤害
    /// </summary>
    public class CorruptPowerCube : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 32;
            Item.height = 32;
            Item.accessory = true;
            Item.rare = ItemRarityID.Green;
            Item.value = Item.sellPrice(gold: 5);
            Item.defense = 7;
        }
        
        // 4%减伤2%移速，腐化环境+4%伤害
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.endurance += 0.04f;
            player.moveSpeed += 0.02f;

            if (player.ZoneCorrupt)
            {
                player.GetDamage(DamageClass.Generic) += 0.04f;
            }
        }
    }
}
