using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MySqlConnector;
using Xamarin.Essentials;


namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage()
        {
            InitializeComponent();

            

        }

        

        private async void Register(object sender, EventArgs e)
        {
            string login = login_entry.Text;
            string password = password_entry.Text;
            string password_rep = password_rep_entry.Text;

            if (login.Length > 3 && login.Length < 20 && password.Length > 3 && password.Length < 20 && password == password_rep)
            {
                try
                {
                    DB db = new DB();
                    MySqlConnection connection = db.GetConnection();
                    db.OpenConnection();
                    string sql_command = "SELECT * FROM `users` WHERE login = '" + login + "'";
                    MySqlCommand command = new MySqlCommand(sql_command, connection);
                    MySqlDataReader reader = command.ExecuteReader();

                    string temp = null;

                    while (reader.Read())
                    {
                        temp = reader[0].ToString();
                    }

                    db.CloseConnection();

                    if (temp == null)
                    {
                        db.OpenConnection();
                        sql_command = "INSERT INTO `users` (login, password, categories) VALUES ('" + login + "', '" + Encrypt.Sha256(password) + "', '" + Transaction.default_categories + "')";
                        command = new MySqlCommand(sql_command, connection);
                        command.ExecuteNonQuery();
                        Preferences.Set("login", login);
                        Preferences.Set("categories", Transaction.default_categories);
                        Transaction.ClearTransaction();
                        await DisplayAlert("Успех!", "Вы успешно зарегестрировались!", "Ок");
                        await Navigation.PopAsync();
                    }
                    else
                        await DisplayAlert("Ошибка!", "Пользователь с таким логином уже существует", "Ок");

                    db.CloseConnection();
                }
                catch (Exception)
                {
                    await DisplayAlert("Ошибка!", "Ошибка подключения к базе данных. Проверьте интернет соединение", "OK");
                }


            }
            else if (login.Length <= 3)
                await DisplayAlert("Ошибка!", "Логин должен содержать более 3-х символов", "Ок");
            else if (login.Length >= 20)
                await DisplayAlert("Ошибка!", "Логин должен содержать менее 20-и символов", "Ок");
            else if (password.Length <= 3)
                await DisplayAlert("Ошибка!", "Пароль должен содержать более 3-х символов", "Ок");
            else if (password.Length >= 20)
                await DisplayAlert("Ошибка!", "Пароль должен содержать менее 20-и символов", "Ок");
            else if (password != password_rep)
                await DisplayAlert("Ошибка!", "Пароли не совпадают", "Ок");
            else
                await DisplayAlert("Ошибка!", "Непредвиденная ошибка", "Ок");

        }
    }
}