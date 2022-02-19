using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void Login(object sender, EventArgs e)
        {
            try
            {
                string login = login_entry.Text;
                string password = Encrypt.Sha256(password_entry.Text);

                DB db = new DB();
                MySqlConnection connection = db.GetConnection();
                db.OpenConnection();
                string sql_command = "SELECT * FROM `users` WHERE login = '" + login + "'";
                MySqlCommand command = new MySqlCommand(sql_command, connection);
                MySqlDataReader reader = command.ExecuteReader();

                string temp_login = null;
                string temp_password = null;

                while (reader.Read())
                {
                    temp_login = reader[0].ToString();
                    temp_password = reader[1].ToString();
                    Preferences.Set("categories", reader[2].ToString());
                }

                if (temp_login == login && temp_password == password)
                {
                    Preferences.Set("login", login);
                    db.CloseConnection();
                    await DisplayAlert("Успех!", "Вы успешно вошли в аккаунт!", "Ок");

                    db = new DB();
                    connection = db.GetConnection();
                    db.OpenConnection();
                    sql_command = "SELECT * FROM `money` WHERE user = '" + Preferences.Get("login", null) + "'";
                    command = new MySqlCommand(sql_command, connection);
                    reader = command.ExecuteReader();

                    Transaction.ClearTransaction();

                    while (reader.Read())
                    {
                        string temp = reader[4].ToString();
                        string[] tempArr = temp.Split(' ');
                        int[] intDateArr = new int[3];
                        int[] intTimeArr = new int[3];
                        DateTime dateTime;
                        try
                        {
                            string[] dateArr = tempArr[0].Split('.');
                            string[] timeArr = tempArr[1].Split(':');

                            for (byte i = 0; i < 3; i++)
                            {
                                intDateArr[i] = Convert.ToInt32(dateArr[i]);
                                intTimeArr[i] = Convert.ToInt32(timeArr[i]);
                            }
                            dateTime = new DateTime(intDateArr[2], intDateArr[1], intDateArr[0], intTimeArr[0], intTimeArr[1], intTimeArr[2]);
                        }
                        catch (Exception)
                        {
                            string[] dateArr = tempArr[0].Split('/');
                            string[] timeArr = tempArr[1].Split(':');


                            for (byte i = 0; i < 3; i++)
                            {
                                intDateArr[i] = Convert.ToInt32(dateArr[i]);
                                intTimeArr[i] = Convert.ToInt32(timeArr[i]);
                            }
                            dateTime = new DateTime(intDateArr[2], intDateArr[0], intDateArr[1], intTimeArr[0], intTimeArr[1], intTimeArr[2]);
                        }



                        Transaction transaction = new Transaction((decimal)reader[1], reader[2].ToString(), reader[3].ToString(), dateTime);
                        Transaction.AddTransaction(transaction);
                    }

                    db.CloseConnection();

                    await Navigation.PopAsync();
                }
                else if (temp_login != login)
                    await DisplayAlert("Ошибка!", "Пользователя с таким логином не существует", "Ок");
                else if (temp_password != password)
                    await DisplayAlert("Ошибка!", "Неверный пароль", "Ок");
                else
                    await DisplayAlert("Ошибка!", "Непредвиденная ошибка!", "Ок");
                db.CloseConnection();
            }
            catch (Exception)
            {
                await DisplayAlert("Ошибка!", "Ошибка подключения к базе данных. Проверьте интернет соединение", "OK");
            }
        }
    }
}