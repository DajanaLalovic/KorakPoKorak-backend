using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace KorakPoKorak.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBadgesAndCertificates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BadgeTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                    IconUrl = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    Category = table.Column<int>(type: "integer", nullable: false),
                    Scope = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CertificateTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Code = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    Title = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    Description = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BadgeAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    BadgeTemplateId = table.Column<int>(type: "integer", nullable: false),
                    ChildProfileId = table.Column<int>(type: "integer", nullable: false),
                    WorkshopId = table.Column<int>(type: "integer", nullable: true),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: true),
                    Scope = table.Column<int>(type: "integer", nullable: false),
                    AwardedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BadgeAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_BadgeTemplates_BadgeTemplateId",
                        column: x => x.BadgeTemplateId,
                        principalTable: "BadgeTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_ChildProfiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "ChildProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_BadgeAwards_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "CertificateAwards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    CertificateTemplateId = table.Column<int>(type: "integer", nullable: false),
                    ChildProfileId = table.Column<int>(type: "integer", nullable: false),
                    WorkshopId = table.Column<int>(type: "integer", nullable: true),
                    EnrollmentId = table.Column<int>(type: "integer", nullable: false),
                    ChildFullName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    WorkshopTitle = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    MentorName = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    CertificateTitle = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    CertificateNumber = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    IssuedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CertificateAwards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CertificateAwards_CertificateTemplates_CertificateTemplateId",
                        column: x => x.CertificateTemplateId,
                        principalTable: "CertificateTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CertificateAwards_ChildProfiles_ChildProfileId",
                        column: x => x.ChildProfileId,
                        principalTable: "ChildProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateAwards_Enrollments_EnrollmentId",
                        column: x => x.EnrollmentId,
                        principalTable: "Enrollments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CertificateAwards_Workshops_WorkshopId",
                        column: x => x.WorkshopId,
                        principalTable: "Workshops",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.InsertData(
                table: "BadgeTemplates",
                columns: new[] { "Id", "Category", "Code", "Description", "IconUrl", "Name", "Scope" },
                values: new object[,]
                {
                    { 1, 0, "WORKSHOP_MASTER", "Dodeli se za svaku uspešno završenu radionicu.", "/badges/workshop_master.svg", "Majstor radionice", 1 },
                    { 2, 1, "FIRST_WORKSHOP", "Dodeli se za prvu uspešno završenu radionicu.", "/badges/first_workshop.svg", "Prvi korak", 0 },
                    { 3, 1, "PERSISTENT", "Dodeli se za drugu uspešno završenu radionicu.", "/badges/persistent.svg", "Upornost", 0 },
                    { 4, 1, "THREE_WORKSHOPS", "Dodeli se za treću uspešno završenu radionicu.", "/badges/three_workshops.svg", "Iskusni istraživač", 0 },
                    { 5, 1, "CURIOUS_MIND", "Dodeli se za četvrtu uspešno završenu radionicu.", "/badges/curious_mind.svg", "Radoznali um", 0 },
                    { 6, 1, "FIVE_WORKSHOPS", "Dodeli se za petu uspešno završenu radionicu.", "/badges/five_workshops.svg", "Veliki istraživač", 0 },
                    { 7, 1, "GREAT_PROGRESS", "Dodeli se za šestu uspešno završenu radionicu.", "/badges/great_progress.svg", "Veliki napredak", 0 },
                    { 8, 1, "DEDICATED", "Dodeli se za sedmu uspešno završenu radionicu.", "/badges/dedicated.svg", "Posvećeni učenik", 0 },
                    { 9, 1, "EXPLORER", "Dodeli se za osmu uspešno završenu radionicu.", "/badges/explorer.svg", "Istraživač", 0 },
                    { 10, 1, "TEN_WORKSHOPS", "Dodeli se za desetu uspešno završenu radionicu.", "/badges/ten_workshops.svg", "Avanturista", 0 },
                    { 11, 1, "SUPER_LEARNER", "Dodeli se za petnaestu uspešno završenu radionicu.", "/badges/super_learner.svg", "Super učenik", 0 },
                    { 12, 1, "CHAMPION", "Dodeli se za dvadesetu uspešno završenu radionicu.", "/badges/champion.svg", "Šampion", 0 },
                    { 13, 2, "KNOWLEDGE_STAR", "Dodeli se za prvu završenu radionicu srednje težine.", "/badges/knowledge_star.svg", "Zvezda znanja", 0 },
                    { 14, 2, "BRAVE_STEP", "Dodeli se za prvu završenu naprednu radionicu.", "/badges/brave_step.svg", "Hrabri korak", 0 },
                    { 15, 0, "CREATIVE_STAR", "Dodeli se za prvu završenu radionicu sa više vrsta aktivnosti.", "/badges/creative_star.svg", "Kreativna zvezda", 0 }
                });

            migrationBuilder.InsertData(
                table: "CertificateTemplates",
                columns: new[] { "Id", "Code", "Description", "Title" },
                values: new object[] { 1, "DEFAULT_WORKSHOP", "Potvrda o uspešno završenoj radionici.", "Diploma" });

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_BadgeTemplateId",
                table: "BadgeAwards",
                column: "BadgeTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_ChildProfileId",
                table: "BadgeAwards",
                column: "ChildProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_EnrollmentId",
                table: "BadgeAwards",
                column: "EnrollmentId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_OncePerChild",
                table: "BadgeAwards",
                columns: new[] { "ChildProfileId", "BadgeTemplateId" },
                unique: true,
                filter: "\"Scope\" = 0");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_OncePerChildWorkshop",
                table: "BadgeAwards",
                columns: new[] { "ChildProfileId", "BadgeTemplateId", "WorkshopId" },
                unique: true,
                filter: "\"Scope\" = 1 AND \"WorkshopId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeAwards_WorkshopId",
                table: "BadgeAwards",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_BadgeTemplates_Code",
                table: "BadgeTemplates",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAwards_CertificateNumber",
                table: "CertificateAwards",
                column: "CertificateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAwards_CertificateTemplateId",
                table: "CertificateAwards",
                column: "CertificateTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAwards_ChildProfileId",
                table: "CertificateAwards",
                column: "ChildProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAwards_EnrollmentId",
                table: "CertificateAwards",
                column: "EnrollmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CertificateAwards_WorkshopId",
                table: "CertificateAwards",
                column: "WorkshopId");

            migrationBuilder.CreateIndex(
                name: "IX_CertificateTemplates_Code",
                table: "CertificateTemplates",
                column: "Code",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BadgeAwards");

            migrationBuilder.DropTable(
                name: "CertificateAwards");

            migrationBuilder.DropTable(
                name: "BadgeTemplates");

            migrationBuilder.DropTable(
                name: "CertificateTemplates");
        }
    }
}
