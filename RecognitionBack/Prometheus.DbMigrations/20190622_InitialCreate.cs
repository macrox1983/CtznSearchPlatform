using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Prometheus.Infrastructure.Component.DbConfigurator;
using Prometheus.Infrastructure.Component.DbContext;
using Prometheus.Infrastructure.Component.DbMigration;
using System;

namespace Prometheus.DbMigrations
{
    [ComponentDbMigration(typeof(ComponentDbContext<>), "20190622_InitialCreate", "1.0.0")]
    public class InitialCreate : ComponentDbMigration
    {
        public InitialCreate(IEfDbConfigurator databaseConfiguration) : base(databaseConfiguration)
        {
        }

        protected override void UpImpl(MigrationBuilder migrationBuilder)
        {            

            //// Таблица роли пользователя
            //migrationBuilder.CreateTable(
            //    name: "Role",
            //    columns: table => new
            //    {
            //        RoleId = table.Column<Guid>(nullable: false),
            //        RoleName = table.Column<string>(nullable: false),
            //        RoleDescription = table.Column<string>(nullable: false),
            //    },
            //    constraints: table =>
            //    {
            //        table.PrimaryKey("PK_Role", columns => columns.RoleId);
            //    });

            // Таблица пользователь системы
            migrationBuilder.CreateTable(
                name: "ApplicationUser",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    Login = table.Column<string>(maxLength: 50),
                    PasswordHash = table.Column<byte[]>(maxLength: 20),
                    PasswordSalt = table.Column<byte[]>(maxLength: 20),
                    UserName = table.Column<string>(maxLength: 250),
                    RoleId = table.Column<int>(),
                    Phone = table.Column<string>(),
                    Email = table.Column<string>(),
                    IsDeleted = table.Column<bool>()
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", columns => columns.UserId);
                    //table.ForeignKey("FK_ApplicationUser_Role", columns => columns.RoleId, "Role", "RoleId");
                });

            // Таблица Город
            migrationBuilder.CreateTable(
                name: "City",
                columns: table => new
                {
                    //Ид города
                    CityId = table.Column<Guid>(),
                    // Код города
                    CityCode = table.Column<string>(nullable: false),
                    // Название
                    Name = table.Column<string>(nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_City", columns => columns.CityId);
                });


            migrationBuilder.CreateTable
                (
                name: "Camera",
                columns: table=> new
                {
                    CameraId = table.Column<Guid>(),
                    CameraStreamUrl = table.Column<string>(),
                    CameraDescription =table.Column<string>(),
                    UserId = table.Column<Guid>(),
                    Longitude = table.Column<decimal>(),
                    Latitude = table.Column<decimal>()
                },
                constraints: table => 
                {
                    table.PrimaryKey("PK_Camera", columns => columns.CameraId);
                    table.ForeignKey("FK_Camera_ApplicationUser", columns => columns.UserId, "ApplicationUser", "UserId");
                }
                );

            migrationBuilder.CreateTable(
                name:"SearchTicket",
                columns: table=> new
                {
                    SearchTicketId = table.Column<Guid>(),
                    LostPersonName= table.Column<string>(),
                    LostPersonBirthDate=table.Column<DateTime>(),
                    IsMale=table.Column<bool>(),
                    LostPersonGrowth=table.Column<int>(),
                    LostPersonWeight=table.Column<int>(),
                    LostPersonEyeColor=table.Column<string>(),
                    LostPersonHairColor=table.Column<string>(),
                    LostPersonDescription=table.Column<string>(),
                    LastLongitude=table.Column<decimal>(),
                    LastLatitude=table.Column<decimal>(),
                    UserId=table.Column<Guid>(),
                    LostDateTime=table.Column<DateTime>(),
                    SearchTicketTime=table.Column<DateTime>(),
                    SearchTicketStatus = table.Column<int>()
                },
                constraints: table=>
                {
                    table.PrimaryKey("PK_SearchTicket", columns => columns.SearchTicketId);
                    table.ForeignKey("FK_SearchTicket_ApplicationUser", columns => columns.UserId, "ApplicationUser", "UserId");
                }
                );
            migrationBuilder.CreateTable(
                name: "SearchParticipant",
                columns: table => new
                {
                    SearchParticipantId =table.Column<Guid>(),
                    UserId =table.Column<Guid>(),
                    SearchTicketId=table.Column<Guid>(),
                    ParticipantSearchStatus = table.Column<int>(),
                },
                constraints: table => 
                {
                    table.PrimaryKey("PK_SearchParticipant", columns => columns.SearchParticipantId);
                    table.ForeignKey("FK_SearchParticipant_ApplicationUser", columns => columns.UserId, "ApplicationUser", "UserId");
                    table.ForeignKey("FK_SearchParticipant_SearchTicket", columns => columns.SearchTicketId, "SearchTicket", "SearchTicketId");
                }
                );

            migrationBuilder.CreateTable(
                name: "FaceRecognitionHistory",
                columns: table => new
                {
                    FaceRecognitionHistoryId =table.Column<Guid>(),
                    SearchTicketId =table.Column<Guid>(),
                    Message =table.Column<string>()
                },
                constraints: table => 
                {
                    table.PrimaryKey("PK_FaceRecognitionHistory", columns => columns.FaceRecognitionHistoryId);
                    table.ForeignKey("FK_FaceRecognitionHistory_SearchTicket", columns => columns.SearchTicketId, "SearchTicket", "SearchTicketId");
                }
                );

                 
        }

        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "1.0.0");
        }
    }
}
