using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainRunConfigPathAndParamsTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "train_params_id",
                table: "training_runs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "model_config_path",
                table: "trained_models",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "training_run_train_params",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    training_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    num_epochs = table.Column<int>(type: "integer", nullable: true),
                    batch_size = table.Column<int>(type: "integer", nullable: true),
                    num_frozen_stages = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_training_run_train_params", x => x.id);
                    table.ForeignKey(
                        name: "fk_training_runs_training_run_train_params_train_params_id1",
                        column: x => x.training_run_id,
                        principalTable: "training_runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_training_run_train_params_training_run_id",
                table: "training_run_train_params",
                column: "training_run_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "training_run_train_params");

            migrationBuilder.DropColumn(
                name: "train_params_id",
                table: "training_runs");

            migrationBuilder.DropColumn(
                name: "model_config_path",
                table: "trained_models");
        }
    }
}
