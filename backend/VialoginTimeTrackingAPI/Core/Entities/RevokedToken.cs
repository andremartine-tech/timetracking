namespace VialoginTimeTrackingAPI.Core.Entities
{
    public class RevokedToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime RevokedAt { get; set; }
    }
}
