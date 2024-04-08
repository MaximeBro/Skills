using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using Skills.Databases;
using Skills.Models;
using Skills.Models.Enums;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Skills.Services;

public class SkillService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public async Task InitAsync()
    {
        var db = await factory.CreateDbContextAsync();
        var softSkillExists = db.SkillsTypes.AsNoTracking().FirstOrDefault(x => x.Type == SkillDataType.Type && x.Value == "Soft-Skill") != null;
        if (!softSkillExists)
        {
            db.SkillsTypes.Add(new SKillInfo { Type = SkillDataType.Type, Value = "Soft-Skill" });
            await db.SaveChangesAsync();
        }

        await db.DisposeAsync();
    }
    
    public async Task ImportXlsxAsync(Stream stream, string startCell, ImportType importType = ImportType.Purge)
    {
        var ms = new MemoryStream();
        await stream.CopyToAsync(ms);
        ms.Position = 0;
    
        await stream.DisposeAsync();
        
        var excelFile = await ms.QueryAsync<SkillRowModel>(null, ExcelType.XLSX, startCell);
        if (excelFile is null) return;

        switch (importType)
        {
            case ImportType.Purge: await PurgeSkillsAsync(excelFile);
                break;
        }
        await ms.DisposeAsync();
    }

    private async Task PurgeSkillsAsync(IEnumerable<SkillRowModel> skills)
    {
        var db = await factory.CreateDbContextAsync();
        var savedTypes = db.SkillsTypes.ToList();
        var savedSkills = db.Skills.ToList();
        db.RemoveRange(savedTypes);
        db.RemoveRange(savedSkills);
        await db.SaveChangesAsync();

        foreach (var row in skills)
        {
            
        }
        
        await db.SaveChangesAsync();
        await db.DisposeAsync();
    }

    /// <summary>
    /// Class used for xlsx deserialization.
    /// </summary>
    private sealed class SkillRowModel
    {
        [ExcelColumn(Name = "TYPE")] public string Type { get; set; } = string.Empty;
        [ExcelColumn(Name = "CATEGORIE")] public string Category { get; set; } = string.Empty;
        [ExcelColumn(Name = "SS-CATEGORIE")] public string SubCategory { get; set; } = string.Empty;
        [ExcelColumn(Name = "DESCRIPTION")] public string Description { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 1")] public string Lvl1 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 2")] public string Lvl2 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 3")] public string Lvl3 { get; set; } = string.Empty;
        [ExcelColumn(Name = "Niveau 4")] public string Lvl4 { get; set; } = string.Empty;
    }
}