using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MyTaskList.Startup))]
namespace MyTaskList
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
