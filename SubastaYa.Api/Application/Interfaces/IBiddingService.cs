using Application.Common;
using Application.DTOs.Auctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IBiddingService
    {
        Task<Result<BidDto>> PlaceBidAsync(
            int auctionId, int userId, decimal amount, CancellationToken ct = default);

        Task<IReadOnlyList<BidDto>> GetBidsAsync(
            int auctionId, CancellationToken ct = default);
    }
}
