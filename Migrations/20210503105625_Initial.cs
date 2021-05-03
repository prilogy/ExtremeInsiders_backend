using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace ExtremeInsiders.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cultures",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cultures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Currencies",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Currencies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EntitiesBase",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Playlist_SportId = table.Column<int>(nullable: true),
                    SportId = table.Column<int>(nullable: true),
                    PlaylistId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitiesBase", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitiesBase_EntitiesBase_SportId",
                        column: x => x.SportId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitiesBase_EntitiesBase_Playlist_SportId",
                        column: x => x.Playlist_SportId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitiesBase_EntitiesBase_PlaylistId",
                        column: x => x.PlaylistId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SocialAccountsProviders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccountsProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionsPlans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Duration = table.Column<TimeSpan>(nullable: false),
                    Color = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionsPlans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannerEntities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    EntityId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerEntities_EntitiesBase_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EntitySaleablePrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EntitySaleablePrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EntitySaleablePrices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_EntitySaleablePrices_EntitiesBase_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MoviesTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageId = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MoviesTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MoviesTranslations_EntitiesBase_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MoviesTranslations_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PlaylistsTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlaylistsTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PlaylistsTranslations_EntitiesBase_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistsTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PlaylistsTranslations_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SportsTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SportsTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SportsTranslations_EntitiesBase_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportsTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SportsTranslations_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "VideosTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    ImageId = table.Column<int>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VideosTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VideosTranslations_EntitiesBase_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VideosTranslations_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Email = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Password = table.Column<string>(nullable: true),
                    AvatarId = table.Column<int>(nullable: true),
                    DateBirthday = table.Column<DateTime>(nullable: false),
                    DateSignUp = table.Column<DateTime>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    RoleId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Images_AvatarId",
                        column: x => x.AvatarId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    SubscriptionPlanId = table.Column<int>(nullable: true),
                    EntitySaleableId = table.Column<int>(nullable: true),
                    IsInfinite = table.Column<bool>(nullable: false),
                    IsValid = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodes_EntitiesBase_EntitySaleableId",
                        column: x => x.EntitySaleableId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PromoCodes_SubscriptionsPlans_SubscriptionPlanId",
                        column: x => x.SubscriptionPlanId,
                        principalTable: "SubscriptionsPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionsPlansPrices",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Value = table.Column<decimal>(nullable: false),
                    DiscountValue = table.Column<decimal>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionsPlansPrices", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionsPlansPrices_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionsPlansPrices_SubscriptionsPlans_EntityId",
                        column: x => x.EntityId,
                        principalTable: "SubscriptionsPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubscriptionsPlansTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubscriptionsPlansTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubscriptionsPlansTranslations_SubscriptionsPlans_BaseEntit~",
                        column: x => x.BaseEntityId,
                        principalTable: "SubscriptionsPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubscriptionsPlansTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BannerEntitiesTranslations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BaseEntityId = table.Column<int>(nullable: false),
                    CultureId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    ImageId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannerEntitiesTranslations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannerEntitiesTranslations_BannerEntities_BaseEntityId",
                        column: x => x.BaseEntityId,
                        principalTable: "BannerEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerEntitiesTranslations_Cultures_CultureId",
                        column: x => x.CultureId,
                        principalTable: "Cultures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BannerEntitiesTranslations_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ConfirmationCodes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    IsConfirmed = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    DateValidUntil = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConfirmationCodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ConfirmationCodes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Favorites",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Favorites_EntitiesBase_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Favorites_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Likes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Likes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Likes_EntitiesBase_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Likes_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Payments",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(nullable: true),
                    Value = table.Column<decimal>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    DateCreated = table.Column<DateTime>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Payments_Currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "Currencies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Payments_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sales",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    EntityId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sales_EntitiesBase_EntityId",
                        column: x => x.EntityId,
                        principalTable: "EntitiesBase",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Sales_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SocialAccounts",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Key = table.Column<string>(nullable: true),
                    ProviderId = table.Column<int>(nullable: false),
                    UserId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SocialAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_SocialAccountsProviders_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "SocialAccountsProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SocialAccounts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PromoCodesUsers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    PromoCodeId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PromoCodesUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PromoCodesUsers_PromoCodes_PromoCodeId",
                        column: x => x.PromoCodeId,
                        principalTable: "PromoCodes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PromoCodesUsers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subscriptions",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    DateStart = table.Column<DateTime>(nullable: false),
                    DateEnd = table.Column<DateTime>(nullable: false),
                    PlanId = table.Column<int>(nullable: true),
                    PaymentId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Payments_PaymentId",
                        column: x => x.PaymentId,
                        principalTable: "Payments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_SubscriptionsPlans_PlanId",
                        column: x => x.PlanId,
                        principalTable: "SubscriptionsPlans",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Subscriptions_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cultures",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 1, "ru" },
                    { 2, "en" }
                });

            migrationBuilder.InsertData(
                table: "Currencies",
                columns: new[] { "Id", "Key" },
                values: new object[,]
                {
                    { 1, "RUB" },
                    { 2, "EUR" },
                    { 3, "USD" }
                });

            migrationBuilder.InsertData(
                table: "EntitiesBase",
                columns: new[] { "Id", "DateCreated", "Discriminator" },
                values: new object[] { 1, new DateTime(2021, 5, 3, 10, 56, 24, 847, DateTimeKind.Utc).AddTicks(9428), "Sport" });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "user" },
                    { 2, "admin" }
                });

            migrationBuilder.InsertData(
                table: "SocialAccountsProviders",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { 1, "google" },
                    { 2, "vk" },
                    { 3, "facebook" }
                });

            migrationBuilder.InsertData(
                table: "EntitiesBase",
                columns: new[] { "Id", "DateCreated", "Discriminator", "SportId" },
                values: new object[] { 4, new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(758), "Movie", 1 });

            migrationBuilder.InsertData(
                table: "EntitiesBase",
                columns: new[] { "Id", "DateCreated", "Discriminator", "Playlist_SportId" },
                values: new object[] { 2, new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(27), "Playlist", 1 });

            migrationBuilder.InsertData(
                table: "EntitiesBase",
                columns: new[] { "Id", "DateCreated", "Discriminator", "PlaylistId" },
                values: new object[] { 3, new DateTime(2021, 5, 3, 10, 56, 24, 848, DateTimeKind.Utc).AddTicks(517), "Video", 2 });

            migrationBuilder.CreateIndex(
                name: "IX_BannerEntities_EntityId",
                table: "BannerEntities",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerEntitiesTranslations_BaseEntityId",
                table: "BannerEntitiesTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerEntitiesTranslations_ImageId",
                table: "BannerEntitiesTranslations",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_BannerEntitiesTranslations_CultureId_BaseEntityId",
                table: "BannerEntitiesTranslations",
                columns: new[] { "CultureId", "BaseEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ConfirmationCodes_UserId",
                table: "ConfirmationCodes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitiesBase_SportId",
                table: "EntitiesBase",
                column: "SportId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitiesBase_Playlist_SportId",
                table: "EntitiesBase",
                column: "Playlist_SportId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitiesBase_PlaylistId",
                table: "EntitiesBase",
                column: "PlaylistId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySaleablePrices_EntityId",
                table: "EntitySaleablePrices",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_EntitySaleablePrices_CurrencyId_EntityId",
                table: "EntitySaleablePrices",
                columns: new[] { "CurrencyId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_EntityId",
                table: "Favorites",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Favorites_UserId",
                table: "Favorites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_EntityId",
                table: "Likes",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_EntityId",
                table: "Likes",
                columns: new[] { "UserId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MoviesTranslations_BaseEntityId",
                table: "MoviesTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesTranslations_ImageId",
                table: "MoviesTranslations",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_MoviesTranslations_CultureId_BaseEntityId",
                table: "MoviesTranslations",
                columns: new[] { "CultureId", "BaseEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Payments_CurrencyId",
                table: "Payments",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                table: "Payments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistsTranslations_BaseEntityId",
                table: "PlaylistsTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistsTranslations_ImageId",
                table: "PlaylistsTranslations",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_PlaylistsTranslations_CultureId_BaseEntityId",
                table: "PlaylistsTranslations",
                columns: new[] { "CultureId", "BaseEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_EntitySaleableId",
                table: "PromoCodes",
                column: "EntitySaleableId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodes_SubscriptionPlanId",
                table: "PromoCodes",
                column: "SubscriptionPlanId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodesUsers_UserId",
                table: "PromoCodesUsers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_PromoCodesUsers_PromoCodeId_UserId",
                table: "PromoCodesUsers",
                columns: new[] { "PromoCodeId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sales_EntityId",
                table: "Sales",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Sales_UserId_EntityId",
                table: "Sales",
                columns: new[] { "UserId", "EntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_ProviderId",
                table: "SocialAccounts",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialAccounts_UserId",
                table: "SocialAccounts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsTranslations_BaseEntityId",
                table: "SportsTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsTranslations_ImageId",
                table: "SportsTranslations",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_SportsTranslations_CultureId_BaseEntityId",
                table: "SportsTranslations",
                columns: new[] { "CultureId", "BaseEntityId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PaymentId",
                table: "Subscriptions",
                column: "PaymentId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_PlanId",
                table: "Subscriptions",
                column: "PlanId");

            migrationBuilder.CreateIndex(
                name: "IX_Subscriptions_UserId",
                table: "Subscriptions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionsPlansPrices_CurrencyId",
                table: "SubscriptionsPlansPrices",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionsPlansPrices_EntityId",
                table: "SubscriptionsPlansPrices",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionsPlansTranslations_BaseEntityId",
                table: "SubscriptionsPlansTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_SubscriptionsPlansTranslations_CultureId",
                table: "SubscriptionsPlansTranslations",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AvatarId",
                table: "Users",
                column: "AvatarId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CultureId",
                table: "Users",
                column: "CultureId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CurrencyId",
                table: "Users",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                table: "Users",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosTranslations_BaseEntityId",
                table: "VideosTranslations",
                column: "BaseEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosTranslations_ImageId",
                table: "VideosTranslations",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_VideosTranslations_CultureId_BaseEntityId",
                table: "VideosTranslations",
                columns: new[] { "CultureId", "BaseEntityId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BannerEntitiesTranslations");

            migrationBuilder.DropTable(
                name: "ConfirmationCodes");

            migrationBuilder.DropTable(
                name: "EntitySaleablePrices");

            migrationBuilder.DropTable(
                name: "Favorites");

            migrationBuilder.DropTable(
                name: "Likes");

            migrationBuilder.DropTable(
                name: "MoviesTranslations");

            migrationBuilder.DropTable(
                name: "PlaylistsTranslations");

            migrationBuilder.DropTable(
                name: "PromoCodesUsers");

            migrationBuilder.DropTable(
                name: "Sales");

            migrationBuilder.DropTable(
                name: "SocialAccounts");

            migrationBuilder.DropTable(
                name: "SportsTranslations");

            migrationBuilder.DropTable(
                name: "Subscriptions");

            migrationBuilder.DropTable(
                name: "SubscriptionsPlansPrices");

            migrationBuilder.DropTable(
                name: "SubscriptionsPlansTranslations");

            migrationBuilder.DropTable(
                name: "VideosTranslations");

            migrationBuilder.DropTable(
                name: "BannerEntities");

            migrationBuilder.DropTable(
                name: "PromoCodes");

            migrationBuilder.DropTable(
                name: "SocialAccountsProviders");

            migrationBuilder.DropTable(
                name: "Payments");

            migrationBuilder.DropTable(
                name: "EntitiesBase");

            migrationBuilder.DropTable(
                name: "SubscriptionsPlans");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.DropTable(
                name: "Cultures");

            migrationBuilder.DropTable(
                name: "Currencies");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
