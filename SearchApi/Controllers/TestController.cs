using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchApi.Entity;
using SearchApi.Services;

namespace SearchApi.Controllers
{    
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly SearchIndex _searchIndex;

        private readonly IElasticClient _elasticClient;

        public TestController(SearchIndex searchIndex, IElasticClient elasticClient)
        {
            _searchIndex = searchIndex;
            _elasticClient = elasticClient;
        }

        // GET api/values
        [HttpGet("findbycategory")]
        public ActionResult<object> FindByCategory(string category1)
        {
            var goods = _searchIndex.FilterByCategory(category1);
            return goods;            
        }

        [HttpGet("reindex")]
        public async Task<IActionResult> ReIndex()
        {
            var count = _searchIndex.ReindexAll();
            return Ok($"{count} post(s) reindexed");
        }

    }
}
