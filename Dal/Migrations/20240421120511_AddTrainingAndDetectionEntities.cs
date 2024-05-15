using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddTrainingAndDetectionEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "detection_ignore_zones",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    geom = table.Column<Polygon>(type: "geometry(Polygon)", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detection_ignore_zones", x => x.id);
                    table.ForeignKey(
                        name: "fk_detection_ignore_zones_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detection_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    image_file_name = table.Column<string>(type: "text", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detection_runs", x => x.id);
                    table.ForeignKey(
                        name: "fk_detection_runs_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "detected_dump_sites",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    confidence_rate = table.Column<double>(type: "double precision", nullable: false),
                    dataset_class_id = table.Column<Guid>(type: "uuid", nullable: false),
                    detection_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    geom = table.Column<Polygon>(type: "geometry(Polygon)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_detected_dump_sites", x => x.id);
                    table.ForeignKey(
                        name: "fk_detected_dump_sites_dataset_classes_dataset_class_id",
                        column: x => x.dataset_class_id,
                        principalTable: "dataset_classes",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_detected_dump_sites_detection_runs_detection_run_id",
                        column: x => x.detection_run_id,
                        principalTable: "detection_runs",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "trained_model_statistics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    trained_model_id = table.Column<Guid>(type: "uuid", nullable: false),
                    training_duration = table.Column<TimeSpan>(type: "interval", nullable: true),
                    total_parameters = table.Column<int>(type: "integer", nullable: true),
                    num_epochs = table.Column<double>(type: "double precision", nullable: true),
                    learning_rate = table.Column<double>(type: "double precision", nullable: true),
                    avg_box_loss = table.Column<double>(type: "double precision", nullable: true),
                    m_app = table.Column<double>(type: "double precision", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trained_model_statistics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "trained_models",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    model_file_path = table.Column<string>(type: "text", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    training_run_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_model_id = table.Column<Guid>(type: "uuid", nullable: true),
                    trained_model_statistics_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_trained_models", x => x.id);
                    table.ForeignKey(
                        name: "fk_trained_models_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trained_models_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_trained_models_trained_models_base_model_id",
                        column: x => x.base_model_id,
                        principalTable: "trained_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "training_runs",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    is_completed = table.Column<bool>(type: "boolean", nullable: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trained_model_id = table.Column<Guid>(type: "uuid", nullable: true),
                    base_model_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_training_runs", x => x.id);
                    table.ForeignKey(
                        name: "FK_training_runs_trained_models_base_model_id",
                        column: x => x.base_model_id,
                        principalTable: "trained_models",
                        principalColumn: "id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "fk_training_runs_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_training_runs_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_detected_dump_sites_dataset_class_id",
                table: "detected_dump_sites",
                column: "dataset_class_id");

            migrationBuilder.CreateIndex(
                name: "IX_detected_dump_sites_detection_run_id",
                table: "detected_dump_sites",
                column: "detection_run_id");

            migrationBuilder.CreateIndex(
                name: "IX_detection_ignore_zones_created_by_id",
                table: "detection_ignore_zones",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_detection_runs_created_by_id",
                table: "detection_runs",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_trained_model_statistics_trained_model_id",
                table: "trained_model_statistics",
                column: "trained_model_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_trained_models_base_model_id",
                table: "trained_models",
                column: "base_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_trained_models_created_by_id",
                table: "trained_models",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_trained_models_dataset_id",
                table: "trained_models",
                column: "dataset_id");

            migrationBuilder.CreateIndex(
                name: "IX_trained_models_training_run_id",
                table: "trained_models",
                column: "training_run_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_training_runs_base_model_id",
                table: "training_runs",
                column: "base_model_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_runs_created_by_id",
                table: "training_runs",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_training_runs_dataset_id",
                table: "training_runs",
                column: "dataset_id");

            migrationBuilder.AddForeignKey(
                name: "fk_trained_models_trained_model_statistics_trained_model_statist~",
                table: "trained_model_statistics",
                column: "trained_model_id",
                principalTable: "trained_models",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_trained_models_training_runs_training_run_id",
                table: "trained_models",
                column: "training_run_id",
                principalTable: "training_runs",
                principalColumn: "id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_training_runs_trained_models_base_model_id",
                table: "training_runs");

            migrationBuilder.DropTable(
                name: "detected_dump_sites");

            migrationBuilder.DropTable(
                name: "detection_ignore_zones");

            migrationBuilder.DropTable(
                name: "trained_model_statistics");

            migrationBuilder.DropTable(
                name: "detection_runs");

            migrationBuilder.DropTable(
                name: "trained_models");

            migrationBuilder.DropTable(
                name: "training_runs");
        }
    }
}
