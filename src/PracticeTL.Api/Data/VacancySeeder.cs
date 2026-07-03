using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class VacancySeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Vacancies.Any())
            return;

        var items = new (string Title, string Address, string Url)[]
        {
            ("Senior Site Reliability Engineer (SRE)", "удаленно", "https://hh.ru/vacancy/132518636"),
            ("Аналитик данных (финансовое направление)", "удаленно", "https://kazan.hh.ru/vacancy/132543183?hhtmFrom=employer_vacancies"),
            ("Python Developer AI team (в офис, г. Йошкар-Ола)", "Йошкар-Ола", "https://kazan.hh.ru/vacancy/133500742?hhtmFrom=employer_vacancies"),
        };

        var order = 1;
        foreach (var i in items)
        {
            db.Vacancies.Add(new Vacancy { Title = i.Title, Address = i.Address, Url = i.Url, SortOrder = order++ });
        }
        db.SaveChanges();
    }
}
