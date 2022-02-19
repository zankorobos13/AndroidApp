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
    public partial class ListPage : ContentPage
    {
        StackLayout[] lay_arr = null;

        public ListPage()
        {
            InitializeComponent();
        }

        private async void SortBy(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SortPage());
        }

        private async void ShowInfo(object sender, EventArgs e)
        {
            for (int i = 0; i < lay_arr.Length; i++)
            {
                if (lay_arr[i] == ( sender as StackLayout) )
                {
                    Transaction.current_transaction = Transaction.transactions[i];
                    await Navigation.PushAsync(new TransactionInfoPage());
                }
            }

        }

        protected override void OnAppearing()
        {
            StackLayout layout = new StackLayout();

            Transaction.UploadTransactionsFromMemory();

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
                Content = layout;
            }
            else if (Transaction.transactions.Length > 0)
            {
                Button sort_button = new Button()
                {
                    Text = "Сортировать",
                    FontSize = 25,
                    Margin = new Thickness(0, 5, 0, 0)
                };

                sort_button.Clicked += SortBy;

                layout.Children.Add(sort_button);

                if (Transaction.sort_method == "dateTime_newest")
                    Transaction.SortByDateTime();
                else if (Transaction.sort_method == "dateTime_oldest")
                    Transaction.SortByDateTime(false);
                else if (Transaction.sort_method.Contains("category:"))
                    Transaction.SortByCategory(Transaction.sort_method.Split(':')[1]);
                else if (Transaction.sort_method.Contains("timeInterval:"))
                    Transaction.SortByTimeInterval((TimeInterval)Convert.ToInt32(Transaction.sort_method.Split(':')[1]));
                else if (Transaction.sort_method == "income")
                    Transaction.SortByIncomeExpense();
                else if (Transaction.sort_method == "expense")
                    Transaction.SortByIncomeExpense(false);

                if (Transaction.transactions.Length > 0)
                {

                    StackLayout[] new_lay_arr = new StackLayout[Transaction.transactions.Length];
                    lay_arr = new_lay_arr;


                    for (int i = 0; i < Transaction.transactions.Length; i++)
                    {
                        string sum = Transaction.transactions[i].sum.ToString();
                        string category = Transaction.transactions[i].category.ToString();
                        string dateTime = Transaction.transactions[i].dateTime.ToString();

                        ScrollView scrollView = new ScrollView();
                        StackLayout horizontalLayout = new StackLayout() { Orientation = StackOrientation.Horizontal };
                        StackLayout verticalLayout = new StackLayout() { Padding = new Thickness(50, 0, 0, 0) };
                        Frame frame = new Frame() { Content = horizontalLayout };
                        Label sum_label = new Label()
                        {
                            Text = sum,
                            FontSize = 25,
                            WidthRequest = 150
                        };

                        if (int.Parse(sum) >= 0)
                        {
                            sum_label.TextColor = Color.Green;
                            sum_label.Text = "+" + sum_label.Text;
                        }
                        else
                            sum_label.TextColor = Color.Red;

                        Label category_label = new Label() { Text = category };
                        Label time_label = new Label() { Text = dateTime };


                        verticalLayout.Children.Add(category_label);
                        verticalLayout.Children.Add(time_label);
                        horizontalLayout.Children.Add(sum_label);
                        horizontalLayout.Children.Add(verticalLayout);
                        layout.Children.Add(frame);
                        scrollView.Content = layout;
                        Content = scrollView;


                        TapGestureRecognizer tapGestureRecognizer = new TapGestureRecognizer();
                        tapGestureRecognizer.Tapped += ShowInfo;
                        horizontalLayout.GestureRecognizers.Add(tapGestureRecognizer);

                        lay_arr[i] = horizontalLayout;
                    }
                }
                else
                {
                    Label info_label = new Label()
                    {
                        Text = "Похоже, что у вас нет ни одной транзакции в выбранной вами области сортировки",
                        FontSize = 25,
                        Padding = new Thickness(5, 100, 5, 5),
                        HorizontalOptions = LayoutOptions.Center,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalOptions = LayoutOptions.Center,
                        VerticalTextAlignment = TextAlignment.Center,
                        TextColor = Color.Black
                    };

                    layout.Children.Add(info_label);
                    Content = layout;
                }
            }
            else
            {
                Label info_label = new Label()
                {
                    Text = "У вас нет совершенных транзакций, чтобы добавить доход или расход, зайдите в раздел \"Добавить\"",
                    FontSize = 25,
                    TextColor = Color.Black,
                    HorizontalOptions = LayoutOptions.Center,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Margin = new Thickness(5, 100, 5, 5)
                };

                layout.Children.Add(info_label);

                Content = layout;
            }
            
        }
    }   
}