using Etiket.Api.Entity;
using Etiket.Api.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Etiket.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KumasTopController : ControllerBase
    {
        private readonly KumasRepository _kumasrepository;

        public KumasTopController(KumasRepository kumasrepository)
        {
            _kumasrepository = kumasrepository;
        }
        [HttpGet("{topNo}")]
        public async Task<ActionResult<IEnumerable<KumasTop>>> Get(string topNo)
        {
            var result = await _kumasrepository.GetKumasTopAsync(topNo);
            return Ok(result);
        }
    }
}
