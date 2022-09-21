using Microsoft.AspNetCore.Mvc;
using RedisExchange.Web.Services;
using StackExchange.Redis;

namespace RedisExchange.Web.Controllers
{
    public class StringTypesController : Controller
    {
        private readonly RedisService _redisService;
        private readonly IDatabase db;
        public StringTypesController(RedisService redisService)
        {
            _redisService = redisService;
            db = _redisService.GetDb(0);
        }
        [HttpGet]
        public IActionResult Index()
        {        
            db.StringSet("name", "Nurettin EKİZ");
            db.StringSet("ziyaretci",100);
            return View();
        }
        [HttpGet]
        public IActionResult Show()
        {
            var value = db.StringGet("name");
            db.StringIncrement("ziyaretci", 1);

            var count = db.StringDecrementAsync("ziyaretci", 1).Result;

            if (value.HasValue)
            {
                ViewBag.Value = value + " " + count;
            }
            return View();
        }
    }
}
