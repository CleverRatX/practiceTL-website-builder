using PracticeTL.Api.Models;

namespace PracticeTL.Api.Data;

public static class GallerySeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.GalleryItems.Any())
            return;

        var items = new (string Type, string ImageUrl, string Caption)[]
        {
            ("img", @"/media/main/photogallery/m0euwozke0u8vke8q63fmtif73qm3vhm.png", @"— Лена рассказывает студентам, что у нас на работе бесплатные обеды"),
            ("img", @"/media/main/photogallery/wqhgklblrdfh91duv5b6nfsdm27c32xg.png", @"— Эд на нашей конфе для отельеров “Overbooking” рассказывает, как хвалить сотрудников"),
            ("video", @"/media/main/photogallery/bxlhh2bko0jjbbg0bz1bbj14deankr1u.mp4", @"— у нас есть традиция летать в день рождения(и ломать потолок)"),
            ("img", @"/media/main/photogallery/bpd526tbcdfun58uwp6c5ohnkj55bou5.png", @"— поем под гитару у костра"),
            ("video", @"/media/main/photogallery/m95maqfvn7s807xpx4d0sq4i1mwcdcum.mp4", @"— у нас в компании несколько музыкальных групп, ребята дают жару!"),
            ("img", @"/media/main/photogallery/ub05v3k8gv79d307qa03qdkd2e76l0y3.png", @"— в офис часто приходят студенты и дети послушать, как устроена работа в  IT-компании. Денис рассказывает про дизайн"),
            ("img", @"/media/main/photogallery/b1ywyy4m35qwftkoardpfsfj1zehpjwe.png", @"— ребята на новогоднем корпорате решают срочные вопросики"),
            ("img", @"/media/main/photogallery/863a5cp2kqslo707gpj2lu0x4ty7gp41.png", @"— проводим митапы в офисе и в онлайне"),
            ("img", @"/media/main/photogallery/d2z81odrjf2xmyxo8ai17fiujr4xwwyx.png", @"—  дети сотрудников на “IT-лыжне” вместе"),
            ("img", @"/media/main/photogallery/58iuxfdoz01g037kt13f2sq9mlhp61vc.png", @"— просто Данил греется на солнышке"),
            ("video", @"/media/main/photogallery/59cfq0v8amb03ye2zzbdxety13rlj0xf.mp4", @"— любим вместе выезжать на природу, проводить выходные на берегу Волги"),
            ("img", @"/media/main/photogallery/3dy1df42llybzsot7uad0a5yltmo7e53.png", @"— Миша и его собака пишут код из Новосибирска"),
        };

        var order = 1;
        foreach (var i in items)
        {
            db.GalleryItems.Add(new GalleryItem { Type = i.Type, ImageUrl = i.ImageUrl, Caption = i.Caption, SortOrder = order++ });
        }
        db.SaveChanges();
    }
}
