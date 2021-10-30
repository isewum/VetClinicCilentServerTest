using VetClinicServerTest.Models;

namespace VetClinicServerTest.Controllers
{
    public class AnimalsController : GenericControllerBase<Animal>
    {
        public AnimalsController(ClinicContext context) : base(context) { }
    }
}