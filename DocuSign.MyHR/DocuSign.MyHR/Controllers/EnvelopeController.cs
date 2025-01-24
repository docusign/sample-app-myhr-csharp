using System;
using DocuSign.MyHR.Models;
using DocuSign.MyHR.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DocuSign.MyHR.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class EnvelopeController : Controller
    {
        private readonly IEnvelopeService _envelopeService;

        public EnvelopeController(IEnvelopeService envelopeService)
        {
            _envelopeService = envelopeService;
        }

        [HttpPost]
        public IActionResult Index([FromBody]RequestEnvelopeModel model)
        {
            if (model == null)
            {
                return BadRequest("Invalid model");
            }
            string scheme = Url.ActionContext.HttpContext.Request.Scheme;
            CreateEnvelopeResponse createEnvelopeResponse = _envelopeService.CreateEnvelope(
                model.Type,
                Context.Account.Id,
                Context.User.Id,
                Context.User.LoginType,
                model.AdditionalUser,
                model.RedirectUrl,
                Url.Action("ping", "info", null, scheme));

            return Ok(new ResponseEnvelopeModel
            {
                RedirectUrl = createEnvelopeResponse.RedirectUrl,
                EnvelopeId = createEnvelopeResponse.EnvelopeId
            });
        }

        [HttpGet]
        public IActionResult Index([FromQuery]string envelopeId)
        {
            return Ok(_envelopeService.GetEnvelopData(Context.Account.Id, envelopeId));
        }
    }
}