using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DomainCopilot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.TenantId);
                });

            migrationBuilder.CreateTable(
                name: "Policies",
                columns: table => new
                {
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PolicyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Version = table.Column<int>(type: "int", nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Policies", x => x.PolicyId);
                    table.ForeignKey(
                        name: "FK_Policies_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "TenantId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Claims",
                columns: table => new
                {
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PolicyNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ClaimedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Claims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_Claims_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Coverages",
                columns: table => new
                {
                    CoverageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CoverageName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Limit = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Deductible = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coverages", x => x.CoverageId);
                    table.ForeignKey(
                        name: "FK_Coverages_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Exclusions",
                columns: table => new
                {
                    ExclusionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PolicyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ExclusionName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Exclusions", x => x.ExclusionId);
                    table.ForeignKey(
                        name: "FK_Exclusions_Policies_PolicyId",
                        column: x => x.PolicyId,
                        principalTable: "Policies",
                        principalColumn: "PolicyId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdjudicationDecisions",
                columns: table => new
                {
                    AdjudicationDecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Decision = table.Column<int>(type: "int", nullable: false),
                    ApprovedAmount = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdjudicationDecisions", x => x.AdjudicationDecisionId);
                    table.ForeignKey(
                        name: "FK_AdjudicationDecisions_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Anomalies",
                columns: table => new
                {
                    AnomalyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ClaimId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AnomalyType = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Severity = table.Column<int>(type: "int", nullable: false),
                    DetectedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Anomalies", x => x.AnomalyId);
                    table.ForeignKey(
                        name: "FK_Anomalies_Claims_ClaimId",
                        column: x => x.ClaimId,
                        principalTable: "Claims",
                        principalColumn: "ClaimId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalRequests",
                columns: table => new
                {
                    ApprovalRequestId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdjudicationDecisionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ReviewerComment = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApprovalRequests", x => x.ApprovalRequestId);
                    table.ForeignKey(
                        name: "FK_ApprovalRequests_AdjudicationDecisions_AdjudicationDecisionId",
                        column: x => x.AdjudicationDecisionId,
                        principalTable: "AdjudicationDecisions",
                        principalColumn: "AdjudicationDecisionId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdjudicationDecisions_ClaimId",
                table: "AdjudicationDecisions",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_Anomalies_ClaimId",
                table: "Anomalies",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalRequests_AdjudicationDecisionId",
                table: "ApprovalRequests",
                column: "AdjudicationDecisionId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Claims_PolicyId",
                table: "Claims",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Claims_TenantId_ClaimNumber",
                table: "Claims",
                columns: new[] { "TenantId", "ClaimNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coverages_PolicyId",
                table: "Coverages",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Exclusions_PolicyId",
                table: "Exclusions",
                column: "PolicyId");

            migrationBuilder.CreateIndex(
                name: "IX_Policies_TenantId",
                table: "Policies",
                column: "TenantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Anomalies");

            migrationBuilder.DropTable(
                name: "ApprovalRequests");

            migrationBuilder.DropTable(
                name: "Coverages");

            migrationBuilder.DropTable(
                name: "Exclusions");

            migrationBuilder.DropTable(
                name: "AdjudicationDecisions");

            migrationBuilder.DropTable(
                name: "Claims");

            migrationBuilder.DropTable(
                name: "Policies");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
