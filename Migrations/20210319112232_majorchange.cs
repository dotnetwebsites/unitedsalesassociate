using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace UnitedSales.Migrations
{
    public partial class majorchange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ChannelHeadId",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CountryCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CountryName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Enquiries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    EmpCode = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LeadAllocTo = table.Column<string>(nullable: true),
                    FirstName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    LastName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 10, nullable: false),
                    SpouseName = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    CityId = table.Column<int>(nullable: true),
                    StateId = table.Column<int>(nullable: true),
                    AlternateNo = table.Column<string>(type: "nvarchar(20)", maxLength: 10, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Enquiries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LeadsAllocations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    LeadId = table.Column<string>(nullable: false),
                    CallerId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LeadsAllocations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    LocationName = table.Column<string>(type: "nvarchar(500)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MailLibraries",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MailUserId = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    EmailAddress = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    EmailName = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(200)", nullable: true),
                    Host = table.Column<string>(type: "nvarchar(50)", nullable: true),
                    Port = table.Column<int>(nullable: false),
                    EnableSsl = table.Column<bool>(nullable: false),
                    UseDefaultCredentials = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MailLibraries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectAreas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    SqYard = table.Column<int>(nullable: false),
                    Sqft = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectAreas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectExtents",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    YardSft = table.Column<string>(nullable: true),
                    TotValue = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectExtents", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProjectName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ChannelHead = table.Column<string>(nullable: true),
                    ChannelHeadPerSqYard = table.Column<int>(nullable: false),
                    ChannelHeadPerSqft = table.Column<int>(nullable: false),
                    Ambassador = table.Column<string>(nullable: true),
                    AmbassadorPerSqYard = table.Column<int>(nullable: false),
                    AmbassadorPerSqft = table.Column<int>(nullable: false),
                    CHBA = table.Column<string>(nullable: true),
                    CHBAPerSqYard = table.Column<int>(nullable: false),
                    CHBAPerSqft = table.Column<int>(nullable: false),
                    Telecaller = table.Column<string>(nullable: true),
                    Fixed = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SendSMS",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    ProjectId = table.Column<int>(nullable: false),
                    ViewMsg = table.Column<string>(type: "nvarchar(1000)", nullable: true),
                    Message = table.Column<string>(type: "nvarchar(1000)", nullable: false),
                    IsActive = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SendSMS", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TeleCallings",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    EnquiryId = table.Column<int>(nullable: false),
                    LocationId = table.Column<int>(nullable: false),
                    ProjectId = table.Column<int>(nullable: false),
                    ExtentId = table.Column<int>(nullable: false),
                    Status = table.Column<string>(nullable: true),
                    FollowUpDate = table.Column<DateTime>(nullable: true),
                    CallerId = table.Column<string>(nullable: true),
                    Remark = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeleCallings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "States",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    CountryId = table.Column<int>(nullable: false),
                    StateCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    StateName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_States", x => x.Id);
                    table.ForeignKey(
                        name: "FK_States_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    CreatedDate = table.Column<DateTime>(nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(nullable: true),
                    StateId = table.Column<int>(nullable: false),
                    CityCode = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(100)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cities_States_StateId",
                        column: x => x.StateId,
                        principalTable: "States",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cities_StateId",
                table: "Cities",
                column: "StateId");

            migrationBuilder.CreateIndex(
                name: "IX_Enquiries_PhoneNumber",
                table: "Enquiries",
                column: "PhoneNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_States_CountryId",
                table: "States",
                column: "CountryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");

            migrationBuilder.DropTable(
                name: "Enquiries");

            migrationBuilder.DropTable(
                name: "LeadsAllocations");

            migrationBuilder.DropTable(
                name: "Locations");

            migrationBuilder.DropTable(
                name: "MailLibraries");

            migrationBuilder.DropTable(
                name: "ProjectAreas");

            migrationBuilder.DropTable(
                name: "ProjectExtents");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SendSMS");

            migrationBuilder.DropTable(
                name: "TeleCallings");

            migrationBuilder.DropTable(
                name: "States");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropColumn(
                name: "ChannelHeadId",
                table: "AspNetUsers");
        }
    }
}
