using CoreXF.Abstractions;
using Extension3.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Extension3.ViewComponents
{
    //[Export]
    public class MenuItemsViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(int parentId, string email)
        {
            var response = await new MenuServices().GetModulesItemsForUserAsync(parentId, email);
            return View(response);
        }
    }
}