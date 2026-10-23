
using Dapper;
using Microsoft.Data.SqlClient;
using PlantCare.Models;
using System.Collections.Generic;

public class BD
{
    private static string _connectionString =
        @"Server=localhost;Database=PlantCare;Trusted_Connection=True;TrustServerCertificate=True;";

    // USUARIOS

    public static Usuario ObtenerUsuario(string usuario, string contraseña)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"SELECT ID, Nombre, Apellido, Mail,
                                  Usuario AS UsuarioNombre,
                                  Contraseña, Teléfono, Edad
                           FROM Usuario
                           WHERE Usuario = @usuario
                           AND Contraseña = @contraseña";

            return db.QueryFirstOrDefault<Usuario>(
                sql, new { usuario, contraseña });
        }
    }

    public static bool RegistrarUsuario(Usuario usuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"INSERT INTO Usuario
                           (Nombre, Apellido, Mail, Usuario,
                            Contraseña, Teléfono, Edad)
                           VALUES
                           (@Nombre, @Apellido, @Mail, @UsuarioNombre,
                            @Contraseña, @Teléfono, @Edad)";

            return db.Execute(sql, usuario) > 0;
        }
    }

    // PLANTAS

    public static List<Planta> ObtenerPlantas()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Planta";

            return db.Query<Planta>(sql).AsList();
        }
    }

    public static Planta ObtenerPlanta(int id)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Planta WHERE ID = @id";

            return db.QueryFirstOrDefault<Planta>(sql, new { id });
        }
    }

    // PUBLICACIONES

    public static List<Publicacion> ObtenerPublicaciones()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"SELECT ID, Texto, Imagen, Likes, IdUsuario
                           FROM [Publicación]
                           ORDER BY ID DESC";

            return db.Query<Publicacion>(sql).AsList();
        }
    }

    public static bool CrearPublicacion(Publicacion publicacion)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"INSERT INTO [Publicación]
                           (Texto, Imagen, Likes, IdUsuario)
                           VALUES
                           (@Texto, @Imagen, @Likes, @IdUsuario)";

            return db.Execute(sql, publicacion) > 0;
        }
    }

    // COMENTARIOS

    public static List<Comentario> ObtenerComentarios(int idPlanta)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"SELECT *
                           FROM Comentario
                           WHERE IdPlanta = @idPlanta
                           ORDER BY Fecha DESC";

            return db.Query<Comentario>(sql, new { idPlanta }).AsList();
        }
    }

    public static bool CrearComentario(Comentario comentario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"INSERT INTO Comentario
                           (Texto, Fecha, IdUsuario, IdPlanta)
                           VALUES
                           (@Texto, @Fecha, @IdUsuario, @IdPlanta)";

            return db.Execute(sql, comentario) > 0;
        }
    }

    // TIPOS DE PLANTAS

    public static List<Tipo> ObtenerTipos()
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = "SELECT * FROM Tipo";

            return db.Query<Tipo>(sql).AsList();
        }
    }

    // PLANTAS DEL USUARIO

    public static List<Planta> ObtenerPlantasUsuario(int idUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"SELECT P.ID, P.Nombre, P.Descripción,
                                  P.TiempoRiego
                           FROM Planta P
                           INNER JOIN UsuarioPlanta UP
                               ON P.ID = UP.IdPlanta
                           WHERE UP.IdUsuario = @idUsuario";

            return db.Query<Planta>(sql, new { idUsuario }).AsList();
        }
    }

    public static bool AgregarPlantaUsuario(UsuarioPlanta plantaUsuario)
    {
        using (SqlConnection db = new SqlConnection(_connectionString))
        {
            string sql = @"INSERT INTO UsuarioPlanta
                           (FechaAdquisición, IdUsuario, IdPlanta)
                           VALUES
                           (@FechaAdquisición, @IdUsuario, @IdPlanta)";

            return db.Execute(sql, plantaUsuario) > 0;
        }
    }
}
