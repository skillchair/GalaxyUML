using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GalaxyUML.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Username = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Attributes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdClassBox = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attributes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BannedUsers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdUser = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTeam = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BannedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BannedUsers_Users_IdUser",
                        column: x => x.IdUser,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Chats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdMeeting = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chats", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Diagrams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    StartX = table.Column<int>(type: "int", nullable: false),
                    StartY = table.Column<int>(type: "int", nullable: false),
                    EndX = table.Column<int>(type: "int", nullable: false),
                    EndY = table.Column<int>(type: "int", nullable: false),
                    IdMeeting = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdParent = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Discriminator = table.Column<string>(type: "nvarchar(21)", maxLength: 21, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Stereotype = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextSize = table.Column<double>(type: "float", nullable: true),
                    IdBox1 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IdBox2 = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Text1 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Text2 = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    MiddleText = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Format = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Size = table.Column<double>(type: "float", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(9)", maxLength: 9, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Diagrams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Diagrams_Diagrams_IdBox1",
                        column: x => x.IdBox1,
                        principalTable: "Diagrams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Diagrams_Diagrams_IdBox2",
                        column: x => x.IdBox2,
                        principalTable: "Diagrams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Diagrams_Diagrams_IdParent",
                        column: x => x.IdParent,
                        principalTable: "Diagrams",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Methods",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdClassBox = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Method = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Methods", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Methods_Diagrams_IdClassBox",
                        column: x => x.IdClassBox,
                        principalTable: "Diagrams",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Meetings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartingTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndingTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdOrganizer = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTeam = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdChat = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdBoard = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    TeamEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Meetings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Members",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdTeam = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdMember = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    TextEntityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Members", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Members_Diagrams_TextEntityId",
                        column: x => x.TextEntityId,
                        principalTable: "Diagrams",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Members_Users_IdMember",
                        column: x => x.IdMember,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Participants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdMeeting = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdParticipant = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Participants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Participants_Meetings_IdMeeting",
                        column: x => x.IdMeeting,
                        principalTable: "Meetings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Participants_Members_IdParticipant",
                        column: x => x.IdParticipant,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teams",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdTeamOwner = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TeamCode = table.Column<string>(type: "nvarchar(6)", maxLength: 6, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teams", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teams_Members_IdTeamOwner",
                        column: x => x.IdTeamOwner,
                        principalTable: "Members",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdChat = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IdSender = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Chats_IdChat",
                        column: x => x.IdChat,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Messages_Participants_IdSender",
                        column: x => x.IdSender,
                        principalTable: "Participants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Attributes_IdClassBox",
                table: "Attributes",
                column: "IdClassBox");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_IdTeam",
                table: "BannedUsers",
                column: "IdTeam");

            migrationBuilder.CreateIndex(
                name: "IX_BannedUsers_IdUser_IdTeam",
                table: "BannedUsers",
                columns: new[] { "IdUser", "IdTeam" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Chats_IdMeeting",
                table: "Chats",
                column: "IdMeeting",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagrams_IdBox1",
                table: "Diagrams",
                column: "IdBox1");

            migrationBuilder.CreateIndex(
                name: "IX_Diagrams_IdBox2",
                table: "Diagrams",
                column: "IdBox2");

            migrationBuilder.CreateIndex(
                name: "IX_Diagrams_IdMeeting",
                table: "Diagrams",
                column: "IdMeeting",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Diagrams_IdParent",
                table: "Diagrams",
                column: "IdParent");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_IdOrganizer",
                table: "Meetings",
                column: "IdOrganizer");

            migrationBuilder.CreateIndex(
                name: "IX_Meetings_TeamEntityId",
                table: "Meetings",
                column: "TeamEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Members_IdMember",
                table: "Members",
                column: "IdMember");

            migrationBuilder.CreateIndex(
                name: "IX_Members_IdTeam_IdMember",
                table: "Members",
                columns: new[] { "IdTeam", "IdMember" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Members_TextEntityId",
                table: "Members",
                column: "TextEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdChat",
                table: "Messages",
                column: "IdChat");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_IdSender",
                table: "Messages",
                column: "IdSender");

            migrationBuilder.CreateIndex(
                name: "IX_Methods_IdClassBox",
                table: "Methods",
                column: "IdClassBox");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_IdMeeting",
                table: "Participants",
                column: "IdMeeting");

            migrationBuilder.CreateIndex(
                name: "IX_Participants_IdParticipant",
                table: "Participants",
                column: "IdParticipant");

            migrationBuilder.CreateIndex(
                name: "IX_Teams_IdTeamOwner",
                table: "Teams",
                column: "IdTeamOwner");

            migrationBuilder.AddForeignKey(
                name: "FK_Attributes_Diagrams_IdClassBox",
                table: "Attributes",
                column: "IdClassBox",
                principalTable: "Diagrams",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BannedUsers_Teams_IdTeam",
                table: "BannedUsers",
                column: "IdTeam",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Meetings_IdMeeting",
                table: "Chats",
                column: "IdMeeting",
                principalTable: "Meetings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Diagrams_Meetings_IdMeeting",
                table: "Diagrams",
                column: "IdMeeting",
                principalTable: "Meetings",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Participants_IdOrganizer",
                table: "Meetings",
                column: "IdOrganizer",
                principalTable: "Participants",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Meetings_Teams_TeamEntityId",
                table: "Meetings",
                column: "TeamEntityId",
                principalTable: "Teams",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Members_Teams_IdTeam",
                table: "Members",
                column: "IdTeam",
                principalTable: "Teams",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Members_Diagrams_TextEntityId",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Meetings_Teams_TeamEntityId",
                table: "Meetings");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Teams_IdTeam",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Members_Users_IdMember",
                table: "Members");

            migrationBuilder.DropForeignKey(
                name: "FK_Participants_Meetings_IdMeeting",
                table: "Participants");

            migrationBuilder.DropTable(
                name: "Attributes");

            migrationBuilder.DropTable(
                name: "BannedUsers");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Methods");

            migrationBuilder.DropTable(
                name: "Chats");

            migrationBuilder.DropTable(
                name: "Diagrams");

            migrationBuilder.DropTable(
                name: "Teams");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Meetings");

            migrationBuilder.DropTable(
                name: "Participants");

            migrationBuilder.DropTable(
                name: "Members");
        }
    }
}
