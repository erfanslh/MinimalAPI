using AutoMapper;
using MinimalAPIMoviez.Repositories;

namespace MinimalAPIMoviez.DTOs.CommentRequestDTO
{
    public class GetByIDCommentsRequestDTO
    {
        public int MovieID { get; set; }
        public int ID { get; set; }
        public ICommentRepository CommentRepository { get; set; } = null!;
        public IMovieRepository MovieRepository { get; set; } = null!;
        public IMapper Mapper { get; set; } = null!;

    }
}
