using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using BPMN;
using System.Drawing;

namespace BPMN_MVC.Controllers
{
    public class HomeController : Controller
    {
        String dbconnStr = ConfigurationManager.ConnectionStrings["NegentropyConnection"].ConnectionString;

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

        //[Authorize]
        public ActionResult AbrirDiagrama()
        {
            ViewBag.Message = "Modificar Diagrama";
            ViewBag.BPMNDiagramURL = "https://cdn.rawgit.com/bpmn-io/bpmn-js-examples/dfceecba/starter/diagram.bpmn";

            return View();
        }

        //[Authorize]
        public ActionResult NuevoDiagrama()
        {
            ViewBag.Message = "Nuevo Diagrama";
            ViewBag.BPMNDiagramURL = "https://raw.githubusercontent.com/bayroxyz/bayroxyz.github.io/master/assets/diagram1.bpmn";

            return View();
        }

        public ActionResult DiagramaACQP(string nombreProceso= "Desarrollar visión y estrategia", int numeroProceso = 10002)
        {
            ViewBag.Message = "Diagrama ACQP";
            ViewBag.NombreProceso = nombreProceso;
            int idPadre = numeroProceso;

            //SELECT [TRADUCCION] FROM [dbo].[ACQPACTIVIDADES] WHERE [PADRE]='10002'
            string query = "SELECT TRADUCCION FROM ACQPACTIVIDADES WHERE PADRE='" + idPadre + "'";
            List<string> actividades = new List<string>();
            DataSet ds = new DataSet();

            using (SqlConnection sCon = new SqlConnection(dbconnStr))
            {
                using (SqlDataAdapter sAda = new SqlDataAdapter(query, sCon))
                {
                    sAda.Fill(ds);
                    if (ds.Tables["Table"] != null)
                    {
                        foreach (DataRow dr in ds.Tables["Table"].Rows)
                        {
                            actividades.Add((string)dr["TRADUCCION"]);
                        }
                    }
                }
                sCon.Dispose();
            }
            ViewBag.xmlBPMN = ConstruyeDiagramaBPMN(actividades, nombreProceso);

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

        private string ConstruyeDiagramaBPMN(List<string> actividades, string nombreProceso)
        {
            Editor editor = new Editor();
            editor.Create("BPMN Model", "Negentropy");

            //string[] lines = File.ReadAllLines(tareasProceso10002);
            string[] tareas = new string[actividades.Count];
            string[] flujos = new string[actividades.Count + 1];
            int i = 0;

            string idStartEvent = editor.AddEvent(null, null, "Start Event", EventType.Start, EventTrigger.None, EventRole.None);
            foreach (string line in actividades)
            {
                //string sub = line.Substring(line.IndexOf(',') + 1);
                //sub = sub.Substring(0, sub.IndexOf(','));
                Console.WriteLine(line.Trim('"'));
                //tareas[i++] = sub;
                tareas[i] = editor.AddActivity(null, line.Trim('"'), ActivityType.Task, ActivityMarker.None, TaskType.None, null);

                i++;
            }
            string idEndEvent = editor.AddEvent(null, null, "End Event", EventType.End, EventTrigger.None, EventRole.None);

            Console.WriteLine("\ni = " + i);

            int f = 0;
            for (f = 0; f < actividades.Count; f++)
            {
                if (f == 0)
                    flujos[f] = editor.AddFlow(null, null, idStartEvent, tareas[f], null, FlowType.Sequence, null, false, FlowDirection.None);
                else
                    flujos[f] = editor.AddFlow(null, null, tareas[f - 1], tareas[f], null, FlowType.Sequence, null, false, FlowDirection.None);
            }
            flujos[f] = editor.AddFlow(null, null, tareas[f - 1], idEndEvent, null, FlowType.Sequence, null, false, FlowDirection.None);

            string id = editor.AddDiagram("Diagrama de Procesos QPAC", 96);

            Shape shape = new Shape();
            Rectangle rect = new Rectangle(60, 50, 80, 70);  //Rectangulos para las tareas
            Rectangle rectSE = new Rectangle(120, 70, 30, 30);  //Rectangulos para Start y End Events
            shape.Bounds = new List<Rectangle>();

            shape.Bounds.Add(rectSE);
            shape.ElementRef = idStartEvent;
            editor.AddShape(id, shape);
            //rect.Width = 100; //70;
            rect.Offset(120, 0);
            //shape.Bounds.Add(rect);
            shape.Bounds[0] = rect;

            int offset = 1;
            foreach (string tarea in tareas)
            {
                shape.ElementRef = tarea;
                editor.AddShape(id, shape);
                rect.Offset(120, 0);
                shape.Bounds[0] = rect;
                offset++;
            }

            //shape.Bounds.Add(rectSEE);
            Rectangle rectEE = new Rectangle((tareas.Length * 80) + (flujos.Length * 40) + 140, 70, 30, 30);
            shape.Bounds[0] = rectEE;
            shape.ElementRef = idEndEvent;
            editor.AddShape(id, shape);

            List<Point> points = new List<Point>();
            points.Add(new Point()); points.Add(new Point());
            int x1 = 150;
            int x2 = 180;

            foreach (string flujo in flujos)
            {
                points[0] = new Point(x1, 85); points[1] = new Point(x2, 85);
                Edge edge = new Edge() { ElementRef = flujo, Points = points };
                editor.AddEdge(id, edge);

                x1 = x1 + 110;
                x2 = x2 + 120;
            }
            return editor.Serialize();
            //editor.Save("diagrama.bpmn"); //(nombreProceso.Trim(' ').ToLower() + ".bpmn");
        }
    }
}