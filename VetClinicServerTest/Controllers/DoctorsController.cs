using VetClinicModelLibTest;
using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class DoctorsController : GenericControllerBase<Doctor>
    {
        public DoctorsController(ClinicContext context) : base(context) { }
    }
}