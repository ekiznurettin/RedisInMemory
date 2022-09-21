using InMemory.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace InMemory.Web.Controllers
{
    public class ProductController : Controller
    {
        private IMemoryCache _memoryCache;

        public ProductController(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public IActionResult Index()
        {
            if (!_memoryCache.TryGetValue("zaman", out string dateCache))
            {
                MemoryCacheEntryOptions options = new MemoryCacheEntryOptions();
                options.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(10);
                options.SlidingExpiration = TimeSpan.FromSeconds(10);

                options.Priority = CacheItemPriority.Normal;

                options.RegisterPostEvictionCallback((key, value, reason, state) =>///bellekten düşen verinin sebebini verir;
                {
                    _memoryCache.Set("callback", $"{key}->{value}=>Sebep : {reason}");
                });

                _memoryCache.Set<string>("zaman", DateTime.Now.ToShortDateString(), options);
            }


            Product product = new Product() { Id = 1, Name = "Kalem", Price = 200 };

            _memoryCache.Set<Product>("product:1", product);


            return View();
        }

        public IActionResult Show()
        {
            _memoryCache.TryGetValue("zaman", out string dateCache);
            _memoryCache.TryGetValue("callback", out string callback);
            ViewBag.Date = dateCache;
            ViewBag.Callback = callback;
            ViewBag.Product = _memoryCache.Get<Product>("product:1");
            return View();
        }
    }
}
