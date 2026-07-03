using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class BrandSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Brands.Any())
            return;

        var brands = new (string Name, string File)[]
        {
            ("Catherine art", "catherine-art.svg"),
            ("Cosmos", "cosmos.svg"),
            ("Helvetia", "helvetia.svg"),
            ("Ohta park", "ohta-park.svg"),
            ("Русь", "rus.svg"),
            ("Sochi park", "sochi-park.svg"),
            ("Премиум парк", "premium-park.svg"),
            ("Акваклуб", "aquaclub.svg"),
            ("РЖД Здоровье", "rzd-health.svg"),
        };

        var order = 1;
        foreach (var b in brands)
        {
            db.Brands.Add(new Brand { Name = b.Name, Logo = "/media/main/hotels/" + b.File, SortOrder = order++ });
        }
        db.SaveChanges();
    }
}
