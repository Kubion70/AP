using System.Collections.Generic;
using System.Linq;
using AutoMapper;

namespace AP.Entities.Mappings
{
    public class MappingEntity : Profile
    {
        public MappingEntity()
        {
            ModelToEager();
            EagerToModel();
        }

        private void ModelToEager()
        {
            // Post
            CreateMap<Models.Post, Models.Eager.Post>()
            .AfterMap((m, e) =>
            {
                e.Author = m.Author.Id;
                e.Categories = m.Categories.Select(c => c.Id);
            });
        }

        private void EagerToModel()
        {
            // Post
            CreateMap<Models.Eager.Post, Models.Post>()
            .AfterMap((e, m) =>
            {
                m.Author = new Models.User(e.Id);
                m.Categories = new List<Models.Category>();
                foreach (var categoryId in e.Categories)
                {
                    m.Categories.Append(new Models.Category(categoryId));
                }
            });
        }
    }
}
