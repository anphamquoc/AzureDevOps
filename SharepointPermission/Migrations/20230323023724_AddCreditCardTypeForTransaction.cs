using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SharepointPermission.Migrations
{
    /// <inheritdoc />
    public partial class AddCreditCardTypeForTransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreditCardType",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreditCardType",
                table: "Transactions");
        }
    }
}
