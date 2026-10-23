namespace PlantCare.Models
{
    public class Comentario
    {
        public int ID { get; set; }
        public string Texto { get; set; }
        public DateTime Fecha { get; set; }
        public int IdUsuario { get; set; }
        public int IdPlanta { get; set; }
    }
}