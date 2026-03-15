using System;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.GameContent.Bestiary;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Utilities;

namespace DarklysEnemyExpansion.Content.NPCs
{
    public class AssassinMossHornet : ModNPC
    {
        private int lastProcessedProjectile = -1;
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
            NPC.width = 34;
            NPC.height = 32;
            NPC.aiStyle = NPCAIStyleID.Flying;
            NPC.damage = 70;
            NPC.defense = 22;
            NPC.lifeMax = 220;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.knockBackResist = 0.5f;
            NPC.DeathSound = SoundID.NPCDeath1;
            NPC.value = 600f;
            NPC.noGravity = true;
            NPC.npcSlots = 1.5f;

            AIType = NPCID.MossHornet;
            AnimationType = NPCID.MossHornet;

            NPC.scale = 1.2f;
            Banner = Item.NPCtoBanner(NPCID.MossHornet);
            BannerItem = Item.BannerToItem(Banner);
        }
        public override void PostAI()
        {
            TryAddExtraStingers();
        }
        private void TryAddExtraStingers()
        {
            // 在原版青苔黄蜂的基础上添加额外的刺
            NPCAimedTarget targetData = NPC.GetTargetData();
            if (NPC.ai[1] == 101f)
            {
                if (targetData.Type != NPCTargetType.None && Collision.CanHit(NPC, targetData))
                {
                    float velocity = 8f;
                    // 发射口的位置
                    Vector2 spawnPos = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)(NPC.height / 2));
                    float targetX = targetData.Center.X - spawnPos.X + (float)Main.rand.Next(-20, 21);
                    float targetY = targetData.Center.Y - spawnPos.Y + (float)Main.rand.Next(-20, 21);
                    if ((targetX < 0f && NPC.velocity.X < 0f) || (targetX > 0f && NPC.velocity.X > 0f))
                    {
                        float distance = (float)Math.Sqrt(targetX * targetX + targetY * targetY);
                        float norm = velocity / distance;
                        targetX *= norm;
                        targetY *= norm;
                        int damage = (int)(30f * NPC.scale);

                        Vector2 baseVelocity = new Vector2(targetX, targetY);
                        Vector2 vel1 = baseVelocity.RotatedBy(MathHelper.ToRadians(15));
                        Vector2 vel2 = baseVelocity.RotatedBy(MathHelper.ToRadians(-15));

                        int proj1 = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel1, ProjectileID.Stinger, damage, 0f, Main.myPlayer);
                        Main.projectile[proj1].timeLeft = 300;
                        NPC.netUpdate = true;

                        int proj2 = Projectile.NewProjectile(NPC.GetSource_FromThis(), spawnPos, vel2, ProjectileID.Stinger, damage, 0f, Main.myPlayer);
                        Main.projectile[proj2].timeLeft = 300;
                        NPC.netUpdate = true;
                    }
                }
            }
        }
        public override void ModifyNPCLoot(NPCLoot npcLoot)
        {
            var dropRules = Main.ItemDropsDB.GetRulesForNPCID(NPCID.MossHornet, false); // false is important here
            foreach (var dropRule in dropRules)
            {
                // In this foreach loop, we simple add each drop to the AssassinHornet drop pool. 
                npcLoot.Add(dropRule);
            }
        }
        public override float SpawnChance(NPCSpawnInfo spawnInfo)
        {
            Player player = spawnInfo.Player;

            if (Main.hardMode &&
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
                new FlavorTextBestiaryInfoElement("Mods.DarklysEnemyExpansion.Bestiary.AssassinMossHornet")
            ]);
        }
        public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
        {
            // 如果是专家模式以上，则造成中毒
            if (Main.expertMode)
            {
                target.AddBuff(BuffID.Poisoned, 480);
            }
        }
    }
}
