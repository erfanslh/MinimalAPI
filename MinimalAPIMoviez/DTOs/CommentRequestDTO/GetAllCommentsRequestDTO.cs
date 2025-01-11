using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.CommentRequestDTO
{
    public class GetAllCommentsRequestDTO
    {
        public int MovieID { get; set; }
        public ICommentRepository CommentRepository { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;
    }
}
