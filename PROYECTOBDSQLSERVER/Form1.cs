using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTOBDSQLSERVER
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void BtnIngresar_Click(object sender, EventArgs e)
        {
            string usuario = txtUsuario.Text.Trim();
            string contraseña = txtContraseña.Text.Trim();

            if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(contraseña))
            {
                MessageBox.Show("Por favor ingrese un usuario y una contraseña.");
                return;
            }

            byte[] contraseñaCifrada = CifrarSHA256(contraseña);

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    string query = "SELECT ID_Usuario FROM Usuario WHERE Nombre = @Nombre AND Contraseña = @Contraseña";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.Add("@Nombre", SqlDbType.NVarChar, 256).Value = usuario;
                        cmd.Parameters.Add("@Contraseña", SqlDbType.VarBinary).Value = contraseñaCifrada;

                        var result = cmd.ExecuteScalar();

                        if (result != null)
                        {
                            // Obtener el ID del usuario logueado y guardarlo en la clase estática UsuarioSesion
                            UsuarioSesion.ID_Usuario = (int)result;

                            // Abrir el formulario de Proveedores
                            PaginaPrincipal formProveedores = new PaginaPrincipal();
                            formProveedores.Show();
                            this.Hide();
                        }
                        else
                        {
                            lblError.Text = "Credenciales incorrectas.";
                            lblError.Visible = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al conectar con la base de datos: " + ex.Message);
                }
            }
        }

        public static byte[] CifrarSHA256(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
               
                byte[] bytes = Encoding.UTF8.GetBytes(contrasena);

             
                return sha256Hash.ComputeHash(bytes);
            }
        }

        

        private void button1_Click(object sender, EventArgs e)
        {


            

        }
    }
}