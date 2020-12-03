using Extension3.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Extension3.Services
{
    public class MenuServices
    {
        internal async Task<MainMenuModel> GetModulesItemsForUserAsync(int parentId, string email)
        {
            var menu = new MainMenuModel();
            var items = new List<MenuItem>
            {
                new MenuItem { Routing = "/Home/Index",  DisplayText =  "Home" },
                new MenuItem { Routing = "/Home/Contact",  DisplayText =  "Contact" },
                new MenuItem { Routing = "/Home/About",  DisplayText =  "About" },
                new MenuItem { Routing = "/Home/Extras",  DisplayText =  "Extras" }
            };

            menu.MenuItems = items;

            return await Task.Run(() => menu);
        }
    }
}