using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class OwnersController : GenericControllerBase<Owner>
    {
        public OwnersController(ClinicContext context) : base(context) { }
    }
}