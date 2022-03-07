using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SortPage : ContentPage
    {
        

        public SortPage()
        {
            InitializeComponent();
        }

        

        protected override void OnAppearing()
        {
            StackLayout layout = new StackLayout() { Padding = new Thickness(5, 100, 5, 5) };
            StackLayout horizontalLayout = new StackLayout() { Orientation = StackOrientation.Horizontal };

            Label label = new Label()
            {
                Text = "Сортировать по: ",
                FontSize = 18,
                TextColor = Color.Black,
                Margin = new Thickness(0, 10, 0, 0)
            };

            Picker type_picker = new Picker()
            {
                Title = "Выберите тип сортировки",
                FontSize = 18
            };

            type_picker.Items.Add("Дате и времени");
            type_picker.Items.Add("Категории");
            type_picker.Items.Add("Доход / Расход");

            Picker dateTime_picker = new Picker() { Title = "Выберите тип сортировки по дате и времени" };

            dateTime_picker.Items.Add("Сначала новые");
            dateTime_picker.Items.Add("Сначала старые");
            dateTime_picker.Items.Add("Временной промежуток");

            Picker category_picker = new Picker() { Title = "Выберите категорию" };

            string[] categories = Transaction.used_categories;

            foreach (var item in categories)
                category_picker.Items.Add(item);

            Picker time_interval_picker = new Picker() { Title = "Выберите временной промежуток" };

            time_interval_picker.Items.Add("Сегодня");
            time_interval_picker.Items.Add("За неделю");
            time_interval_picker.Items.Add("В этом месяце");
            time_interval_picker.Items.Add("За последние три месяца");
            time_interval_picker.Items.Add("В этому год");

            Picker transaction_type_picker = new Picker() { Title = "Доход / Расход" };

            transaction_type_picker.Items.Add("Доход");
            transaction_type_picker.Items.Add("Расход");

            Button submit_button = new Button()
            {
                Text = "Сортировать",
                FontSize = 25,
                VerticalOptions = LayoutOptions.End,
                Margin = new Thickness(0, 300, 0, 0)
            };

            submit_button.Clicked += Submit;
            type_picker.SelectedIndexChanged += TypeSelectionChanged;
            dateTime_picker.SelectedIndexChanged += DateTimeSelectionChanged;

            horizontalLayout.Children.Add(label);
            horizontalLayout.Children.Add(type_picker);

            layout.Children.Add(horizontalLayout);
            layout.Children.Add(submit_button);

            Content = layout;

            // Выбор метода сортировки

            async void Submit(object sender, EventArgs e)
            {
                if (type_picker.SelectedIndex == 0)
                {
                    if (dateTime_picker.SelectedIndex == 0)
                    {
                        Transaction.sort_method = "dateTime_newest";
                        await Navigation.PopAsync();
                    }
                    else if (dateTime_picker.SelectedIndex == 1)
                    {
                        Transaction.sort_method = "dateTime_oldest";
                        await Navigation.PopAsync();
                    }
                    else if (dateTime_picker.SelectedIndex == 2)
                    {
                        if (time_interval_picker.SelectedIndex != -1)
                        {
                            Transaction.sort_method = "timeInterval:" + time_interval_picker.SelectedIndex;
                            await Navigation.PopAsync();
                        }
                        else await DisplayAlert("Ошибка!", "Выберите временной промежуток", "OK");

                    }
                    else
                        await DisplayAlert("Ошибка!", "Выберите тип сортировки по дате и времени", "OK");
                }
                else if (type_picker.SelectedIndex == 1)
                {
                    if (category_picker.SelectedIndex != -1)
                    {
                        Transaction.sort_method = "category:" + category_picker.Items[category_picker.SelectedIndex];
                        await Navigation.PopAsync();
                    }
                    else
                        await DisplayAlert("Ошибка!", "Выберите категорию", "OK");
                }
                else if (type_picker.SelectedIndex == 2)
                {
                    if (transaction_type_picker.SelectedIndex == 0)
                    {
                        Transaction.sort_method = "income";
                        await Navigation.PopAsync();
                    }
                    else if (transaction_type_picker.SelectedIndex == 1)
                    {
                        Transaction.sort_method = "expense";
                        await Navigation.PopAsync();
                    }
                    else
                        await DisplayAlert("Ошибка!", "Выберите тип, доход или расход", "OK");

                }
                else
                {
                    await DisplayAlert("Ошибка!", "Выберите тип сортировки", "OK");
                }
                
            }

            


            void TypeSelectionChanged(object sender, EventArgs e)
            {
                switch (type_picker.SelectedIndex)
                {
                    case 0:
                        layout.Children.Add(dateTime_picker);
                        try
                        {
                            layout.Children.Remove(category_picker);
                            layout.Children.Remove(transaction_type_picker);

                        }
                        catch (Exception) { }

                        layout.Children.Remove(submit_button);
                        layout.Children.Add(submit_button);
                        submit_button.Margin = new Thickness(0, 250, 0, 0);

                        break;
                    case 1:
                        layout.Children.Add(category_picker);
                        try
                        {
                            layout.Children.Remove(dateTime_picker);
                            layout.Children.Remove(time_interval_picker);
                            layout.Children.Remove(transaction_type_picker);
                           
                        }
                        catch (Exception) { }

                        layout.Children.Remove(submit_button);
                        layout.Children.Add(submit_button);
                        submit_button.Margin = new Thickness(0, 250, 0, 0);

                        break;
                    case 2:
                        layout.Children.Add(transaction_type_picker);
                        try
                        {
                            layout.Children.Remove(category_picker);
                            layout.Children.Remove(dateTime_picker);
                            layout.Children.Remove(time_interval_picker);

                        }
                        catch (Exception) { }

                        layout.Children.Remove(submit_button);
                        layout.Children.Add(submit_button);
                        submit_button.Margin = new Thickness(0, 250, 0, 0);

                        break;

                    default:
                        break;
                }
            }

            void DateTimeSelectionChanged(object sender, EventArgs e)
            {
                if (dateTime_picker.SelectedIndex == 2)
                {
                    layout.Children.Add(time_interval_picker);
                    layout.Children.Remove(submit_button);
                    layout.Children.Add(submit_button);
                    submit_button.Margin = new Thickness(0, 200, 0, 0);
                }
                else
                {
                    try
                    {
                        layout.Children.Remove(time_interval_picker);
                    }
                    catch (Exception) { }
                }
            }

        }

        
    }
}