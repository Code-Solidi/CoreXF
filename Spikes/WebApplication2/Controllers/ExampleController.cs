using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using System.Collections;
using System.Collections.Generic;
using System.Linq;

using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    public class ExampleController : Controller
    {
        // GET: ExampleController
        public ActionResult Index()
        {
            var data = ExampleData.Get();
            return View(data);
        }

        // GET: ExampleController/Details/5
        public ActionResult Details(int id)
        {
            var model = ExampleData.Get(id);
            return View(model);
        }

        // GET: ExampleController/Create
        [Route("[controller]/add")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: ExampleController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("[controller]/add")]
        public ActionResult Create(ExampleModel example)
        {
            try
            {
                var data = ExampleData.Get();
                data.Add(example);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExampleController/Edit/5
        public ActionResult Edit(int id)
        {
            var model = ExampleData.Get(id);
            return View(model);
        }

        // POST: ExampleController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, ExampleModel example)
        {
            try
            {
                var model = ExampleData.Get(id);
                model.Name = example.Name;
                model.Description = example.Description;
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ExampleController/Delete/5
        public ActionResult Delete(int id)
        {
            var model = ExampleData.Get(id);
            return View(model);
        }

        // POST: ExampleController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                var item = ExampleData.Get(id);
                ExampleData.Get().Remove(item);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
