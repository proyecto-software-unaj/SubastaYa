using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;
using Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (await context.Users.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;
      
            var tech = new Category { Name = "Tecnología", IconUrl = "" };
            var collectibles = new Category { Name = "Coleccionables", IconUrl = "" };
            var clothing = new Category { Name = "Indumentaria", IconUrl = "" };
            var vehicles = new Category { Name = "Vehículos", IconUrl = "" };

            context.Categories.AddRange(tech, collectibles, clothing, vehicles);

            var seller = new User
            {
                Email = "vendedor@test.com",
                Name = "Vendedor",
                PasswordHash = "seed",
                RegisteredAt = now,
                Wallet = new Wallet { TotalBalance = 0m, HeldBalance = 0m }
            };

            var buyer1 = new User
            {
                Email = "comprador1@test.com",
                Name = "Comprador Uno",
                PasswordHash = "seed",
                RegisteredAt = now,
                
                Wallet = new Wallet { TotalBalance = 150_000m, HeldBalance = 45_000m }
            };

            var buyer2 = new User
            {
                Email = "comprador2@test.com",
                Name = "Comprador Dos",
                PasswordHash = "seed",
                RegisteredAt = now,
                Wallet = new Wallet { TotalBalance = 200_000m, HeldBalance = 8_500m }
            };

            var noFunds = new User
            {
                Email = "sinfondos@test.com",
                Name = "Sin Fondos",
                PasswordHash = "seed",
                RegisteredAt = now,
                Wallet = new Wallet { TotalBalance = 500m, HeldBalance = 0m }
            };

            context.Users.AddRange(seller, buyer1, buyer2, noFunds);

            await context.SaveChangesAsync();

            var standardActive = new Auction
            {
                SellerId = seller.Id,
                CategoryId = tech.Id,
                Title = "Notebook Gamer",
                Description = "Notebook para juegos de alto rendimiento.",
                ImageUrl = "https://picsum.photos/seed/notebook/400/300",
                BasePrice = 40_000m,
                MinimumIncrement = 1_000m,
                StartDate = now.AddHours(-1),
                EndDate = now.AddMinutes(25),
                Status = AuctionStatus.Active
            };

            var criticalActive = new Auction
            {
                SellerId = seller.Id,
                CategoryId = collectibles.Id,
                Title = "Figura de colección edición limitada",
                Description = "Pieza rara, pocas unidades en el mundo.",
                ImageUrl = "https://picsum.photos/seed/figure/400/300",
                BasePrice = 10_000m,
                MinimumIncrement = 500m,
                StartDate = now.AddHours(-1),
                EndDate = now.AddSeconds(90),
                Status = AuctionStatus.Active
            };

            var scheduled = new Auction
            {
                SellerId = seller.Id,
                CategoryId = vehicles.Id,
                Title = "Motocicleta clásica",
                Description = "Restaurada, lista para circular.",
                ImageUrl = "https://picsum.photos/seed/moto/400/300",
                BasePrice = 500_000m,
                MinimumIncrement = 10_000m,
                StartDate = now.AddHours(24),
                EndDate = now.AddHours(48),
                Status = AuctionStatus.Scheduled
            };

            var endedWithWinner = new Auction
            {
                SellerId = seller.Id,
                CategoryId = clothing.Id,
                Title = "Campera de cuero vintage",
                Description = "Talle L, excelente estado.",
                ImageUrl = "https://picsum.photos/seed/jacket/400/300",
                BasePrice = 8_000m,
                MinimumIncrement = 500m,
                StartDate = now.AddHours(-3),
                EndDate = now.AddMinutes(-5),
                Status = AuctionStatus.Active 
            };

            var endedDeserted = new Auction
            {
                SellerId = seller.Id,
                CategoryId = tech.Id,
                Title = "Teclado mecánico usado",
                Description = "Funciona, faltan dos teclas.",
                ImageUrl = "https://picsum.photos/seed/keyboard/400/300",
                BasePrice = 3_000m,
                MinimumIncrement = 200m,
                StartDate = now.AddHours(-3),
                EndDate = now.AddMinutes(-10),
                Status = AuctionStatus.Active 
            };

            context.Auctions.AddRange(
                standardActive, criticalActive, scheduled, endedWithWinner, endedDeserted);

            await context.SaveChangesAsync();

            var bid1 = new Bid
            {
                AuctionId = standardActive.Id,
                UserId = buyer2.Id,
                Amount = 44_000m,
                BidDate = now.AddMinutes(-30)
            };

            var bid2 = new Bid
            {
                AuctionId = standardActive.Id,
                UserId = buyer1.Id,
                Amount = 45_000m,
                BidDate = now.AddMinutes(-20)
            };

            context.Bids.AddRange(bid1, bid2);

            var winningBid = new Bid
            {
                AuctionId = endedWithWinner.Id,
                UserId = buyer2.Id,
                Amount = 8_500m,
                BidDate = now.AddMinutes(-60)
            };

            context.Bids.Add(winningBid);

            context.LedgerTransactions.Add(new LedgerTransaction
            {
                WalletId = buyer1.Wallet.Id,
                Type = LedgerTransactionType.Deposit,
                Amount = 150_000m,
                CreatedAt = now.AddDays(-2)
            });

            context.LedgerTransactions.Add(new LedgerTransaction
            {
                WalletId = buyer1.Wallet.Id,
                Type = LedgerTransactionType.Hold,
                Amount = 45_000m,
                CreatedAt = now.AddMinutes(-20),
                AuctionId = standardActive.Id
            });

            context.LedgerTransactions.Add(new LedgerTransaction
            {
                WalletId = buyer2.Wallet.Id,
                Type = LedgerTransactionType.Deposit,
                Amount = 200_000m,
                CreatedAt = now.AddDays(-2)
            });

            context.LedgerTransactions.Add(new LedgerTransaction
            {
                WalletId = buyer2.Wallet.Id,
                Type = LedgerTransactionType.Hold,
                Amount = 8_500m,
                CreatedAt = now.AddMinutes(-60),
                AuctionId = endedWithWinner.Id
            });

            context.LedgerTransactions.Add(new LedgerTransaction
            {
                WalletId = noFunds.Wallet.Id,
                Type = LedgerTransactionType.Deposit,
                Amount = 500m,
                CreatedAt = now.AddDays(-2)
            });

            await context.SaveChangesAsync();
        }
    }
}
