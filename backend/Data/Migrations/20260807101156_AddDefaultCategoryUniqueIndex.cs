using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monefy.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDefaultCategoryUniqueIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_categories_user_id_name_type",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "ix_categories_name_type_default",
                table: "categories",
                columns: new[] { "name", "type" },
                unique: true,
                filter: "user_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_categories_user_id_name_type",
                table: "categories",
                columns: new[] { "user_id", "name", "type" },
                unique: true,
                filter: "user_id IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_categories_name_type_default",
                table: "categories");

            migrationBuilder.DropIndex(
                name: "ix_categories_user_id_name_type",
                table: "categories");

            migrationBuilder.CreateIndex(
                name: "ix_categories_user_id_name_type",
                table: "categories",
                columns: new[] { "user_id", "name", "type" },
                unique: true);
        }
    }
}
