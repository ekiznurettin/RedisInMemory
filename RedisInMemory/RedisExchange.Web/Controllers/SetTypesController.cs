using Microsoft.AspNetCore.Mvc;
using RedisExchange.Web.Services;
using StackExchange.Redis;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisExchange.Web.Controllers
{
    public class SetTypesController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;

        private string listKey = "hashnames";

        public SetTypesController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(2);
        }
        public IActionResult Index()
        {
            HashSet<string> namesList = new HashSet<string>();
            if (db.KeyExists(listKey))
            {
                db.SetMembers(listKey).ToList().ForEach(name => namesList.Add(name));
            }
            return View(namesList);
        }

        [HttpPost]
        public IActionResult Add(string name)
        {
            if (!db.KeyExists(listKey))
            {
                db.KeyExpire(listKey, System.DateTime.Now.AddMinutes(5));
            }

            db.SetAdd(listKey, name);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteItem(string name)
        {
            await db.SetRemoveAsync(listKey, name);
            return RedirectToAction("Index");
        }
    }
}
