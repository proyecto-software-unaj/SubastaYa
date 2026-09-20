using Application.Common;
using Application.DTOs.Auctions;
using Application.Interfaces;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class AuctionService : IAuctionService
    {
        private readonly AppDbContext _context;
        private readonly AuditService _audit;

        public AuctionService(AppDbContext context, AuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        public async Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsAsync(
            string? status, int? categoryId, decimal? minPrice, decimal? maxPrice,
            string? sort, int page, int pageSize, CancellationToken ct = default)
        {
            var query = _context.Auctions
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Bids)
                .AsQueryable();

            
            if (!string.IsNullOrWhiteSpace(status) &&
                Enum.TryParse<AuctionStatus>(status, ignoreCase: true, out var parsedStatus))
            {
                query = query.Where(a => a.Status == parsedStatus);
            }

            if (categoryId.HasValue)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            
            if (minPrice.HasValue)
            {
                query = query.Where(a => a.BasePrice >= minPrice.Value);
            }
            if (maxPrice.HasValue)
            {
                query = query.Where(a => a.BasePrice <= maxPrice.Value);
            }

            
            query = sort switch
            {
                "endingSoon" => query.OrderBy(a => a.EndDate),
                "highestBid" => query.OrderByDescending(a => a.Bids.Max(b => (decimal?)b.Amount) ?? a.BasePrice),
                _ => query.OrderByDescending(a => a.StartDate)
            };

            
            var skip = (page - 1) * pageSize;

            var items = await query
                .Skip(skip)
                .Take(pageSize)
                .Select(a => new AuctionListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImageUrl = a.ImageUrl,
                    CategoryName = a.Category.Name,
                    CurrentHighestBid = a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice,
                    BidCount = a.Bids.Count,
                    EndDate = a.EndDate,
                    Status = a.Status.ToString()
                })
                .ToListAsync(ct);

            return items;
        }

        public async Task<Result<AuctionDetailDto>> GetAuctionByIdAsync(int id, CancellationToken ct = default)
        {
            var auction = await _context.Auctions
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Bids)
                .FirstOrDefaultAsync(a => a.Id == id, ct);

            if (auction is null)
            {
                return Result<AuctionDetailDto>.Failure(ErrorType.NotFound, "La subasta no existe.");
            }

            var highestBid = auction.Bids
                .OrderByDescending(b => b.Amount)
                .FirstOrDefault();

            var currentHighest = highestBid?.Amount ?? auction.BasePrice;

            var dto = new AuctionDetailDto
            {
                Id = auction.Id,
                Title = auction.Title,
                Description = auction.Description,
                ImageUrl = auction.ImageUrl,
                CategoryName = auction.Category.Name,
                BasePrice = auction.BasePrice,
                MinimumIncrement = auction.MinimumIncrement,
                CurrentHighestBid = currentHighest,
                CurrentWinnerId = highestBid?.UserId,
                StartDate = auction.StartDate,
                EndDate = auction.EndDate,
                Status = auction.Status.ToString(),
                SuggestedNextBid = highestBid is null
                    ? auction.BasePrice
                    : currentHighest + auction.MinimumIncrement,
                RecentBids = auction.Bids
                    .OrderByDescending(b => b.BidDate)
                    .Take(10)
                    .Select(b => new BidDto
                    {
                        Id = b.Id,
                        Amount = b.Amount,
                        BidDate = b.BidDate,
                        BidderAlias = "User#" + b.UserId
                    })
                    .ToList()
            };

            return Result<AuctionDetailDto>.Success(dto);
        }

        public async Task<Result<AuctionDetailDto>> CreateAuctionAsync(
            int sellerId, CreateAuctionRequest request, CancellationToken ct = default)
        {
           
            if (request.EndDate <= request.StartDate)
            {
                return Result<AuctionDetailDto>.Failure(
                    ErrorType.Validation, "La fecha de fin debe ser posterior a la de inicio.");
            }

            if (request.BasePrice <= 0 || request.MinimumIncrement <= 0)
            {
                return Result<AuctionDetailDto>.Failure(
                    ErrorType.Validation, "El precio base y el incremento mínimo deben ser positivos.");
            }

            var sellerExists = await _context.Users.AnyAsync(u => u.Id == sellerId, ct);
            if (!sellerExists)
            {
                return Result<AuctionDetailDto>.Failure(ErrorType.NotFound, "El vendedor no existe.");
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.Id == request.CategoryId, ct);
            if (!categoryExists)
            {
                return Result<AuctionDetailDto>.Failure(ErrorType.Validation, "La categoría no existe.");
            }

            
            var now = DateTime.UtcNow;
            var status = request.StartDate > now ? AuctionStatus.Scheduled : AuctionStatus.Active;

            var auction = new Auction
            {
                SellerId = sellerId,
                CategoryId = request.CategoryId,
                Title = request.Title,
                Description = request.Description,
                ImageUrl = request.ImageUrl,
                BasePrice = request.BasePrice,
                MinimumIncrement = request.MinimumIncrement,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                Status = status
            };

            _context.Auctions.Add(auction);
            _audit.Register("Auction", 0, "AuctionCreated", sellerId, new { request.Title });
            await _context.SaveChangesAsync(ct);

            return await GetAuctionByIdAsync(auction.Id, ct);
        }

        public async Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsBySellerAsync(
            int sellerId, CancellationToken ct = default)
        {
            return await _context.Auctions
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Bids)
                .Where(a => a.SellerId == sellerId)
                .OrderByDescending(a => a.StartDate)
                .Select(a => new AuctionListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImageUrl = a.ImageUrl,
                    CategoryName = a.Category.Name,
                    CurrentHighestBid = a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice,
                    BidCount = a.Bids.Count,
                    EndDate = a.EndDate,
                    Status = a.Status.ToString()
                })
                .ToListAsync(ct);
        }

        public async Task<IReadOnlyList<AuctionListItemDto>> GetAuctionsWithUserBidsAsync(
            int userId, CancellationToken ct = default)
        {
           
            return await _context.Auctions
                .AsNoTracking()
                .Include(a => a.Category)
                .Include(a => a.Bids)
                .Where(a => a.Bids.Any(b => b.UserId == userId))
                .OrderByDescending(a => a.StartDate)
                .Select(a => new AuctionListItemDto
                {
                    Id = a.Id,
                    Title = a.Title,
                    ImageUrl = a.ImageUrl,
                    CategoryName = a.Category.Name,
                    CurrentHighestBid = a.Bids.Any() ? a.Bids.Max(b => b.Amount) : a.BasePrice,
                    BidCount = a.Bids.Count,
                    EndDate = a.EndDate,
                    Status = a.Status.ToString()
                })
                .ToListAsync(ct);
        }

    }
}
