using Terraria;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DarklysEnemyExpansion.Content.NPCs
{
    public class GiantCaveBat : ModNPC
    {
        public override void SetStaticDefaults()
        {
            Main.npcFrameCount[NPC.type] = Main.npcFrameCount[NPCID.GiantFlyingFox];
            NPCID.Sets.NPCBestiaryDrawModifiers value = new NPCID.Sets.NPCBestiaryDrawModifiers()
            { // Influences how the NPC looks in the Bestiary
                Velocity = 1f // Draws the NPC in the bestiary as if its walking +1 tiles in the x direction
            };
            NPCID.Sets.NPCBestiaryDrawOffset.Add(Type, value);
        }
        public override void SetDefaults()
        {
            NPC.npcSlots = 0.5f;
            NPC.width = 38;
            NPC.height = 34;
            NPC.aiStyle = NPCAIStyleID.Bat;
            NPC.damage = 80;
            NPC.defense = 24;
            NPC.lifeMax = 220;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.65f;
            NPC.DeathSound = SoundID.NPCDeath4;
            NPC.value = 400f;

            AIType = NPCID.GiantFlyingFox;
            AnimationType = NPCID.GiantFlyingFox;

            NPC.scale = 1.2f;
            Banner = Item.NPCtoBanner(NPCID.GiantFlyingFox);
            BannerItem = Item.BannerToItem(Banner);

        }

        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.GiantBat, false); // false is important here
            foreach (var dropRule in dropRules)
            {
                // In this foreach loop, we simple add each drop to the GiantCaveBat drop pool. 
                npcLoot.Add(dropRule);
            }
        }
        // 占用原版巨型飞狐的生成权重1/5
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (Main.hardMode &&
                !Main.dayTime &&
                player.ZoneJungle &&
                player.ZoneOverworldHeight &&
                !spawnInfo.PlayerSafe)
            {
                return SpawnCondition.HardmodeJungle.Chance * 0.2f;
            }

            return 0f;
        }
        public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
        {
            bestiaryEntry.Info.AddRange([
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Times.NightTime,
                BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Jungle,
                new FlavorTextBestiaryInfoElement("Mods.DarklysEnemyExpansion.Bestiary.GiantCaveBat")
            ]);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            // 如果是专家模式以上，则造成野性撕咬
            if (Main.expertMode)
            {
                target.AddBuff(BuffID.Rabies, 240);
            }
        }
    }
}
