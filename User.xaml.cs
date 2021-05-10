/*начальная страница пользователя*/
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
    public partial class User : Page {
    public class catalog_goods {
        public int product_id { get; set; }
        public string product_name { get; set; }
        public string description { get; set; }
        public string price { get; set; }
        public int count { get; set; }
        public byte[] image { get; set; }
    }

    public static string tmp { get; set; }

        // Кнопка обновить каталог
        private void Write_User_Goods_Button(object sender, RoutedEventArgs e) {

            User_Catalog_ListView.ItemsSource = null;

            List<catalog_goods> goods_list = new List<catalog_goods>();
            string client_id = tmp;

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            string sqlExpression = "SELECT * FROM Goods";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    catalog_goods st = new catalog_goods();
                    st.product_id = reader.GetInt32(0);
                    st.product_name = reader.GetString(1);
                    st.description = reader.GetValue(2).ToString();
                    st.price = reader.GetValue(3).ToString();
                    st.count = reader.GetInt32(4);
                    st.image = reader[5] as byte[];

                    goods_list.Add(st);

                }
                User_Catalog_ListView.ItemsSource = goods_list;

                reader.Close();

            } else {
                reader.Close();
            }
        }


        public User(string client_id) {
            InitializeComponent();
            tmp = client_id;
            List<catalog_goods> goods_list = new List<catalog_goods>();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            string sqlExpression = "SELECT * FROM Goods";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    catalog_goods st = new catalog_goods();
                    st.product_id = reader.GetInt32(0);
                    st.product_name = reader.GetString(1);
                    st.description = reader.GetValue(2).ToString();
                    st.price = reader.GetValue(3).ToString();
                    st.count = reader.GetInt32(4);
                    st.image = reader[5] as byte[];

                    goods_list.Add(st);

                }
                User_Catalog_ListView.ItemsSource = goods_list;

                reader.Close();

            } else {
                reader.Close();
            }
        }

        private void Basket_Button(object sender, RoutedEventArgs e) {
            string client_id = tmp;
            Manager.MainFrame.Navigate(new Admin_Basket(client_id));
        }


        private void Basket_Add_Button(object sender, RoutedEventArgs e) {

            string client_id = tmp;
            Button button = sender as Button; // !!!
            string sqlExpression;
            SqlCommand command;
            SqlParameter client_id_param;
            SqlParameter product_id_param;
            SqlParameter count_goods_param;
            SqlDataReader reader;

            int count_of_product_depot = 0;

            List<catalog_goods> goods_list = new List<catalog_goods>();


            if (button != null) {
                catalog_goods good = button.DataContext as catalog_goods; // !!!

                SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
                connection.Open();




                sqlExpression = "SELECT [count] FROM Goods WHERE product_id = @product_id_value";
                command = new SqlCommand(sqlExpression, connection);
                product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                reader = command.ExecuteReader();
                if (reader.HasRows) {
                    while (reader.Read()) {
                        count_of_product_depot = reader.GetInt32(0);

                    }
                }
                reader.Close();

                if (count_of_product_depot > 0) {


                    sqlExpression = "SELECT * FROM Basket WHERE client_id=@client_id_value AND product_id=@product_id_value";
                    command = new SqlCommand(sqlExpression, connection);
                    client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                    product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                    reader = command.ExecuteReader();
                    if (reader.HasRows) {

                        MessageBox.Show("Продукт уже в корзине");
                        reader.Close();
                    } else {
                        reader.Close();
                        sqlExpression = "insert into Basket (client_id, product_id, count_goods) VALUES(@client_id_value, @product_id_value, @count_goods_value)";
                        command = new SqlCommand(sqlExpression, connection);
                        client_id_param = new SqlParameter("@client_id_value", client_id); command.Parameters.Add(client_id_param);
                        product_id_param = new SqlParameter("@product_id_value", good.product_id); command.Parameters.Add(product_id_param);
                        count_goods_param = new SqlParameter("@count_goods_value", 1 /*good.count*/); command.Parameters.Add(count_goods_param);
                        command.ExecuteNonQuery();

                        MessageBox.Show("Продукт в корзине");
                    }
                } else { MessageBox.Show("Товар закончился"); }
            } else { MessageBox.Show("Выберите элемент в таблице"); }

            User_Catalog_ListView.SelectedItem = null;
        }



        private void Search_Button(object sender, RoutedEventArgs e) {
            List<catalog_goods> goods_list = new List<catalog_goods>();
            User_Catalog_ListView.ItemsSource = null;
            string client_id = tmp;

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            String SearchString = SearchTextBox.Text;

            string sqlExpression = "SELECT * FROM Goods WHERE product_name Like '%" + SearchString + "%'";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    catalog_goods st = new catalog_goods();
                    st.product_id = reader.GetInt32(0);
                    st.product_name = reader.GetString(1);
                    st.description = reader.GetValue(2).ToString();
                    st.price = reader.GetValue(3).ToString();
                    st.count = reader.GetInt32(4);
                    st.image = reader[5] as byte[];

                    goods_list.Add(st);

                }
                User_Catalog_ListView.ItemsSource = goods_list;

                reader.Close();

            } else {
                reader.Close();
            }
        }




        private void Del_Search_Button(object sender, RoutedEventArgs e) {
            SearchTextBox.Text = "";

            List<catalog_goods> goods_list = new List<catalog_goods>();

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            string sqlExpression = "SELECT * FROM Goods";
            SqlCommand command = new SqlCommand(sqlExpression, connection);
            SqlDataReader reader = command.ExecuteReader();
            if (reader.HasRows) {
                while (reader.Read()) {
                    catalog_goods st = new catalog_goods();
                    st.product_id = reader.GetInt32(0);
                    st.product_name = reader.GetString(1);
                    st.description = reader.GetValue(2).ToString();
                    st.price = reader.GetValue(3).ToString();
                    st.count = reader.GetInt32(4);
                    st.image = reader[5] as byte[];

                    goods_list.Add(st);

                }
                User_Catalog_ListView.ItemsSource = goods_list;

                reader.Close();

            } else {
                reader.Close();
            }
        }



        
        private void Details_Button(object sender, RoutedEventArgs e) {

            Button button = sender as Button; // !!!          
            catalog_goods good = button.DataContext as catalog_goods; // !!!

            Manager.MainFrame.Navigate(new User_Product_Details(good.product_id));
        }
    }
}
