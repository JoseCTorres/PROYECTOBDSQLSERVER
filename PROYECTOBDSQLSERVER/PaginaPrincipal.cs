using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PROYECTOBDSQLSERVER
{
    public partial class PaginaPrincipal : Form
    {
        public PaginaPrincipal()
        {
            InitializeComponent();
        }
        private string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";

     
        public static byte[] CifrarSHA256(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(contrasena);
                return sha256Hash.ComputeHash(bytes);
            }
        }
        private void ConsultarUsuarios()
        {

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT Id_Usuario, Nombre, Apellido, CorreoElectronico, Telefono, Direccion, TipoUsuario FROM Usuario WHERE Estatus = 1"; // Solo usuarios activos

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt); 

               
                dgvUsuarios.DataSource = dt;
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregar_Click(object sender, EventArgs e)
        {
            string nombreUsuario = txtNombre.Text.Trim();
            string apellido = txtApellido.Text.Trim();
            string correoElectronico = txtCorreo.Text.Trim();
            string telefono = txtTelefono.Text.Trim();
            string direccion = txtDireccion.Text.Trim();
            string tipoUsuario = cmbTipoUsuario.SelectedItem.ToString(); 
            string contrasena = txtContraseña.Text.Trim();

            if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(correoElectronico) ||
                string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(tipoUsuario) || string.IsNullOrWhiteSpace(contrasena))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

            byte[] contrasenaCifrada = CifrarSHA256(contrasena);
            DateTime fechaRegistro = dtpFechaRegistro.Value; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Usuario (Nombre, Apellido, CorreoElectronico, Contraseña, Telefono, Direccion, FechaRegistro, TipoUsuario, Estatus) " +
                                   "VALUES (@Nombre, @Apellido, @CorreoElectronico, @Contraseña, @Telefono, @Direccion, @FechaRegistro, @TipoUsuario, @Estatus)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombreUsuario);
                        cmd.Parameters.AddWithValue("@Apellido", apellido);
                        cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                        cmd.Parameters.AddWithValue("@Contraseña", contrasenaCifrada);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                        cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                        cmd.Parameters.AddWithValue("@TipoUsuario", tipoUsuario);  
                        cmd.Parameters.AddWithValue("@Estatus", 1);  

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Usuario agregado correctamente.");
                        ConsultarUsuarios();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el usuario: " + ex.Message);
                }

            }
    }

        private void btnEliminar_Click(object sender, EventArgs e)
        {
           
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
               
                string id = dgvUsuarios.SelectedRows[0].Cells["Id_Usuario"].Value.ToString();

                
                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este usuario?", "Confirmar eliminación", MessageBoxButtons.YesNo);

                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();

                           
                            string query = "DELETE FROM Usuario WHERE  Id_Usuario = @Id_Usuario";

                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@Id_Usuario", id);
                                cmd.ExecuteNonQuery();
                            }

                           
                            ConsultarUsuarios();

                            MessageBox.Show("Usuario eliminado correctamente.");
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el usuario: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un usuario para eliminar.");
            }
        }
        

        private void PaginaPrincipal_Load(object sender, EventArgs e)
        {
            ConsultarUsuarios();
            ConsultarRoles();
            ConsultarProveedores();
            ConsultarAlmacenes();
            CargarInventarios();
            ConsultarCuentasPorPagar();
            ConsultarSucursales();
            ConsultarEmpleados();
            ConsultarDepartamentos();
            ConsultarTurnos();
        }

        private void btnEditar_Click(object sender, EventArgs e)
        {
            if (dgvUsuarios.SelectedRows.Count > 0)
            {
                string id = dgvUsuarios.SelectedRows[0].Cells["Id_Usuario"].Value.ToString();
                string nombreUsuario = txtNombre.Text.Trim();
                string apellido = txtApellido.Text.Trim();
                string correoElectronico = txtCorreo.Text.Trim();
                string telefono = txtTelefono.Text.Trim();
                string direccion = txtDireccion.Text.Trim();
                string tipoUsuario = cmbTipoUsuario.SelectedItem.ToString();  
                string contrasena = txtContraseña.Text.Trim();

                if (string.IsNullOrWhiteSpace(nombreUsuario) || string.IsNullOrWhiteSpace(apellido) || string.IsNullOrWhiteSpace(correoElectronico) ||
                    string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(tipoUsuario) || string.IsNullOrWhiteSpace(contrasena))
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

                byte[] contrasenaCifrada = CifrarSHA256(contrasena);
                DateTime fechaRegistro = dtpFechaRegistro.Value;

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        string query = "UPDATE Usuario SET Nombre = @Nombre, Apellido = @Apellido, CorreoElectronico = @CorreoElectronico, " +
                                       "Contraseña = @Contraseña, Telefono = @Telefono, Direccion = @Direccion, FechaRegistro = @FechaRegistro, " +
                                       "TipoUsuario = @TipoUsuario WHERE Id_Usuario = @Id_Usuario";

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Id_Usuario", id);
                            cmd.Parameters.AddWithValue("@Nombre", nombreUsuario);
                            cmd.Parameters.AddWithValue("@Apellido", apellido);
                            cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                            cmd.Parameters.AddWithValue("@Contraseña", contrasenaCifrada);
                            cmd.Parameters.AddWithValue("@Telefono", telefono);
                            cmd.Parameters.AddWithValue("@Direccion", direccion);
                            cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistro);
                            cmd.Parameters.AddWithValue("@TipoUsuario", tipoUsuario);  

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Usuario actualizado correctamente.");
                            ConsultarUsuarios();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al actualizar el usuario: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un usuario para editar.");
            }
        }

        private void brnBuscar_Click(object sender, EventArgs e)
        {

        }

        private void btnAgregarRol_Click(object sender, EventArgs e)
        {
            string nombreRol = txtNombreRol.Text.Trim();
            string descripcion = txtDescripcionRol.Text.Trim();
            int idUsuario = ObtenerIdUsuarioLogueado(); 

            if (string.IsNullOrWhiteSpace(nombreRol) || string.IsNullOrWhiteSpace(descripcion))
            {
                MessageBox.Show("Por favor ingrese un nombre y una descripción para el rol.");
                return;
            }

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Rol (NombreRol, Descripcion, Estatus, ID_Usuario) VALUES (@NombreRol, @Descripcion, 1, @ID_Usuario)"; // Estatus por defecto 1

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreRol", nombreRol);
                        cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Rol agregado correctamente.");
                        ConsultarRoles();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el rol: " + ex.Message);
                }
            }
        }
        private void ConsultarRoles()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT ID_Rol, NombreRol, Descripcion, ID_Usuario FROM Rol WHERE Estatus = 1"; 

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlDataAdapter da = new SqlDataAdapter(query, conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                dgvroles.DataSource = dt;
            }
        }
        private int ObtenerIdUsuarioLogueado()
        {
           
            return 1; 
        }

        private void btnEliminarRol_Click(object sender, EventArgs e)
        {
            if (dgvroles.SelectedRows.Count > 0)
            {
                string idRol = dgvroles.SelectedRows[0].Cells["ID_Rol"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este rol?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Rol WHERE ID_Rol = @ID_Rol";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Rol", idRol);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Rol eliminado correctamente.");
                                ConsultarRoles();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el rol: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un rol para eliminar.");
            }
        }

        private void btnModificarRol_Click(object sender, EventArgs e)
        {
            if (dgvroles.SelectedRows.Count > 0)
            {
                string idRol = dgvroles.SelectedRows[0].Cells["ID_Rol"].Value.ToString();
                string nombreRol = dgvroles.SelectedRows[0].Cells["NombreRol"].Value.ToString();
                string descripcion = dgvroles.SelectedRows[0].Cells["Descripcion"].Value.ToString();
                int idUsuario = UsuarioSesion.ID_Usuario;

                txtNombreRol.Text = nombreRol;
                txtDescripcionRol.Text = descripcion;

              
                BtnGuardarRol.Click += (s, args) =>
                {
                    string newNombreRol = txtNombreRol.Text.Trim();
                    string newDescripcion = txtDescripcionRol.Text.Trim();

                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "UPDATE Rol SET NombreRol = @NombreRol, Descripcion = @Descripcion, ID_Usuario = @ID_Usuario WHERE ID_Rol = @ID_Rol";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@NombreRol", newNombreRol);
                                cmd.Parameters.AddWithValue("@Descripcion", newDescripcion);
                                cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);
                                cmd.Parameters.AddWithValue("@ID_Rol", idRol);

                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Rol actualizado correctamente.");
                                ConsultarRoles();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al actualizar el rol: " + ex.Message);
                        }
                    }
                };
            }
            else
            {
                MessageBox.Show("Seleccione un rol para editar.");
            }
        }

        private void tabRol_Click(object sender, EventArgs e)
        {

        }

        private void btnagregarProveedor_Click(object sender, EventArgs e)
        {
            string nombrep = TxtNombreProveedor.Text.Trim();
            string correop = txtCorreoElectronico.Text.Trim();
            string telefonop = txtTelefonoProveedor.Text.Trim();
            string direccionp = txtDireccionProveedor.Text.Trim();
            string tipoProveedor = cmbTipoProveedor.SelectedItem.ToString(); 
            DateTime fechaRegistrop = DateTime.Now;
            bool estatus = true; 

           
            if (string.IsNullOrWhiteSpace(nombrep) || string.IsNullOrWhiteSpace(correop) || string.IsNullOrWhiteSpace(telefonop) ||
                string.IsNullOrWhiteSpace(direccionp) || string.IsNullOrWhiteSpace(tipoProveedor))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return; 
            }
           
            int idUsuario = UsuarioSesion.ID_Usuario;

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "INSERT INTO Proveedor (Nombre, CorreoElectronico, Telefono, Direccion, TipoProveedor, FechaRegistro, Estatus, ID_Usuario) " +
                                   "VALUES (@Nombre, @CorreoElectronico, @Telefono, @Direccion, @TipoProveedor, @FechaRegistro, @Estatus, @ID_Usuario)";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombrep);
                        cmd.Parameters.AddWithValue("@CorreoElectronico", correop);
                        cmd.Parameters.AddWithValue("@Telefono", telefonop);
                        cmd.Parameters.AddWithValue("@Direccion", direccionp);
                        cmd.Parameters.AddWithValue("@TipoProveedor", tipoProveedor);
                        cmd.Parameters.AddWithValue("@FechaRegistro", fechaRegistrop);
                        cmd.Parameters.AddWithValue("@Estatus", estatus);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario); 

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Proveedor agregado correctamente.");
                        ConsultarProveedores();
                        LimpiarCampos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el proveedor: " + ex.Message);
                }
            }

        }
        private void LimpiarCampos()
        {
            
            TxtNombreProveedor.Clear();
            txtCorreoElectronico.Clear();
            txtTelefonoProveedor.Clear();
            txtDireccionProveedor.Clear();

          
            cmbTipoProveedor.SelectedIndex = -1;  
        }
        private void ConsultarProveedores()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Proveedor";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvProveedores.DataSource = dt; 
                        dgvProveedores2.DataSource = dt;
                       
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los proveedores: " + ex.Message);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                string idProveedor = dgvProveedores.SelectedRows[0].Cells["ID_Proveedor"].Value.ToString();
                string nombre = TxtNombreProveedor.Text.Trim();
                string correo = txtCorreoElectronico.Text.Trim();
                string telefono = txtTelefonoProveedor.Text.Trim();
                string direccion = txtDireccionProveedor.Text.Trim();
                string tipoProveedor = cmbTipoProveedor.SelectedItem.ToString();

                if (string.IsNullOrWhiteSpace(nombre) || string.IsNullOrWhiteSpace(correo) || string.IsNullOrWhiteSpace(telefono) || string.IsNullOrWhiteSpace(direccion) || cmbTipoProveedor.SelectedIndex == -1)
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Proveedor SET Nombre = @Nombre, CorreoElectronico = @CorreoElectronico, Telefono = @Telefono, Direccion = @Direccion, TipoProveedor = @TipoProveedor " +
                               "WHERE ID_Proveedor = @ID_Proveedor";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_Proveedor", idProveedor);
                            cmd.Parameters.AddWithValue("@Nombre", nombre);
                            cmd.Parameters.AddWithValue("@CorreoElectronico", correo);
                            cmd.Parameters.AddWithValue("@Telefono", telefono);
                            cmd.Parameters.AddWithValue("@Direccion", direccion);
                            cmd.Parameters.AddWithValue("@TipoProveedor", tipoProveedor);

                            cmd.ExecuteNonQuery();  

                            MessageBox.Show("Proveedor actualizado correctamente.");
                            ConsultarProveedores();  
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar el proveedor: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para editar.");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dgvProveedores.SelectedRows.Count > 0)
            {
                string idProveedor = dgvProveedores.SelectedRows[0].Cells["ID_Proveedor"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este proveedor?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Proveedor WHERE ID_Proveedor = @ID_Proveedor";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Proveedor", idProveedor);
                                cmd.ExecuteNonQuery();  

                                MessageBox.Show("Proveedor eliminado correctamente.");
                                ConsultarProveedores();  
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el proveedor: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un proveedor para eliminar.");
            }
        }

        private void btnagregaralmacen_Click(object sender, EventArgs e)
        {
            
            string nombreAlmacen = txtNombreAlmacen.Text.Trim();
            string ubicaciona = txtUbicacionAlmacen.Text.Trim();
            int capacidadTotala = (int)numCapacidadTotalalmacen.Value;  
           
           
            if (string.IsNullOrWhiteSpace(nombreAlmacen) || string.IsNullOrWhiteSpace(ubicaciona) || capacidadTotala <= 0)
            {
                MessageBox.Show("Por favor complete todos los campos correctamente.");
                return;
            }

          
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Almacen (NombreAlmacen, Ubicacion, CapacidadTotal, Estatus, ID_Usuario) " +
                           "VALUES (@NombreAlmacen, @Ubicacion, @CapacidadTotal, 1, @ID_Usuario)";
            int idUsuario = UsuarioSesion.ID_Usuario;
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreAlmacen", nombreAlmacen);
                        cmd.Parameters.AddWithValue("@Ubicacion", ubicaciona);
                        cmd.Parameters.AddWithValue("@CapacidadTotal", capacidadTotala);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery(); 
                        MessageBox.Show("Almacen agregado correctamente.");
                        ConsultarAlmacenes();  
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el almacen: " + ex.Message);
                }
            }
        }
        private void ConsultarAlmacenes()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";


            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT * FROM Almacen";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        SqlDataAdapter da = new SqlDataAdapter(cmd);
                        DataTable dt = new DataTable();
                        da.Fill(dt);

                        dgvAlmacenes.DataSource = dt; 
                        dgvAlmacenes2.DataSource = dt;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los proveedores: " + ex.Message);
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           
            if (dgvAlmacenes.SelectedRows.Count > 0)
            {
               
                string idAlmacen = dgvAlmacenes.SelectedRows[0].Cells["ID_Almacen"].Value.ToString();

               
                string nombreAlmacen = txtNombreAlmacen.Text.Trim();
                string ubicacion = txtUbicacionAlmacen.Text.Trim();
                int capacidadTotal = (int)numCapacidadTotalalmacen.Value;  

  
                if (string.IsNullOrWhiteSpace(nombreAlmacen) || string.IsNullOrWhiteSpace(ubicacion) || capacidadTotal <= 0)
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

               
                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Almacen SET NombreAlmacen = @NombreAlmacen, Ubicacion = @Ubicacion, CapacidadTotal = @CapacidadTotal " +
                               "WHERE ID_Almacen = @ID_Almacen";
               
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            
                            cmd.Parameters.AddWithValue("@ID_Almacen", idAlmacen);
                            cmd.Parameters.AddWithValue("@NombreAlmacen", nombreAlmacen);
                            cmd.Parameters.AddWithValue("@Ubicacion", ubicacion);
                            cmd.Parameters.AddWithValue("@CapacidadTotal", capacidadTotal);
                           

                           
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Almacen actualizado correctamente.");

                            
                            ConsultarAlmacenes();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar el almacen: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un almacen para editar.");
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (dgvAlmacenes.SelectedRows.Count > 0)
            {
                string idAlmacen = dgvAlmacenes.SelectedRows[0].Cells["ID_Almacen"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este almacen?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Almacen WHERE ID_Almacen = @ID_Almacen";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Almacen", idAlmacen);
                                cmd.ExecuteNonQuery();
                                MessageBox.Show("Almacen eliminado correctamente.");
                                ConsultarAlmacenes(); 
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el almacen: " + ex.Message);
                        }
                    }
                }
    }
        }

        private void tabInventario_Click(object sender, EventArgs e)
        {

        }
        private void CargarInventarios()
        {
            string query = "SELECT * FROM Inventario";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);
                    dgvInventarios.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar inventarios: " + ex.Message);
                }
            }
        }
        private void BtnAgregarINV_Click(object sender, EventArgs e)
        {
          
            int cantidadDisponible = (int)numCantidadDisponible.Value;
            int stockMinimo = (int)numStockMinimo.Value;

            
            int idAlmacen;
            if (!int.TryParse(txtIDAlmacen2.Text, out idAlmacen) || idAlmacen <= 0)
            {
                MessageBox.Show("Por favor ingrese un ID de Almacen válido.");
                return;
            }

            int idUsuario = UsuarioSesion.ID_Usuario;

           
            if (cantidadDisponible < 0 || stockMinimo < 0)
            {
                MessageBox.Show("Por favor complete todos los campos correctamente.");
                return;
            }

           
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Inventario (CantidadDisponible, StockMinimo, FechaUltimaActualizacion, ID_Almacen, Estatus, ID_Usuario) " +
                           "VALUES (@CantidadDisponible, @StockMinimo, GETDATE(), @ID_Almacen, 1, @ID_Usuario)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@CantidadDisponible", cantidadDisponible);
                        cmd.Parameters.AddWithValue("@StockMinimo", stockMinimo);
                        cmd.Parameters.AddWithValue("@ID_Almacen", idAlmacen);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery(); 
                        MessageBox.Show("Inventario agregado correctamente.");
                        ConsultarInventarios();  
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el inventario: " + ex.Message);
                }
            }
        }
       
        private void ConsultarInventarios()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT * FROM Inventario WHERE Estatus = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvInventarios.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar inventarios: " + ex.Message);
                }
            }
        }

        private void BtnEliminarInv_Click(object sender, EventArgs e)
        {
            if (dgvInventarios.SelectedRows.Count > 0)
            {
                string idInventario = dgvInventarios.SelectedRows[0].Cells["ID_Inventario"].Value.ToString();

               
                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este inventario?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Inventario WHERE ID_Inventario = @ID_Inventario";

                   
                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Inventario", idInventario);
                                cmd.ExecuteNonQuery();  

                                MessageBox.Show("Inventario eliminado correctamente.");
                                ConsultarInventarios(); 
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el inventario: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un inventario para eliminar.");
            }
        }

        private void BtnEditarInv_Click(object sender, EventArgs e)
        {
            
            if (dgvInventarios.SelectedRows.Count > 0)
            {
                
                string idInventario = dgvInventarios.SelectedRows[0].Cells["ID_Inventario"].Value.ToString();

                
                int cantidadDisponible = (int)numCantidadDisponible.Value;  
                int stockMinimo = (int)numStockMinimo.Value;  
                int idAlmacen;

                
                if (!int.TryParse(txtIDAlmacen2.Text.Trim(), out idAlmacen) || idAlmacen <= 0)
                {
                    MessageBox.Show("Por favor, ingrese un ID de almacén válido.");
                    return;
                }

                int idUsuario = UsuarioSesion.ID_Usuario;


                
                if (cantidadDisponible < 0 || stockMinimo < 0 || idAlmacen <= 0)
                {
                    MessageBox.Show("Por favor complete todos los campos correctamente.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Inventario SET CantidadDisponible = @CantidadDisponible, StockMinimo = @StockMinimo, FechaUltimaActualizacion = GETDATE(), ID_Almacen = @ID_Almacen, ID_Usuario = @ID_Usuario " +
                               "WHERE ID_Inventario = @ID_Inventario";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            
                            cmd.Parameters.AddWithValue("@ID_Inventario", idInventario);
                            cmd.Parameters.AddWithValue("@CantidadDisponible", cantidadDisponible);
                            cmd.Parameters.AddWithValue("@StockMinimo", stockMinimo);
                            cmd.Parameters.AddWithValue("@ID_Almacen", idAlmacen);
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            
                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Inventario actualizado correctamente.");

                            
                            ConsultarInventarios();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar el inventario: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un inventario para editar.");
            }
        }

        private void BtnAgregarPagos_Click(object sender, EventArgs e)
        {
            decimal montoTotal = numMontoTotal.Value;
            DateTime fechaEmision = dtpFechaEmision.Value;
            DateTime fechaVencimiento = dtpFechaVencimiento.Value;
            string estadoPago = cmbEstadoPago.SelectedItem?.ToString();
            int idProveedor;
            if (!int.TryParse(txtIDProveedor.Text, out idProveedor) || idProveedor <= 0)
            {
                MessageBox.Show("Por favor ingrese un ID de Proveedor válido.");
                return;
            }

            int idUsuario = UsuarioSesion.ID_Usuario;

            if (string.IsNullOrEmpty(estadoPago))
            {
                MessageBox.Show("Por favor seleccione un estado de pago.");
                return;
            }

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO CuentasPorPagar (MontoTotal, FechaEmision, FechaVencimiento, EstadoPago, ID_Proveedor, Estatus, ID_Usuario) " +
                           "VALUES (@MontoTotal, @FechaEmision, @FechaVencimiento, @EstadoPago, @ID_Proveedor, 1, @ID_Usuario)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MontoTotal", montoTotal);
                        cmd.Parameters.AddWithValue("@FechaEmision", fechaEmision);
                        cmd.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                        cmd.Parameters.AddWithValue("@EstadoPago", estadoPago);
                        cmd.Parameters.AddWithValue("@ID_Proveedor", idProveedor);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Cuenta por pagar agregada correctamente.");
                        ConsultarCuentasPorPagar();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar la cuenta por pagar: " + ex.Message);
                }
            }
        }
        private void ConsultarCuentasPorPagar()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT * FROM CuentasPorPagar WHERE Estatus = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvCuentasPorPagar.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar las cuentas por pagar: " + ex.Message);
                }
            }
        }
        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void BtnEliminarPago_Click(object sender, EventArgs e)
        {
            if (dgvCuentasPorPagar.SelectedRows.Count > 0)
            {
                string idCuenta = dgvCuentasPorPagar.SelectedRows[0].Cells["ID_CuentaPagar"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar esta cuenta por pagar?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM CuentasPorPagar WHERE ID_CuentaPagar = @ID_CuentaPagar";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_CuentaPagar", idCuenta);
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Cuenta por pagar eliminada correctamente.");
                                ConsultarCuentasPorPagar();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar la cuenta por pagar: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una cuenta por pagar para eliminar.");
            }
        }

        private void BtnEditarPago_Click(object sender, EventArgs e)
        {
            if (dgvCuentasPorPagar.SelectedRows.Count > 0)
            {
                string idCuenta = dgvCuentasPorPagar.SelectedRows[0].Cells["ID_CuentaPagar"].Value.ToString();
                decimal montoTotal = numMontoTotal.Value;
                DateTime fechaEmision = dtpFechaEmision.Value;
                DateTime fechaVencimiento = dtpFechaVencimiento.Value;
                string estadoPago = cmbEstadoPago.SelectedItem?.ToString();
                int idProveedor;

                if (!int.TryParse(txtIDProveedor.Text, out idProveedor) || idProveedor <= 0)
                {
                    MessageBox.Show("Por favor, ingrese un ID de proveedor válido.");
                    return;
                }

                int idUsuario = UsuarioSesion.ID_Usuario;

                if (string.IsNullOrEmpty(estadoPago))
                {
                    MessageBox.Show("Por favor, seleccione un estado de pago.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE CuentasPorPagar SET MontoTotal = @MontoTotal, FechaEmision = @FechaEmision, FechaVencimiento = @FechaVencimiento, " +
                               "EstadoPago = @EstadoPago, ID_Proveedor = @ID_Proveedor, ID_Usuario = @ID_Usuario WHERE ID_CuentaPagar = @ID_CuentaPagar";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_CuentaPagar", idCuenta);
                            cmd.Parameters.AddWithValue("@MontoTotal", montoTotal);
                            cmd.Parameters.AddWithValue("@FechaEmision", fechaEmision);
                            cmd.Parameters.AddWithValue("@FechaVencimiento", fechaVencimiento);
                            cmd.Parameters.AddWithValue("@EstadoPago", estadoPago);
                            cmd.Parameters.AddWithValue("@ID_Proveedor", idProveedor);
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Cuenta por pagar actualizada correctamente.");
                            ConsultarCuentasPorPagar();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar la cuenta por pagar: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione una cuenta por pagar para editar.");
            }
        }

        private void BtnAgregarSucursal_Click(object sender, EventArgs e)
        {
            string nombreSucursal = txtNombreSucursal.Text.Trim();
            string direccion = txtDireccionSucursal.Text.Trim();
            string telefono = txtTelefonoSucursal.Text.Trim();
            int idUsuario = UsuarioSesion.ID_Usuario;

           
            if (string.IsNullOrWhiteSpace(nombreSucursal) || string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(telefono))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

           
            if (telefono.Length != 10)
            {
                MessageBox.Show("Por favor ingrese un número de teléfono válido.");
                return;
            }

         
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Sucursal (NombreSucursal, Direccion, Telefono, Estatus, ID_Usuario) " +
                           "VALUES (@NombreSucursal, @Direccion, @Telefono, 1, @ID_Usuario)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreSucursal", nombreSucursal);
                        cmd.Parameters.AddWithValue("@Direccion", direccion);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Sucursal agregada correctamente.");
                            ConsultarSucursales(); 
                        }
                        else
                        {
                            MessageBox.Show("No se pudo agregar la sucursal.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar la sucursal: " + ex.Message);
                }
            }


        }
        private void ConsultarSucursales()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT * FROM Sucursal WHERE Estatus = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    
                    dgvSucursales.DataSource = dt;
                    dgvSucursales2.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar las sucursales: " + ex.Message);
                }
            }
        }

        private void BtnEliminarSucursal_Click(object sender, EventArgs e)
        {
            if (dgvSucursales.SelectedRows.Count > 0)
            {
                string idSucursal = dgvSucursales.SelectedRows[0].Cells["ID_Sucursal"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar esta sucursal?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Sucursal WHERE ID_Sucursal = @ID_Sucursal";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Sucursal", idSucursal);
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Sucursal eliminada correctamente.");
                                ConsultarSucursales();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar la sucursal: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione una sucursal para eliminar.");
            }
        }

        private void BtnEditarSucursal_Click(object sender, EventArgs e)
        {
            if (dgvSucursales.SelectedRows.Count > 0)
            {
                string idSucursal = dgvSucursales.SelectedRows[0].Cells["ID_Sucursal"].Value.ToString();
                string nombreSucursal = txtNombreSucursal.Text.Trim();
                string direccion = txtDireccionSucursal.Text.Trim();
                string telefono = txtTelefonoSucursal.Text.Trim();

                int idUsuario = UsuarioSesion.ID_Usuario;

              
                if (string.IsNullOrWhiteSpace(nombreSucursal) || string.IsNullOrWhiteSpace(direccion) || string.IsNullOrWhiteSpace(telefono))
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

               
                if (telefono.Length != 10)
                {
                    MessageBox.Show("Por favor ingrese un número de teléfono válido.");
                    return;
                }

               
                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Sucursal SET NombreSucursal = @NombreSucursal, Direccion = @Direccion, Telefono = @Telefono, ID_Usuario = @ID_Usuario " +
                               "WHERE ID_Sucursal = @ID_Sucursal";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_Sucursal", idSucursal);
                            cmd.Parameters.AddWithValue("@NombreSucursal", nombreSucursal);
                            cmd.Parameters.AddWithValue("@Direccion", direccion);
                            cmd.Parameters.AddWithValue("@Telefono", telefono);
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Sucursal actualizada correctamente.");
                            ConsultarSucursales(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar la sucursal: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione una sucursal para editar.");
            }
        }

        private void BtnAgregarEMP_Click(object sender, EventArgs e)
        {
            string nombre = txtNombreEmpleado.Text.Trim();
            string apellido = txtApellidoEmpleado.Text.Trim();
            string correoElectronico = txtCorreoElectronicoEmpleado.Text.Trim();
            string telefono = txtTelefonoEmpleado.Text.Trim();
            DateTime fechaContratacion = dtpFechaContratacionEmpleado.Value;
            string cargo = txtCargoEmpleado.Text.Trim();

            int idSucursal;
            if (!int.TryParse(txtIDSucursal2.Text.Trim(), out idSucursal) || idSucursal <= 0)
            {
                MessageBox.Show("Por favor ingrese un ID de sucursal válido.");
                return;
            }

            int idUsuario = UsuarioSesion.ID_Usuario; // Obtener el ID del usuario en sesión

            // Validar campos vacíos
            if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(correoElectronico) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(cargo))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Empleado (Nombre, Apellido, CorreoElectronico, Telefono, FechaContratacion, Cargo, ID_Sucursal, ID_Usuario, Estatus) " +
                           "VALUES (@Nombre, @Apellido, @CorreoElectronico, @Telefono, @FechaContratacion, @Cargo, @ID_Sucursal, @ID_Usuario, 1)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Nombre", nombre);
                        cmd.Parameters.AddWithValue("@Apellido", apellido);
                        cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                        cmd.Parameters.AddWithValue("@Telefono", telefono);
                        cmd.Parameters.AddWithValue("@FechaContratacion", fechaContratacion);
                        cmd.Parameters.AddWithValue("@Cargo", cargo);
                        cmd.Parameters.AddWithValue("@ID_Sucursal", idSucursal);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Empleado agregado correctamente.");
                        ConsultarEmpleados(); // Actualizar DataGridView
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el empleado: " + ex.Message);
                }
            }
        }
        private void ConsultarEmpleados()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = @"SELECT ID_Empleado, Nombre, Apellido, CorreoElectronico, Telefono, FechaContratacion, Cargo, ID_Sucursal, Estatus
                     FROM Empleado
                     WHERE Estatus = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    dgvEmpleados.DataSource = dt;
                    dgvEmpleados2.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar empleados: " + ex.Message);
                }
            }
        }

        private void btnEditarEmpl_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.SelectedRows.Count > 0)
            {
                string idEmpleado = dgvEmpleados.SelectedRows[0].Cells["ID_Empleado"].Value.ToString();

                string nombre = txtNombreEmpleado.Text.Trim();
                string apellido = txtApellidoEmpleado.Text.Trim();
                string correoElectronico = txtCorreoElectronicoEmpleado.Text.Trim();
                string telefono = txtTelefonoEmpleado.Text.Trim();
                DateTime fechaContratacion = dtpFechaContratacionEmpleado.Value;
                string cargo = txtCargoEmpleado.Text.Trim();

                int idSucursal;
                if (!int.TryParse(txtIDSucursal2.Text.Trim(), out idSucursal) || idSucursal <= 0)
                {
                    MessageBox.Show("Por favor ingrese un ID de sucursal válido.");
                    return;
                }

                int idUsuario = UsuarioSesion.ID_Usuario; 

            
                if (string.IsNullOrEmpty(nombre) || string.IsNullOrEmpty(apellido) || string.IsNullOrEmpty(correoElectronico) || string.IsNullOrEmpty(telefono) || string.IsNullOrEmpty(cargo))
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Empleado SET Nombre = @Nombre, Apellido = @Apellido, CorreoElectronico = @CorreoElectronico, Telefono = @Telefono, " +
                               "FechaContratacion = @FechaContratacion, Cargo = @Cargo, ID_Sucursal = @ID_Sucursal, ID_Usuario = @ID_Usuario " +
                               "WHERE ID_Empleado = @ID_Empleado";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                            cmd.Parameters.AddWithValue("@Nombre", nombre);
                            cmd.Parameters.AddWithValue("@Apellido", apellido);
                            cmd.Parameters.AddWithValue("@CorreoElectronico", correoElectronico);
                            cmd.Parameters.AddWithValue("@Telefono", telefono);
                            cmd.Parameters.AddWithValue("@FechaContratacion", fechaContratacion);
                            cmd.Parameters.AddWithValue("@Cargo", cargo);
                            cmd.Parameters.AddWithValue("@ID_Sucursal", idSucursal); 
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Empleado modificado correctamente.");
                            ConsultarEmpleados(); 
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al modificar el empleado: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para modificar.");
            }
        }

        private void BtnEliminarEmpl_Click(object sender, EventArgs e)
        {
            if (dgvEmpleados.SelectedRows.Count > 0)
            {
                string idEmpleado = dgvEmpleados.SelectedRows[0].Cells["ID_Empleado"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este empleado?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "UPDATE Empleado SET Estatus = 0 WHERE ID_Empleado = @ID_Empleado";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Empleado eliminado correctamente.");
                                ConsultarEmpleados(); 
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el empleado: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un empleado para eliminar.");
            }
        }

        private void BtnAgregarD_Click(object sender, EventArgs e)
        {
            string nombreDepartamento = txtNombreDepartamento.Text.Trim();
            string descripcion = txtDescripcionD.Text.Trim();

            int idEmpleado;
            if (!int.TryParse(txtIDEmpleado3.Text, out idEmpleado) || idEmpleado <= 0)
            {
                MessageBox.Show("Por favor ingrese un ID de Empleado válido.");
                return;
            }

            int idUsuario = UsuarioSesion.ID_Usuario;

            if (string.IsNullOrEmpty(nombreDepartamento) || string.IsNullOrEmpty(descripcion))
            {
                MessageBox.Show("Por favor complete todos los campos correctamente.");
                return;
            }

            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "INSERT INTO Departamento3 (NombreDepartamento, Descripcion, ID_Empleado, Estatus, ID_Usuario) " +
                           "VALUES (@NombreDepartamento, @Descripcion, @ID_Empleado, 1, @ID_Usuario)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();

                    // Validar que el ID_Empleado exista
                    string queryValidarEmpleado = "SELECT COUNT(*) FROM Empleado WHERE ID_Empleado = @ID_Empleado";
                    using (SqlCommand cmdValidar = new SqlCommand(queryValidarEmpleado, conn))
                    {
                        cmdValidar.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                        int empleadoExiste = (int)cmdValidar.ExecuteScalar();
                        if (empleadoExiste == 0)
                        {
                            MessageBox.Show("El ID del empleado no existe. Por favor, ingrese un ID válido.");
                            return;
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreDepartamento", nombreDepartamento);
                        cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                        cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Departamento agregado correctamente.");
                        ConsultarDepartamentos();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el departamento: " + ex.Message);
                }
            }
        }
        private void ConsultarDepartamentos()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT * FROM Departamento3 WHERE Estatus = 1";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);
                    dgvDepartamentos.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar los departamentos: " + ex.Message);
                }
            }
        }

        private void BtnEditard_Click(object sender, EventArgs e)
        {
            if (dgvDepartamentos.SelectedRows.Count > 0)
            {
                string idDepartamento = dgvDepartamentos.SelectedRows[0].Cells["ID_Departamento"].Value.ToString();
                string nombreDepartamento = txtNombreDepartamento.Text.Trim();
                string descripcion = txtDescripcionD.Text.Trim();

                int idEmpleado;
                if (!int.TryParse(txtIDEmpleado3.Text, out idEmpleado) || idEmpleado <= 0)
                {
                    MessageBox.Show("Por favor ingrese un ID de Empleado válido.");
                    return;
                }

                int idUsuario = UsuarioSesion.ID_Usuario;

                if (string.IsNullOrEmpty(nombreDepartamento) || string.IsNullOrEmpty(descripcion))
                {
                    MessageBox.Show("Por favor complete todos los campos correctamente.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE Departamento3 SET NombreDepartamento = @NombreDepartamento, Descripcion = @Descripcion, ID_Empleado = @ID_Empleado, ID_Usuario = @ID_Usuario " +
                               "WHERE ID_Departamento = @ID_Departamento";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();

                        // Validar que el ID_Empleado exista
                        string queryValidarEmpleado = "SELECT COUNT(*) FROM Empleado WHERE ID_Empleado = @ID_Empleado";
                        using (SqlCommand cmdValidar = new SqlCommand(queryValidarEmpleado, conn))
                        {
                            cmdValidar.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                            int empleadoExiste = (int)cmdValidar.ExecuteScalar();
                            if (empleadoExiste == 0)
                            {
                                MessageBox.Show("El ID del empleado no existe. Por favor, ingrese un ID válido.");
                                return;
                            }
                        }

                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_Departamento", idDepartamento);
                            cmd.Parameters.AddWithValue("@NombreDepartamento", nombreDepartamento);
                            cmd.Parameters.AddWithValue("@Descripcion", descripcion);
                            cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Departamento modificado correctamente.");
                            ConsultarDepartamentos();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al modificar el departamento: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un departamento para modificar.");
            }
        }

        private void btnEkiminard_Click(object sender, EventArgs e)
        {
            if (dgvDepartamentos.SelectedRows.Count > 0)
            {
                string idDepartamento = dgvDepartamentos.SelectedRows[0].Cells["ID_Departamento"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este departamento?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM Departamento3 WHERE ID_Departamento = @ID_Departamento";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Departamento", idDepartamento);
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Departamento eliminado correctamente.");
                                ConsultarDepartamentos();
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el departamento: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un departamento para eliminar.");
            }
        }

        private void label55_Click(object sender, EventArgs e)
        {

        }

        private void BtnAgregarT_Click(object sender, EventArgs e)
        {
            string nombreTurno = txtNombreTurno.Text.Trim();
            string horaInicio = txtHoraInicio.Text.Trim();
            string horaFin = txtHoraFin.Text.Trim();
            int idEmpleado = int.Parse(txtIDEmpleado5.Text.Trim());
            int idUsuario = UsuarioSesion.ID_Usuario;

            // Validaciones
            if (string.IsNullOrWhiteSpace(nombreTurno) || string.IsNullOrWhiteSpace(horaInicio) || string.IsNullOrWhiteSpace(horaFin))
            {
                MessageBox.Show("Por favor complete todos los campos.");
                return;
            }

            // Validar que las horas sean correctas
            TimeSpan startTime;
            TimeSpan endTime;
            if (!TimeSpan.TryParse(horaInicio, out startTime) || !TimeSpan.TryParse(horaFin, out endTime))
            {
                MessageBox.Show("Por favor ingrese un formato de hora válido.");
                return;
            }

            string query = "INSERT INTO TurnoTrabajo (NombreTurno, HoraInicio, HoraFin, Estatus, ID_Empleado, ID_Usuario) " +
                "VALUES (@NombreTurno, @HoraInicio, @HoraFin, 1, @ID_Empleado, @ID_Usuario)";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@NombreTurno", nombreTurno);
                        cmd.Parameters.AddWithValue("@HoraInicio", startTime);
                        cmd.Parameters.AddWithValue("@HoraFin", endTime);
                        cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                        cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Turno agregado correctamente.");
                            ConsultarTurnos(); // Actualiza la lista de turnos
                        }
                        else
                        {
                            MessageBox.Show("No se pudo agregar el turno.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al agregar el turno: " + ex.Message);
                }
            }
        }
        private void ConsultarTurnos()
        {
            string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
            string query = "SELECT * FROM TurnoTrabajo WHERE Estatus = 1"; // Solo turnos activos

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    SqlDataAdapter da = new SqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    // Actualiza los DataGridView con los datos obtenidos
                    dgvTurnos.DataSource = dt;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al consultar los turnos: " + ex.Message);
                }
            }
        }

        private void dtpHoraInicio_ValueChanged(object sender, EventArgs e)
        {
            
        }

        private void dtpHoraFin_ValueChanged(object sender, EventArgs e)
        {
           
        }

        private void btneditart_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.SelectedRows.Count > 0)
            {
                string idTurno = dgvTurnos.SelectedRows[0].Cells["ID_Turno"].Value.ToString();
                string nombreTurno = txtNombreTurno.Text.Trim();
                string horaInicio = txtHoraInicio.Text.Trim();
                string horaFin = txtHoraFin.Text.Trim();
                int idEmpleado = int.Parse(txtIDEmpleado5.Text.Trim());
                int idUsuario = UsuarioSesion.ID_Usuario;

                // Validaciones
                if (string.IsNullOrWhiteSpace(nombreTurno) || string.IsNullOrWhiteSpace(horaInicio) || string.IsNullOrWhiteSpace(horaFin))
                {
                    MessageBox.Show("Por favor complete todos los campos.");
                    return;
                }

                // Validar el formato de la hora
                TimeSpan startTime;
                TimeSpan endTime;
                if (!TimeSpan.TryParse(horaInicio, out startTime) || !TimeSpan.TryParse(horaFin, out endTime))
                {
                    MessageBox.Show("Por favor ingrese horas válidas.");
                    return;
                }

                string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                string query = "UPDATE TurnoTrabajo SET NombreTurno = @NombreTurno, HoraInicio = @HoraInicio, HoraFin = @HoraFin, ID_Empleado = @ID_Empleado, ID_Usuario = @ID_Usuario " +
                               "WHERE ID_Turno = @ID_Turno";

                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@ID_Turno", idTurno);
                            cmd.Parameters.AddWithValue("@NombreTurno", nombreTurno);
                            cmd.Parameters.AddWithValue("@HoraInicio", startTime);
                            cmd.Parameters.AddWithValue("@HoraFin", endTime);
                            cmd.Parameters.AddWithValue("@ID_Empleado", idEmpleado);
                            cmd.Parameters.AddWithValue("@ID_Usuario", idUsuario);

                            cmd.ExecuteNonQuery();
                            MessageBox.Show("Turno actualizado correctamente.");
                            ConsultarTurnos(); // Actualiza la lista de turnos
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error al editar el turno: " + ex.Message);
                    }
                }
            }
            else
            {
                MessageBox.Show("Seleccione un turno para editar.");
            }
        }

        private void btnEkiminarT_Click(object sender, EventArgs e)
        {
            if (dgvTurnos.SelectedRows.Count > 0)
            {
                string idTurno = dgvTurnos.SelectedRows[0].Cells["ID_Turno"].Value.ToString();

                DialogResult result = MessageBox.Show("¿Está seguro de que desea eliminar este turno?", "Confirmar eliminación", MessageBoxButtons.YesNo);
                if (result == DialogResult.Yes)
                {
                    string connectionString = "Server=LAPTOP-4GQN8FL3\\SQLEXPRESS02;Database=Papeleria;Integrated Security=True;";
                    string query = "DELETE FROM TurnoTrabajo WHERE ID_Turno = @ID_Turno";

                    using (SqlConnection conn = new SqlConnection(connectionString))
                    {
                        try
                        {
                            conn.Open();
                            using (SqlCommand cmd = new SqlCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@ID_Turno", idTurno);
                                cmd.ExecuteNonQuery();

                                MessageBox.Show("Turno eliminado correctamente.");
                                ConsultarTurnos(); // Actualiza la lista de turnos
                            }
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Error al eliminar el turno: " + ex.Message);
                        }
                    }
                }
            }
            else
            {
                MessageBox.Show("Por favor, seleccione un turno para eliminar.");
            }
        }
    }
}




