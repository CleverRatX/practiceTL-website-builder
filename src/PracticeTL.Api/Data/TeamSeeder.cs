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
            ("Юра Костин", "CTO", "yura_kostin.png"),
            ("Ваня Потехин", "старший руководитель проектов", "vanya_potehin.png"),
            ("Настя Ягодарова", "руководитель проекта", "nastya_yagodarova.png"),
            ("Гасан Агаев", "руководитель проекта", "gasan_agaev.png"),
            ("Игорь Егошин", "старший инженер-программист", "igor_egoshin.png"),
            ("Костя Дмитриев", "специалист по информационным системам", "kostya_dmitriev.png"),
            ("Лена Мочалова", "руководитель HR-отдела", "lena_mochalova.png"),
            ("Настя Волкова", "руководитель отдела дизайна", "nastya_volkova.png"),
            ("Оля Рядова", "руководитель отдела аналитики", "olya_ryadova.png"),
            ("Саша Очеев", "инженер-программист", "sasha_ocheev.png"),
            ("Таня Глазырина", "продуктовый дизайнер", "tanya_glazirina.png"),
            ("Женя Гермогенов", "старший руководитель проектов", "zhenya_germogenov.png")
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
