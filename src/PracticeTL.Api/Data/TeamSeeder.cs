using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class TeamSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.TeamMembers.Any())
            return;

        var members = new (string Name, string Position, string File)[]
        {
            ("Алексей Герасимов", "CTO", "aleksey_gerasimov.png"),
            ("Юра Костин", "CPO", "yura_kostin.png"),
            ("Ваня Потехин", "старший руководитель проектов", "vanya_potehin.png"),
            ("Настя Ягодарова", "руководитель проекта", "nastya_yagodarova.png"),
            ("Гасан Агаев", "разработчик", "gasan_agaev.png"),
            ("Игорь Егошин", "разработчик", "igor_egoshin.png"),
            ("Костя Дмитриев", "разработчик", "kostya_dmitriev.png"),
            ("Лена Мочалова", "руководитель проекта", "lena_mochalova.png"),
            ("Настя Волкова", "аналитик", "nastya_volkova.png"),
            ("Оля Рядова", "дизайнер", "olya_ryadova.png"),
            ("Саша Очеев", "разработчик", "sasha_ocheev.png"),
            ("Таня Глазырина", "тестировщик", "tanya_glazirina.png"),
            ("Женя Гермогенов", "разработчик", "zhenya_germogenov.png")
        };

        var order = 1;
        foreach (var m in members)
        {
            db.TeamMembers.Add(new TeamMember
            {
                Name = m.Name,
                Position = m.Position,
                Photo = "/media/main/team/" + m.File,
                SortOrder = order++
            });
        }

        db.SaveChanges();
    }
}
