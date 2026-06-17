using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Models;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class ReportService : IReportService
    {
        private readonly GrooveOnDbContext _context;

        public ReportService(GrooveOnDbContext context)
        {
            _context = context;
        }

        public async Task<SubscriptionAnalyticsResponse> GetNewUsersSubscriptionAnalyticsAsync(int year, int? month)
        {
            if (year <= 0)
                throw new UserException("Year is required.");

            if (month.HasValue && (month.Value < 1 || month.Value > 12))
                throw new UserException("Month must be between 1 and 12.");

            var newUsersQuery = _context.Users.Where(x => x.JoinDate.Year == year);

            if (month.HasValue)
            {
                newUsersQuery = newUsersQuery.Where(x => x.JoinDate.Month == month.Value);
            }

            var newUserIds = await newUsersQuery
                .Select(x => x.Id)
                .ToListAsync();

            var basicPlanId = await _context.SubscriptionPlans
                .Where(x => x.PlanCode == SubscriptionPlanCodes.Basic)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var query = _context.Subscriptions
                .Where(x => newUserIds.Contains(x.UserId));

            var basicCount = await query.CountAsync(x => x.SubscriptionPlanId == basicPlanId);
            var premiumCount = await query.CountAsync(x => x.SubscriptionPlanId != basicPlanId);

            var total = basicCount + premiumCount;

            double basicPercentage = total == 0 ? 0 : (double)basicCount / total * 100;
            double premiumPercentage = total == 0 ? 0 : (double)premiumCount / total * 100;

            return new SubscriptionAnalyticsResponse
            {
                BasicCount = basicCount,
                PremiumCount = premiumCount,
                BasicPercentage = Math.Round(basicPercentage, 2),
                PremiumPercentage = Math.Round(premiumPercentage, 2),
                TotalCount = total,
                PeriodLabel = month.HasValue
                    ? $"{month.Value:D2}/{year}"
                    : year.ToString()
            };
        }

        public async Task<MobileHomeResponse> GetMobileHomeAsync(int takeTracks = 4, int takeArtists = 8)
        {
            // Single DB query: GroupBy includes song columns in the key so EF Core
            // generates one JOIN + GROUP BY without a second round-trip.
            var pool = Math.Max(takeTracks, 10);

            var topTrackPool = await _context.PlayHistories
                .Where(ph => ph.Song != null)
                .GroupBy(ph => new
                {
                    ph.SongId,
                    Title = ph.Song!.Title,
                    CoverUrl = ph.Song!.CoverUrl
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.SongId,
                    Title = g.Key.Title,
                    ImageUrl = g.Key.CoverUrl,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .Take(pool)
                .ToListAsync();

            var topTracksResponse = topTrackPool.Take(takeTracks).ToList();

            MusicStatItemResponse? songOfTheDay = null;
            if (topTrackPool.Count > 0)
                songOfTheDay = topTrackPool[new Random().Next(topTrackPool.Count)];

            // Single DB query: GroupBy includes artist columns so no extra round-trip.
            var topArtistsResponse = await _context.PlayHistories
                .Where(ph => ph.Song != null && ph.Song.Artist != null)
                .GroupBy(ph => new
                {
                    ArtistId = ph.Song!.ArtistId,
                    Name = ph.Song!.Artist!.Name,
                    Picture = ph.Song!.Artist!.Picture
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.ArtistId,
                    Title = g.Key.Name,
                    ImageUrl = g.Key.Picture,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .Take(takeArtists)
                .ToListAsync();

            return new MobileHomeResponse
            {
                SongOfTheDay = songOfTheDay,
                TopTracks = topTracksResponse,
                TopArtists = topArtistsResponse
            };
        }

        public async Task<SubscriptionAnalyticsResponse> GetSubscriptionAnalyticsAsync(int year, int? month = null)
        {
            var basicPlanId = await _context.SubscriptionPlans
                .Where(x => x.PlanCode == SubscriptionPlanCodes.Basic)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var query = _context.Subscriptions.AsQueryable();

            query = query.Where(x => x.StartDate.Year == year);

            if (month.HasValue)
            {
                query = query.Where(x => x.StartDate.Month == month.Value);
            }

            var basicCount = await query.CountAsync(x => x.SubscriptionPlanId == basicPlanId);
            var premiumCount = await query.CountAsync(x => x.SubscriptionPlanId != basicPlanId);

            var total = basicCount + premiumCount;

            double basicPercentage = total == 0 ? 0 : (double)basicCount / total * 100;
            double premiumPercentage = total == 0 ? 0 : (double)premiumCount / total * 100;

            return new SubscriptionAnalyticsResponse
            {
                BasicCount = basicCount,
                PremiumCount = premiumCount,
                BasicPercentage = Math.Round(basicPercentage, 2),
                PremiumPercentage = Math.Round(premiumPercentage, 2),
                TotalCount = total,
                PeriodLabel = month.HasValue ? $"{month.Value:D2}/{year}" : year.ToString()
            };
        }

        public async Task<List<UserGrowthPointResponse>> GetUserGrowthByMonthAsync(int year)
        {
            var today = DateTime.UtcNow;
            int currentYear = today.Year;
            int currentMonth = today.Month;

            int maxMonth;

            if (year < currentYear)
            {
                maxMonth = 12;
            }
            else if (year == currentYear)
            {
                maxMonth = currentMonth - 1;
            }
            else
            {
                return new List<UserGrowthPointResponse>();
            }

            if (maxMonth <= 0)
            {
                return new List<UserGrowthPointResponse>();
            }

            var result = await _context.Users
                .Where(x => x.JoinDate.Year == year && x.JoinDate.Month <= maxMonth)
                .GroupBy(x => x.JoinDate.Month)
                .Select(g => new UserGrowthPointResponse
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            var monthLabels = new[]
            {
                "", "Jan", "Feb", "Mar", "Apr", "May", "Jun",
                "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"
            };

            var completed = Enumerable.Range(1, maxMonth)
                .Select(month => new UserGrowthPointResponse
                {
                    Month = month,
                    Label = monthLabels[month],
                    Count = result.FirstOrDefault(x => x.Month == month)?.Count ?? 0
                })
                .ToList();

            return completed;
        }

        public async Task<List<IncomeByMonthResponse>> GetIncomeByMonthAsync(int year)
        {
            var basicPlanId = await _context.SubscriptionPlans
                .Where(x => x.PlanCode == SubscriptionPlanCodes.Basic)
                .Select(x => x.Id)
                .FirstOrDefaultAsync();

            var result = await _context.Payments
                .Include(x => x.Subscription)
                .Where(x =>
                    x.PaymentStatus == "Paid" &&
                    x.PaymentDate.HasValue &&
                    x.PaymentDate.Value.Year == year &&
                    x.Subscription != null &&
                    x.Subscription.SubscriptionPlanId != basicPlanId)
                .GroupBy(x => x.PaymentDate!.Value.Month)
                .Select(g => new IncomeByMonthResponse
                {
                    Month = g.Key,
                    TotalIncome = g.Sum(x => x.PaymentAmount)
                })
                .OrderBy(x => x.Month)
                .ToListAsync();

            return Enumerable.Range(1, 12)
                .Select(month => new IncomeByMonthResponse
                {
                    Month = month,
                    TotalIncome = result.FirstOrDefault(x => x.Month == month)?.TotalIncome ?? 0
                })
                .ToList();
        }

        public async Task<MusicOverviewResponse> GetMusicOverviewAsync(MusicOverviewRequest request)
        {
            ValidateMusicOverviewRequest(request);

            var mode = request.Mode.Trim().ToLower();

            var baseQuery = _context.PlayHistories
                .AsNoTracking()
                .Include(x => x.Song)
                    .ThenInclude(s => s.Album)
                        .ThenInclude(a => a.AlbumGenres)
                            .ThenInclude(ag => ag.Genre)
                .Include(x => x.Song)
                    .ThenInclude(s => s.Artist)
                .Where(x => x.PlayedAt.Year == request.Year);

            if (mode == "month")
            {
                baseQuery = baseQuery.Where(x => x.PlayedAt.Month == request.Month!.Value);
            }

            return new MusicOverviewResponse
            {
                Mode = mode,
                Year = request.Year,
                Month = mode == "month" ? request.Month : null,

                MostPlayedAlbums = await GetMostPlayedAlbumsAsync(baseQuery, request.Take),
                MostPlayedSongs = await GetMostPlayedSongsAsync(baseQuery, request.Take),
                MostPlayedArtists = await GetMostPlayedArtistsAsync(baseQuery, request.Take),

                LeastPlayedAlbums = await GetLeastPlayedAlbumsAsync(baseQuery, request.Take),
                LeastPlayedSongs = await GetLeastPlayedSongsAsync(baseQuery, request.Take),
                LeastPlayedArtists = await GetLeastPlayedArtistsAsync(baseQuery, request.Take),

                TrendingGenres = await GetTrendingGenresAsync(baseQuery, request.Take)
            };
        }

        private void ValidateMusicOverviewRequest(MusicOverviewRequest request)
        {
            if (request == null)
                throw new UserException("Request is null");

            if (request.Year <= 0)
                throw new UserException("Year is required");

            if (request.Take <= 0)
                request.Take = 4;

            var mode = request.Mode?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(mode))
                throw new UserException("Mode is required");

            if (mode != "year" && mode != "month")
                throw new UserException("Mode needs to be 'year' or 'month'");

            if (mode == "month")
            {
                if (!request.Month.HasValue)
                    throw new InvalidOperationException("Month value is required when the mode is 'month'");

                if (request.Month.Value < 1 || request.Month.Value > 12)
                    throw new UserException("Month needs to be between 1 and 12.");
            }
            else
            {
                request.Month = null;
            }
        }

        private Task<List<MusicStatItemResponse>> GetMostPlayedSongsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .GroupBy(x => new
                {
                    x.SongId,
                    x.Song.Title,
                    x.Song.CoverUrl
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.SongId,
                    Title = g.Key.Title,
                    ImageUrl = g.Key.CoverUrl,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<MusicStatItemResponse>> GetLeastPlayedSongsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .GroupBy(x => new
                {
                    x.SongId,
                    x.Song.Title,
                    x.Song.CoverUrl
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.SongId,
                    Title = g.Key.Title,
                    ImageUrl = g.Key.CoverUrl,
                    PlayCount = g.Count()
                })
                .OrderBy(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<MusicStatItemResponse>> GetMostPlayedAlbumsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .Where(x => x.Song.Album != null)
                .GroupBy(x => new
                {
                    x.Song.Album!.Id,
                    x.Song.Album.Title,
                    x.Song.Album.CoverUrl
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    ImageUrl = g.Key.CoverUrl,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<MusicStatItemResponse>> GetLeastPlayedAlbumsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .Where(x => x.Song.Album != null)
                .GroupBy(x => new
                {
                    x.Song.Album!.Id,
                    x.Song.Album.Title,
                    x.Song.Album.CoverUrl
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.Id,
                    Title = g.Key.Title,
                    ImageUrl = g.Key.CoverUrl,
                    PlayCount = g.Count()
                })
                .OrderBy(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<MusicStatItemResponse>> GetMostPlayedArtistsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .Where(x => x.Song.Artist != null)
                .GroupBy(x => new
                {
                    x.Song.Artist!.Id,
                    x.Song.Artist.Name,
                    x.Song.Artist.Picture
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.Id,
                    Title = g.Key.Name,
                    ImageUrl = g.Key.Picture,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<MusicStatItemResponse>> GetLeastPlayedArtistsAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .Where(x => x.Song.Artist != null)
                .GroupBy(x => new
                {
                    x.Song.Artist!.Id,
                    x.Song.Artist.Name,
                    x.Song.Artist.Picture
                })
                .Select(g => new MusicStatItemResponse
                {
                    Id = g.Key.Id,
                    Title = g.Key.Name,
                    ImageUrl = g.Key.Picture,
                    PlayCount = g.Count()
                })
                .OrderBy(x => x.PlayCount)
                .ThenBy(x => x.Title)
                .Take(take)
                .ToListAsync();
        }

        private Task<List<GenreStatItemResponse>> GetTrendingGenresAsync(IQueryable<PlayHistory> query, int take)
        {
            return query
                .Where(x => x.Song.Album != null)
                .SelectMany(x => x.Song.Album!.AlbumGenres)
                .Where(x => x.Genre != null && !string.IsNullOrWhiteSpace(x.Genre.Name))
                .GroupBy(x => new
                {
                    x.Genre!.Id,
                    x.Genre.Name
                })
                .Select(g => new GenreStatItemResponse
                {
                    Genre = g.Key.Name,
                    PlayCount = g.Count()
                })
                .OrderByDescending(x => x.PlayCount)
                .ThenBy(x => x.Genre)
                .Take(take)
                .ToListAsync();
        }
    }
}
