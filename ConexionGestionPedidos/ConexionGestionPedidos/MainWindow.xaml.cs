using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace ConexionGestionPedidos
{
   
    public partial class MainWindow : Window
    {
        SqlConnection miConexionSql;
        public MainWindow()
        {
            InitializeComponent();

            //establecer cadena de conexion con el origen de datos
            //que ya configure, al momento de crear el origen de datos me dio el nombre: GestionPedidosConnectionString
            string miConexion = ConfigurationManager.ConnectionStrings["ConexionGestionPedidos.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;

            //especificar que voy a realizar consultas sql con esta bbdd
            //parametro: cadena de conexion
            miConexionSql = new SqlConnection(miConexion);

            //llamo el metodo de la consulta
            MuestraClientes();

            MuestraTodosPedidos();


        }

        //metodo para extraer los datos sql de tabla clientes...........................................................................
        private void MuestraClientes()
        {

            try
            {
                string consulta = "SELECT * FROM Cliente";

                //datatable para almacenar la info que viene de una tabla
                //usando esta conexion, me ejecutas esta consulta
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                //almacenar en un datatable la informacion que viene de la table de cliente
                //usando este adaptador vas a hacer lo siguiente
                using (miAdaptadorSql)
                {

                    DataTable clientesTabla = new DataTable();

                    //relleno la informacion que viene de ejecutar la consulta sql
                    //usando esta conexion dentro del datatable
                    miAdaptadorSql.Fill(clientesTabla);


                    //que informacion quiero ver de la tabla en el listBox
                    listaClientes.DisplayMemberPath = "nombre";

                    //cual es el campo clave de esa tabla
                    listaClientes.SelectedValuePath = "Id";

                    //cual es origen de los datos
                    listaClientes.ItemsSource = clientesTabla.DefaultView;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

            }
        }

        //cargar los pedidos en en listbox, lo llamare desde el evento.....................................

        private void MuestraPedidos()
        {
            try
            {
                //consulta parametrica, que admite un parametro que sea el cliente seleccionado
                string consulta = "SELECT * FROM Pedido P INNER JOIN Cliente C ON C.Id=P.cCliente WHERE C.Id=@ClienteId";

                //clase para ejecutar una consulta parametrica 
                SqlCommand sqlComando = new SqlCommand(consulta, miConexionSql);

                //va a recibir los registros que devuelve una consulta parametrica
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(sqlComando);

                //almacenar en un datatable la informacion que viene de la table de cliente
                //usando este adaptador vas a hacer lo siguiente
                using (miAdaptadorSql)
                {
                    //agregar el valor a parametro clienteId, seleccionado en listaClientes
                    sqlComando.Parameters.AddWithValue("@ClienteId", listaClientes.SelectedValue);

                    DataTable pedidosTabla = new DataTable();

                    //relleno la informacion que viene de ejecutar la consulta sql
                    //usando esta conexion dentro del datatable
                    miAdaptadorSql.Fill(pedidosTabla);


                    //que informacion quiero ver de la tabla en el listBox
                    pedidosClientes.DisplayMemberPath = "fechaPedido";

                    //cual es el campo clave de esa tabla
                    pedidosClientes.SelectedValuePath = "Id";

                    //cual es origen de los datos
                    pedidosClientes.ItemsSource = pedidosTabla.DefaultView;
                }

            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }

        }

        //metodo que muestra todos los pedidos..........................................
        private void MuestraTodosPedidos()
        {
            try
            {
                //consulta sql de campo nuevo calculado, campo que no existe en la tabla
                //agrego tambien todos los campos asi luego puedo usar los campos
                string consulta = "SELECT *, CONCAT(cCliente, ' ', fechaPedido, ' ', formaPago) AS INFOCOMPLETA FROM Pedido";

                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(consulta, miConexionSql);

                using (miAdaptadorSql)
                {
                    DataTable pedidosTabla = new DataTable();

                    miAdaptadorSql.Fill(pedidosTabla);

                    todosPedidos.DisplayMemberPath = "INFOCOMPLETA";

                    todosPedidos.SelectedValuePath = "Id";

                    todosPedidos.ItemsSource = pedidosTabla.DefaultView;
                }
            }catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        
        
        }

        //creado al agregar nuevo controlador en listaClientes ListBox...........................
      //muestra los pedidos del cliente seleccionado
        private void listaClientes_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            MuestraPedidos();
        }


        //borrar pedidos
        //evento creado haciendo doble click sobre el elemento Boton en xaml
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //consulta con parametro, donde le paso el id del pedido seleccionado para borrar ese pedido entero
            string consulta = "DELETE FROM Pedido WHERE Id=@PEDIDOID";

            SqlCommand miSqlComand = new SqlCommand(consulta,miConexionSql);

            //abrir la conexion
            miConexionSql.Open();

            //agregar al parametro pedidoid, lo que viene de elegir en todospedidos 
            miSqlComand.Parameters.AddWithValue("@PEDIDOID", todosPedidos.SelectedValue);

            //ejecutar 
            miSqlComand.ExecuteNonQuery();

            //cerrar conexion
            miConexionSql.Close();

            //refrescamos la ventana
            MuestraTodosPedidos();
        }

        //evento generado por darle 2click al boton insertar
        //inserto campo nombre escrito en el textbox a la tabla clientes
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            //consulta con parametro
            string consulta = "INSERT INTO Cliente (nombre) VALUES (@nombre)";

            SqlCommand miSqlComand = new SqlCommand(consulta, miConexionSql);

            //abrir la conexion
            miConexionSql.Open();

            //agregar el parametro, sacandolo del cuadro de texto InsertaCliente
            miSqlComand.Parameters.AddWithValue("@nombre", InsertaCliente.Text);

            //ejecutar 
            miSqlComand.ExecuteNonQuery();

            //cerrar conexion
            miConexionSql.Close();

            //refrescamos la ventana
            MuestraClientes();

            //borrar lo que hay en el cuadro de texto
            InsertaCliente.Text = "";
        }

        //evento generado al dar 2 click en boton de borrar
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            //consulta con parametro
            string consulta = "DELETE FROM Cliente WHERE Id=@CLIENTEID";

            SqlCommand miSqlComand = new SqlCommand(consulta, miConexionSql);

            //abrir la conexion
            miConexionSql.Open();

            //agregar al parametro lo que se elige en listaClientes
            miSqlComand.Parameters.AddWithValue("@CLIENTEID", listaClientes.SelectedValue);

            //ejecutar 
            miSqlComand.ExecuteNonQuery();

            //cerrar conexion
            miConexionSql.Close();

            //refrescamos la ventana
            MuestraClientes();
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            //parametro id del cliente
            Actualiza ventanaActualizar = new Actualiza((int)listaClientes.SelectedValue);

          

            try
            {
                string consulta = "SELECT nombre FROM Cliente WHERE Id=@ClId";

                //para procesar una consulta con parametro
                SqlCommand misqlCommand = new SqlCommand(consulta, miConexionSql);


                //datatable para almacenar la info que viene de una tabla
                SqlDataAdapter miAdaptadorSql = new SqlDataAdapter(misqlCommand);




                //rescatar el cliente cuyo id he seleccionado
                //usando este adaptador vas a hacer lo siguiente
                using (miAdaptadorSql)
                {
                    misqlCommand.Parameters.AddWithValue("@ClId", listaClientes.SelectedValue);

                    //creo una tabla virtual
                    DataTable clientesTabla = new DataTable();

                    //la lleno con lo que hay dentro del adaptador
                    miAdaptadorSql.Fill(clientesTabla);

                    //rescato el campo nombre, lo meto dentro del cuadro de texto de la ventana actualizar
                    ventanaActualizar.cuadroActualiza.Text= clientesTabla.Rows[0]["nombre"].ToString();
                }
            }
            catch (Exception e1)
            {
                MessageBox.Show(e1.ToString());

            }

            //ventana modal, 1 plano y detiene flujo de ejecucion hasta que se cierra la ventana
            ventanaActualizar.ShowDialog();

            //refresco el cuadro de texto 
            MuestraClientes();
        }
    }
}
