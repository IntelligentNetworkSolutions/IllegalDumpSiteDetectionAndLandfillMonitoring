using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddDatasetEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .Annotation("Npgsql:PostgresExtension:uuid-ossp", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.CreateTable(
                name: "datasets",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "text", nullable: false),
                    is_published = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    parent_dataset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_datasets", x => x.id);
                    table.ForeignKey(
                        name: "fk_datasets_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_datasets_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_datasets_datasets_parent_dataset_id",
                        column: x => x.parent_dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "dataset_classes",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    class_id = table.Column<int>(type: "integer", nullable: false),
                    class_name = table.Column<string>(type: "text", nullable: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'")
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dataset_classes", x => x.id);
                    table.ForeignKey(
                        name: "fk_dataset_classes_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dataset_classes_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "dataset_images",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "text", nullable: false),
                    image_path = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataset_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dataset_images", x => x.id);
                    table.ForeignKey(
                        name: "fk_dataset_images_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_dataset_images_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_dataset_images_datasets_dataset_id",
                        column: x => x.dataset_id,
                        principalTable: "datasets",
                        principalColumn: "id");
                });

            migrationBuilder.CreateTable(
                name: "image_annotations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    annotations_geo_json = table.Column<string>(type: "text", nullable: false),
                    geom = table.Column<string>(type: "text", nullable: false),
                    is_enabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    dataset_image_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by_id = table.Column<string>(type: "text", nullable: false),
                    created_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "CURRENT_TIMESTAMP AT TIME ZONE 'UTC'"),
                    updated_by_id = table.Column<string>(type: "text", nullable: true),
                    updated_on = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_image_annotations", x => x.id);
                    table.ForeignKey(
                        name: "fk_image_annotations_asp_net_users_created_by_id",
                        column: x => x.created_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_image_annotations_asp_net_users_updated_by_id",
                        column: x => x.updated_by_id,
                        principalTable: "asp_net_users",
                        principalColumn: "id");
                    table.ForeignKey(
                        name: "fk_image_annotations_dataset_images_dataset_image_id",
                        column: x => x.dataset_image_id,
                        principalTable: "dataset_images",
                        principalColumn: "id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_created_by_id",
                table: "dataset_classes",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_classes_dataset_id",
                table: "dataset_classes",
                column: "dataset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_created_by_id",
                table: "dataset_images",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_dataset_id",
                table: "dataset_images",
                column: "dataset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_dataset_images_updated_by_id",
                table: "dataset_images",
                column: "updated_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datasets_created_by_id",
                table: "datasets",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datasets_parent_dataset_id",
                table: "datasets",
                column: "parent_dataset_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_datasets_updated_by_id",
                table: "datasets",
                column: "updated_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_created_by_id",
                table: "image_annotations",
                column: "created_by_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_dataset_image_id",
                table: "image_annotations",
                column: "dataset_image_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_image_annotations_updated_by_id",
                table: "image_annotations",
                column: "updated_by_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "dataset_classes");

            migrationBuilder.DropTable(
                name: "image_annotations");

            migrationBuilder.DropTable(
                name: "dataset_images");

            migrationBuilder.DropTable(
                name: "datasets");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:postgis", ",,")
                .OldAnnotation("Npgsql:PostgresExtension:uuid-ossp", ",,");
        }
    }
}
