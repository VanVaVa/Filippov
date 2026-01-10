using Microsoft.EntityFrameworkCore;
using HRPlatform.Data.Context;
using HRPlatform.Data.Models;

namespace HRPlatform.Repositories;

public class SkillRepository : ISkillRepository
{
    private readonly HRDbContext _context;

    public SkillRepository(HRDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Skill>> GetAllAsync()
    {
        return await _context.Skills.ToListAsync();
    }

    public async Task<Skill?> GetByIdAsync(int id)
    {
        return await _context.Skills.FindAsync(id);
    }

    public async Task<Skill> CreateAsync(Skill skill)
    {
        _context.Skills.Add(skill);
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task<Skill> UpdateAsync(Skill skill)
    {
        _context.Skills.Update(skill);
        await _context.SaveChangesAsync();
        return skill;
    }

    public async Task DeleteAsync(int id)
    {
        var skill = await _context.Skills.FindAsync(id);
        if (skill != null)
        {
            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Skills.AnyAsync(s => s.Id == id);
    }
}

