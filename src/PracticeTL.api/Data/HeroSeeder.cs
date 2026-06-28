using Microsoft.EntityFrameworkCore;
using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class HeroSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Heroes.Any())
            return;

        var hero = new Hero
        {
            Title = "TL:TECH",
            Subtitle = "Стабильно растем\nи принимаем новые вызовы\nTravelTech индустрии",
            Stats = new List<HeroStat>
            {
                new() { Type = "text", Value = "с 2008 года", Label = "создаем инструменты для отельеров", SortOrder = 1 },
                new() { Type = "text", Value = "300+", Label = "сотрудников в техотделе и более 700 в компании", SortOrder = 2 },
                new() { Type = "text", Value = "12 000+", Label = "клиентов работают с нами", SortOrder = 3 },
                new() { Type = "text", Value = "топ-50", Label = "в рейтинге работодателей hh.ru 2022-2024 гг.", SortOrder = 4 },
                new() { Type = "text", Value = ">1,5 млн", Label = "гостей в месяц бронируют через TravelLine", SortOrder = 5 },
                new() { Type = "text", Value = "300+", Label = "интеграций со сторонними сервисами", SortOrder = 6 },
                new() { Type = "img", Value = "/media/main/icons/tl-logo.svg", Label = "аккредитованная IT-компания", SortOrder = 7 },
                new() { Type = "img", Value = "/media/main/icons/tl-logo.svg", Label = "серебро в рейтинге лучших работодателей Forbes 2024 г.", SortOrder = 8 }
            }
        };

        db.Heroes.Add(hero);
        db.SaveChanges();
    }
}
