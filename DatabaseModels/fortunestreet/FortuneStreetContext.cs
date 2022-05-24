using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet
{
    public class FortuneStreetAppContext : FortuneStreetContext
    {
        public FortuneStreetAppContext(DbContextOptions<FortuneStreetAppContext> options) : base(options)
        {
        }
    }

    public class FortuneStreetSaveAnalyzerInstanceLogContext : FortuneStreetContext
    {
        public FortuneStreetSaveAnalyzerInstanceLogContext(DbContextOptions<FortuneStreetSaveAnalyzerInstanceLogContext> options) : base(options)
        {
        }
    }

    public partial class FortuneStreetContext : DbContext
    {
        public FortuneStreetContext(DbContextOptions options) : base(options)
        {
            SqlConnection sqlConnection = (SqlConnection) Database.GetDbConnection();
            sqlConnection.AccessToken = new DefaultAzureCredential().GetToken(new TokenRequestContext(new[]
            {
                "https://database.windows.net/.default"
            })).Token;
        }

        public virtual DbSet<AnalyzerInstances> AnalyzerInstances { get; set; }
        public virtual DbSet<AnalyzerInstanceLogs> AnalyzerInstanceLogs { get; set; }
        public virtual DbSet<Boards> Boards { get; set; }
        public virtual DbSet<BoardCharacterCrosslist> BoardCharacterCrosslist { get; set; }
        public virtual DbSet<BoardCharacteristics> BoardCharacteristics { get; set; }
        public virtual DbSet<Characters> Characters { get; set; }
        public virtual DbSet<CharacterColorCrosslist> CharacterColorCrosslist { get; set; }
        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<Districts> Districts { get; set; }
        public virtual DbSet<GameSettings> GameSettings { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<Shops> Shops { get; set; }
        public virtual DbSet<Spaces> Spaces { get; set; }
        public virtual DbSet<SpaceConstraints> SpaceConstraints { get; set; }
        public virtual DbSet<SpaceLayouts> SpaceLayouts { get; set; }
        public virtual DbSet<SpaceTypes> SpaceTypes { get; set; }
        public virtual DbSet<TurnOrderDetermination> TurnOrderDetermination { get; set; }
        public virtual DbSet<TurnAfterRoll> TurnAfterRoll { get; set; }
        public virtual DbSet<TurnBeforeRoll> TurnBeforeRoll { get; set; }
        public virtual DbSet<CurrentAnalyzerInstancesTVF> CurrentAnalyzerInstancesTVF { get; set; }
        public virtual DbSet<CurrentAnalyzerInstanceLogsTVF> CurrentAnalyzerInstanceLogsTVF { get; set; }
        public virtual DbSet<GetAnalyzerInstancesInProgressTVF> GetAnalyzerInstancesInProgressTVF { get; set; }
        public virtual DbSet<GetBoardCharactersTVF> GetBoardCharactersTVF { get; set; }
        public virtual DbSet<GetCharacterColorsTVF> GetCharacterColorsTVF { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<AnalyzerInstances>(entity =>
            {
                entity.ToTable("analyzerinstances");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("ip_address");

                entity.Property(e => e.Name)
                    .HasMaxLength(900)
                    .HasColumnName("name")
                    .HasComment("The name of the analyzer instance.");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("type")
                    .HasComment("The type of the analyzer instance.");
            });

            modelBuilder.Entity<AnalyzerInstanceLogs>(entity =>
            {
                entity.ToTable("analyzerinstancelogs");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("key");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });

            modelBuilder.Entity<Boards>(entity =>
            {
                entity.ToTable("boards");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<BoardCharacterCrosslist>(entity =>
            {
                entity.ToTable("boardcharactercrosslist");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.BoardCharacterCrosslists)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardcharactercrosslist_boards");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.BoardCharacterCrosslists)
                    .HasForeignKey(d => d.CharacterID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardcharactercrosslist_characters");
            });

            modelBuilder.Entity<BoardCharacteristics>(entity =>
            {
                entity.ToTable("boardcharacteristics");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.MaxDieRoll).HasColumnName("max_die_roll");

                entity.Property(e => e.NetWorthThreshold).HasColumnName("net_worth_threshold");

                entity.Property(e => e.ReadyCashStart).HasColumnName("ready_cash_start");

                entity.Property(e => e.RuleID).HasColumnName("rule_id");

                entity.Property(e => e.SalaryIncrease).HasColumnName("salary_increase");

                entity.Property(e => e.SalaryStart).HasColumnName("salary_start");

                entity.Property(e => e.StandingThreshold).HasColumnName("standing_threshold");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.BoardCharacteristics)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardcharacteristics_boards");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.BoardCharacteristics)
                    .HasForeignKey(d => d.RuleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardcharacteristics_rules");
            });

            modelBuilder.Entity<Characters>(entity =>
            {
                entity.ToTable("characters");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.CharacterPortraitURI)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("character_portrait_uri");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Rank)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("rank");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<CharacterColorCrosslist>(entity =>
            {
                entity.ToTable("charactercolorcrosslist");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.ColorID).HasColumnName("color_id");

                entity.Property(e => e.Position).HasColumnName("position");

                entity.Property(e => e.Priority).HasColumnName("priority");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Character)
                    .WithMany(p => p.CharacterColorCrosslists)
                    .HasForeignKey(d => d.CharacterID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_charactercolorcrosslist_characters");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.CharacterColorCrosslists)
                    .HasForeignKey(d => d.ColorID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_charactercolorcrosslist_colors");
            });

            modelBuilder.Entity<Colors>(entity =>
            {
                entity.ToTable("colors");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.CharacterColor)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("character_color");

                entity.Property(e => e.SystemColor)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("system_color");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<Districts>(entity =>
            {
                entity.ToTable("districts");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Color)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("color");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(1)
                    .HasColumnName("name");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<GameSettings>(entity =>
            {
                entity.ToTable("gamesettings");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.MiiColorID).HasColumnName("mii_color_id");

                entity.Property(e => e.RuleID).HasColumnName("rule_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.GameSettings)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_gamesettings_boards");

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.GameSettings)
                    .HasForeignKey(d => d.MiiColorID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_gamesettings_colors");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.GameSettings)
                    .HasForeignKey(d => d.RuleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_gamesettings_rules");
            });

            modelBuilder.Entity<Rules>(entity =>
            {
                entity.ToTable("rules");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<Shops>(entity =>
            {
                entity.ToTable("shops");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Spaces>(entity =>
            {
                entity.ToTable("spaces");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AdditionalProperties)
                    .HasMaxLength(900)
                    .HasColumnName("additional_properties");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.DistrictID).HasColumnName("district_id");

                entity.Property(e => e.RuleID).HasColumnName("rule_id");

                entity.Property(e => e.ShopID).HasColumnName("shop_id");

                entity.Property(e => e.SpaceTypeID).HasColumnName("space_type_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_boards");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.DistrictID)
                    .HasConstraintName("FK_spaces_districts");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.RuleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_rules");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.ShopID)
                    .HasConstraintName("FK_spaces_shops");

                entity.HasOne(d => d.SpaceType)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.SpaceTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_spacetypes");
            });

            modelBuilder.Entity<SpaceConstraints>(entity =>
            {
                entity.ToTable("spaceconstraints");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.SpaceID).HasColumnName("space_id");

                entity.Property(e => e.SpaceIDFrom).HasColumnName("space_id_from");

                entity.Property(e => e.SpaceIDTo).HasColumnName("space_id_to");

                entity.Property(e => e.SpaceLayoutIndex).HasColumnName("space_layout_index");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.HasOne(d => d.Space)
                    .WithMany(p => p.SpaceConstraints)
                    .HasForeignKey(d => d.SpaceID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaceconstraints_spaces");

                entity.HasOne(d => d.SpaceFrom)
                    .WithMany(p => p.SpaceConstraintsFrom)
                    .HasForeignKey(d => d.SpaceIDFrom)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaceconstraints_spaces1");

                entity.HasOne(d => d.SpaceTo)
                    .WithMany(p => p.SpaceConstraintsTo)
                    .HasForeignKey(d => d.SpaceIDTo)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaceconstraints_spaces2");
            });

            modelBuilder.Entity<SpaceLayouts>(entity =>
            {
                entity.ToTable("spacelayouts");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.CenterXFactor)
                    .HasColumnType("numeric(10, 8)")
                    .HasColumnName("center_x_factor");

                entity.Property(e => e.CenterYFactor)
                    .HasColumnType("numeric(10, 8)")
                    .HasColumnName("center_y_factor");

                entity.Property(e => e.LayoutIndex).HasColumnName("layout_index");

                entity.Property(e => e.SpaceID).HasColumnName("space_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<SpaceTypes>(entity =>
            {
                entity.ToTable("spacetypes");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("description");

                entity.Property(e => e.Icon)
                    .HasMaxLength(50)
                    .HasColumnName("icon");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Title)
                    .HasMaxLength(50)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<TurnAfterRoll>(entity =>
            {
                entity.ToTable("turnafterroll");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.DieRollValue).HasColumnName("die_roll_value");

                entity.Property(e => e.SpaceID).HasColumnName("space_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
            });

            modelBuilder.Entity<TurnBeforeRoll>(entity =>
            {
                entity.ToTable("turnbeforeroll");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.CollectedSuits)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("collected_suits");

                entity.Property(e => e.DieRollRestrictions)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("die_roll_restrictions");

                entity.Property(e => e.Level).HasColumnName("level");

                entity.Property(e => e.NetWorth).HasColumnName("net_worth");

                entity.Property(e => e.OwnedShopIndices)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("owned_shop_indices");

                entity.Property(e => e.Placing).HasColumnName("placing");

                entity.Property(e => e.ReadyCash).HasColumnName("ready_cash");

                entity.Property(e => e.SpaceIDCurrent).HasColumnName("space_id_current");

                entity.Property(e => e.SpaceIDFrom).HasColumnName("space_id_from");

                entity.Property(e => e.SpaceLayoutIndex).HasColumnName("space_layout_index");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.TotalShopValue).HasColumnName("total_shop_value");

                entity.Property(e => e.TotalStockValue).HasColumnName("total_stock_value");

                entity.Property(e => e.TotalSuitCards).HasColumnName("total_suit_cards");

                entity.Property(e => e.TurnNumber).HasColumnName("turn_number");
            });

            modelBuilder.Entity<TurnOrderDetermination>(entity =>
            {
                entity.ToTable("turnorderdetermination");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<CurrentAnalyzerInstancesTVF>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("ip_address");

                entity.Property(e => e.Name)
                    .HasMaxLength(900)
                    .HasColumnName("name");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<CurrentAnalyzerInstancesTVF>().HasNoKey();

            modelBuilder.Entity<CurrentAnalyzerInstanceLogsTVF>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.Key)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("key");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasColumnName("value");
            });

            modelBuilder.Entity<CurrentAnalyzerInstancesTVF>().HasNoKey();

            modelBuilder.Entity<GetAnalyzerInstancesInProgressTVF>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("ip_address");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("type");
            });

            modelBuilder.Entity<GetAnalyzerInstancesInProgressTVF>().HasNoKey();

            modelBuilder.Entity<GetBoardCharactersTVF>(entity =>
            {
                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.CharacterPortraitURI)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("character_portrait_uri");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<GetBoardCharactersTVF>().HasNoKey();

            modelBuilder.Entity<GetCharacterColorsTVF>(entity =>
            {
                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.ColorIDAssigned).HasColumnName("color_id_assigned");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<GetCharacterColorsTVF>().HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
