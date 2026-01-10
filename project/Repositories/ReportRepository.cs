using System.Data;
using Dapper;
using Npgsql;
using HRPlatform.DTO.Reports;
using Microsoft.Extensions.Configuration;

namespace HRPlatform.Repositories;

public class ReportRepository : IReportRepository
{
    private readonly string _connectionString;

    public ReportRepository(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DefaultConnection") 
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
    }

    public async Task<IEnumerable<EmployeeSkillsSummaryDto>> GetEmployeeSkillsSummaryAsync()
    {
        await using var connection = new NpgsqlConnection(_connectionString);
        await connection.OpenAsync();

        await using var transaction = await connection.BeginTransactionAsync();

        try
        {
            var sql = @"
                SELECT 
                    e.id AS EmployeeId,
                    e.first_name AS FirstName,
                    e.last_name AS LastName,
                    e.email AS Email,
                    d.name AS DepartmentName,
                    p.title AS PositionTitle,
                    COUNT(es.skill_id) AS SkillCount,
                    COUNT(CASE WHEN es.proficiency_level = 'Advanced' THEN 1 END) AS AdvancedSkillsCount,
                    COUNT(CASE WHEN es.proficiency_level = 'Intermediate' THEN 1 END) AS IntermediateSkillsCount,
                    COUNT(CASE WHEN es.proficiency_level = 'Basic' THEN 1 END) AS BasicSkillsCount,
                    STRING_AGG(DISTINCT s.name, ', ' ORDER BY s.name) AS SkillsList
                FROM employees e
                LEFT JOIN departments d ON e.department_id = d.id
                LEFT JOIN positions p ON e.position_id = p.id
                LEFT JOIN employee_skills es ON e.id = es.employee_id
                LEFT JOIN skills s ON es.skill_id = s.id
                WHERE e.status = 'Active'
                GROUP BY e.id, e.first_name, e.last_name, e.email, d.name, p.title
                ORDER BY e.last_name, e.first_name";

            var results = await connection.QueryAsync<EmployeeSkillsSummaryDto>(
                sql, 
                transaction: transaction);

            await transaction.CommitAsync();

            return results;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}

