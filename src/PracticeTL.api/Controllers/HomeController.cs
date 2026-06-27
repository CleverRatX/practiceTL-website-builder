using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using PracticeTL.Api.Models;
using PracticeTL.Api.Services;

namespace PracticeTL.Api.Controllers;

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
        // 1. Читаем HTML-шаблон(папка Templates) с диска 
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "index.html");
        var html = await System.IO.File.ReadAllTextAsync(templatePath);

        // 2. Берём данные блока hero из базы
        var items = await _heroService.GetAllAsync();

        // 3. Строим HTML для двух мест на странице и подставляем вместо маркеров
        html = html
            .Replace("<!--HERO_TITLE_PARTS-->", BuildTitleParts(items))
            .Replace("<!--HERO_ITEMS-->", BuildListItems(items));

        // 4. Отдаём готовую страницу
        return Content(html, "text/html; charset=utf-8");
    }

    // advantages__title-part
    private static string BuildTitleParts(List<HeroItem> items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            if (item.Type == "img")
            {
                var src = WebUtility.HtmlEncode(item.Value);
                sb.Append($"<span class=\"advantages__title-part advantages__title-part--icon\"><img src=\"{src}\" alt=\"tl-logo\" width=\"80\" height=\"80\"></span>");
            }
            else
            {
                var value = WebUtility.HtmlEncode(item.Value);
                sb.Append($"<span class=\"advantages__title-part\">{value}</span>");
            }
        }
        return sb.ToString();
    }

    // advantages__item
    private static string BuildListItems(List<HeroItem> items)
    {
        var sb = new StringBuilder();
        foreach (var item in items)
        {
            var label = WebUtility.HtmlEncode(item.Label);

            string titleInner;
            if (item.Type == "img")
            {
                var src = WebUtility.HtmlEncode(item.Value);
                titleInner = $"<img src=\"{src}\" alt=\"tl-logo\" width=\"80\" height=\"80\">";
            }
            else
            {
                titleInner = WebUtility.HtmlEncode(item.Value);
            }

            sb.Append($"<li class=\"advantages__item\"><span class=\"advantages__item-title\">{titleInner}</span> {label}</li>");
        }
        return sb.ToString();
    }
}
