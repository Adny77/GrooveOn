using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Services.Database;
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

        public SubscriptionAnalyticsResponse GetSubscriptionAnalytics(int year, int? month = null)
        {
            var query = _context.Subscriptions.AsQueryable();

            query = query.Where(x => x.StartDate.Year == year);

            if (month.HasValue)
            {
                query = query.Where(x => x.StartDate.Month == month.Value);
            }

            var basicCount = query.Count(x => x.SubscriptionPlanId == 1);
            var premiumCount = query.Count(x => x.SubscriptionPlanId == 2 || x.SubscriptionPlanId == 3);

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

        public List<UserGrowthPointResponse> GetUserGrowthByMonth(int year)
        {
            var today = DateTime.Today;
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

            var result = _context.Users
                .Where(x => x.JoinDate.Year == year && x.JoinDate.Month <= maxMonth)
                .GroupBy(x => x.JoinDate.Month)
                .Select(g => new UserGrowthPointResponse
                {
                    Month = g.Key,
                    Count = g.Count()
                })
                .OrderBy(x => x.Month)
                .ToList();

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

        public List<IncomeByMonthResponse> GetIncomeByMonth(int year)
        {
            var result = _context.Subscriptions
                .Where(x =>
                    x.SubscriptionPlanId == 2 &&
                    x.PaymentDate.HasValue &&
                    x.PaymentDate.Value.Year == year)
                .GroupBy(x => x.PaymentDate!.Value.Month)
                .Select(g => new IncomeByMonthResponse
                {
                    Month = g.Key,
                    TotalIncome = g.Sum(x => x.PaymentAmount)
                })
                .OrderBy(x => x.Month)
                .ToList();

            var completed = Enumerable.Range(1, 12)
                .Select(month => new IncomeByMonthResponse
                {
                    Month = month,
                    TotalIncome = result.FirstOrDefault(x => x.Month == month)?.TotalIncome ?? 0
                })
                .ToList();

            return completed;
        }

        public MusicOverviewResponse GetMusicOverview(MusicOverviewRequest request)
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

                MostPlayedAlbums = GetMostPlayedAlbums(baseQuery, request.Take),
                MostPlayedSongs = GetMostPlayedSongs(baseQuery, request.Take),
                MostPlayedArtists = GetMostPlayedArtists(baseQuery, request.Take),

                LeastPlayedAlbums = GetLeastPlayedAlbums(baseQuery, request.Take),
                LeastPlayedSongs = GetLeastPlayedSongs(baseQuery, request.Take),
                LeastPlayedArtists = GetLeastPlayedArtists(baseQuery, request.Take),

                TrendingGenres = GetTrendingGenres(baseQuery, request.Take)
            };
        }

        private void ValidateMusicOverviewRequest(MusicOverviewRequest request)
        {
            if (request == null)
                throw new Exception("Zahtjev ne može biti null.");

            if (request.Year <= 0)
                throw new Exception("Year je obavezan.");

            if (request.Take <= 0)
                request.Take = 4;

            var mode = request.Mode?.Trim().ToLower();

            if (string.IsNullOrWhiteSpace(mode))
                throw new Exception("Mode je obavezan.");

            if (mode != "year" && mode != "month")
                throw new Exception("Mode mora biti 'year' ili 'month'.");

            if (mode == "month")
            {
                if (!request.Month.HasValue)
                    throw new Exception("Month je obavezan kada je mode = 'month'.");

                if (request.Month.Value < 1 || request.Month.Value > 12)
                    throw new Exception("Month mora biti između 1 i 12.");
            }
            else
            {
                request.Month = null;
            }
        }

        private List<MusicStatItemResponse> GetMostPlayedSongs(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<MusicStatItemResponse> GetLeastPlayedSongs(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<MusicStatItemResponse> GetMostPlayedAlbums(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<MusicStatItemResponse> GetLeastPlayedAlbums(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<MusicStatItemResponse> GetMostPlayedArtists(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<MusicStatItemResponse> GetLeastPlayedArtists(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }

        private List<GenreStatItemResponse> GetTrendingGenres(IQueryable<PlayHistory> query, int take)
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
                .ToList();
        }
    }
}