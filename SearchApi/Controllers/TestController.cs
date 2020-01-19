using Microsoft.AspNetCore.Mvc;
using Nest;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly IElasticClient _elasticClient;

        public TestController(IElasticClient elasticClient)
        {            
            _elasticClient = elasticClient;
        }     
    }
}
