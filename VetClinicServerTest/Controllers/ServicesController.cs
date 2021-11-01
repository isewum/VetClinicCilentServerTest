using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class ServicesController : GenericControllerBase<Service>
    {
        public ServicesController(ClinicContext context) : base(context) { }
    }
}