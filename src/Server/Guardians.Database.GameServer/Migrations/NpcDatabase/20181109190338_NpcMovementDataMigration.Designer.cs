﻿// <auto-generated />
using Guardians;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Guardians.Database.GameServer.Migrations.NpcDatabase
{
    [DbContext(typeof(NpcDatabaseContext))]
    [Migration("20181109190338_NpcMovementDataMigration")]
    partial class NpcMovementDataMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("Guardians.NPCEntryModel", b =>
                {
                    b.Property<int>("EntryId")
                        .ValueGeneratedOnAdd();

                    b.Property<float>("InitialOrientation");

                    b.Property<int>("MapId");

                    b.Property<int>("MovementData");

                    b.Property<byte>("MovementType");

                    b.Property<int>("NpcTemplateId");

                    b.HasKey("EntryId");

                    b.HasIndex("NpcTemplateId");

                    b.ToTable("npc_entry");
                });

            modelBuilder.Entity("Guardians.NPCTemplateModel", b =>
                {
                    b.Property<int>("TemplateId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("NpcName")
                        .IsRequired();

                    b.Property<int>("PrefabId");

                    b.HasKey("TemplateId");

                    b.ToTable("npc_template");
                });

            modelBuilder.Entity("Guardians.NPCEntryModel", b =>
                {
                    b.HasOne("Guardians.NPCTemplateModel", "NpcTemplate")
                        .WithMany()
                        .HasForeignKey("NpcTemplateId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.OwnsOne("Guardians.Database.Vector3<float>", "SpawnPosition", b1 =>
                        {
                            b1.Property<int>("NPCEntryModelEntryId");

                            b1.Property<float>("X");

                            b1.Property<float>("Y");

                            b1.Property<float>("Z");

                            b1.ToTable("npc_entry");

                            b1.HasOne("Guardians.NPCEntryModel")
                                .WithOne("SpawnPosition")
                                .HasForeignKey("Guardians.Database.Vector3<float>", "NPCEntryModelEntryId")
                                .OnDelete(DeleteBehavior.Cascade);
                        });
                });
#pragma warning restore 612, 618
        }
    }
}
