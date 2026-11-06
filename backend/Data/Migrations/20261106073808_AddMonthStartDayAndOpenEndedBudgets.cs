using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Monefy.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthStartDayAndOpenEndedBudgets : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "month_start_day",
                table: "users",
                type: "integer",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                table: "budgets",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "month_start_day",
                table: "users");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "end_date",
                table: "budgets",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
