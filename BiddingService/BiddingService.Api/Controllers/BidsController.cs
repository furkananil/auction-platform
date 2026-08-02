namespace BiddingService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using BiddingService.Application.Features.Bids.Commands.PlaceBid;

[ApiController]
[Route("api/[controller]")]
public class BidsController : ControllerBase
{
    private readonly IMediator _mediator;

    public BidsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceBid([FromBody] PlaceBidCommand command)
    {
        try
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
    }
}