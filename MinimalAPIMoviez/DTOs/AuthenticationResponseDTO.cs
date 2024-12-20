namespace MinimalAPIMoviez.DTOs
{
    public class AuthenticationResponseDTO
    {
        public string Token { get; set; } = null!;
        public DateTime ExpireDate { get; set; }
    }
}
