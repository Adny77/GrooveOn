using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Model.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Exceptions;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class AnswerService
        : BaseCRUDService<AnswerResponse, AnswerSearchObject, Answer, AnswerUpsertRequest, AnswerUpsertRequest>, IAnswerService
    {
        private readonly INotificationService _notificationService;

        public AnswerService(
            GrooveOnDbContext context,
            IMapper mapper,
            INotificationService notificationService)
            : base(context, mapper)
        {
            _notificationService = notificationService;
        }

        protected override IQueryable<Answer> AddInclude(IQueryable<Answer> query, AnswerSearchObject search)
        {
            query = base.AddInclude(query, search);

            query = query
                .Include(x => x.Question)
                .Include(x => x.Admin);

            return query;
        }

        protected override IQueryable<Answer> ApplyFilter(IQueryable<Answer> query, AnswerSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.QuestionId.HasValue)
            {
                query = query.Where(x => x.QuestionId == search.QuestionId.Value);
            }

            if (search.AdminId.HasValue)
            {
                query = query.Where(x => x.AdminId == search.AdminId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Message.ToLower().Contains(fts) ||
                    (x.Question != null && x.Question.Title.ToLower().Contains(fts)) ||
                    (x.Admin != null && x.Admin.Username.ToLower().Contains(fts))
                );
            }

            return query;
        }

        protected override async Task BeforeInsert(Answer entity, AnswerUpsertRequest request)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == request.QuestionId);
            if (question == null)
                throw new NotFoundException("Question not found.");

            var admin = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.AdminId);
            if (admin == null)
                throw new NotFoundException("Admin not found.");

            entity.CreatedAt = DateTime.UtcNow;

            question.Answer = request.Message;
            question.AnsweredAt = entity.CreatedAt;
            question.Status = "Answered";

            _notificationService.AddForUser(
                question.UserId,
                $"Answer to your question: {question.Title}",
                "An administrator has answered your question. Open the questions section for details.",
                "question_answered");

            await base.BeforeInsert(entity, request);
        }

        protected override async Task BeforeUpdate(Answer entity, AnswerUpsertRequest request)
        {
            var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == entity.QuestionId);
            if (question == null)
                throw new NotFoundException("Question not found.");

            question.Answer = request.Message;
            question.AnsweredAt = DateTime.UtcNow;
            question.Status = "Answered";

            _notificationService.AddForUser(
                question.UserId,
                $"Updated answer: {question.Title}",
                "An administrator has updated the answer to your question.",
                "question_answered");

            await base.BeforeUpdate(entity, request);
        }

        protected override AnswerResponse MapToResponse(Answer entity)
        {
            var response = base.MapToResponse(entity);

            response.QuestionTitle = entity.Question?.Title;
            response.AdminUserName = entity.Admin?.Username;

            return response;
        }
    }
}
