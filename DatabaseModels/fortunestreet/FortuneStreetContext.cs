using Microsoft.Azure.Services.AppAuthentication;
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

    public partial class FortuneStreetContext : DbContext
    {
        public FortuneStreetContext(DbContextOptions options) : base(options)
        {
            SqlConnection connection = (SqlConnection) Database.GetDbConnection();
            connection.AccessToken = new AzureServiceTokenProvider().GetAccessTokenAsync("https://database.windows.net/").Result;
        }

        public virtual DbSet<AnalyzerInstances> AnalyzerInstances { get; set; }
        public virtual DbSet<AnalyzerInstanceLogs> AnalyzerInstanceLogs { get; set; }
        public virtual DbSet<Boards> Boards { get; set; }
        public virtual DbSet<BoardCharacterCrosslist> BoardCharacterCrosslists { get; set; }
        public virtual DbSet<BoardCharacteristics> BoardCharacteristics { get; set; }
        public virtual DbSet<Characters> Characters { get; set; }
        public virtual DbSet<CharacterColorCrosslist> CharacterColorCrosslists { get; set; }
        public virtual DbSet<Colors> Colors { get; set; }
        public virtual DbSet<Rules> Rules { get; set; }
        public virtual DbSet<Shops> Shops { get; set; }
        public virtual DbSet<Spaces> Spaces { get; set; }
        public virtual DbSet<SpaceTypes> SpaceTypes { get; set; }
        public virtual DbSet<CurrentAnalyzerInstancesTVF> CurrentAnalyzerInstancesTVF { get; set; }

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

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added")
                    .HasDefaultValueSql("(getutcdate())");
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
                    .HasColumnName("timestamp_added");
            });

            modelBuilder.Entity<BoardCharacterCrosslist>(entity =>
            {
                entity.ToTable("boardcharactercrosslist");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.CharacterID).HasColumnName("character_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

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

                entity.Property(e => e.NetWorthThreshold).HasColumnName("net_worth_threshold");

                entity.Property(e => e.RuleID).HasColumnName("rule_id");

                entity.Property(e => e.StandingThreshold).HasColumnName("standing_threshold");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.BoardGoals)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardgoals_boards");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.BoardGoals)
                    .HasForeignKey(d => d.RuleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_boardgoals_rules");
            });

            modelBuilder.Entity<Characters>(entity =>
            {
                entity.ToTable("characters");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.CharacterSpriteURL)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("character_sprite_url");

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
                    .HasColumnName("timestamp_added");
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
                    .HasColumnName("timestamp_added");

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

                entity.Property(e => e.GameColor)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("game_color");

                entity.Property(e => e.MiiColor)
                    .IsRequired()
                    .HasMaxLength(6)
                    .HasColumnName("mii_color");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");
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
                    .HasColumnName("timestamp_added");
            });

            modelBuilder.Entity<Shops>(entity =>
            {
                entity.ToTable("shops");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.District)
                    .HasMaxLength(1)
                    .HasColumnName("district");

                entity.Property(e => e.MaxCapital).HasColumnName("max_capital");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Price).HasColumnName("price");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.Property(e => e.Value).HasColumnName("value");
            });

            modelBuilder.Entity<Spaces>(entity =>
            {
                entity.ToTable("spaces");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.BoardID).HasColumnName("board_id");

                entity.Property(e => e.CenterX)
                    .HasColumnType("numeric(9, 8)")
                    .HasColumnName("center_x");

                entity.Property(e => e.CenterY)
                    .HasColumnType("numeric(9, 8)")
                    .HasColumnName("center_y");

                entity.Property(e => e.RuleID).HasColumnName("rule_id");

                entity.Property(e => e.SpaceTypeID).HasColumnName("space_type_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.HasOne(d => d.Board)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.BoardID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_boards");

                entity.HasOne(d => d.Rule)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.RuleID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_rules");

                entity.HasOne(d => d.SpaceType)
                    .WithMany(p => p.Spaces)
                    .HasForeignKey(d => d.SpaceTypeID)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_spaces_spacetypes");
            });

            modelBuilder.Entity<SpaceTypes>(entity =>
            {
                entity.ToTable("spacetypes");

                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.Icon)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("icon");

                entity.Property(e => e.ShopID).HasColumnName("shop_id");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");

                entity.Property(e => e.Value)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("value");

                entity.HasOne(d => d.Shop)
                    .WithMany(p => p.SpaceTypes)
                    .HasForeignKey(d => d.ShopID)
                    .HasConstraintName("FK_spacetypes_shops");
            });

            modelBuilder.Entity<CurrentAnalyzerInstancesTVF>(entity =>
            {
                entity.Property(e => e.ID).HasColumnName("id");

                entity.Property(e => e.AnalyzerInstanceID).HasColumnName("analyzer_instance_id");

                entity.Property(e => e.IPAddress)
                    .IsRequired()
                    .HasMaxLength(900)
                    .HasColumnName("ip_address");

                entity.Property(e => e.Status)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("status");

                entity.Property(e => e.TimestampAdded)
                    .HasColumnType("datetime")
                    .HasColumnName("timestamp_added");
            });

            modelBuilder.Entity<CurrentAnalyzerInstancesTVF>().HasNoKey();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
