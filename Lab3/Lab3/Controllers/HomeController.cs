using Microsoft.AspNetCore.Mvc;
using Lab3.Models;

namespace Lab3.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult SongForm() => View();

        [HttpPost]
        public IActionResult Sing()
        {
            ViewBag.numOfMonkeys = Request.Form["numOfMonkeys"];
            return View();
        }

        public IActionResult CreateStudent() => View();

        [HttpPost]
        public IActionResult DisplayStudent(Student student)
        {
            if (String.IsNullOrWhiteSpace(student.FirstName) ||
                String.IsNullOrWhiteSpace(student.LastName) ||
                student.StudentId is null ||
                String.IsNullOrWhiteSpace(student.EmailAddress) ||
                String.IsNullOrWhiteSpace(student.Password) ||
                String.IsNullOrWhiteSpace(student.DescriptionOfStudent))
            {
                return Error();
            }
            student = new Student
            {
                FirstName = Request.Form["FirstName"],
                LastName = Request.Form["LastName"],
                EmailAddress = Request.Form["EmailAddress"],
                StudentId = Int32.Parse(Request.Form["StudentId"]),
                Password = Request.Form["Password"],
                DescriptionOfStudent = Request.Form["DescriptionOfStudent"]
            };
            return View(student);
        }

        public IActionResult Error()
        {
            return View("Error");
        }

    }
}
