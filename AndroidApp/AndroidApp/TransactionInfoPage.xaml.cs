using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace AndroidApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TransactionInfoPage : ContentPage
    {
        public TransactionInfoPage()
        {
            InitializeComponent();
        }

        // Информация о current_transaction
        protected override void OnAppearing()
        {
            StackLayout layout = new StackLayout();

            Label category_label = new Label()
            {
                Text = Transaction.current_transaction.category,
                FontSize = 25,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Label sum_label = new Label()
            {
                Text = Transaction.current_transaction.sum.ToString(),
                FontSize = 25,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            if (Transaction.current_transaction.sum >= 0)
            {
                sum_label.TextColor = Color.Green;
                sum_label.Text = "+" + sum_label.Text;
            }
            else
                sum_label.TextColor = Color.Red;



            Label comment_label = new Label()
            {
                Text = Transaction.current_transaction.comment,
                FontSize = 20,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            Label dateTime_label = new Label()
            {
                Text = Transaction.current_transaction.dateTime.ToString(),
                FontSize = 25,
                TextColor = Color.Black,
                HorizontalTextAlignment = TextAlignment.Center,
                HorizontalOptions = LayoutOptions.Center
            };

            layout.Children.Add(category_label);
            layout.Children.Add(sum_label);
            layout.Children.Add(comment_label);
            layout.Children.Add(dateTime_label);

            Content = layout;
        }
    }
}