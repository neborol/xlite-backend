using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace EliteForce.Migrations
{
    public partial class EventsAddition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EventItems",
                columns: table => new
                {
                    EventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 60, nullable: false),
                    Description = table.Column<string>(maxLength: 250, nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    Time = table.Column<string>(maxLength: 60, nullable: false),
                    Venue = table.Column<string>(maxLength: 140, nullable: false),
                    Comment = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventItems", x => x.EventId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EventItems");
        }
    }
}
