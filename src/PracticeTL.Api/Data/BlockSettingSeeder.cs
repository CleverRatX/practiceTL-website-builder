using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class BlockSettingSeeder
{
    public static void Seed(AppDbContext db)
    {
        var blocks = new (string Key, string Title, int SortOrder)[]
        {
            ("hero", "Главный экран", 1),
            ("advantages", "Цифры", 2),
            ("team", "Команда", 3),
            ("platform", "Платформа", 4),
            ("directions", "Направления", 5),
            ("slogan", "Слоган", 6),
            ("vacancies", "Вакансии", 7),
            ("gallery", "Фотогалерея", 8),
            ("work", "Офисы", 9),
            ("bonus", "Плюшки", 10)
        };

        var byKey = db.BlockSettings.ToDictionary(b => b.Key);
        var changed = false;

        foreach (var b in blocks)
        {
            if (byKey.TryGetValue(b.Key, out var existing))
            {
                if (existing.Title != b.Title || existing.SortOrder != b.SortOrder)
                {
                    existing.Title = b.Title;
                    existing.SortOrder = b.SortOrder;
                    changed = true;
                }
            }
            else
            {
                db.BlockSettings.Add(new BlockSetting
                {
                    Key = b.Key,
                    Title = b.Title,
                    Visible = true,
                    SortOrder = b.SortOrder
                });
                changed = true;
            }
        }

        if (changed)
            db.SaveChanges();
    }
}
