using Application.DTOs.Auctions;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace SubastaYa.Api.Controllers
{
    public class AuctionsController : ApiControllerBase
    {
        private readonly IAuctionService _auctionService;
        private readonly IBiddingService _biddingService;

        public AuctionsController(IAuctionService auctionService, IBiddingService biddingService)
        {
            _auctionService = auctionService;
            _biddingService = biddingService;
        }

        // GET /api/auctions
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AuctionListItemDto>>> GetAuctions(
            [FromQuery] string? status,
            [FromQuery] int? categoryId,
            [FromQuery] decimal? minPrice,
            [FromQuery] decimal? maxPrice,
            [FromQuery] string? sort,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken ct = default)
        {
            var result = await _auctionService.GetAuctionsAsync(
                status, categoryId, minPrice, maxPrice, sort, page, pageSize, ct);
            return Ok(result);
        }

        // GET /api/auctions/{id}
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuctionDetailDto>> GetAuction(int id, CancellationToken ct)
        {
            var result = await _auctionService.GetAuctionByIdAsync(id, ct);
            return HandleResult(result);
        }

        // POST /api/auctions
        [HttpPost]
        public async Task<ActionResult<AuctionDetailDto>> CreateAuction(
            [FromBody] CreateAuctionRequest request, CancellationToken ct)
        {
            var sellerId = GetUserId();
            var result = await _auctionService.CreateAuctionAsync(sellerId, request, ct);

            if (result.IsSuccess && result.Value is not null)
            {
                return CreatedAtAction(nameof(GetAuction), new { id = result.Value.Id }, result.Value);
            }
            return HandleResult(result);
        }

        // GET /api/auctions/{id}/bids
        [HttpGet("{id:int}/bids")]
        public async Task<ActionResult<IReadOnlyList<BidDto>>> GetBids(int id, CancellationToken ct)
        {
            var result = await _biddingService.GetBidsAsync(id, ct);
            return Ok(result);
        }

        // POST /api/auctions/{id}/bids
        [HttpPost("{id:int}/bids")]
        public async Task<ActionResult<BidDto>> PlaceBid(
            int id, [FromBody] PlaceBidRequest request, CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _biddingService.PlaceBidAsync(id, userId, request.Amount, ct);
            return HandleResult(result);
        }

        // GET /api/auctions/mine  
        [HttpGet("mine")]
        public async Task<ActionResult<IReadOnlyList<AuctionListItemDto>>> GetMyAuctions(CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _auctionService.GetAuctionsBySellerAsync(userId, ct);
            return Ok(result);
        }

        // GET /api/auctions/participating  
        [HttpGet("participating")]
        public async Task<ActionResult<IReadOnlyList<AuctionListItemDto>>> GetParticipating(CancellationToken ct)
        {
            var userId = GetUserId();
            var result = await _auctionService.GetAuctionsWithUserBidsAsync(userId, ct);
            return Ok(result);
        }

    }
}
