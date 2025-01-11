using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.DTOs.CommentRequestDTO
{
    public class CreateCommentsRequestDTO
    {
        public int MovieID { get; set; }
        public IMapper Mapper { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IUserServices UserServices { get; set; } = null!;
        public ICommentRepository CommentRepository { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;

    }
}
