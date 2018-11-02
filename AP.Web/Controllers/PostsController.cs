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
        [ProducesResponseType(200, Type = typeof(Eager.Post))]
        [ProducesResponseType(204)] 
        public async Task<IActionResult> Get([FromQuery] PagingOptions pagingOptions)
        {
            if(pagingOptions == null)
            {
                pagingOptions = new PagingOptions
                {
                    Limit = 100,
                    Offset = 0
                };
            }

            var posts = await _postRepository.GetPostsByPagingOptions(pagingOptions);

            var mappedPosts = _mapper.Map<IEnumerable<Models.Post>, IEnumerable<Eager.Post>>(posts);

            return !mappedPosts.Any() ? NoContent() : (IActionResult)new JsonResult(mappedPosts);
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
                return BadRequest("INVALID_SLUG");
            }

            var post = await _postRepository.GetPostBySlug(slug);

            var mappedPost = _mapper.Map<Eager.Post>(post);

            return mappedPost == null ? NoContent() : (IActionResult)new JsonResult(mappedPost);
        }

        /// <summary>
        /// Get categories related with post
        /// </summary>
        /// <param name="slug">Post url identifier</param>
        [HttpGet("{slug}/categories")]
        [AllowAnonymous]
        [Produces("application/json")]
        [ProducesResponseType(200, Type = typeof(Eager.Category))]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> GetCategories(string slug)
        {
            if (String.IsNullOrWhiteSpace(slug))
                return BadRequest("INVALID_SLUG");

            var post = await _postRepository.GetPostBySlug(slug);

            if (post == null)
                return BadRequest("NO_POST_FOUND");

            if (post.Categories == null || !post.Categories.Any())
                return NoContent();

            return new JsonResult(post.Categories);
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
                return BadRequest("INVALID_SLUG");

            var post = await _postRepository.GetPostBySlug(slug);
            if (post == null)
                return BadRequest("NO_POST_FOUND");

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
                validationErrors.Append("AUTHOR_DOES_NOT_EXISTS");

            if(!postMapped.Categories.All(c => _categoryRepository.Exists(c.Id)))
                validationErrors.Append("ONE_OF_CATEGORIES_DOES_NOT_EXISTS");

            if(validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }
            else
            {
                postMapped.ModifiedOn = null;
                
                var createTask = await _postRepository.Create(postMapped);
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
                return BadRequest("NO_ID");
            else if(!_postRepository.Exists(post.Id.Value))
                return NotFound("POST_DOES_NOT_EXISTS");

            var postMapped = _mapper.Map<Models.Post>(post);

            var validationErrors = PostValidator.OnPostCreateValidation(postMapped);            
            
            if(!_userRepository.Exists(postMapped.Author.Id))
                validationErrors.Append("AUTHOR_DOES_NOT_EXISTS");

            if(!postMapped.Categories.All(c => _categoryRepository.Exists(c.Id)))
                validationErrors.Append("ONE_OF_CATEGORIES_DOES_NOT_EXISTS");

            if(validationErrors.Any())
            {
                return BadRequest(validationErrors);
            }
            else
            {
                postMapped.ModifiedOn = DateTime.Now;
                
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
                return BadRequest("NO_ID");

            if(!_postRepository.Exists(id))
                return NotFound("POST_DOES_NOT_EXISTS");

            await _postRepository.Delete(id);
            return Ok();
        }

        #endregion DELETE
    }
}
