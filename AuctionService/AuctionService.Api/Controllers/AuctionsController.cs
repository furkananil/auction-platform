namespace AuctionService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using AuctionService.Application.Features.Auctions.Commands.CreateAuction;

[ApiController]
[Route("api/[controller]")]
public class AuctionsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuctionsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAuctionCommand command)
    {
        var result = await _mediator.Send(command);
        return Ok(result);
    }
}