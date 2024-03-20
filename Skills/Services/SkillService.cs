using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using Skills.Databases;
using Skills.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Skills.Services;

public class SkillService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    public async Task InitAsync()
    {
        var file = configuration["excel-file"];
        if (!string.IsNullOrWhiteSpace(file))
        {
            var fileStream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "..\\data\\config", file));
            var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms);
            ms.Position = 0;

            await fileStream.DisposeAsync();
            
            var excelFile = await ms.QueryAsync<SkillRowModel>(null, ExcelType.XLSX);
            if (excelFile is null) return;

            var db = await factory.CreateDbContextAsync();

            await db.Skills.AddRangeAsync(excelFile.Select(x => new SkillModel
            {
                Type = x.Type,
                Category = x.Category,
                SubCategory = x.SubCategory,
                Description = x.Description
            }));
            
            await db.SaveChangesAsync();
            await db.DisposeAsync();
            await ms.DisposeAsync();
        }
    }

    private sealed class SkillRowModel
    {
        [ExcelColumn(Name = "Type")] public string Type { get; set; } = string.Empty;
        [ExcelColumn(Name = "Catégorie")] public string Category { get; set; } = string.Empty;
        [ExcelColumn(Name = "Sous catégorie")] public string SubCategory { get; set; } = string.Empty;
        [ExcelColumn(Name = "Description")] public string Description { get; set; } = string.Empty;
    }
}