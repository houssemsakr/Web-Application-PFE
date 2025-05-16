using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Web_Application_PFE.Migrations
{
    /// <inheritdoc />
    public partial class addelement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatorId",
                table: "VersionEntities",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VersionEntities_CreatorId",
                table: "VersionEntities",
                column: "CreatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_VersionEntities_AspNetUsers_CreatorId",
                table: "VersionEntities",
                column: "CreatorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VersionEntities_AspNetUsers_CreatorId",
                table: "VersionEntities");

            migrationBuilder.DropIndex(
                name: "IX_VersionEntities_CreatorId",
                table: "VersionEntities");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "VersionEntities");
        }
    }
}
