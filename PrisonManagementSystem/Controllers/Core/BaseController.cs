using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Server.IIS.Core;
using PrisonManagementSystem.BL.DTOs.ResponseModel;

namespace PrisonManagementSystem.API.Controllers.Base
{
    [ApiController]
 
    public class BaseController : ControllerBase
    {
        protected ActionResult CreateResponse<T>(GenericResponseModel<T> response)
        {
            return StatusCode(response.StatusCode, response);
        }
    }
}
