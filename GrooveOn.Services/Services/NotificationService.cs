using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Models.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class NotificationService
        : BaseCRUDService<NotificationResponse, NotificationSearchObject, Notification, NotificationUpsertRequest, NotificationUpsertRequest>,
          INotificationService
    {
        public NotificationService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Notification> ApplyFilter(
            IQueryable<Notification> query,
            NotificationSearchObject search)
        {
            query = base.ApplyFilter(query, search);

            if (search.UserId.HasValue)
                query = query.Where(x => x.UserId == search.UserId.Value);

            if (search.IsRead.HasValue)
                query = query.Where(x => x.IsRead == search.IsRead.Value);

            if (!string.IsNullOrWhiteSpace(search.Type))
                query = query.Where(x => x.Type == search.Type);

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.Trim().ToLower();
                query = query.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    x.Content.ToLower().Contains(fts) ||
                    x.Type.ToLower().Contains(fts));
            }

            return query.OrderBy(x => x.IsRead).ThenByDescending(x => x.CreatedAt);
        }

        protected override async Task BeforeInsert(Notification entity, NotificationUpsertRequest request)
        {
            var userExists = await _context.Users.AnyAsync(x => x.Id == request.UserId);
            if (!userExists)
                throw new NotFoundException("User was not found.");

            entity.CreatedAt = DateTime.UtcNow;
            await base.BeforeInsert(entity, request);
        }

        public async Task<NotificationResponse?> MarkAsReadAsync(int id, int userId)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(x => x.Id == id && x.UserId == userId);

            if (notification == null)
                return null;

            notification.IsRead = true;
            await _context.SaveChangesAsync();

            return MapToResponse(notification);
        }

        public async Task<int> MarkAllAsReadAsync(int userId)
        {
            var unread = await _context.Notifications
                .Where(x => x.UserId == userId && !x.IsRead)
                .ToListAsync();

            foreach (var n in unread)
                n.IsRead = true;

            await _context.SaveChangesAsync();
            return unread.Count;
        }

        public void AddForUser(int userId, string title, string content, string type)
        {
            _context.Notifications.Add(new Notification
            {
                UserId = userId,
                Title = title,
                Content = content,
                Type = type,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            });
        }
    }
}
