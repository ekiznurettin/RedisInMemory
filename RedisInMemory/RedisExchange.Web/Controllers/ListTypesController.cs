using Microsoft.AspNetCore.Mvc;
using RedisExchange.Web.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;

namespace RedisExchange.Web.Controllers
{
    public class ListTypesController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;

        private string listKey = "names";

        public ListTypesController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(1);
        }

        public IActionResult Index()
        {
            List<string> nameList = new List<string>();
            if (db.KeyExists(listKey))
            {
                db.ListRange(listKey).ToList().ForEach(x =>
                {
                    nameList.Add(x);
                });
            }
            return View(nameList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            db.ListRightPush(listKey, name);

            return RedirectToAction("Index");
        }
        public IActionResult DeleteItem(string name)
        {
            db.ListRemoveAsync(listKey, name).Wait();
            return RedirectToAction("Index");
        }

        public IActionResult DeleteFirstItem()
        {
            db.ListLeftPopAsync(listKey).Wait();//ilk elemanı siler
            return RedirectToAction("Index");
        }
    }
}
