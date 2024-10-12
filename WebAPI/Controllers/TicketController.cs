using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.Services;
using static QRCoder.PayloadGenerator;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class TicketController : ControllerBase
{
	private readonly QRService _QRService;
	private readonly IEmailService _EmailService;

	public TicketController(QRService qrService, IEmailService emailService)
	{
		_QRService = qrService;
		_EmailService = emailService;
	}


	[HttpPost]
	[Route("Main")]
	public async Task<IActionResult> Main(string buyerEmail, string subject, string body)
	{
		if (!ModelState.IsValid) { return BadRequest("Model Invalid"); }

		// Collect info

		// Create QR code
		Guid guid = _QRService.GenerateGuid();
		var qrCodeData = _QRService.GenerateQRCode(guid.ToString());

		// Save booking

		// Send email
		await _EmailService.SendAsync("TicketRhino", buyerEmail, subject, body, qrCodeData);

		return Ok();
	}
}
