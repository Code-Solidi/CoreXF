using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using CoreXF.Abstractions.Attributes;

using Extension3.Models;

using Microsoft.AspNetCore.Mvc;

namespace Extension3.ViewComponents
{
    [Export]
    public class PriorityListViewComponent : ViewComponent
    {
        private readonly IList<TodoItem> items = new List<TodoItem> {
            new TodoItem { IsDone = false, Priority = 1, Name = "Task1" },
            new TodoItem { IsDone = false, Priority = 2, Name = "Task2" },
            new TodoItem { IsDone = true, Priority = 3, Name = "Task3" },
            new TodoItem { IsDone = true, Priority = 4, Name = "Task4" },
            new TodoItem { IsDone = false, Priority = 3, Name = "Task5" },
            new TodoItem { IsDone = true, Priority = 4, Name = "Task6" }
        };

        public PriorityListViewComponent(/*inject task source (service) here*/)
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(int maxPriority, bool isDone)
        {
            var todos = this.GetItemsAsync(maxPriority, isDone);
            return await Task.Run<IViewComponentResult>(() => this.View(todos));
        }

        private List<TodoItem> GetItemsAsync(int maxPriority, bool isDone)
        {
            return this.items.Where(x => x.IsDone == isDone && x.Priority <= maxPriority).ToList();
        }
    }
}