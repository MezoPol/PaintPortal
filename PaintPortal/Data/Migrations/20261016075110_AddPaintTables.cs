using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaintPortal.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddPaintTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Paints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HexColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Range = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Paints", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PaintEquivalents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaintId = table.Column<int>(type: "int", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    HexColor = table.Column<string>(type: "nvarchar(7)", maxLength: 7, nullable: false),
                    MatchLevel = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PaintEquivalents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PaintEquivalents_Paints_PaintId",
                        column: x => x.PaintId,
                        principalTable: "Paints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPaints",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PaintId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPaints", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPaints_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPaints_Paints_PaintId",
                        column: x => x.PaintId,
                        principalTable: "Paints",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PaintEquivalents_PaintId",
                table: "PaintEquivalents",
                column: "PaintId");

            migrationBuilder.CreateIndex(
                name: "IX_Paints_Brand_Name",
                table: "Paints",
                columns: new[] { "Brand", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserPaints_PaintId",
                table: "UserPaints",
                column: "PaintId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPaints_UserId_PaintId",
                table: "UserPaints",
                columns: new[] { "UserId", "PaintId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PaintEquivalents");

            migrationBuilder.DropTable(
                name: "UserPaints");

            migrationBuilder.DropTable(
                name: "Paints");
        }
    }
}
