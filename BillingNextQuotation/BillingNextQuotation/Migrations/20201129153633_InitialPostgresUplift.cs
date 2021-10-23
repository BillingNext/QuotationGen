using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace BillingNextQuotation.Migrations
{
    public partial class InitialPostgresUplift : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    UserName = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(maxLength: 256, nullable: true),
                    Email = table.Column<string>(maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    PasswordHash = table.Column<string>(nullable: true),
                    SecurityStamp = table.Column<string>(nullable: true),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PhysicalAddress = table.Column<string>(maxLength: 200, nullable: false),
                    CustomerCatagory = table.Column<int>(nullable: false),
                    ProfilePicture = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    CompanyId = table.Column<string>(nullable: false),
                    CompanyName = table.Column<string>(maxLength: 100, nullable: false),
                    CompanyCreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "NOW()"),
                    CompanyLogoImg = table.Column<byte[]>(nullable: true),
                    GSTIN = table.Column<string>(nullable: false),
                    BankName = table.Column<string>(maxLength: 100, nullable: false),
                    BankAccountType = table.Column<int>(nullable: false),
                    AccountNumber = table.Column<string>(nullable: false),
                    IFSCcode = table.Column<string>(nullable: false),
                    PAN = table.Column<string>(nullable: false),
                    CompanyOwner = table.Column<string>(nullable: true),
                    CompanyEmail = table.Column<string>(nullable: false),
                    CompanyPhoneNumber = table.Column<string>(maxLength: 10, nullable: false),
                    CompanyAddress = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Company", x => x.CompanyId);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RoleId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<string>(nullable: false),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    LoginProvider = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    ProductId = table.Column<string>(nullable: false),
                    ProductName = table.Column<string>(nullable: false),
                    ProductFullSheetPrice = table.Column<float>(nullable: false),
                    ProductPieceCutPrice = table.Column<float>(nullable: false),
                    ProductDimensionX = table.Column<float>(nullable: false),
                    ProductDimensionY = table.Column<float>(nullable: false),
                    ProductCreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "NOW()"),
                    ProductModificationDate = table.Column<DateTime>(nullable: false),
                    CreatedByUserId = table.Column<string>(nullable: false),
                    ModifiedByUserId = table.Column<string>(nullable: true),
                    CompanyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.ProductId);
                    table.ForeignKey(
                        name: "FK_Products_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Products_AspNetUsers_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationNote",
                columns: table => new
                {
                    QuotationNoteId = table.Column<string>(nullable: false),
                    NoteName = table.Column<string>(nullable: false),
                    Note = table.Column<string>(nullable: false),
                    IsNoteDefault = table.Column<bool>(nullable: false),
                    NoteCreationDate = table.Column<DateTime>(nullable: false),
                    CompanyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationNote", x => x.QuotationNoteId);
                    table.ForeignKey(
                        name: "FK_QuotationNote_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SpecialCharges",
                columns: table => new
                {
                    SpecialChargesId = table.Column<string>(nullable: false),
                    SpecialChargeName = table.Column<string>(nullable: false),
                    IsDefault = table.Column<bool>(nullable: false),
                    SpecialChargeType = table.Column<int>(nullable: false),
                    SpecialChargeFixedAmount = table.Column<float>(nullable: false),
                    SpecialChargePercentage = table.Column<float>(nullable: true),
                    CompanyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpecialCharges", x => x.SpecialChargesId);
                    table.ForeignKey(
                        name: "FK_SpecialCharges_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Quotation",
                columns: table => new
                {
                    QuotationId = table.Column<string>(nullable: false),
                    QuotationNumber = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    QuotationAmount = table.Column<float>(nullable: false),
                    QuotationGrandTotalAmount = table.Column<float>(nullable: false),
                    QuotationTo = table.Column<string>(nullable: false),
                    QuotationStatus = table.Column<int>(nullable: false),
                    QuotationCreationDate = table.Column<DateTime>(nullable: false, defaultValueSql: "NOW()"),
                    ActualQuotationCreationDate = table.Column<DateTime>(nullable: false),
                    QuotationModificationDate = table.Column<DateTime>(nullable: false),
                    CreatedByUserId = table.Column<string>(nullable: false),
                    ModifiedByUserId = table.Column<string>(nullable: true),
                    SecretCode = table.Column<string>(maxLength: 6, nullable: false),
                    QuotationNoteId = table.Column<string>(nullable: false),
                    CompanyId = table.Column<string>(nullable: false),
                    QuotationForId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotation", x => x.QuotationId);
                    table.ForeignKey(
                        name: "FK_Quotation_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "CompanyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotation_AspNetUsers_QuotationForId",
                        column: x => x.QuotationForId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Quotation_QuotationNote_QuotationNoteId",
                        column: x => x.QuotationNoteId,
                        principalTable: "QuotationNote",
                        principalColumn: "QuotationNoteId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationDetails",
                columns: table => new
                {
                    QuotationDetailsId = table.Column<string>(nullable: false),
                    SequenceNumber = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: false),
                    SheetMeasurementOptions = table.Column<int>(nullable: false),
                    SheetSizingOptions = table.Column<int>(nullable: false),
                    ProductDimensionX = table.Column<float>(nullable: false),
                    ProductDimensionY = table.Column<float>(nullable: false),
                    ProductQuantity = table.Column<int>(nullable: false),
                    ProductRate = table.Column<float>(nullable: false),
                    TotalArea = table.Column<float>(nullable: false),
                    ProductAmount = table.Column<float>(nullable: false),
                    QuotationId = table.Column<string>(nullable: false),
                    ProductId = table.Column<string>(nullable: false),
                    CompanyId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationDetails", x => x.QuotationDetailsId);
                    table.ForeignKey(
                        name: "FK_QuotationDetails_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "ProductId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationDetails_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "QuotationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationSpecialCharges",
                columns: table => new
                {
                    SpecialChargesId = table.Column<string>(nullable: false),
                    QuotationId = table.Column<string>(nullable: false),
                    CompanyId = table.Column<string>(nullable: false),
                    SpecialChargeAmount = table.Column<float>(nullable: false),
                    DefaultCalculationOverride = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationSpecialCharges", x => new { x.SpecialChargesId, x.QuotationId });
                    table.ForeignKey(
                        name: "FK_QuotationSpecialCharges_Quotation_QuotationId",
                        column: x => x.QuotationId,
                        principalTable: "Quotation",
                        principalColumn: "QuotationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationSpecialCharges_SpecialCharges_SpecialChargesId",
                        column: x => x.SpecialChargesId,
                        principalTable: "SpecialCharges",
                        principalColumn: "SpecialChargesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_CompanyId",
                table: "Products",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_CreatedByUserId",
                table: "Products",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_CompanyId",
                table: "Quotation",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_QuotationForId",
                table: "Quotation",
                column: "QuotationForId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_QuotationNoteId",
                table: "Quotation",
                column: "QuotationNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_Quotation_QuotationNumber",
                table: "Quotation",
                column: "QuotationNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_ProductId",
                table: "QuotationDetails",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDetails_QuotationId",
                table: "QuotationDetails",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationNote_CompanyId",
                table: "QuotationNote",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationSpecialCharges_QuotationId",
                table: "QuotationSpecialCharges",
                column: "QuotationId");

            migrationBuilder.CreateIndex(
                name: "IX_SpecialCharges_CompanyId",
                table: "SpecialCharges",
                column: "CompanyId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens");

            migrationBuilder.DropTable(
                name: "QuotationDetails");

            migrationBuilder.DropTable(
                name: "QuotationSpecialCharges");

            migrationBuilder.DropTable(
                name: "AspNetRoles");

            migrationBuilder.DropTable(
                name: "Products");

            migrationBuilder.DropTable(
                name: "Quotation");

            migrationBuilder.DropTable(
                name: "SpecialCharges");

            migrationBuilder.DropTable(
                name: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "QuotationNote");

            migrationBuilder.DropTable(
                name: "Company");
        }
    }
}
