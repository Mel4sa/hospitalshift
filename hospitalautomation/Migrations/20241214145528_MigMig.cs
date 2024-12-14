using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hospitalautomation.Migrations
{
    /// <inheritdoc />
    public partial class MigMig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DepartmantId",
                table: "Shifts",
                newName: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_AssistantId",
                table: "Shifts",
                column: "AssistantId");

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_DepartmentId",
                table: "Shifts",
                column: "DepartmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Assistants_AssistantId",
                table: "Shifts",
                column: "AssistantId",
                principalTable: "Assistants",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Departments_DepartmentId",
                table: "Shifts",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Assistants_AssistantId",
                table: "Shifts");

            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Departments_DepartmentId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_AssistantId",
                table: "Shifts");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_DepartmentId",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Shifts",
                newName: "DepartmantId");
        }
    }
}
