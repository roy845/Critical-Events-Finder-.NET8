using Critical_Events_Finder_Api.Interfaces;
using Critical_Events_Finder_Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Critical_Events_Finder_Api.Controllers
{
    [ApiController]
    [Route("api/critical-events")]
    public class CriticalEventsController : ControllerBase
    {
        private readonly ICriticalEventsService _criticalEventsService;
        public CriticalEventsController(ICriticalEventsService criticalEventsService)
        {
            _criticalEventsService = criticalEventsService;
        }
        [HttpPost]
        public ActionResult<CriticalEventsResponse> IdentifyCriticalEvents([FromBody] DaysListRequest request)
        {
            var (criticalEvents, statusCode, message) = _criticalEventsService.FindCriticalEvents(request.days_list);

            if (statusCode == StatusCodes.Status404NotFound)
                return NotFound(new CriticalEventsResponse { status_code = statusCode, message = message });

            return Ok(new CriticalEventsResponse { critical_events = criticalEvents, status_code = statusCode, message = message });
        }
    }
}
