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

using System.Data;
using System.Data.SqlClient;

namespace CourseWork {
    
    public partial class Admin_UsersStat : Page {
        public Admin_UsersStat() {
            InitializeComponent();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            string sqlExpression = "SELECT * FROM clients";
            SqlCommand command = new SqlCommand(sqlExpression, connection);        
            command.ExecuteNonQuery();

            SqlDataAdapter Sql_Data_Adapter = new SqlDataAdapter(command);
            DataTable Data_Table = new DataTable("clients");
            Sql_Data_Adapter.Fill(Data_Table);
            Admin_UsersStat_Grid.ItemsSource = Data_Table.DefaultView;

        }

    }
}
