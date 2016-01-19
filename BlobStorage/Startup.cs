using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BlobStorage.Startup))]
namespace BlobStorage
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
