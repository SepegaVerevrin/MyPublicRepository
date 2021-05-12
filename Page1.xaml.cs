/*Самая первая страница входа, имеет переход на страницу регистрации, а так же на начальные страницы рядового пользователя или админа*/
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

using System.Text.RegularExpressions;
using System.Security.Cryptography;


namespace CourseWork {
    public partial class Page1 : Page {

        private static string GetMD5Hash(string text)
        {
            using (var hashAlg = MD5.Create()) // Создаем экземпляр класса реализующего алгоритм MD5
            {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(text)); // Хешируем байты строки text
                var builder = new StringBuilder(hash.Length*2); // Создаем экземпляр StringBuilder. Этот класс предназначен для эффективного конструирования строк
                for (int i = 0; i < hash.Length; i++)
                {
                    builder.Append(hash[i].ToString("X2")); // Добавляем к строке очередной байт в виде строки в 16-й системе счисления
                }
                return builder.ToString(); // Возвращаем значение хеша
            }
        }

        public Page1() {
            InitializeComponent();     

            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder {
                DataSource = @"DESKTOP-52L8N5J\SQLEXPRESS02",
                InitialCatalog = "Pharmacy",
                IntegratedSecurity = true
            };
            SqlConnection connection = new SqlConnection(builder.ConnectionString);
            connection.Open();

        }


        private void Button_Click(object sender, RoutedEventArgs e) {
            Manager.MainFrame.Navigate(new Registering());
        }


        private void Button_log_in(object sender, RoutedEventArgs e) {

            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            int err = 0;

            string name_local = name.Text;
            string password_local = password.Password;

            string sqlExpression;
            SqlCommand command;
            SqlParameter name_param;
            SqlParameter password_param;
            SqlDataReader reader;

            string client_id="";

            Regex regex_name = new Regex(@"^[A-Za-zёЁ0-9-]+$");
            Regex regex_password = new Regex(@"^[A-Za-zёЁ0-9-]+$");

            /*-------------------------Checks----------------------------------*/           
            if ((name_local.Length == 0) || (!regex_name.IsMatch(name_local))) {
                Error_name.Visibility = Visibility.Visible;
                err = -1;
            } else { Error_name.Visibility = Visibility.Hidden; }

            if ((password_local.Length == 0) || (!regex_password.IsMatch(password_local))) {
                Error_password.Visibility = Visibility.Visible;
                err = -1;
            } else { Error_password.Visibility = Visibility.Hidden; }


            if (err == 0) {

                /*---------------------------by the name we get the mail--------------------------*/
                sqlExpression = "SELECT TOP (1) Client_id FROM clients WHERE name = @name_value";
                command = new SqlCommand(sqlExpression, connection);
                name_param = new SqlParameter("@name_value", name.Text); command.Parameters.Add(name_param);

                reader = command.ExecuteReader();

                if (reader.HasRows) {
                    while (reader.Read()) {
                        client_id = reader[0].ToString();
                    }

                } else {
                    Error_name.Visibility = Visibility.Hidden;
                    err = -1;
                }
                connection.Close();
   

                if (err == 0) {

                    connection.Open();

                    /*----------------------------name and password-----------------------------------*/
                    sqlExpression = "SELECT * FROM Clients WHERE name COLLATE Cyrillic_General_CS_AS = @name_value AND password COLLATE Cyrillic_General_CS_AS = @password_value";
                    command = new SqlCommand(sqlExpression, connection);
                    name_param = new SqlParameter("@name_value", name_local); command.Parameters.Add(name_param);
                    password_param = new SqlParameter("@password_value", GetMD5Hash(password_local)); command.Parameters.Add(password_param);

                    reader = command.ExecuteReader();

                    if (reader.HasRows) {
                        Error_name.Visibility = Visibility.Hidden;
                        Error_password.Visibility = Visibility.Hidden;


                        reader.Read();
                        string type = reader.GetString(2);
                        if (type == "user") {
                            Manager.MainFrame.Navigate(new User(client_id));
                        } else if (type == "admin") {
                            Manager.MainFrame.Navigate(new Admin(client_id));
                        }
                        reader.Close();

                    } else {
                        Error_name.Visibility = Visibility.Visible;
                        Error_password.Visibility = Visibility.Visible;
                        err = -1;
                    }

                    connection.Close();

                } else {
                    Error_name.Visibility = Visibility.Visible;
                    Error_password.Visibility = Visibility.Visible;
                }
            }              
        }

        private void Forgot_My_Data_Button(object sender, RoutedEventArgs e) {
            MessageBox.Show("В разработке");
            Manager.MainFrame.Navigate(new Recovery());
        }
    }
}
