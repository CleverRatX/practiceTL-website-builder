using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class HeroSeeder
{
    public static void Seed(AppDbContext db)
    {
        // Если данные уже есть, то ничего не делаем
        if (db.HeroItems.Any())
            return;

        db.HeroItems.AddRange(
            new HeroItem { Type = "text", Value = "с 2008 года", Label = "создаем инструменты для отельеров", SortOrder = 1 },
            new HeroItem { Type = "text", Value = "300+", Label = "сотрудников в техотделе и более 700 в компании", SortOrder = 2 },
            new HeroItem { Type = "text", Value = "12 000+", Label = "клиентов работают с нами", SortOrder = 3 },
            new HeroItem { Type = "text", Value = "топ-50", Label = "в рейтинге работодателей hh.ru 2022-2024 гг.", SortOrder = 4 },
            new HeroItem { Type = "text", Value = ">1,5 млн", Label = "гостей в месяц бронируют через TravelLine", SortOrder = 5 },
            new HeroItem { Type = "text", Value = "300+", Label = "интеграций со сторонними сервисами", SortOrder = 6 },
            new HeroItem { Type = "img", Value = "/media/main/icons/tl-logo.svg", Label = "аккредитованная IT-компания", SortOrder = 7 },
            new HeroItem { Type = "img", Value = "/media/main/icons/tl-logo.svg", Label = "серебро в рейтинге лучших работодателей Forbes 2024 г.", SortOrder = 8 },
            new HeroItem { Type = "text", Value = "123", Label = "456", SortOrder = 9 }
        );

        db.SaveChanges();
    }
}
