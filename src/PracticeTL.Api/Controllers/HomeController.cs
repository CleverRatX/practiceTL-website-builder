using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authorization;
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
    private readonly IDirectionService _directionService;
    private readonly IVacancyService _vacancyService;
    private readonly IGalleryService _galleryService;
    private readonly IWebHostEnvironment _env;

    public HomeController(
        IHeroService heroService,
        ITeamService teamService,
        IPlatformService platformService,
        IBrandService brandService,
        IDirectionService directionService,
        IVacancyService vacancyService,
        IGalleryService galleryService,
        IWebHostEnvironment env)
    {
        _heroService = heroService;
        _teamService = teamService;
        _platformService = platformService;
        _brandService = brandService;
        _directionService = directionService;
        _vacancyService = vacancyService;
        _galleryService = galleryService;
        _env = env;
    }

    [Authorize]
    [HttpGet("/admin.html")]
    public async Task<IActionResult> Admin()
    {
        var path = Path.Combine(_env.ContentRootPath, "Templates", "admin.html");
        var html = await System.IO.File.ReadAllTextAsync(path);
        return Content(html, "text/html; charset=utf-8");
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
        var directions = await _directionService.GetAllAsync();
        var vacancies = await _vacancyService.GetAllAsync();
        var gallery = await _galleryService.GetAllAsync();

        html = html
            .Replace("<!--HERO_TITLE-->", Enc(hero.Title))
            .Replace("<!--HERO_SUBTITLE-->", MultilineToHtml(hero.Subtitle))
            .Replace("<!--HERO_STAT_PARTS-->", BuildStatParts(hero.Stats))
            .Replace("<!--HERO_STATS-->", BuildStatItems(hero.Stats))
            .Replace("<!--TEAM_ITEMS-->", BuildTeamCards(team))
            .Replace("<!--PLATFORM_ITEMS-->", BuildPlatform(platform))
            .Replace("<!--BRANDS_ITEMS-->", BuildBrands(brands))
            .Replace("<!--DIRECTIONS_ITEMS-->", BuildDirections(directions))
            .Replace("<!--VACANCIES_ITEMS-->", BuildVacancies(vacancies))
            .Replace("<!--GALLERY_ITEMS-->", BuildGallery(gallery));

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
        var columns = items
            .GroupBy(p => p.Year)
            .OrderBy(g => YearNumber(g.Key))
            .ToList();

        var sb = new StringBuilder();
        foreach (var col in columns)
        {
            sb.Append("<div class=\"platform-chart__col\">");
            sb.Append($"<p class=\"platform-chart__col-title\">{Enc(col.Key)}</p>");
            sb.Append("<div class=\"platform-chart__col-items\">");
            foreach (var it in col.OrderBy(x => x.SortOrder))
            {
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
            }
            sb.Append("</div></div>");
        }
        return sb.ToString();
    }

    private static int YearNumber(string year)
    {
        var digits = new string(year.Where(char.IsDigit).ToArray());
        return int.TryParse(digits, out var n) ? n : 0;
    }

    private static string BuildBrands(List<Brand> brands)
    {
        var sb = new StringBuilder();
        const int slides = 4;
        for (int s = 0; s < slides; s++)
        {
            sb.Append("<ul class=\"brands__list swiper-slide\">");
            foreach (var b in brands)
                sb.Append($"<li class=\"brands__item\"><img src=\"{Enc(b.Logo)}\" alt=\"{Enc(b.Name)}\"></li>");
            sb.Append("</ul>");
        }
        return sb.ToString();
    }

    private static string BuildDirections(List<Direction> directions)
    {
        var sb = new StringBuilder();
        foreach (var d in directions)
        {
            sb.Append("<article class=\"directions__item accordion\">");
            sb.Append("<div class=\"accordion__title\">");
            sb.Append($"<span>{Enc(d.Name)}</span>");

            var badges = (d.Badges ?? "")
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
            if (badges.Length > 0)
            {
                sb.Append("<div class=\"accordion__title-stak-list bages-list\">");
                foreach (var b in badges)
                    sb.Append($"<div class=\"bages-list__item\">{Enc(b)}</div>");
                sb.Append("</div>");
            }

            sb.Append($"<div class=\"accordion__toggle\">{ToggleIcon}</div>");
            sb.Append("</div>");

            sb.Append("<div class=\"accordion__body\">");
            sb.Append($"<div class=\"accordion__content ustyle\">{d.Content}</div>");
            sb.Append("</div>");

            sb.Append("</article>");
        }
        return sb.ToString();
    }

    private static string BuildVacancies(List<Vacancy> vacancies)
    {
        var sb = new StringBuilder();
        foreach (var v in vacancies)
        {
            sb.Append("<article class=\"vacancies__item card card--half-rounded\">");
            sb.Append($"<h3 class=\"vacancies__item-title card__title heading heading--type-card\">{Enc(v.Title)}</h3>");
            sb.Append("<div class=\"card__footer\">");
            sb.Append($"<p class=\"vacancies__item-address card__address\">{Enc(v.Address)}</p>");
            sb.Append($"<div class=\"card__footer-link\">{ArrowIcon}</div>");
            sb.Append("</div>");
            sb.Append($"<a href=\"{Enc(v.Url)}\" class=\"vacancies__item-lik\" target=\"_blank\"></a>");
            sb.Append("</article>");
        }

        sb.Append("<article class=\"vacancies__item vacancies__item--type-more card card--half-rounded\">");
        sb.Append($"<h3 class=\"vacancies__item-title card__title heading heading--type-card\">Еще больше вакансий на HeadHunter {MoreIcon}</h3>");
        sb.Append("<a href=\"https://hh.ru/employer/1136961\" class=\"vacancies__item-lik\" target=\"_blank\"></a>");
        sb.Append("</article>");

        return sb.ToString();
    }

    private static string BuildGallery(List<GalleryItem> items)
    {
        var sb = new StringBuilder();
        foreach (var g in items)
        {
            sb.Append("<div class=\"gallery__item gallery__item--type-img\">");

            if (g.Type == "img")
                sb.Append($"<img src=\"{Enc(g.ImageUrl)}\" alt=\"\" class=\"card card--rounded\">");
            else
                sb.Append($"<video src=\"{Enc(g.ImageUrl)}\" alt=\"\" class=\"card card--rounded\"></video>");

            sb.Append($"<p class=\"gallery__item-text\">{Enc(g.Caption)}</p>");
            sb.Append("</div>");
        }
        return sb.ToString();
    }

    private const string ToggleIcon = "<svg width=\"24\" height=\"14\" viewBox=\"0 0 24 14\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><circle cx=\"2\" cy=\"2\" r=\"2\" fill=\"#507BCE\"></circle><circle cx=\"7\" cy=\"7\" r=\"2\" fill=\"#507BCE\"></circle><circle cx=\"12\" cy=\"12\" r=\"2\" fill=\"#507BCE\"></circle><circle cx=\"17\" cy=\"7\" r=\"2\" fill=\"#507BCE\"></circle><circle cx=\"22\" cy=\"2\" r=\"2\" fill=\"#507BCE\"></circle></svg>";
    private const string MoreIcon = "<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"14\" height=\"24\" viewBox=\"0 0 14 24\" fill=\"none\"><circle cx=\"2\" cy=\"22\" r=\"2\" transform=\"rotate(-90 2 22)\" fill=\"white\"></circle><circle cx=\"7\" cy=\"17\" r=\"2\" transform=\"rotate(-90 7 17)\" fill=\"white\"></circle><circle cx=\"12\" cy=\"12\" r=\"2\" transform=\"rotate(-90 12 12)\" fill=\"white\"></circle><circle cx=\"7\" cy=\"7\" r=\"2\" transform=\"rotate(-90 7 7)\" fill=\"white\"></circle><circle cx=\"2\" cy=\"2\" r=\"2\" transform=\"rotate(-90 2 2)\" fill=\"white\"></circle></svg>";
    private const string ArrowIcon = "<img src=\"/media/main/icons/hh.png\" width=\"48\" height=\"48\" alt=\"hh.ru\">";
    private const string DefaultIcon = "<svg class=\"icon-block__img\" xmlns=\"http://www.w3.org/2000/svg\" width=\"40\" height=\"40\" viewBox=\"0 0 40 40\" fill=\"none\"><circle cx=\"20\" cy=\"20\" r=\"16\" stroke=\"white\" stroke-width=\"3\"></circle></svg>";
}
