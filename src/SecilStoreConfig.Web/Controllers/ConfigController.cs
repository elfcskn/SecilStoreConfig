using Microsoft.AspNetCore.Mvc;
using SecilStoreConfig.Library.Models;
using SecilStoreConfig.Library.Services;

namespace SecilStoreConfig.Web.Controllers;

public class ConfigController : Controller
{
    private readonly IConfigurationService _service;

    public ConfigController(IConfigurationService service) => _service = service;

    public async Task<IActionResult> Index()
    {
        var items = await _service.GetAllAsync();
        return View(items);
    }

    public IActionResult Create() => View(new ConfigItem { IsActive = true, Type = "string" });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ConfigItem item)
    {
        if (!ModelState.IsValid) return View(item);
        try
        {
            await _service.CreateAsync(item);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(item);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _service.GetByIdAsync(id);
        if (item is null) return NotFound();
        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ConfigItem item)
    {
        if (!ModelState.IsValid) return View(item);
        try
        {
            await _service.UpdateAsync(item);
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(item);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        await _service.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
