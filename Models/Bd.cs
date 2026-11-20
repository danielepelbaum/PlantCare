
using Dapper;
using System.Data.SqlClient;
namespace PlantCare.Models;

public class BD
{
    private static string _connectionString =
        @"Server=localhost;Database=PlantCare;Trusted_Connection=True;TrustServerCertificate=True;";

    public static Usuario ObtenerUsuario(string usuario, string contraseña)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.QueryFirstOrDefault<Usuario>("ObtenerUsuario", new { usuario, contraseña }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }

    public static bool ExisteUsuario(string usuario, string mail)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.QueryFirstOrDefault<int>("ExisteUsuario", new { usuario, mail }, commandType: System.Data.CommandType.StoredProcedure) == 1;
        }
    }

    public static bool RegistrarUsuario(Usuario usuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Execute("RegistrarUsuario", new
            {
                usuario = usuario.UsuarioNombre,
                nombre = usuario.Nombre,
                apellido = usuario.Apellido,
                mail = usuario.Mail,
                contraseña = usuario.Contraseña,
                telefono = usuario.Teléfono,
                edad = usuario.Edad
            }, commandType: System.Data.CommandType.StoredProcedure) > 0;
        }
    }

    public static List<Planta> ObtenerPlantas()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Query<Planta>("ObtenerPlantas", commandType: System.Data.CommandType.StoredProcedure).AsList();
        }
    }

    public static Planta ObtenerPlanta(int id)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.QueryFirstOrDefault<Planta>("ObtenerPlanta", new { id }, commandType: System.Data.CommandType.StoredProcedure);
        }
    }

    public static List<Publicacion> ObtenerPublicaciones()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Query<Publicacion>("ObtenerPublicaciones", commandType: System.Data.CommandType.StoredProcedure).AsList();
        }
    }

    public static bool CrearPublicacion(Publicacion publicacion)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Execute("CrearPublicacion", new
            {
                texto = publicacion.Texto,
                Imagen = publicacion.Imagen,
                Likes = publicacion.Likes,
                IdUsuario = publicacion.IdUsuario
            }, commandType: System.Data.CommandType.StoredProcedure) > 0;
        }
    }

    public static List<Comentario> ObtenerComentarios(int idPlanta)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Query<Comentario>("ObtenerComentarios", new { idPlanta }, commandType: System.Data.CommandType.StoredProcedure).AsList();
        }
    }

    public static bool CrearComentario(Comentario comentario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Execute("CrearComentario", new
            {
                texto = comentario.Texto,
                fecha = comentario.Fecha,
                IdUsuario = comentario.IdUsuario,
                IdPlanta = comentario.IdPlanta
            }, commandType: System.Data.CommandType.StoredProcedure) > 0;
        }
    }

    public static List<Tipo> ObtenerTipos()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Query<Tipo>("ObtenerTipos", commandType: System.Data.CommandType.StoredProcedure).AsList();
        }
    }

    public static List<Planta> ObtenerPlantasUsuario(int idUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Query<Planta>("ObtenerPlantasUsuario", new { idUsuario }, commandType: System.Data.CommandType.StoredProcedure).AsList();
        }
    }

    public static bool AgregarPlantaUsuario(UsuarioPlanta plantaUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            return db.Execute("AgregarPlantaUsuario", new
            {
                FechaAdq = plantaUsuario.FechaAdquisición,
                IdUsuario = plantaUsuario.IdUsuario,
                IdPlanta = plantaUsuario.IdPlanta
            }, commandType: System.Data.CommandType.StoredProcedure) > 0;
        }
    }
}
