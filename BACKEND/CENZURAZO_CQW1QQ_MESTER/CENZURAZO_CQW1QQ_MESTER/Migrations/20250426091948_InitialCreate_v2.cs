using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CENZURAZO_CQW1QQ_MESTER.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate_v2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReplacementDatas",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Word = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReplacementDatas", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "AlternativeWords",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Alternative = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReplacementDataID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlternativeWords", x => x.ID);
                    table.ForeignKey(
                        name: "FK_AlternativeWords_ReplacementDatas_ReplacementDataID",
                        column: x => x.ReplacementDataID,
                        principalTable: "ReplacementDatas",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlternativeWords_ReplacementDataID",
                table: "AlternativeWords",
                column: "ReplacementDataID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlternativeWords");

            migrationBuilder.DropTable(
                name: "ReplacementDatas");
        }
    }
}
