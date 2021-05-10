/*Страница регистрации нового пользователя*/
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

using System.Net.Mail;
using System.Text.RegularExpressions;

using System.Security.Cryptography;

// id_client=mail //

namespace CourseWork {
    public partial class Registering : Page {
        public Registering() {
            InitializeComponent();
        }

        /*private int Check_mail(string mail) { // -- for email 
            try {
                MailAddress m = new MailAddress(mail);
                return 0;
            }
            catch (FormatException) { return -1; }
        }*/


        private static string GetMD5Hash(string text) {
            using (var hashAlg = MD5.Create()) {
                byte[] hash = hashAlg.ComputeHash(Encoding.UTF8.GetBytes(text));
                var builder = new StringBuilder(hash.Length * 2);
                for (int i = 0; i < hash.Length; i++) {
                    builder.Append(hash[i].ToString("X2"));
                }
                return builder.ToString();
            }
        }

        private void add_client(object sender, RoutedEventArgs e) {
            
            SqlConnection connection = new SqlConnection(@"Data Source=DESKTOP-52L8N5J\SQLEXPRESS02;;Initial Catalog=Pharmacy;" + "Integrated Security=True;Connect Timeout=15;Encrypt=False;" + "TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");
            connection.Open();

            int err = 0;

            string name_local = name.Text;
            string mail_local = mail.Text;
            string password_local = password.Password;
            string password2_local = password2.Password;

            string sqlExpression;
            SqlCommand command;
            SqlParameter id_client_param;
            SqlParameter name_param;
            SqlParameter type_param;
            SqlParameter password_param;
            SqlDataReader reader;

            Regex regex_name = new Regex(@"^[A-Za-zёЁ0-9_]+$");
            Regex regex_mail = new Regex(@"[a-zA-Z0-9_.]+[@]+[.a-zA-Z0-9]+[.]+[a-z]*[a-z]$");
            Regex regex_password = new Regex(@"^[A-Za-zёЁ0-9_]+$");

            //-------------------------------------Checks-----------------------------------------------

            if ((mail_local.Length == 0) || (!regex_mail.IsMatch(mail_local))) {
                Error_mail.Text= "неверная почта";
                err = -1;
            } else { Error_mail.Text=""; }

            if ((name_local.Length == 0) || (!regex_name.IsMatch(name_local))) {
                Error_name.Text = "неверное имя, нельзя использовать спец. символы: ./{(*#@#) и т.п.";
                err = -1;
            } else { Error_name.Text = ""; }
                        
            if ((password_local.Length < 4) || (!regex_password.IsMatch(password_local)) ) {
                Error_password.Text = "Пароль слишком короткий (4 символов) или содержит спецсимволы (*#./{@#)";
                err = -1;
            } else { Error_password.Text = ""; }

            if (password_local != password2_local) {
                Error_password_repeat.Text = "Пароли не совпадают";
                err = -1;
            } else { Error_password_repeat.Text = ""; }

            if (err == 0) {
                //-------------------------------mail-----------------------------------
                sqlExpression = "SELECT * FROM Clients WHERE client_id = @id_client_value";
                command = new SqlCommand(sqlExpression, connection);
                id_client_param = new SqlParameter("@id_client_value", mail.Text); command.Parameters.Add(id_client_param);

                reader = command.ExecuteReader();

                if (reader.HasRows) {
                    MessageBox.Show("Почта, введенная вами уже занята");
                    Error_mail.Visibility = Visibility.Visible;
                    err = -1;

                } else { Error_mail.Visibility = Visibility.Hidden; }

                connection.Close();
                connection.Open();

                //-------------------------------name------------------------------------
                sqlExpression = "SELECT * FROM Clients WHERE name = @name_value";
                command = new SqlCommand(sqlExpression, connection);
                name_param = new SqlParameter("@name_value", name.Text); command.Parameters.Add(name_param);

                reader = command.ExecuteReader();

                if (reader.HasRows) {
                    MessageBox.Show("Логин, введенный вами уже занят");
                    Error_name.Visibility = Visibility.Visible;
                    err = -1;

                } else { Error_name.Visibility = Visibility.Hidden; }

                connection.Close();
                connection.Open();


                //----------------------------add-client----------------------------------------
                if (err == 0) {
                    sqlExpression = "insert into Clients (client_id,name,type,password) VALUES(@id_client_value,@name_value,@type_value,@password_value)";
                    command = new SqlCommand(sqlExpression, connection);
                    id_client_param = new SqlParameter("@id_client_value", mail.Text); command.Parameters.Add(id_client_param);
                    name_param = new SqlParameter("@name_value", name.Text); command.Parameters.Add(name_param);
                    type_param = new SqlParameter("@type_value", "user"); command.Parameters.Add(type_param);
                    password_param = new SqlParameter("@password_value", GetMD5Hash(password.Password)); command.Parameters.Add(password_param);
                    command.ExecuteNonQuery();
                    Manager.MainFrame.GoBack();
                }
            }           
        }
    }
}
