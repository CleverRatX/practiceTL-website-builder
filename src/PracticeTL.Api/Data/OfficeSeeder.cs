using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class OfficeSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Offices.Any())
            return;

        var files = new[]
        {
            "b2ckfpv63kt740x426fxyryw7hsipu06.jpg",
            "882dh2yvq6k5i3epabqigrms68pyrd5n.jpg",
            "i21490yl4hrqwaepo6wasfgthyx23eya.jpg",
            "jqecmmx6ar3bz3g5ovrhp0wws532ztqf.jpg",
            "1xd2do64ycohcgzt95sb2tlu3vruhm2s.jpg"
        };

        var order = 1;
        foreach (var f in files)
        {
            db.Offices.Add(new Office
            {
                ImageUrl = "/media/main/offices/" + f,
                SortOrder = order++
            });
        }

        db.SaveChanges();
    }
}
