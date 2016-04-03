using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SecretShareProject.Startup))]
namespace SecretShareProject
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
