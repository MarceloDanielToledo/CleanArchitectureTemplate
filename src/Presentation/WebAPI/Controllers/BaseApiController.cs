using Application.Wrappers;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers
{
    [ApiController]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ProducesResponseType<Response<object>>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<Response<object>>(StatusCodes.Status500InternalServerError)]
    public class BaseApiController : ControllerBase
    {
    }
}
