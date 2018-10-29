using System;
using System.Linq;
using System.Threading.Tasks;
using AP.Repositories.Post;
using Microsoft.AspNetCore.Mvc;
using Eager = AP.Entities.Models.Eager;
using Models = AP.Entities.Models;
using AP.Entities.Options;
using AP.Repositories.User;
using AutoMapper;
using System.Collections.Generic;

namespace AP.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        readonly IMapper _mapper;
        readonly IPostRepository _postRepository;
        readonly IUserRepository _userRepository;

        public PostsController(IMapper mapper, IPostRepository postRepository, IUserRepository userRepository)
        {
            _mapper = mapper;
            _postRepository = postRepository;
            _userRepository = userRepository;
        }

        /// <summary>
        /// Returns all posts
        /// </summary>
        [HttpGet]
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
        /// Returns post with specific slug
        /// </summary>
        /// <param name="slug">Post url identifier</param>
        [HttpGet("{slug}")]
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
    }
}
