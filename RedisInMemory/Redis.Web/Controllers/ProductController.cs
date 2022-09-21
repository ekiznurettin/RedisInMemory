using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Redis.Web.Models;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Redis.Web.Controllers
{
    public class ProductController : Controller
    {
        private IDistributedCache _distributedCache;

        public ProductController(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<IActionResult> Index2()
        {
            DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();
            cacheOptions.AbsoluteExpiration = DateTime.Now.AddSeconds(30);
            cacheOptions.SlidingExpiration = TimeSpan.FromSeconds(30);
            _distributedCache.SetString("name", "Nurettin", cacheOptions);
            await _distributedCache.SetStringAsync("surname", "EKİZ", cacheOptions);

            return View();
        }

        public async Task<IActionResult> Index()
        {
            DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions();

            Product product = new Product() { Id = 1, Name = "Kalem", Price = 200 };

            string jsonProduct = JsonConvert.SerializeObject(product);

            Byte[] byteProduct = Encoding.UTF8.GetBytes(jsonProduct);

            _distributedCache.Set("product:1", byteProduct);///bu şekilde de olur

            await _distributedCache.SetStringAsync("product:1", jsonProduct, cacheOptions);//bu şekildede

            return View();
        }
        public IActionResult Show2()
        {
            string name = _distributedCache.GetString("name");
            string surname = _distributedCache.GetString("surname");
            ViewBag.Name = name + " " + surname;
            return View();
        }
        public IActionResult Show()
        {
            ///1 yol
            string jsonProduct = _distributedCache.GetString("product:1");
            Product product = JsonConvert.DeserializeObject<Product>(jsonProduct);

            ///2.Yol
            byte[] byteProduct = _distributedCache.Get("poroduct:1");
            string jsonProduct1 = Encoding.UTF8.GetString(byteProduct);
            Product product1 = JsonConvert.DeserializeObject<Product>(jsonProduct1);
            //Yazdırma
            ViewBag.Name = product;
            return View();
        }

        public IActionResult Remove()
        {
            _distributedCache.Remove("name");
            return View();
        }

        public IActionResult ImageShow()
        {
            byte[] resimByte = _distributedCache.Get("resim");

            
            return File(resimByte,"image/jpg");
        }

        public IActionResult ImageCache()
        {
            string path = Path.Combine(Directory.GetCurrentDirectory(),"wwwroot/image/resim1.jpg");
            byte[] imageByte=System.IO.File.ReadAllBytes(path);
            _distributedCache.Set("resim",imageByte);
            return View();
        }
    }
}
