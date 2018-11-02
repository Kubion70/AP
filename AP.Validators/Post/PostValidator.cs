using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Models = AP.Entities.Models;

namespace AP.Validators.Post
{
    public static class PostValidator
    {
        public static IEnumerable<string> OnPostCreateValidation(Models.Post post)
        {
            var validationErrors = new List<string>();

            // Title
            if(string.IsNullOrWhiteSpace(post.Title))
                validationErrors.Add("EMPTY_TITLE");
            else if(post.Title.Length > 100)
                validationErrors.Add("TITLE_TOO_LONG");

            // Content
            if(string.IsNullOrWhiteSpace(post.Content))
                validationErrors.Add("EMPTY_CONTENT");
            else if(post.Content.Length > 7000)
                validationErrors.Add("CONTET_TOO_LONG");

            // Slug
            string slugPattern = @"^[a-z0-9]+(?:-[a-z0-9]+)*$";
            if(!string.IsNullOrWhiteSpace(post.Slug) && !Regex.IsMatch(post.Slug, slugPattern))
                validationErrors.Add("SLUG_INVALID_FORMAT");

            // Author
            if(post.Author.Equals(Guid.Empty))
                validationErrors.Add("EMPTY_AUTHOR");

            return validationErrors;
        }
    }
}