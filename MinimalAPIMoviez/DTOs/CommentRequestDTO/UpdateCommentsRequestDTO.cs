using AutoMapper;
using Microsoft.AspNetCore.OutputCaching;
using MinimalAPIMoviez.Repositories;
using MinimalAPIMoviez.Services;

namespace MinimalAPIMoviez.DTOs.CommentRequestDTO
{
    public class UpdateCommentsRequestDTO
    {
        public int MovieID { get; set; }
        public int ID { get; set; }
        public ICommentRepository CommentRepository { get; set; } = null!;
        public IOutputCacheStore CacheStore { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
        public IUserServices UserServices { get; set; } = null!;

    }
}
