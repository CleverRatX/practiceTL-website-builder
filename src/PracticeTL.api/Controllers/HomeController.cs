using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Models;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

// Отдаёт публичную страницу "/" с подставленными данными блока hero.
// Данные вставляются ДО отправки в браузер, поэтому к запуску build.min.js
// элементы уже в DOM и анимация к ним привязывается.
public class HomeController : ControllerBase
{
    private readonly IHeroService _heroService;
    private readonly IWebHostEnvironment _env;

    public HomeController(IHeroService heroService, IWebHostEnvironment env)
    {
        _heroService = heroService;
        _env = env;
    }

    [HttpGet("/")]
    [HttpGet("/index.html")]
    public async Task<IActionResult> Index()
    {
        // 1. Читаем HTML-шаблон с диска
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "index.html");
        var html = await System.IO.File.ReadAllTextAsync(templatePath);

        // 2. Берём блок hero из базы (с статистикой)
        var hero = await _heroService.GetHeroAsync();

        // 3. Подставляем всё вместо маркеров
        html = html
            .Replace("<!--HERO_TITLE-->", WebUtility.HtmlEncode(hero.Title))
            .Replace("<!--HERO_SUBTITLE-->", MultilineToHtml(hero.Subtitle))
            .Replace("<!--HERO_STAT_PARTS-->", BuildStatParts(hero.Stats))
            .Replace("<!--HERO_STATS-->", BuildStatItems(hero.Stats));

        // 4. Отдаём готовую страницу (charset обязателен для кириллицы)
        return Content(html, "text/html; charset=utf-8");
    }

    // Многострочный текст → строки, разделённые <br>
    private static string MultilineToHtml(string text)
    {
        var lines = text.Split('\n').Select(line => WebUtility.HtmlEncode(line.Trim()));
        return string.Join("<br>", lines);
    }

    // Крупные «бегущие» значения сверху блока
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

    // Список карточек статистики
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
}
