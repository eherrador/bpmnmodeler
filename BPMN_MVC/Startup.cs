using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BPMN_MVC.Startup))]
namespace BPMN_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
