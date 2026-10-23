namespace PlantCare.Models
{
    public class Publicacion
    {
        public int ID { get; set; }
        public string Texto { get; set; }
        public string Imagen { get; set; }
        public int Likes { get; set; }
        public int IdUsuario { get; set; }
    }
}