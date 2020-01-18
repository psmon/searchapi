using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Models.Filter;
using SearchApi.Models.Result;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly SearchIndex _searchIndex;

        public SearchController(SearchIndex searchIndex)
        {
            _searchIndex = searchIndex;
        }

        [HttpGet("search")]
        public async Task<SearchResult> Search([FromQuery]SearchFilter filterOpt)
        {
            return await _searchIndex.FindByFilter(filterOpt);
        }
    }
}