using System;
using System.Linq;
using System.Threading.Tasks;
using AP.Repositories.Post;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Eager = AP.Entities.Models.Eager;
using Models = AP.Entities.Models;
using AP.Entities.Options;
using AP.Repositories.User;
using AutoMapper;
using System.Collections.Generic;
using AP.Validators.Post;
using AP.Repositories.Category;
using System.ComponentModel.DataAnnotations;
using AP.Entities.Enums;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;

namespace AP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        readonly IMapper _mapper;
        readonly IPostRepository _postRepository;
        readonly IUserRepository _userRepository;
        readonly ICategoryRepository _categoryRepository;

        public PostsController(IMapper mapper, IPostRepository postRepository, IUserRepository userRepository, ICategoryRepository categoryRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _userRepository = userRepository;
            _categoryRepository = categoryRepository;
        }

        #region GET

        /// <summary>
        /// Returns posts
        /// </summary>
        /// <remarks>
        /// Returns limited number of posts.async By default (no parameters) you will get last 100 posts (ordered by date ascending)
        /// </remarks>
        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Eager.Post>))]
        [ProducesResponseType(204)] 
        public async Task<IActionResult> Get([FromQuery] PagingOptions pagingOptions, [FromQuery] Conditions<Models.Post> conditions)
        {
            conditions.Validate();
            if(pagingOptions == null)
            {
                pagingOptions = new PagingOptions
                {
                    Limit = 100,
                    Offset = 0
                };
            }

            var posts = await _postRepository.GetPosts(pagingOptions, conditions);

            var mappedPosts = _mapper.Map<IEnumerable<Models.Post>, IEnumerable<Eager.Post>>(posts);

            if(!mappedPosts.Any())
            {
                return NoContent();
            }
            else
            {
                var result = Ok(mappedPosts);

                Response.Headers.Add("X-Total-Count", _postRepository.CountAllPosts().Result.ToString());
                
                return result;
            }
        }

        /// <summary>
        /// Returns post by slug
        /// </summary>
        /// <param name="slug">Post url identifier</param>
        [HttpGet("{slug}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.Post))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(string slug)
        {
            if(String.IsNullOrWhiteSpace(slug))
            {
                return BadRequest(CommonResponseMessages.InvalidSlug);
            }

            var post = await _postRepository.GetPostBySlug(slug);

            var mappedPost = _mapper.Map<Eager.Post>(post);

            return mappedPost == null ? NoContent() : (IActionResult)new JsonResult(mappedPost);
        }

        /// <summary>
        /// Returns post by id
        /// </summary>
        /// <param name="id">Post unique identifier</param>
        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.Post))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Get(Guid id)
        {
            if(id.Equals(Guid.Empty))
            {
                return BadRequest(CommonResponseMessages.NoId);
            }

            var post = await _postRepository.GetPostsById(id);

            if(post == null)
                return NoContent();

            var mappedPost = _mapper.Map<Eager.Post>(post);

            if(mappedPost == null)
                return NoContent();
            else
                return Ok(mappedPost);
        }

        /// <summary>
        /// Get categories related with post
        /// </summary>
        /// <param name="slug">Post url identifier</param>
        [HttpGet("{slug}/categories")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Eager.Category>))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCategories(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
                return BadRequest(CommonResponseMessages.InvalidSlug);

            var post = await _postRepository.GetPostBySlug(slug);

            if (post == null)
                return BadRequest(CommonResponseMessages.NoPostFound);

            if (post.PostCategories == null || !post.PostCategories.Any())
                return NoContent();

            return new JsonResult(post.PostCategories);
        }

        /// <summary>
        /// Get post's author
        /// </summary>
        /// <param name="slug">Post url identifier</param>
        [HttpGet("{slug}/author")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.User))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetAuthor(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
                return BadRequest(CommonResponseMessages.InvalidSlug);

            var post = await _postRepository.GetPostBySlug(slug);
            if (post == null)
                return BadRequest(CommonResponseMessages.NoPostFound);

            var user = await _userRepository.GetUserByPostId(post.Id);
            if (user == null)
                return NoContent();

            // Hide password for security reasons
            user.Password = null;

            return new JsonResult(user);
        }

        #endregion GET

        #region POST

        [HttpPost]
        [Authorize]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Post([FromBody] Eager.Post post)
        {
            var postMapped = _mapper.Map<Models.Post>(post);

            var validationErrors = PostValidator.OnPostCreateValidation(postMapped);            
            
            if(!_userRepository.Exists(postMapped.Author.Id))
                validationErrors.Append(CommonResponseMessages.AuthorDoesNotExists);

            if(!postMapped.PostCategories.All(pc => _categoryRepository.Exists(pc.CategoryId)))
                validationErrors.Append(CommonResponseMessages.OneOfCategoriesDoesNotExists);

            if(validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }
            else
            {
                postMapped.CreatedOn = DateTime.Now;
                postMapped.ModifiedOn = null;
                postMapped.Slug = GenerateSlug(postMapped.Title);
                
                var createdPost = await _postRepository.Create(postMapped);

                return Created($"{this.Request.Scheme}://{this.Request.Host}/api/Posts/{postMapped.Slug}", null);
            }
        }

        #endregion POST

        #region PUT

        [HttpPut]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Put([FromBody] Eager.Post post)
        {
            if(!post.Id.HasValue || post.Id.Value.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);
            else if(!_postRepository.Exists(post.Id.Value))
                return NoContent();

            var postMapped = _mapper.Map<Models.Post>(post);

            var validationErrors = PostValidator.OnPostCreateValidation(postMapped);            
            
            if(!_userRepository.Exists(postMapped.Author.Id))
                validationErrors.Append(CommonResponseMessages.AuthorDoesNotExists);

            if(!postMapped.PostCategories.All(pc => _categoryRepository.Exists(pc.CategoryId)))
                validationErrors.Append(CommonResponseMessages.OneOfCategoriesDoesNotExists);

            if(validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }
            else
            {
                var dbPost = await _postRepository.GetPostsById(postMapped.Id);
                foreach (var relation in dbPost.PostCategories.ToList())
                {
                    if(!postMapped.PostCategories.Any(pc => pc.PostId.Equals(relation.PostId) && pc.CategoryId.Equals(relation.CategoryId)))
                        await _postRepository.RemoveRelation<Models.PostCategory>(relation);
                }
                foreach (var relation in postMapped.PostCategories)
                {
                    if(!dbPost.PostCategories.Any(pc => pc.PostId.Equals(relation.PostId) && pc.CategoryId.Equals(relation.CategoryId)))
                        await _postRepository.CreateRelation<Models.PostCategory>(relation);
                }

                postMapped.CreatedOn = dbPost.CreatedOn;
                postMapped.ModifiedOn = DateTime.Now;
                postMapped.Slug = GenerateSlug(postMapped.Title);
                
                var postUpdated = await _postRepository.Update(postMapped);
                return Ok();
            }
        }

        #endregion PUT

        #region DELETE

        [HttpDelete("{id}")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Delete(Guid id)
        {
            if(id.Equals(Guid.Empty))
                return BadRequest(CommonResponseMessages.NoId);

            if(!_postRepository.Exists(id))
                return NoContent();

            await _postRepository.Delete(id);
            return Ok();
        }

        #endregion DELETE
        
        private string GenerateSlug(string phrase) 
        { 
            string str = RemoveAccent(phrase).ToLower(); 
            // invalid chars           
            str = Regex.Replace(str, @"[^a-z0-9\s-]", ""); 
            // convert multiple spaces into one space   
            str = Regex.Replace(str, @"\s+", " ").Trim(); 
            // cut and trim 
            str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();   
            str = Regex.Replace(str, @"\s", "-"); // hyphens   
            return str; 
        } 

        private string RemoveAccent(string txt) 
        { 
            byte[] bytes = System.Text.Encoding.GetEncoding("Cyrillic").GetBytes(txt); 
            return System.Text.Encoding.ASCII.GetString(bytes); 
        }
    }
}
