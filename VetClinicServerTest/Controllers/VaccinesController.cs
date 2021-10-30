using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class VaccinesController : GenericControllerBase<Vaccine>
    {
        public VaccinesController(ClinicContext context) : base(context) { }
    }
}