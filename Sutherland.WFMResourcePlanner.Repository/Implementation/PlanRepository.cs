using Dapper;
using Sutherland.WFMResourcePlanner.Entities;
using Sutherland.WFMResourcePlanner.Entities.DTO;
using Sutherland.WFMResourcePlanner.Repository.Inerface;
using Sutherland.WFMResourcePlanner.Utilities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Sutherland.WFMResourcePlanner.Repository.Implementation
{
    public class PlanRepository : IPlanRepository
    {
        private readonly IDbConnection _connection;
        public PlanRepository(IDbConnection connection)
        {
            _connection = connection;
        }
        public async Task<int> CreateAsync(Plan plan)
        {
            if (_connection.State != ConnectionState.Open)
                await ((SqlConnection)_connection).OpenAsync();
            using var transaction = _connection.BeginTransaction();

            try
            {
           // 1. Insert Plan
                var planId = await _connection.ExecuteScalarAsync<int>(@"
                                        INSERT INTO Plans (Name, Vertical, Account, Geo, Site, BusinessUnit, SOTracker, AssumptionSheet, WeekStart, PlanFrom, PlanTo)
                                        OUTPUT INSERTED.PlanId
                                        VALUES (@Name, @Vertical, @Account, @Geo, @Site, @BusinessUnit, @SOTracker, @AssumptionSheet, @WeekStart, @PlanFrom, @PlanTo);
                             ", plan, transaction);

            // 2. Generate Week Headers
            var weekStart = plan.WeekStart == "Monday" ? DayOfWeek.Monday : DayOfWeek.Sunday;
            var weekHeaders = WeekHelper.GenerateWeekHeaders(plan.PlanFrom, plan.PlanTo, weekStart);

            var sheets = new List<PlanSheet>();

            // 3. Insert LOBs and LOB sheets
            foreach (var lob in plan.LOBs)
            {
                lob.PlanId = planId;
                var lobId = await _connection.ExecuteScalarAsync<int>(@"
            INSERT INTO LOBs (PlanId, Name, BillingModel, ProjectId)
            OUTPUT INSERTED.LOBId
            VALUES (@PlanId, @Name, @BillingModel, @ProjectId);", lob, transaction);

                var template = lob.BillingModel == "FTE"
                    ? File.ReadAllText("Templates/FTE_Template.json")
                    : File.ReadAllText("Templates/Transaction_Template.json");

                var injectedJson = WeekHelper.InjectWeekHeaders(template, weekHeaders);

                sheets.Add(new PlanSheet
                {
                    PlanId = planId,
                    LOBId = lobId,
                    Name = lob.Name,
                    Type = lob.BillingModel,
                    JsonData = injectedJson
                });
            }

            // 4. Optional sheets
            if (plan.SOTracker)
            {
                sheets.Add(new PlanSheet
                {
                    PlanId = planId,
                    Name = "SO Tracker",
                    Type = "SOTracker",
                    JsonData = File.ReadAllText("Templates/SOTracker_Template.json")
                });
            }

            if (plan.AssumptionSheet)
            {
                sheets.Add(new PlanSheet
                {
                    PlanId = planId,
                    Name = "Assumption Sheet",
                    Type = "Assumption",
                    JsonData = File.ReadAllText("Templates/Assumption_Template.json")
                });
            }

                // 5. Weekly Staffing Summary (placeholder)
                var lobNames = plan.LOBs.Select(l => l.Name).ToList();
                string summaryJson = WeeklyStaffingSummaryBuilder.BuildSummaryJson(weekHeaders, lobNames);
                sheets.Add(new PlanSheet
                {
                    PlanId = planId,
                    Name = "Weekly Staffing Summary",
                    Type = "WeeklySummary",
                    JsonData = summaryJson
                });


            //var summaryTemplate = File.ReadAllText("Templates/Weekly_Staffing_Summary_Template.json");
            //summaryTemplate = WeekHelper.InjectWeekHeaders(summaryTemplate, weekHeaders);
            //sheets.Add(new PlanSheet
            //{
            //    PlanId = planId,
            //    Name = "Weekly Staffing Summary",
            //    Type = "WeeklySummary",
            //    JsonData = summaryTemplate
            //});

            // 6. Insert Sheets
            foreach (var sheet in sheets)
            {
                await _connection.ExecuteAsync(@"
            INSERT INTO PlanSheets (PlanId, LobId, Name, Type, JsonData)
            VALUES (@PlanId, @LobId, @Name, @Type, @JsonData);", sheet, transaction);
            }

                transaction.Commit();
                return planId;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<PlanWithSheetsDto?> GetPlanWithSheetsAsync(int planId)
        {
            if (_connection.State != ConnectionState.Open)
                await ((SqlConnection)_connection).OpenAsync();

            var sqlPlan = "SELECT * FROM Plans WHERE PlanId = @planId";
            var sqlSheets = "SELECT * FROM PlanSheets WHERE PlanId = @planId ORDER BY PlanSheetId";

            var plan = await _connection.QueryFirstOrDefaultAsync<PlanDto>(sqlPlan, new { planId });
            if (plan == null) return null;

            var sheets = (await _connection.QueryAsync<PlanSheetDto>(sqlSheets, new { planId })).ToList();

            return new PlanWithSheetsDto
            {
                PlanId = plan.PlanId,
                Name = plan.Name,
                Sheets = sheets
            };
        }

        public async Task SaveSheetsAsync_1(List<SaveSheetDto> sheets)
        {
            if(_connection.State != ConnectionState.Open)
                await ((SqlConnection)_connection).OpenAsync();

            using var transaction = _connection.BeginTransaction();
            try
            {
                const string sql = @"
            UPDATE PlanSheets
            SET JsonData = @JsonData,
                Name = @Name,
                Type = @Type
            WHERE PlanSheetId = @Id AND PlanId = @PlanId";

            foreach (var sheet in sheets)
            {
                await _connection.ExecuteAsync(sql, new
                {
                    sheet.JsonData,
                    sheet.Name,
                    sheet.Type,
                    sheet.Id,
                    sheet.PlanId
                }, transaction);
            }
                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task SaveSheetsAsync(List<SaveSheetDto> sheets)
        {
            if (_connection.State != ConnectionState.Open)
                await ((SqlConnection)_connection).OpenAsync();

            using var transaction = _connection.BeginTransaction();

            try
            {
                var planId = sheets.FirstOrDefault()?.PlanId ?? 0;
                if (planId == 0)
                    throw new ArgumentException("PlanId is missing in sheets list");

                // 1. Get all existing sheet IDs for the plan
                var existingSheetIds = (await _connection.QueryAsync<int>(
                    "SELECT PlanSheetId FROM PlanSheets WHERE PlanId = @PlanId",
                    new { PlanId = planId },
                    transaction)).ToHashSet();

                var incomingSheetIds = sheets
                    .Where(s => s.Id > 0)
                    .Select(s => s.Id)
                    .ToHashSet();

                // 2. Delete sheets not in the incoming list
                var sheetsToDelete = existingSheetIds.Except(incomingSheetIds).ToList();
                if (sheetsToDelete.Any())
                {
                    await _connection.ExecuteAsync(
                        "DELETE FROM PlanSheets WHERE PlanSheetId IN @Ids",
                        new { Ids = sheetsToDelete },
                        transaction);
                }

                // 3. Update or insert each sheet
                const string updateSql = @"
            UPDATE PlanSheets
            SET JsonData = @JsonData,
                Name = @Name,
                Type = @Type
            WHERE PlanSheetId = @Id AND PlanId = @PlanId";

                const string insertSql = @"
            INSERT INTO PlanSheets (PlanId, Name, Type, JsonData)
            VALUES (@PlanId, @Name, @Type, @JsonData)";

                foreach (var sheet in sheets)
                {
                    if (sheet.Id > 0)
                    {
                        var affected = await _connection.ExecuteAsync(updateSql, new
                        {
                            sheet.JsonData,
                            sheet.Name,
                            sheet.Type,
                            sheet.Id,
                            sheet.PlanId
                        }, transaction);

                        if (affected == 0)
                        {
                            await _connection.ExecuteAsync(insertSql, new
                            {
                                sheet.PlanId,
                                sheet.Name,
                                sheet.Type,
                                sheet.JsonData
                            }, transaction);
                        }
                    }
                    else
                    {
                        await _connection.ExecuteAsync(insertSql, new
                        {
                            sheet.PlanId,
                            sheet.Name,
                            sheet.Type,
                            sheet.JsonData
                        }, transaction);
                    }
                }

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }



        public async Task<IEnumerable<Plan>> GetAllPlansAsync()
		{
			if (_connection.State != ConnectionState.Open)
				await ((SqlConnection)_connection).OpenAsync();

			var sql = @"SELECT PlanId, Name, Vertical, Account, Geo, Site, PlanFrom, PlanTo 
                    FROM Plans 
                    ORDER BY CreatedAt DESC";

			var plans = await _connection.QueryAsync<Plan>(sql);
			return plans;
		}

		public async Task<int> CopyPlanAsync(CopyPlanDto dto)
		{
			var newPlanId = 0;

			if (_connection.State != ConnectionState.Open)
				await ((SqlConnection)_connection).OpenAsync();

			using var transaction = _connection.BeginTransaction();
			try
			{
				// Copy Plan metadata
				var insertPlanSql = @"
             INSERT INTO Plans (Name, Vertical, Account, Geo, Site, BusinessUnit, SOTracker, AssumptionSheet, WeekStart, PlanFrom, PlanTo)
            SELECT @NewPlanTitle, Vertical, Account, Geo, Site, BusinessUnit, SOTracker, AssumptionSheet, WeekStart, PlanFrom, PlanTo
            FROM Plans WHERE PlanId = @SourcePlanId;
            SELECT CAST(SCOPE_IDENTITY() AS INT);";

				newPlanId = await _connection.ExecuteScalarAsync<int>(insertPlanSql, new
				{
					dto.NewPlanTitle,
					dto.SourcePlanId
				}, transaction);

				// Copy sheets
				var insertSheetsSql = @"
            INSERT INTO PlanSheets (PlanId, LobId, Name, Type, JsonData)
            SELECT @NewPlanId,LobId, Name, Type, JsonData
            FROM PlanSheets WHERE PlanId = @SourcePlanId;";

				await _connection.ExecuteAsync(insertSheetsSql, new
				{
					NewPlanId = newPlanId,
					dto.SourcePlanId
				}, transaction);

				transaction.Commit();
				return newPlanId;
			}
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

	}
}
