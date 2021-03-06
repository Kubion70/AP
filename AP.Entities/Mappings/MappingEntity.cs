﻿using System;
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
            GuidToModel();
        }

        private void ModelToEager()
        {
            // Post
            CreateMap<Models.Post, Models.Eager.Post>()
                .AfterMap((m, e) =>
                {
                    e.Author = m.Author.Id;
                    e.Categories = m.PostCategories.Select(pc => pc.CategoryId);
                });

            // User
            CreateMap<Models.User, Models.Eager.User>()
                .AfterMap((m, e) =>
                {
                    e.Password = null;
                });
        }

        private void EagerToModel()
        {
            // Post
            CreateMap<Models.Eager.Post, Models.Post>()
                .ConstructUsing((e, m) => {
                    if(e.Id.HasValue)
                        return new Models.Post(e.Id.Value);
                    else
                        return new Models.Post();
                })
                .BeforeMap((e, m) =>
                {
                    m.Author = new Models.User(e.Author);
                    var postCategories = new List<Models.PostCategory>();
                    if(e.Categories != null) {
                        foreach (var categoryId in e.Categories)
                        {
                            postCategories.Add(new Models.PostCategory(){
                                CategoryId = categoryId,
                                PostId = e.Id.HasValue ? e.Id.Value : Guid.Empty
                            });
                        }
                    }
                    m.PostCategories = postCategories;
                });

            // Category
            CreateMap<Models.Eager.Category, Models.Category>()
                .ConstructUsing((e, m) => {
                    if(e.Id.HasValue)
                        return new Models.Category(e.Id.Value);
                    else
                        return new Models.Category();
                });

            // User
            CreateMap<Models.Eager.User, Models.User>()
                .ConstructUsing((e, m) => {
                    if(e.Id.HasValue)
                        return new Models.User(e.Id.Value);
                    else
                        return new Models.User();
                });
        }

        private void GuidToModel()
        {
            // User
            CreateMap<Guid, Models.User>()
                .ConstructUsing((id, m) => {
                    if(id.Equals(Guid.Empty))
                        return new Models.User();
                    else
                        return new Models.User(id);
                })
                .ForMember(m => m.Id, opt => opt.MapFrom(g => g))
                .ForAllOtherMembers(m => m.Ignore());
        }
    }
}
