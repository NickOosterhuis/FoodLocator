using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcessService.Migrations
{
    public partial class UserStuff : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Profile",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Profile",
                columns: new[] { "Id", "Name" },
                values: new object[] { "", "FoodLocator" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profile",
                keyColumn: "Id",
                keyValue: "");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Profile");
        }
    }
}
