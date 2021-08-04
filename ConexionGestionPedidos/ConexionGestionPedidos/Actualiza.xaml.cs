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
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace ConexionGestionPedidos
{
  
    public partial class Actualiza : Window
    {
        SqlConnection miConexionSql;
        private int z;

        //uso el constructor como medio de transporte del Id del cliente seleccionado
        public Actualiza(int elId)
        {
            InitializeComponent();

            z = elId;

            //establecer cadena de conexion con el origen de datos
            //que ya configure, al momento de crear el origen de datos me dio el nombre: GestionPedidosConnectionString
            string miConexion = ConfigurationManager.ConnectionStrings["ConexionGestionPedidos.Properties.Settings.GestionPedidosConnectionString"].ConnectionString;

            //especificar que voy a realizar consultas sql con esta bbdd
            //parametro: cadena de conexion
            miConexionSql = new SqlConnection(miConexion);
        }
        //actualiza la informacion, lleva lo que escribo en el textbox a la bbdd
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //consulta con parametro, el id lo guardo en una variable z, viene de la otra ventana
            string consulta = "UPDATE Cliente SET nombre=@nombre WHERE Id="+ z;

            SqlCommand miSqlComand = new SqlCommand(consulta, miConexionSql);

            //abrir la conexion
            miConexionSql.Open();

            //agregar el parametro, sacandolo del cuadro de texto 
            miSqlComand.Parameters.AddWithValue("@nombre", cuadroActualiza.Text);

            //ejecutar 
            miSqlComand.ExecuteNonQuery();

            //cerrar conexion
            miConexionSql.Close();

            //cierro la venta Actualiza
            this.Close();



           

            
        }
    }
}
