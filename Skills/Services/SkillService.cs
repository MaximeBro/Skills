using Microsoft.EntityFrameworkCore;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using Skills.Databases;
using Skills.Models;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace Skills.Services;

public class SkillService(IConfiguration configuration, IDbContextFactory<SkillsContext> factory)
{
    /// <summary>
    /// If the "excel-file" is defined and the "import" is set to true, this will deserialize
    /// the specified .XLSX file as skills and register them in the db.
    /// </summary>
    public async Task InitAsync()
    {
        // var file = configuration["excel-file"];
        // var import = !string.IsNullOrWhiteSpace(configuration["import"]) && bool.Parse(configuration["import"]!);
        // if (import && !string.IsNullOrWhiteSpace(file))
        // {
        //     var fileStream = File.OpenRead(Path.Combine(Directory.GetCurrentDirectory(), "..\\data\\config", file));
        //     var ms = new MemoryStream();
        //     await fileStream.CopyToAsync(ms);
        //     ms.Position = 0;
        //
        //     await fileStream.DisposeAsync();
        //     
        //     var excelFile = await ms.QueryAsync<SkillRowModel>(null, ExcelType.XLSX);
        //     if (excelFile is null) return;
        //
        //     var db = await factory.CreateDbContextAsync();
        //     await db.Skills.AddRangeAsync(excelFile.Select(x => new SkillModel
        //     {
        //         Type = x.Type,
        //         Category = x.Category,
        //         SubCategory = x.SubCategory,
        //         Description = x.Description
        //     }));
        //     
        //     await db.SaveChangesAsync();
        //     await db.DisposeAsync();
        //     await ms.DisposeAsync();
        // }
    }

    /// <summary>
    /// Class used for xlsx deserialization.
    /// </summary>
    private sealed class SkillRowModel
    {
        [ExcelColumn(Name = "Type")] public string Type { get; set; } = string.Empty;
        [ExcelColumn(Name = "Catégorie")] public string Category { get; set; } = string.Empty;
        [ExcelColumn(Name = "Sous catégorie")] public string SubCategory { get; set; } = string.Empty;
        [ExcelColumn(Name = "Description")] public string Description { get; set; } = string.Empty;
    }
}