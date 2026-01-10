using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public class VacancyRepository : IVacancyRepository
{
    private readonly HRDbContext _context;

    public VacancyRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<Vacancy?> GetByIdAsync(int id)
    {
        return await _context.Vacancies
            .Include(v => v.Department)
            .Include(v => v.Position)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<IEnumerable<Vacancy>> GetAllAsync()
    {
        return await _context.Vacancies
            .Include(v => v.Department)
            .Include(v => v.Position)
            .ToListAsync();
    }

    public async Task<IEnumerable<Vacancy>> GetOpenVacanciesAsync()
    {
        return await _context.Vacancies
            .Include(v => v.Department)
            .Include(v => v.Position)
            .Where(v => v.Status == VacancyStatus.Open)
            .ToListAsync();
    }

    public async Task<Vacancy> CreateAsync(Vacancy vacancy)
    {
        _context.Vacancies.Add(vacancy);
        await _context.SaveChangesAsync();
        return vacancy;
    }

    public async Task<Vacancy> UpdateAsync(Vacancy vacancy)
    {
        _context.Vacancies.Update(vacancy);
        await _context.SaveChangesAsync();
        return vacancy;
    }

    public async Task DeleteAsync(int id)
    {
        var vacancy = await _context.Vacancies.FindAsync(id);
        if (vacancy != null)
        {
            _context.Vacancies.Remove(vacancy);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Vacancies.AnyAsync(v => v.Id == id);
    }
}

