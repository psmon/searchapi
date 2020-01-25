using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nest;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IndexController : ControllerBase
    {
        private readonly SearchIndex _searchIndex;
        private readonly IElasticClient _elasticClient;

        public IndexController(SearchIndex searchIndex, IElasticClient elasticClient)
        {
            _searchIndex = searchIndex;
            _elasticClient = elasticClient;
        }

        [HttpPost("update/price")]
        public async Task<int> UpdatePrice(int no,int newprice)
        {
            var updateItem = await _searchIndex.FindById(no);
            updateItem.price = newprice;
            await _searchIndex.UpdateItem(updateItem);
            return 0;
        }

        [HttpPut("reindex")]
        public async Task<IActionResult> ReIndex()
        {
            var count = await _searchIndex.ReindexAll();
            return Ok($"{count} post(s) reindexed");
        }
    }
}