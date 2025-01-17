﻿using System.Windows.Input;
using Xamarin.Forms;
using EcommerceTemplate.Services;
using EcommerceTemplate.Views;
using EcommerceTemplate.Resources;

namespace EcommerceTemplate.ViewModels
{
    public class MyAccountViewModel : BaseViewModel
    {
        IService service => DependencyService.Get<IService>();

        public ICommand FavoritesCommand { get; }
        public ICommand OrdersCommand { get; }
        public ICommand AddressesCommand { get; }
        public ICommand DetailsCommand { get; }
        public ICommand ChangePasswordCommand { get; }
        public ICommand LogoutCommand { get; }

        private string customerImage;
        public string CustomerImage
        {
            get => customerImage;
            set => SetProperty(ref customerImage, value);
        }
            
        private string customerName;
        public string CustomerName
        {
            get => customerName;
            set => SetProperty(ref customerName, value);
        }

        public MyAccountViewModel()
        {
            Title = AppResources.MyAccount;

            var c = service.GetCustomerAsync(Globals.LoggedCustomerId).Result;
            CustomerName = c.FullName;
            CustomerImage = c.Image;

            FavoritesCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(ProductsPage)}" +
                                              $"?{nameof(ProductsViewModel.OnlyFavorite)}=True" +
                                              $"&{nameof(ProductsViewModel.Title)}=Favorites"));

            OrdersCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(OrdersPage)}"));

            AddressesCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(AddressesPage)}"));

            DetailsCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(AccountDetailsPage)}"));

            ChangePasswordCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(ChangePasswordPage)}"));

            LogoutCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(LoginPage)}"));
        }
    }
}
