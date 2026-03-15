using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DarklysEnemyExpansion.Content.NPCs
{
    public class AssassinHornet : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.MossHornet];
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            // 困难模式前最强的黄蜂敌怪
            NPC.width = 34;
            NPC.height = 32;
            NPC.aiStyle = NPCAIStyleID.Flying;

            NPC.damage = 41;
            NPC.defense = 4;
            NPC.lifeMax = 45;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.5f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 200f;
            NPC.noGravity = true;

            AIType = NPCID.Hornet;
            AnimationType = NPCID.MossHornet;

            NPC.scale = 1.2f;
            Banner = Item.NPCtoBanner(NPCID.Hornet);
            BannerItem = Item.BannerToItem(Banner);
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.BigHornetStingy, false); // false is important here
            foreach (var dropRule in dropRules)
            {
                // In this foreach loop, we simple add each drop to the AssassinHornet drop pool. 
                npcLoot.Add(dropRule);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (!Main.hardMode &&
                player.ZoneJungle &&
                player.ZoneUnderworldHeight &&
                !spawnInfo.PlayerSafe)
            {
                return SpawnCondition.UndergroundJungle.Chance * 0.2f;
            }

            return 0f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement("Mods.DarklysEnemyExpansion.Bestiary.AssassinHornet")
            ]);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            // 如果是专家模式以上，则造成中毒
            if (Main.expertMode)
            {
                target.AddBuff(BuffID.Poisoned, 240);
            }
        }
    }
}
