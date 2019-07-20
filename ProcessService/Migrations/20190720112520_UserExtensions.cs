using Microsoft.EntityFrameworkCore.Migrations;

namespace ProcessService.Migrations
{
    public partial class UserExtensions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profile",
                keyColumn: "Id",
                keyValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "LockedOut",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "ProfilePicsture",
                table: "Profile",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "RestaurantFeatured",
                table: "Profile",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Profile",
                columns: new[] { "Id", "LockedOut", "Name", "ProfilePicsture", "RestaurantFeatured" },
                values: new object[] { "9133657b-3a59-45ef-9ad9-82a2d99fb839", false, "FoodLocator", null, false });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Profile",
                keyColumn: "Id",
                keyValue: "9133657b-3a59-45ef-9ad9-82a2d99fb839");

            migrationBuilder.DropColumn(
                name: "LockedOut",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "ProfilePicsture",
                table: "Profile");

            migrationBuilder.DropColumn(
                name: "RestaurantFeatured",
                table: "Profile");

            migrationBuilder.InsertData(
                table: "Profile",
                columns: new[] { "Id", "Name" },
                values: new object[] { "", "FoodLocator" });
        }
    }
}
