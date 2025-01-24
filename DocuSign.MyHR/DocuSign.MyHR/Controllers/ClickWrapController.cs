using System;
using DocuSign.MyHR.Models;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace DocuSign.MyHR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class ClickWrapController : Controller
    {
        private readonly IClickWrapService _clickWrapService;

        public ClickWrapController(IClickWrapService clickWrapService)
        {
            _clickWrapService = clickWrapService;
        }

        [HttpPost]
        public IActionResult Index([FromBody] RequestClickWrapModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }
            var response = _clickWrapService.CreateTimeTrackClickWrap(Context.Account.Id, Context.User.Id, model.WorkLogs);

            return Ok(
                new ResponseClickWrapModel
                {
                    ClickWrap = JsonConvert.DeserializeObject<dynamic>(response.Content.ReadAsStringAsync().Result),
                    DocuSignBaseUrl = Context.Account.BaseUri
                });
        }
    }
}