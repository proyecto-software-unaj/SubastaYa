using Application.Common;
using Application.DTOs.Auctions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuctionService
    {
        Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsAsync(
            string? status, int? categoryId, decimal? minPrice, decimal? maxPrice,
            string? sort, int page, int pageSize, CancellationToken ct = default);

        Task<Result<AuctionDetailDto>> GetAuctionByIdAsync(int id, CancellationToken ct = default);

        Task<Result<AuctionDetailDto>> CreateAuctionAsync(
            int sellerId, CreateAuctionRequest request, CancellationToken ct = default);
        Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsBySellerAsync(int sellerId, CancellationToken ct = default);
        Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsWithUserBidsAsync(int userId, CancellationToken ct = default);

    }
}
