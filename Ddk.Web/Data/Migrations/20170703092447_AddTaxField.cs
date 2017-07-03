using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Ddk.Data.Migrations
{
    public partial class AddTaxField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Tax",
                table: "Order",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tax",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Tax",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "Tax",
                table: "AspNetUsers");
        }
    }
}
