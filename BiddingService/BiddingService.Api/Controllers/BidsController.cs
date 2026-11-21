namespace BiddingService.Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using MediatR;
using BiddingService.Application.Features.Bids.Commands.PlaceBid;
using Shared.Contracts;
using Microsoft.AspNetCore.Authorization;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BidsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ICurrentUserService _currentUser;

    public BidsController(IMediator mediator, ICurrentUserService currentUser)
    {
        _mediator = mediator;
        _currentUser = currentUser;
    }

    [HttpPost]
    public async Task<IActionResult> PlaceBid([FromBody] PlaceBidRequest request)
    {
        if (_currentUser.UserId is not { } bidderId)
            return Unauthorized();

        var command = new PlaceBidCommand(
            request.AuctionId,
            request.ProductId,
            bidderId,                  // artık client'tan değil, token'dan
            request.Amount,
            request.IdempotencyKey);

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

public record PlaceBidRequest(
    Guid AuctionId,
    Guid ProductId,
    decimal Amount,
    string IdempotencyKey);