using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;


namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AccountPage : ContentPage
    {
        public AccountPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            StackLayout layout = new StackLayout();


            if (Preferences.Get("login", null) == null)
            {
                Button login_button = new Button() { Text = "Войти" };
                login_button.Clicked += Login;
                Button reg_button = new Button() { Text = "Зарегистрироваться" };
                reg_button.Clicked += Register;

                layout.Children.Add(login_button);
                layout.Children.Add(reg_button);
            }
            else
            {
                Label label = new Label()
                {
                    Text = Preferences.Get("login", null),
                    HorizontalOptions = LayoutOptions.Center,
                    TextColor = Color.Green,
                    FontSize = 25
                };

                Button exit_button = new Button() { Text = "Выйти" };
                exit_button.Clicked += Exit;

                layout.Children.Add(label);
                layout.Children.Add(exit_button);
            }

            Content = layout;
        }

        // Выход тз аккаунта
        private void Exit(object sender, EventArgs e)
        {
            Preferences.Set("login", null);
            Transaction.ClearTransaction();
            OnAppearing();
        }

        // Переход на страницу логина
        private async void Login(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new LoginPage());
        }

        // Переход на страницу регистрации
        private async void Register(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new RegisterPage());
        }
    }
}