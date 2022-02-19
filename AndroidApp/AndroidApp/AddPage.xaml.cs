using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AddPage : ContentPage
    {
        

        public AddPage()
        {
            InitializeComponent();
        }


        
        protected override void OnAppearing()
        {
            StackLayout layout = new StackLayout() { Padding = 15 } ;

            if (Preferences.Get("login", null) == null)
            {
                Label warning_label = new Label()
                {
                    Text = "Для корректной работы приложения войдите в аккаунт",
                    TextColor = Color.Red,
                    FontSize = 25,
                    Padding = new Thickness(0, 200, 0, 0),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center
                };

                layout.Children.Add(warning_label);
            }
            else
            {
                
                Label info_label = new Label()
                {
                    Text = "Добавить доход или расход",
                    FontSize = 25,
                    TextColor = Color.Black,
                    Padding = new Thickness(0, 30, 0, 0),
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center
                };

                Picker type_picker = new Picker()
                { 
                    Title = "Тип", 
                    WidthRequest = 95,
                    FontSize = 25,
                    Margin = new Thickness(10, 5, 0, 5)
                };

                
                
                type_picker.Items.Add("Доход");
                type_picker.Items.Add("Расход");

                Entry sum_entry = new Entry()
                {
                    Keyboard = Keyboard.Numeric,
                    WidthRequest = 215,
                    FontSize = 25,
                    Margin = new Thickness(30, 5, 10, 5),
                    Placeholder = "Сумма"
                };
                
                StackLayout horizontalLayout = new StackLayout() { Orientation = StackOrientation.Horizontal };

                Editor comment_editor = new Editor()
                {
                    Placeholder = "Комментарий (необязательно)",
                    FontSize = 18,
                    HeightRequest = 200,
                    BackgroundColor = Color.FromHex("#def7fc"),
                };

                string[] categories = Preferences.Get("categories", Transaction.default_categories).Split(';');

                Picker category_picker = new Picker()
                {
                    Title = "Категория",
                    FontSize = 25
                };

                foreach (var item in categories)
                    category_picker.Items.Add(item);


                Button submit_button = new Button()
                { 
                    Text = "Добавить",
                    FontSize = 25,
                    Margin = new Thickness(0, 30, 0, 0)
                };

                submit_button.Clicked += Submit;


                horizontalLayout.Children.Add(type_picker);
                horizontalLayout.Children.Add(sum_entry);

                layout.Children.Add(info_label);
                layout.Children.Add(horizontalLayout);
                layout.Children.Add(comment_editor);
                layout.Children.Add(category_picker);
                layout.Children.Add(submit_button);


                async void Submit(object sender, EventArgs e)
                {
                    try
                    {
                        byte item_picked = (byte)type_picker.SelectedIndex;
                        string category = category_picker.Items[category_picker.SelectedIndex];
                        string comment = comment_editor.Text;
                        decimal sum;
                        if (sum_entry.Text != "")
                        {
                            sum = decimal.Parse(sum_entry.Text);
                            if (sum >= 0)
                            {
                                if (item_picked == 1)
                                    sum *= -1;

                                try
                                {
                                    DB db = new DB();
                                    MySqlConnection connection = db.GetConnection();
                                    db.OpenConnection();
                                    string sql_command = "INSERT INTO `money` (user, sum, category, comment, datetime) VALUES ('" + Preferences.Get("login", null) + "', " + sum + ", '" + category + "', '" + comment + "', '" + DateTime.Now.ToString("yyyy.MM.dd.hh.mm.ss") + "')";
                                    MySqlCommand command = new MySqlCommand(sql_command, connection);
                                    command.ExecuteNonQuery();
                                    db.CloseConnection();

                                    Transaction.AddTransaction(new Transaction(sum, category, comment, DateTime.Now));

                                    await DisplayAlert("Успех!", "Список доходов и расходов успешно пополнен!", "OK");
                                    OnAppearing();
                                }
                                catch (Exception)
                                {
                                    await DisplayAlert("Ошибка!", "Ошибка подключения к базе данных. Проверьте интернет соединение", "OK");
                                }

                            }
                            else 
                                await DisplayAlert("Ошибка!", "Сумма не может быть отрицательной", "OK");
                        }
                        else
                            await DisplayAlert("Ошибка!", "Введите сумму", "OK");
                    }
                    catch (Exception)
                    {
                        await DisplayAlert("Ошибка!", "Выберите тип и категорию", "OK");
                    }



                }

            }


            Content = layout;
        }
    }
}