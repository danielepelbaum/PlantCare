namespace PlantCare.Models
{
    public class UsuarioPlanta
    {
        public int ID { get; set; }
        public DateTime FechaAdquisición { get; set; }
        public int IdUsuario { get; set; }
        public int IdPlanta { get; set; }
    }
}