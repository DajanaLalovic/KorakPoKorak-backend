using System.Security.Claims;
using KorakPoKorak.Application.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KorakPoKorak.API.Controllers
{
    [ApiController]
    [Route("api/child/{childId:int}")]
    [Authorize(Roles = "Parent")]
    public class ChildAwardController : ControllerBase
    {
        private readonly IWorkshopAwardService _service;

        public ChildAwardController(IWorkshopAwardService service)
        {
            _service = service;
        }

        /// <summary>Returns all badges earned by the Parent's child.</summary>
        [HttpGet("badges")]
        public IActionResult GetBadges(int childId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.GetBadgesForChild(childId, parentId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Returns all certificates earned by the Parent's child.</summary>
        [HttpGet("certificates")]
        public IActionResult GetCertificates(int childId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                return Ok(_service.GetCertificatesForChild(childId, parentId));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }

        /// <summary>Returns one certificate award for preview.</summary>
        [HttpGet("certificates/{certificateAwardId:int}")]
        public IActionResult GetCertificate(int childId, int certificateAwardId)
        {
            var parentId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            try
            {
                var certificate = _service.GetCertificateById(certificateAwardId, childId, parentId);
                return certificate == null ? NotFound() : Ok(certificate);
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
        }
    }
}
