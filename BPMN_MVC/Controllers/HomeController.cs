using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BPMN_MVC.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult AbrirDiagrama()
        {
            ViewBag.Message = "Modificar Diagrama";
            ViewBag.BPMNDiagramURL = "https://cdn.rawgit.com/bpmn-io/bpmn-js-examples/dfceecba/starter/diagram.bpmn";

            return View();
        }

        [Authorize]
        public ActionResult NuevoDiagrama()
        {
            ViewBag.Message = "Nuevo Diagrama";
            ViewBag.BPMNDiagramURL = "https://raw.githubusercontent.com/bayroxyz/bayroxyz.github.io/master/assets/diagram1.bpmn";

            return View();
        }

        [HttpPost]
        //[Authorize]
        public ActionResult NuevoDiagrama(string fileName)
        {
            if (ModelState.IsValid)
            {
                /* var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await UserManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                    return RedirectToAction("Index", "Home");
                }*/
            }

            // If we got this far, something failed, redisplay form
            //return View(model);
            return RedirectToAction("Index", "Home");
        }
    }
}