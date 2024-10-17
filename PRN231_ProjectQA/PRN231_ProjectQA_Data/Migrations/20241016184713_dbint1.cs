using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PRN231_ProjectQA_Data.Migrations
{
    /// <inheritdoc />
    public partial class dbint1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "Comments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Img",
                table: "AnswerComments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Img",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "Img",
                table: "AnswerComments");
        }
    }
}
