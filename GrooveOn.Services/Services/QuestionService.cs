using GrooveOn.Model.RequestObjects;
using GrooveOn.Model.ResponseObjects;
using GrooveOn.Models.SearchObjects;
using GrooveOn.Services.Database;
using GrooveOn.Services.Interfaces;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace GrooveOn.Services.Services
{
    public class QuestionService
        : BaseCRUDService<QuestionResponse, QuestionSearchObject, Question, QuestionUpsertRequest, QuestionUpsertRequest>, IQuestionService
    {
        public QuestionService(GrooveOnDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        protected override IQueryable<Question> AddInclude(IQueryable<Question> query, QuestionSearchObject search)
        {
            query = base.AddInclude(query, search);
            query = query.Include(x => x.User);

            return query;
        }

        protected override IQueryable<Question> ApplyFilter(IQueryable<Question> query, QuestionSearchObject? search = null)
        {
            query = base.ApplyFilter(query, search);

            if (search == null)
                return query;

            if (search.UserId.HasValue)
            {
                query = query.Where(x => x.UserId == search.UserId.Value);
            }

            if (!string.IsNullOrWhiteSpace(search.Status))
            {
                var status = search.Status.ToLower();

                query = query.Where(x => x.Status.ToLower() == status);
            }

            if (!string.IsNullOrWhiteSpace(search.FTS))
            {
                var fts = search.FTS.ToLower();

                query = query.Where(x =>
                    x.Title.ToLower().Contains(fts) ||
                    x.Content.ToLower().Contains(fts) ||
                    x.Status.ToLower().Contains(fts) ||
                    (x.Answer != null && x.Answer.ToLower().Contains(fts)) ||
                    (x.User != null && x.User.Username.ToLower().Contains(fts))
                );
            }

            return query;
        }

        protected override Task BeforeInsert(Question entity, QuestionUpsertRequest request)
        {
            entity.CreatedAt = DateTime.UtcNow;

            if (string.IsNullOrWhiteSpace(request.Status))
                entity.Status = "Pending";

            if (!string.IsNullOrWhiteSpace(request.Answer))
            {
                entity.Status = "Answered";
                entity.AnsweredAt = DateTime.UtcNow;
            }

            return base.BeforeInsert(entity, request);
        }

        protected override Task BeforeUpdate(Question entity, QuestionUpsertRequest request)
        {
            if (!string.IsNullOrWhiteSpace(request.Answer) && entity.Answer != request.Answer)
            {
                entity.Status = "Answered";
                entity.AnsweredAt = DateTime.UtcNow;
            }

            return base.BeforeUpdate(entity, request);
        }

        protected override QuestionResponse MapToResponse(Question entity)
        {
            var response = base.MapToResponse(entity);
            response.UserName = entity.User?.Username;

            return response;
        }
    }
}