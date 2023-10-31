using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BlazorHero.CleanArchitecture.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AuditableAndSoftDeletableProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                schema: "Identity",
                table: "Users",
                newName: "Deleted");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureDataUrl",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Users",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Identity",
                table: "Roles",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "Roles",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                schema: "Identity",
                table: "RoleClaims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                schema: "Identity",
                table: "RoleClaims",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "RoleClaims",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "RoleClaims",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageDataUrl",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Products",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Products",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "DocumentTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "DocumentTypes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "DocumentTypes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "DocumentTypes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Documents",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Documents",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Documents",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedOn",
                table: "Brands",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModifiedOn",
                table: "Brands",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                schema: "Identity",
                table: "RoleClaims");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "DocumentTypes");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "CreatedOn",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "LastModifiedOn",
                table: "Brands");

            migrationBuilder.RenameColumn(
                name: "Deleted",
                schema: "Identity",
                table: "Users",
                newName: "IsDeleted");

            migrationBuilder.AlterColumn<string>(
                name: "ProfilePictureDataUrl",
                schema: "Identity",
                table: "Users",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ImageDataUrl",
                table: "Products",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
