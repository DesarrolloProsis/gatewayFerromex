namespace ApiGateway.Models
{
    public class Bolsas
    {
        public int IdBolsa { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string CarrilBolsa { get; set; }
        public int? Bolsa { get; set; }
    }
}

