using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Models;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

public class HomeController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly ITeamService _teamService;
    private readonly IPlatformService _platformService;
    private readonly IBrandService _brandService;
    private readonly IWebHostEnvironment _env;

    public HomeController(
        IHeroService heroService,
        ITeamService teamService,
        IPlatformService platformService,
        IBrandService brandService,
        IWebHostEnvironment env)
    {
        _heroService = heroService;
        _teamService = teamService;
        _platformService = platformService;
        _brandService = brandService;
        _env = env;
    }

    [HttpGet("/")]
    [HttpGet("/index.html")]
    public async Task<IActionResult> Index()
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "index.html");
        var html = await System.IO.File.ReadAllTextAsync(templatePath);

        var hero = await _heroService.GetHeroAsync();
        var team = await _teamService.GetAllAsync();
        var platform = await _platformService.GetAllAsync();
        var brands = await _brandService.GetAllAsync();

        html = html
            .Replace("<!--HERO_TITLE-->", Enc(hero.Title))
            .Replace("<!--HERO_SUBTITLE-->", MultilineToHtml(hero.Subtitle))
            .Replace("<!--HERO_STAT_PARTS-->", BuildStatParts(hero.Stats))
            .Replace("<!--HERO_STATS-->", BuildStatItems(hero.Stats))
            .Replace("<!--TEAM_ITEMS-->", BuildTeamCards(team))
            .Replace("<!--PLATFORM_ITEMS-->", BuildPlatform(platform))
            .Replace("<!--BRANDS_ITEMS-->", BuildBrands(brands));

        return Content(html, "text/html; charset=utf-8");
    }

    private static string Enc(string s) => WebUtility.HtmlEncode(s);

    private static string MultilineToHtml(string text)
        => string.Join("<br>", text.Split('\n').Select(line => Enc(line.Trim())));

    private static string BuildStatParts(List<HeroStat> stats)
    {
        var sb = new StringBuilder();
        foreach (var s in stats)
        {
            if (s.Type == "img")
                sb.Append($"<span class=\"advantages__title-part advantages__title-part--icon\"><img src=\"{Enc(s.Value)}\" alt=\"tl-logo\" width=\"80\" height=\"80\"></span>");
            else
                sb.Append($"<span class=\"advantages__title-part\">{Enc(s.Value)}</span>");
        }
        return sb.ToString();
    }

    private static string BuildStatItems(List<HeroStat> stats)
    {
        var sb = new StringBuilder();
        foreach (var s in stats)
        {
            string inner = s.Type == "img"
                ? $"<img src=\"{Enc(s.Value)}\" alt=\"tl-logo\" width=\"80\" height=\"80\">"
                : Enc(s.Value);
            sb.Append($"<li class=\"advantages__item\"><span class=\"advantages__item-title\">{inner}</span> {Enc(s.Label)}</li>");
        }
        return sb.ToString();
    }

    private static string BuildTeamCards(List<TeamMember> members)
    {
        var sb = new StringBuilder();
        foreach (var m in members)
        {
            sb.Append($@"
            <article class=""swiper-slide team__item"">
                <div class=""team__item-card card card--rounded"">
                    <picture>
                        <img src=""{Enc(m.Photo)}"" class=""team__item-image card__image"" alt=""{Enc(m.Name)}"" width=""336"" height=""477"" decoding=""async"" loading=""lazy"">
                    </picture>
                    <div class=""card__description"">
                        <h3 class=""team__item-title"">{Enc(m.Name)}</h3>
                        <p class=""team__item-description"">{Enc(m.Position)}</p>
                    </div>
                </div>
                <p class=""team__item-text""></p>
            </article>");
        }
        return sb.ToString();
    }

    private static string BuildPlatform(List<PlatformItem> items)
    {
        var sb = new StringBuilder();
        int i = 0;
        while (i < items.Count)
        {
            var year = items[i].Year;
            sb.Append("<div class=\"platform-chart__col\">");
            sb.Append($"<p class=\"platform-chart__col-title\">{Enc(year)}</p>");
            sb.Append("<div class=\"platform-chart__col-items\">");
            while (i < items.Count && items[i].Year == year)
            {
                var it = items[i];
                var variant = it.Variant > 0 ? it.Variant : 1;
                var icon = string.IsNullOrWhiteSpace(it.Icon) ? DefaultIcon : it.Icon;
                sb.Append($"<div class=\"platform-chart__col-item icon-block icon-block--{variant}\">");
                sb.Append($"<div class=\"icon-block__icon\">{icon}");
                if (!string.IsNullOrWhiteSpace(it.Description))
                    sb.Append($"<div class=\"icon-block__description\">{it.Description}</div>");
                sb.Append("</div>");
                sb.Append($"<p class=\"icon-block__text\">{Enc(it.Name)}</p>");
                sb.Append($"<p class=\"icon-block__year\">{Enc(it.Year)}</p>");
                sb.Append("</div>");
                i++;
            }
            sb.Append("</div></div>");
        }
        return sb.ToString();
    }

    private static string BuildBrands(List<Brand> brands)
    {
        var sb = new StringBuilder();
        sb.Append("<ul class=\"brands__list swiper-slide\">");
        foreach (var b in brands)
            sb.Append($"<li class=\"brands__item\"><img src=\"{Enc(b.Logo)}\" alt=\"{Enc(b.Name)}\"></li>");
        sb.Append("</ul>");
        return sb.ToString();
    }

    private const string DefaultIcon = "<svg class=\"icon-block__img\" xmlns=\"http://www.w3.org/2000/svg\" width=\"40\" height=\"40\" viewBox=\"0 0 40 40\" fill=\"none\"><circle cx=\"20\" cy=\"20\" r=\"16\" stroke=\"white\" stroke-width=\"3\"></circle></svg>";
}
