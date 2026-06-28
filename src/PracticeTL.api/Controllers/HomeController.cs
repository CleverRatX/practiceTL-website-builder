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
    private readonly IWebHostEnvironment _env;

    public HomeController(IHeroService heroService, ITeamService teamService, IWebHostEnvironment env)
    {
        _heroService = heroService;
        _teamService = teamService;
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

        html = html
            .Replace("<!--HERO_TITLE-->", WebUtility.HtmlEncode(hero.Title))
            .Replace("<!--HERO_SUBTITLE-->", MultilineToHtml(hero.Subtitle))
            .Replace("<!--HERO_STAT_PARTS-->", BuildStatParts(hero.Stats))
            .Replace("<!--HERO_STATS-->", BuildStatItems(hero.Stats))
            .Replace("<!--TEAM_ITEMS-->", BuildTeamCards(team));

        return Content(html, "text/html; charset=utf-8");
    }

    private static string MultilineToHtml(string text)
    {
        var lines = text.Split('\n').Select(line => WebUtility.HtmlEncode(line.Trim()));
        return string.Join("<br>", lines);
    }

    private static string BuildStatParts(List<HeroStat> stats)
    {
        var sb = new StringBuilder();
        foreach (var s in stats)
        {
            if (s.Type == "img")
            {
                var src = WebUtility.HtmlEncode(s.Value);
                sb.Append($"<span class=\"advantages__title-part advantages__title-part--icon\"><img src=\"{src}\" alt=\"tl-logo\" width=\"80\" height=\"80\"></span>");
            }
            else
            {
                sb.Append($"<span class=\"advantages__title-part\">{WebUtility.HtmlEncode(s.Value)}</span>");
            }
        }
        return sb.ToString();
    }

    private static string BuildStatItems(List<HeroStat> stats)
    {
        var sb = new StringBuilder();
        foreach (var s in stats)
        {
            var label = WebUtility.HtmlEncode(s.Label);
            string inner;
            if (s.Type == "img")
            {
                var src = WebUtility.HtmlEncode(s.Value);
                inner = $"<img src=\"{src}\" alt=\"tl-logo\" width=\"80\" height=\"80\">";
            }
            else
            {
                inner = WebUtility.HtmlEncode(s.Value);
            }
            sb.Append($"<li class=\"advantages__item\"><span class=\"advantages__item-title\">{inner}</span> {label}</li>");
        }
        return sb.ToString();
    }

    private static string BuildTeamCards(List<TeamMember> members)
    {
        var sb = new StringBuilder();
        foreach (var m in members)
        {
            var photo = WebUtility.HtmlEncode(m.Photo);
            var name = WebUtility.HtmlEncode(m.Name);
            var position = WebUtility.HtmlEncode(m.Position);

            sb.Append($@"
            <article class=""swiper-slide team__item"">
                <div class=""team__item-card card card--rounded"">
                    <picture>
                        <img src=""{photo}"" class=""team__item-image card__image"" alt=""{name}"" width=""336"" height=""477"" decoding=""async"" loading=""lazy"">
                    </picture>
                    <div class=""card__description"">
                        <h3 class=""team__item-title"">{name}</h3>
                        <p class=""team__item-description"">{position}</p>
                    </div>
                </div>
                <p class=""team__item-text""></p>
            </article>");
        }
        return sb.ToString();
    }

    private const string Icon1 = "<svg width=\"32\" height=\"32\" viewBox=\"0 0 32 32\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M29.7504 22.2497C29.6752 22.1237 29.2096 21.1121 26.9695 19.033C24.6243 16.8558 24.9393 17.2093 27.764 13.4448C29.4844 11.1521 30.1722 9.75203 29.9569 9.15348C29.7522 8.58294 28.4868 8.73345 28.4868 8.73345L24.2795 8.75795C24.2795 8.75795 23.968 8.71595 23.7352 8.85421C23.5095 8.99072 23.3642 9.30574 23.3642 9.30574C23.3642 9.30574 22.6974 11.0804 21.8083 12.589C19.934 15.7725 19.1832 15.9405 18.8769 15.7427C18.1646 15.2824 18.3431 13.8911 18.3431 12.904C18.3431 9.81853 18.8104 8.53219 17.4313 8.19966C16.9727 8.0894 16.6367 8.0159 15.4659 8.00365C13.9643 7.9879 12.6919 8.0089 11.9726 8.36067C11.4931 8.59519 11.1238 9.11848 11.3496 9.14823C11.6278 9.18499 12.2579 9.318 12.5922 9.77303C13.0245 10.3593 13.0087 11.6789 13.0087 11.6789C13.0087 11.6789 13.2572 15.3104 12.4294 15.762C11.8606 16.0717 11.0818 15.4399 9.41043 12.5505C8.55462 11.0716 7.90707 9.43525 7.90707 9.43525C7.90707 9.43525 7.78282 9.13073 7.56055 8.96797C7.29103 8.77021 6.913 8.7072 6.913 8.7072L2.91221 8.7317C2.91221 8.7317 2.31191 8.7492 2.0914 9.00997C1.89538 9.24274 2.07565 9.72227 2.07565 9.72227C2.07565 9.72227 5.20838 17.0501 8.75414 20.7446C12.0076 24.1311 15.7004 23.9088 15.7004 23.9088H17.3735C17.3735 23.9088 17.8793 23.8528 18.1366 23.5745C18.3746 23.319 18.3658 22.8395 18.3658 22.8395C18.3658 22.8395 18.3326 20.5941 19.3757 20.2633C20.403 19.9378 21.7226 22.4335 23.1209 23.3943C24.178 24.1206 24.9813 23.9613 24.9813 23.9613L28.7214 23.9088C28.7214 23.9088 30.678 23.7881 29.7504 22.2497Z\" fill=\"white\"></path></svg>";

    private const string Icon2 = "<svg width=\"32\" height=\"32\" viewBox=\"0 0 32 32\" fill=\"none\" xmlns=\"http://www.w3.org/2000/svg\"><path d=\"M26.4016 6.10044L5.1175 14.3069C3.65871 14.8771 3.68446 15.6847 4.86732 16.06L10.3162 17.7598L22.956 9.8017C23.5429 9.41171 24.0874 9.63246 23.6422 10.0243L13.4141 19.2646L13.0222 24.8809C13.5925 24.8809 13.8409 24.6325 14.1352 24.3382L16.7971 21.7738L22.3159 25.8411C23.3221 26.4114 24.0414 26.1171 24.31 24.9066L27.934 7.83334L27.9321 7.83518C28.2522 6.34143 27.3895 5.74356 26.4016 6.10044Z\" fill=\"white\"></path></svg>";
}
