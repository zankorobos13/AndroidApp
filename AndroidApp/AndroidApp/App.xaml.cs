using System;
using Xamarin.Forms;
using Xamarin.Essentials;
using Xamarin.Forms.Xaml;
using MySqlConnector;

namespace AndroidApp
{
    
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        // Получение транзакций из базы данных
        protected override void OnStart()
        {
            if (Preferences.Get("login", null) != null)
            {

                DB db = new DB();
                MySqlConnection connection = db.GetConnection();
                db.OpenConnection();
                string sql_command = "SELECT * FROM `money` WHERE user = '" + Preferences.Get("login", null) + "'";
                MySqlCommand command = new MySqlCommand(sql_command, connection);
                MySqlDataReader reader = command.ExecuteReader();
                

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
                Transaction.RememberTransactions();
                db.CloseConnection();


            }
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}

