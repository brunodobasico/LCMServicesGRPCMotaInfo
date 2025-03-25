namespace AMoverGRPC.Models
{
    public class Mota
    {
        public int MotaId { get; set; }
        public string VIN { get; set; }
        public int Battery { get; set; }
        public int Kilometers { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
