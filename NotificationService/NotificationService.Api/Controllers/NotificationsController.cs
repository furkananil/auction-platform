namespace NotificationService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using MassTransit;
using Shared.Contracts;

[ApiController]
[Route("api/[controller]")]
public class NotificationsController : ControllerBase
{
    private readonly IPublishEndpoint _publishEndpoint;

    public NotificationsController(IPublishEndpoint publishEndpoint)
    {
        _publishEndpoint = publishEndpoint;
    }

    [HttpPost("send-email")]
    public async Task<IActionResult> SendEmail([FromBody] EmailRequest request)
    {
        await _publishEndpoint.Publish(new SendEmailCommand(
            request.To,
            request.Subject,
            request.Body));

        return Ok("Mesaj başarıyla kuyruğa iletildi");
    }
}

public record EmailRequest(string To, string Subject, string Body);