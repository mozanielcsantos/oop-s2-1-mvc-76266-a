using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OopS21Mvc76266A.Web.Data;

namespace oop_s2_1_mvc_76266_a.Web.Controllers
{
    public class MembersController : Controller
    {
        private readonly ApplicationDbContext _db;

        public MembersController(ApplicationDbContext db)
        {
            _db = db;
        }

        // GET: /Members
        public async Task<IActionResult> Index()
        {
            var members = await _db.Members
                .AsNoTracking()
                .OrderBy(m => m.FullName)
                .ToListAsync();

            return View(members);
        }
    }
}