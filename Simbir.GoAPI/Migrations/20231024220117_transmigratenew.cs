using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Simbir.GoAPI.Migrations
{
    /// <inheritdoc />
    public partial class transmigratenew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transports_AspNetUsers_OwnerId1",
                table: "Transports");

            migrationBuilder.DropIndex(
                name: "IX_Transports_OwnerId1",
                table: "Transports");

            migrationBuilder.DropColumn(
                name: "OwnerId1",
                table: "Transports");

            migrationBuilder.AlterColumn<long>(
                name: "OwnerId",
                table: "Transports",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Transports",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { 1L, null, "Administrator", "ADMINISTRATOR" },
                    { 2L, null, "Member", "MEMBER" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transports_OwnerId",
                table: "Transports",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_AspNetUsers_OwnerId",
                table: "Transports",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transports_AspNetUsers_OwnerId",
                table: "Transports");

            migrationBuilder.DropIndex(
                name: "IX_Transports_OwnerId",
                table: "Transports");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 1L);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: 2L);

            migrationBuilder.AlterColumn<int>(
                name: "OwnerId",
                table: "Transports",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Transports",
                type: "integer",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn)
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "OwnerId1",
                table: "Transports",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Transports_OwnerId1",
                table: "Transports",
                column: "OwnerId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Transports_AspNetUsers_OwnerId1",
                table: "Transports",
                column: "OwnerId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
