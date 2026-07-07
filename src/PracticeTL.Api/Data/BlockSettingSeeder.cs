using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class BlockSettingSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.BlockSettings.Any())
            return;

        var blocks = new (string Key, string Title)[]
        {
            ("team", "Команда"),
            ("platform", "Платформа"),
            ("directions", "Направления"),
            ("vacancies", "Вакансии"),
            ("gallery", "Фотогалерея")
        };

        var order = 1;
        foreach (var b in blocks)
        {
            db.BlockSettings.Add(new BlockSetting
            {
                Key = b.Key,
                Title = b.Title,
                Visible = true,
                SortOrder = order++
            });
        }

        db.SaveChanges();
    }
}
