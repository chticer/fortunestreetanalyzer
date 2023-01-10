using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace fortunestreetanalyzer.DatabaseModels.fortunestreet;

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
        sqlConnection.AccessToken = new VisualStudioCredential().GetToken(new TokenRequestContext(new[]
        {
            "https://database.windows.net/.default"
        }), new CancellationToken()).Token;
    }

    public virtual DbSet<AnalyzerInstances> AnalyzerInstances { get; set; }
    public virtual DbSet<AuctionBids> AuctionBids { get; set; }
    public virtual DbSet<BoardCharacterCrosslist> BoardCharacterCrosslist { get; set; }
    public virtual DbSet<BoardCharacteristics> BoardCharacteristics { get; set; }
    public virtual DbSet<Boards> Boards { get; set; }
    public virtual DbSet<CharacterColorCrosslist> CharacterColorCrosslist { get; set; }
    public virtual DbSet<Characters> Characters { get; set; }
    public virtual DbSet<Colors> Colors { get; set; }
    public virtual DbSet<Districts> Districts { get; set; }
    public virtual DbSet<GameSettings> GameSettings { get; set; }
    public virtual DbSet<PostRolls> PostRolls { get; set; }
    public virtual DbSet<PreRolls> PreRolls { get; set; }
    public virtual DbSet<Rules> Rules { get; set; }
    public virtual DbSet<ShopOffers> ShopOffers { get; set; }
    public virtual DbSet<ShopOfferNegotiations> ShopOfferNegotiations { get; set; }
    public virtual DbSet<Shops> Shops { get; set; }
    public virtual DbSet<SpaceConstraints> SpaceConstraints { get; set; }
    public virtual DbSet<SpaceLayouts> SpaceLayouts { get; set; }
    public virtual DbSet<Spaces> Spaces { get; set; }
    public virtual DbSet<SpaceTypes> SpaceTypes { get; set; }
    public virtual DbSet<TurnOrderDetermination> TurnOrderDetermination { get; set; }
    public virtual DbSet<CurrentAnalyzerInstancesTVF> CurrentAnalyzerInstancesTVF { get; set; }
    public virtual DbSet<GetBoardCharactersTVF> GetBoardCharactersTVF { get; set; }
    public virtual DbSet<GetCharacterColorsTVF> GetCharacterColorsTVF { get; set; }
    public virtual DbSet<GetPostRollsTVF> GetPostRollsTVF { get; set; }
    public virtual DbSet<GetPreRollsTVF> GetPreRollsTVF { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AI");

        modelBuilder.Entity<AnalyzerInstances>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__analyzer__3213E83FC7616AB1");

            entity.ToTable("analyzerinstances", tb => tb.HasComment("Data of analyzer instances within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.IPAddress)
                .IsRequired()
                .HasMaxLength(900)
                .HasComment("Hash value of the user's IP address.")
                .HasColumnName("ip_address");
            entity.Property(e => e.Name)
                .HasMaxLength(900)
                .HasComment("The name of the analyzer instance.")
                .HasColumnName("name");
            entity.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("Indication of the analyzer instance's state.")
                .HasColumnName("status");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The type of the analyzer instance.")
                .HasColumnName("type");
        });

        modelBuilder.Entity<AuctionBids>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__auctionb__3213E83FA5AC6CA5");

            entity.ToTable("auctionbids", tb => tb.HasComment("Data of auction bids within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Bid)
                .HasComment("The bid of the auction.")
                .HasColumnName("bid");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.ShopOfferNegotiationID)
                .HasComment("Shop offer negotiation identifier reference.")
                .HasColumnName("shop_offer_negotiation_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.ShopOfferNegotiation).WithMany(p => p.AuctionBids)
                .HasForeignKey(d => d.ShopOfferNegotiationID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_auctionbids_shopoffers");
        });

        modelBuilder.Entity<BoardCharacterCrosslist>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__boardcha__3213E83F16592ABB");

            entity.ToTable("boardcharactercrosslist", tb => tb.HasComment("Crosslist for boards and characters tables."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.BoardID)
                .HasComment("Board identifier reference.")
                .HasColumnName("board_id");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Board).WithMany(p => p.BoardCharacterCrosslists)
                .HasForeignKey(d => d.BoardID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_boardcharactercrosslist_boards");
        });

        modelBuilder.Entity<BoardCharacteristics>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__board_go__3213E83FBA945572");

            entity.ToTable("boardcharacteristics", tb => tb.HasComment("Data of the characteristics of boards in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.BoardID)
                .HasComment("Board identifier reference.")
                .HasColumnName("board_id");
            entity.Property(e => e.MaxDieRoll)
                .HasComment("Maximum value that a character can roll the die relative to the rule and board that the user selected.")
                .HasColumnName("max_die_roll");
            entity.Property(e => e.NetWorthThreshold)
                .HasComment("Minimum net worth value required to complete the board relative to the rule and board that the user selected.")
                .HasColumnName("net_worth_threshold");
            entity.Property(e => e.ReadyCashStart)
                .HasComment("Starting ready cash value relative to the rule and board that the user selected.")
                .HasColumnName("ready_cash_start");
            entity.Property(e => e.RuleID)
                .HasComment("Rule identifier reference.")
                .HasColumnName("rule_id");
            entity.Property(e => e.SalaryIncrease)
                .HasComment("The value that the salary increases by for every level up relative to the rule and board that the user selected.")
                .HasColumnName("salary_increase");
            entity.Property(e => e.SalaryStart)
                .HasComment("Starting salary value relative to the rule and board that the user selected.")
                .HasColumnName("salary_start");
            entity.Property(e => e.StandingThreshold)
                .HasComment("Minimum standing value required to complete the board relative to the rule and board that the user selected.")
                .HasColumnName("standing_threshold");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Board).WithMany(p => p.BoardCharacteristics)
                .HasForeignKey(d => d.BoardID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_boardcharacteristics_boards");

            entity.HasOne(d => d.Rule).WithMany(p => p.BoardCharacteristics)
                .HasForeignKey(d => d.RuleID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_boardcharacteristics_rules");
        });

        modelBuilder.Entity<Boards>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__boards__3213E83F4BE111AB");

            entity.ToTable("boards", tb => tb.HasComment("Data of the boards in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The name of the board.")
                .HasColumnName("name");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
        });

        modelBuilder.Entity<CharacterColorCrosslist>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__characte__3213E83F7EF7EEF3");

            entity.ToTable("charactercolorcrosslist", tb => tb.HasComment("Crosslist for characters and colors tables."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.ColorID)
                .HasComment("Color identifier reference.")
                .HasColumnName("color_id");
            entity.Property(e => e.Position)
                .HasComment("The order value relative to the character and color.")
                .HasColumnName("position");
            entity.Property(e => e.Priority)
                .HasComment("The order value, in comparison with other characters, relative to the character and color.")
                .HasColumnName("priority");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Color).WithMany(p => p.CharacterColorCrosslists)
                .HasForeignKey(d => d.ColorID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_charactercolorcrosslist_colors");
        });

        modelBuilder.Entity<Characters>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__characte__3213E83F697FE52F");

            entity.ToTable("characters", tb => tb.HasComment("Data of the characters in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.CharacterPortraitURI)
                .IsRequired()
                .HasMaxLength(900)
                .HasComment("The URI value of the character's portrait image.")
                .HasColumnName("character_portrait_uri");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The name of the character.")
                .HasColumnName("name");
            entity.Property(e => e.Rank)
                .HasMaxLength(1)
                .HasComment("The rank of the character.")
                .HasColumnName("rank");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
        });

        modelBuilder.Entity<Colors>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__colors__3213E83FBA5536D2");

            entity.ToTable("colors", tb => tb.HasComment("Data of the colors in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.CharacterColor)
                .IsRequired()
                .HasMaxLength(6)
                .HasComment("Hex value given to a character in Fortune Street.")
                .HasColumnName("character_color");
            entity.Property(e => e.SystemColor)
                .IsRequired()
                .HasMaxLength(6)
                .HasComment("Hex value given by the Nintendo Wii system.")
                .HasColumnName("system_color");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
        });

        modelBuilder.Entity<Districts>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__district__3213E83FC5F3828A");

            entity.ToTable("districts", tb => tb.HasComment("Data of the districts in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Color)
                .IsRequired()
                .HasMaxLength(6)
                .HasComment("Hex value given to a district in Fortune Street.")
                .HasColumnName("color");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(1)
                .HasComment("The name of the district.")
                .HasColumnName("name");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
        });

        modelBuilder.Entity<GameSettings>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__gamesett__3213E83F0EB9D254");

            entity.ToTable("gamesettings", tb => tb.HasComment("Data of game settings within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.BoardID)
                .HasComment("Board identifier reference.")
                .HasColumnName("board_id");
            entity.Property(e => e.MiiColorID)
                .HasComment("Color identifier reference for the Mii character.")
                .HasColumnName("mii_color_id");
            entity.Property(e => e.RuleID)
                .HasComment("Rule identifier reference.")
                .HasColumnName("rule_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.AnalyzerInstance).WithMany(p => p.GameSettings)
                .HasForeignKey(d => d.AnalyzerInstanceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_gamesettings_analyzerinstances");

            entity.HasOne(d => d.Board).WithMany(p => p.GameSettings)
                .HasForeignKey(d => d.BoardID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_gamesettings_boards");

            entity.HasOne(d => d.MiiColor).WithMany(p => p.GameSettings)
                .HasForeignKey(d => d.MiiColorID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_gamesettings_colors");

            entity.HasOne(d => d.Rule).WithMany(p => p.GameSettings)
                .HasForeignKey(d => d.RuleID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_gamesettings_rules");
        });

        modelBuilder.Entity<PostRolls>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__postroll__3213E83FA3FC2225");

            entity.ToTable("postrolls", tb => tb.HasComment("Data of post rolls within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.DieRollValue)
                .HasComment("The value of the die roll.")
                .HasColumnName("die_roll_value");
            entity.Property(e => e.Logs)
                .IsRequired()
                .HasComment("JSON array of turn outcomes and events.")
                .HasColumnName("logs");
            entity.Property(e => e.SpaceIDLandedOn)
                .HasComment("Space identifier reference of the space landed on.")
                .HasColumnName("space_id_landed_on");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.TurnNumber)
                .HasComment("The turn number value of the player.")
                .HasColumnName("turn_number");
            entity.Property(e => e.TurnResetFlag)
                .HasComment("Whether the turn has been resetted.")
                .HasColumnName("turn_reset_flag");

            entity.HasOne(d => d.AnalyzerInstance).WithMany(p => p.PostRolls)
                .HasForeignKey(d => d.AnalyzerInstanceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_postrolls_analyzerinstances");

            entity.HasOne(d => d.SpaceIdLandedOnNavigation).WithMany(p => p.PostRolls)
                .HasForeignKey(d => d.SpaceIDLandedOn)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_postrolls_spaces");
        });

        modelBuilder.Entity<PreRolls>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__turnchar__3213E83F2786056A");

            entity.ToTable("prerolls", tb => tb.HasComment("Data of pre rolls within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.ArcadeIndex)
                .HasComment("The index value of the arcade mini-game.")
                .HasColumnName("arcade_index");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.CollectedSuits)
                .IsRequired()
                .HasMaxLength(900)
                .HasComment("List of suits the player has collected.")
                .HasColumnName("collected_suits");
            entity.Property(e => e.DieRollRestrictions)
                .HasMaxLength(50)
                .HasComment("List of die roll values that the player is restricted to.")
                .HasColumnName("die_roll_restrictions");
            entity.Property(e => e.LayoutIndex)
                .HasComment("The index value of the board layout.")
                .HasColumnName("layout_index");
            entity.Property(e => e.Level)
                .HasComment("The level of the player.")
                .HasColumnName("level");
            entity.Property(e => e.NetWorth)
                .HasComment("The net worth of the player.")
                .HasColumnName("net_worth");
            entity.Property(e => e.OwnedShopIndices)
                .IsRequired()
                .HasMaxLength(900)
                .HasComment("List of indices of shops the player owns.")
                .HasColumnName("owned_shop_indices");
            entity.Property(e => e.Placing)
                .HasComment("The placing of the player.")
                .HasColumnName("placing");
            entity.Property(e => e.ReadyCash)
                .HasComment("The ready cash value of the player.")
                .HasColumnName("ready_cash");
            entity.Property(e => e.SpaceIDCurrent)
                .HasComment("Space identifier reference of the current space.")
                .HasColumnName("space_id_current");
            entity.Property(e => e.SpaceIDFrom)
                .HasComment("Space identifier reference of the space where the player came from.")
                .HasColumnName("space_id_from");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.TotalShopValue)
                .HasComment("The total value of all shops owned by the player.")
                .HasColumnName("total_shop_value");
            entity.Property(e => e.TotalStockValue)
                .HasComment("The total value of all stocks owned by the player.")
                .HasColumnName("total_stock_value");
            entity.Property(e => e.TotalSuitCards)
                .HasComment("The total number of suit cards the player has.")
                .HasColumnName("total_suit_cards");
            entity.Property(e => e.TurnNumber)
                .HasComment("The turn number value of the player.")
                .HasColumnName("turn_number");
            entity.Property(e => e.TurnResetFlag)
                .HasComment("Whether the turn has been resetted.")
                .HasColumnName("turn_reset_flag");

            entity.HasOne(d => d.AnalyzerInstance).WithMany(p => p.PreRolls)
                .HasForeignKey(d => d.AnalyzerInstanceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prerolls_analyzerinstances");

            entity.HasOne(d => d._SpaceIDCurrent).WithMany(p => p.PreRollsSpaceIDCurrent)
                .HasForeignKey(d => d.SpaceIDCurrent)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_prerolls_spaces");

            entity.HasOne(d => d._SpaceIDFrom).WithMany(p => p.PreRollsSpaceIDFrom)
                .HasForeignKey(d => d.SpaceIDFrom)
                .HasConstraintName("FK_prerolls_spaces1");
        });

        modelBuilder.Entity<Rules>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__rules__3213E83F02AB1ABB");

            entity.ToTable("rules", tb => tb.HasComment("Data of the rules in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The name of the rule.")
                .HasColumnName("name");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
        });

        modelBuilder.Entity<ShopOffers>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__shopoffe__3213E83F100E3DCF");

            entity.ToTable("shopoffers", tb => tb.HasComment("Data of shop offers within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.CharacterIDFrom)
                .HasComment("Character identifier reference of the player initiating the shop offer.")
                .HasColumnName("character_id_from");
            entity.Property(e => e.CharacterIDTo)
                .HasComment("Character identifier reference of the player receiving the shop offer.")
                .HasColumnName("character_id_to");
            entity.Property(e => e.ShopIndicesFrom)
                .HasMaxLength(50)
                .HasComment("List of shop indices that the player is giving to another player.")
                .HasColumnName("shop_indices_from");
            entity.Property(e => e.ShopIndicesTo)
                .HasMaxLength(50)
                .HasComment("List of shop indices that the player is receiving from another player.")
                .HasColumnName("shop_indices_to");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The type of the shop offer.")
                .HasColumnName("type");

            entity.HasOne(d => d.AnalyzerInstance).WithMany(p => p.ShopOffers)
                .HasForeignKey(d => d.AnalyzerInstanceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_shopoffers_analyzerinstances");
        });

        modelBuilder.Entity<ShopOfferNegotiations>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__shopoffe__3213E83FA0ED59E7");

            entity.ToTable("shopoffernegotiations", tb => tb.HasComment("Data of shop offer negotations within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasComment("The amount of the shop offer negotation.")
                .HasColumnName("amount");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.Forced)
                .HasComment("Whether the shop offer negotiation was forced.")
                .HasColumnName("forced");
            entity.Property(e => e.ShopOfferID)
                .HasComment("Shop offer identifier reference.")
                .HasColumnName("shop_offer_id");
            entity.Property(e => e.Success)
                .HasComment("Whether the shop offer negotiation succeeded.")
                .HasColumnName("success");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.ShopOffer).WithMany(p => p.ShopOfferNegotiations)
                .HasForeignKey(d => d.ShopOfferID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_shopoffernegotiations_shopoffers");
        });

        modelBuilder.Entity<Shops>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK_shopstemp");

            entity.ToTable("shops", tb => tb.HasComment("Data of the shops in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The name of the shop.")
                .HasColumnName("name");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Value)
                .HasComment("The value of the shop.")
                .HasColumnName("value");
        });

        modelBuilder.Entity<SpaceConstraints>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__spacecon__3213E83FC93861A5");

            entity.ToTable("spaceconstraints", tb => tb.HasComment("Data of the constraint of spaces in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.LayoutIndex)
                .HasComment("The index value of the board layout.")
                .HasColumnName("layout_index");
            entity.Property(e => e.SpaceID)
                .HasComment("Space identifier reference.")
                .HasColumnName("space_id");
            entity.Property(e => e.SpaceIDFrom)
                .HasComment("Space identifier reference from the source.")
                .HasColumnName("space_id_from");
            entity.Property(e => e.SpaceIDTo)
                .HasComment("Space identifier reference to the destination.")
                .HasColumnName("space_id_to");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Space).WithMany(p => p.SpaceConstraints)
                .HasForeignKey(d => d.SpaceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaceconstraints_spaces");

            entity.HasOne(d => d._SpaceIDFrom).WithMany(p => p.SpaceConstraintsSpaceIDFrom)
                .HasForeignKey(d => d.SpaceIDFrom)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaceconstraints_spaces1");

            entity.HasOne(d => d._SpaceIDTo).WithMany(p => p.SpaceConstraintsSpaceIDTo)
                .HasForeignKey(d => d.SpaceIDTo)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaceconstraints_spaces2");
        });

        modelBuilder.Entity<SpaceLayouts>(entity =>
        {
            entity.ToTable("spacelayouts", tb => tb.HasComment("Data of the layout of spaces in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.CenterXFactor)
                .HasComment("The scale value of the horizontal center position of the space.")
                .HasColumnType("numeric(10, 8)")
                .HasColumnName("center_x_factor");
            entity.Property(e => e.CenterYFactor)
                .HasComment("The scale value of the vertical center position of the space.")
                .HasColumnType("numeric(10, 8)")
                .HasColumnName("center_y_factor");
            entity.Property(e => e.LayoutIndex)
                .HasComment("The index value of the board layout.")
                .HasColumnName("layout_index");
            entity.Property(e => e.SpaceID)
                .HasComment("Space identifier reference.")
                .HasColumnName("space_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Space).WithMany(p => p.SpaceLayouts)
                .HasForeignKey(d => d.SpaceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spacelayouts_spaces");
        });

        modelBuilder.Entity<Spaces>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__spaces__3213E83F1EE42E18");

            entity.ToTable("spaces", tb => tb.HasComment("Data of the spaces in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AdditionalProperties)
                .HasMaxLength(900)
                .HasComment("JSON value of any information pertaining to the space.")
                .HasColumnName("additional_properties");
            entity.Property(e => e.BoardID)
                .HasComment("Board identifier reference.")
                .HasColumnName("board_id");
            entity.Property(e => e.DistrictID)
                .HasComment("District identifier reference.")
                .HasColumnName("district_id");
            entity.Property(e => e.RuleID)
                .HasComment("Rule identifier reference.")
                .HasColumnName("rule_id");
            entity.Property(e => e.ShopID)
                .HasComment("Shop identifier reference.")
                .HasColumnName("shop_id");
            entity.Property(e => e.SpaceTypeID)
                .HasComment("Space type identifier reference.")
                .HasColumnName("space_type_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");

            entity.HasOne(d => d.Board).WithMany(p => p.Spaces)
                .HasForeignKey(d => d.BoardID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaces_boards");

            entity.HasOne(d => d.District).WithMany(p => p.Spaces)
                .HasForeignKey(d => d.DistrictID)
                .HasConstraintName("FK_spaces_districts");

            entity.HasOne(d => d.Rule).WithMany(p => p.Spaces)
                .HasForeignKey(d => d.RuleID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaces_rules");

            entity.HasOne(d => d.Shop).WithMany(p => p.Spaces)
                .HasForeignKey(d => d.ShopID)
                .HasConstraintName("FK_spaces_shops");

            entity.HasOne(d => d.SpaceType).WithMany(p => p.Spaces)
                .HasForeignKey(d => d.SpaceTypeID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_spaces_spacetypes");
        });

        modelBuilder.Entity<SpaceTypes>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__space_ty__3213E83F27F32CF4");

            entity.ToTable("spacetypes", tb => tb.HasComment("Data of the type of spaces in Fortune Street."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .IsRequired()
                .HasMaxLength(900)
                .HasComment("The description of the type of space.")
                .HasColumnName("description");
            entity.Property(e => e.Icon)
                .HasMaxLength(50)
                .HasComment("The Font Awesome icon value of the type of space.")
                .HasColumnName("icon");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasComment("The unique name of the type of space.")
                .HasColumnName("name");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Title)
                .HasMaxLength(50)
                .HasComment("The title of the type of space.")
                .HasColumnName("title");
        });

        modelBuilder.Entity<TurnOrderDetermination>(entity =>
        {
            entity.HasKey(e => e.ID).HasName("PK__characte__3213E83F297308EA");

            entity.ToTable("turnorderdetermination", tb => tb.HasComment("Data of determining the turn order of characters within the program."));

            entity.Property(e => e.ID)
                .HasComment("Unique identifier.")
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasComment("Analyzer instance identifier reference.")
                .HasColumnName("analyzer_instance_id");
            entity.Property(e => e.CharacterID)
                .HasComment("Character identifier reference.")
                .HasColumnName("character_id");
            entity.Property(e => e.TimestampAdded)
                .HasDefaultValueSql("(getutcdate())")
                .HasComment("Default UTC timestamp when record is added.")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Value)
                .HasComment("The value of the number determined relative to the analyzer instance and character.")
                .HasColumnName("value");

            entity.HasOne(d => d.AnalyzerInstance).WithMany(p => p.TurnOrderDeterminations)
                .HasForeignKey(d => d.AnalyzerInstanceID)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_turnorderdetermination_analyzerinstances");
        });

        modelBuilder.Entity<CurrentAnalyzerInstancesTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ID)
                .HasColumnName("id");
            entity.Property(e => e.AnalyzerInstanceID)
                .HasColumnName("analyzer_instance_id");
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
                .HasDefaultValueSql("(getutcdate())")
                .HasColumnType("datetime")
                .HasColumnName("timestamp_added");
            entity.Property(e => e.Type)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("type");
        });

        modelBuilder.Entity<GetAuctionWinnersTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CharacterID)
                .HasColumnName("character_id");
        });

        modelBuilder.Entity<GetBoardCharactersTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CharacterID)
                .HasColumnName("character_id");
            entity.Property(e => e.CharacterPortraitURI)
                .IsRequired()
                .HasMaxLength(900)
                .HasColumnName("character_portrait_uri");
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<GetCharacterColorsTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CharacterID)
                .HasColumnName("character_id");
            entity.Property(e => e.ColorIDAssigned)
                .HasColumnName("color_id_assigned");
            entity.Property(e => e.Value)
                .HasColumnName("value");
        });

        modelBuilder.Entity<GetPostRollsTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.CharacterID)
                .HasColumnName("character_id");
            entity.Property(e => e.DieRollValue)
                .HasColumnName("die_roll_value");
            entity.Property(e => e.Logs)
                .HasColumnName("logs");
            entity.Property(e => e.SpaceIDLandedOn)
                .HasColumnName("space_id_landed_on");
            entity.Property(e => e.TurnNumber)
                .HasColumnName("turn_number");
        });

        modelBuilder.Entity<GetPreRollsTVF>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.ArcadeIndex)
                .HasColumnName("arcade_index");
            entity.Property(e => e.CharacterID)
                .HasColumnName("character_id");
            entity.Property(e => e.CollectedSuits)
                .IsRequired()
                .HasMaxLength(900)
                .HasColumnName("collected_suits");
            entity.Property(e => e.DieRollRestrictions)
                .HasMaxLength(50)
                .HasColumnName("die_roll_restrictions");
            entity.Property(e => e.LayoutIndex)
                .HasColumnName("layout_index");
            entity.Property(e => e.Level)
                .HasColumnName("level");
            entity.Property(e => e.NetWorth)
                .HasColumnName("net_worth");
            entity.Property(e => e.OwnedShopIndices)
                .IsRequired()
                .HasMaxLength(900)
                .HasColumnName("owned_shop_indices");
            entity.Property(e => e.Placing)
                .HasColumnName("placing");
            entity.Property(e => e.ReadyCash)
                .HasColumnName("ready_cash");
            entity.Property(e => e.SpaceIDCurrent)
                .HasColumnName("space_id_current");
            entity.Property(e => e.SpaceIDFrom)
                .HasColumnName("space_id_from");
            entity.Property(e => e.TotalShopValue)
                .HasColumnName("total_shop_value");
            entity.Property(e => e.TotalStockValue)
                .HasColumnName("total_stock_value");
            entity.Property(e => e.TotalSuitCards)
                .HasColumnName("total_suit_cards");
            entity.Property(e => e.TurnNumber)
                .HasColumnName("turn_number");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
