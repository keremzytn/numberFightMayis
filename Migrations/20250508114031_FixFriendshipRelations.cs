using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace numberFightMayis.Migrations
{
    /// <inheritdoc />
    public partial class FixFriendshipRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "Friendships",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId1",
                table: "Friendships",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<string>(type: "TEXT", nullable: false),
                    Player1Id = table.Column<string>(type: "TEXT", nullable: false),
                    Player2Id = table.Column<string>(type: "TEXT", nullable: false),
                    Player1Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Player2Score = table.Column<int>(type: "INTEGER", nullable: false),
                    GameMode = table.Column<int>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Winner = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsGameOver = table.Column<bool>(type: "INTEGER", nullable: false),
                    CurrentRound = table.Column<int>(type: "INTEGER", nullable: false),
                    Player1UsedCards = table.Column<string>(type: "TEXT", nullable: false),
                    Player2UsedCards = table.Column<string>(type: "TEXT", nullable: false),
                    Player1LastCard = table.Column<int>(type: "INTEGER", nullable: true),
                    Player2LastCard = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Games_AspNetUsers_Player1Id",
                        column: x => x.Player1Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Games_AspNetUsers_Player2Id",
                        column: x => x.Player2Id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameRound",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    GameId = table.Column<string>(type: "TEXT", nullable: false),
                    RoundNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Player1Number = table.Column<int>(type: "INTEGER", nullable: true),
                    Player2Number = table.Column<int>(type: "INTEGER", nullable: true),
                    Winner = table.Column<string>(type: "TEXT", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameRound", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameRound_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ApplicationUserId",
                table: "Friendships",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Friendships_ApplicationUserId1",
                table: "Friendships",
                column: "ApplicationUserId1");

            migrationBuilder.CreateIndex(
                name: "IX_DailyRewards_UserId",
                table: "DailyRewards",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameRound_GameId",
                table: "GameRound",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player1Id",
                table: "Games",
                column: "Player1Id");

            migrationBuilder.CreateIndex(
                name: "IX_Games_Player2Id",
                table: "Games",
                column: "Player2Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailyRewards_AspNetUsers_UserId",
                table: "DailyRewards",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId",
                table: "Friendships",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId1",
                table: "Friendships",
                column: "ApplicationUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailyRewards_AspNetUsers_UserId",
                table: "DailyRewards");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropForeignKey(
                name: "FK_Friendships_AspNetUsers_ApplicationUserId1",
                table: "Friendships");

            migrationBuilder.DropTable(
                name: "GameRound");

            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_Friendships_ApplicationUserId1",
                table: "Friendships");

            migrationBuilder.DropIndex(
                name: "IX_DailyRewards_UserId",
                table: "DailyRewards");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "Friendships");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId1",
                table: "Friendships");
        }
    }
}
