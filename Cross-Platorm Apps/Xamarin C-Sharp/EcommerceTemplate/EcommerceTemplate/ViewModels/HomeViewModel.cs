﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using EcommerceTemplate.Models;
using Xamarin.Forms;
using EcommerceTemplate.Services;
using EcommerceTemplate.Views;

namespace EcommerceTemplate.ViewModels
{
    public class HomeViewModel : BaseViewModel
    {
        IService service = DependencyService.Get<IService>();

        public ObservableCollection<Banner> Banners { get; }
        public ObservableCollection<Category> Categories { get; }
        public ObservableCollection<ProductViewModel> NewItems { get; }
        public ObservableCollection<ProductViewModel> FeaturedItems { get; }
        public ObservableCollection<ProductViewModel> PopularItems { get; }
        public Dictionary<string, ProductViewModel> flyWeight { get; }

        public Command LoadPageCommand { get; }
        public Command ShowFeaturedItemsCommand { get; }
        public Command ShowNewItemsCommand { get; }
        public Command ShowPopularItemsCommand { get; }
        public Command<Category> CategoryCommand { get; }
        public Command<ProductViewModel> ItemCommand { get; }
        public Command<Banner> BannerCommand { get; }

        public HomeViewModel()
        {
            Banners = new ObservableCollection<Banner>();
            Categories = new ObservableCollection<Category>();
            NewItems = new ObservableCollection<ProductViewModel>();
            FeaturedItems = new ObservableCollection<ProductViewModel>();
            PopularItems = new ObservableCollection<ProductViewModel>();
            flyWeight = new Dictionary<string, ProductViewModel>();

            LoadPageCommand = new Command(async () => await ExecuteLoadPageCommand());

            ShowFeaturedItemsCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(ProductsPage)}" +
                                              $"?{nameof(ProductsViewModel.OnlyFeatured)}=True" +
                                              $"&{nameof(ProductsViewModel.Title)}=Featured Items"));

            ShowNewItemsCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(ProductsPage)}" +
                                              $"?{nameof(ProductsViewModel.OnlyNew)}=True" +
                                              $"&{nameof(ProductsViewModel.Title)}=New Items"));

            ShowPopularItemsCommand = new Command(async () =>
                await Shell.Current.GoToAsync($"{nameof(ProductsPage)}" +
                                              $"?{nameof(ProductsViewModel.OnlyPopular)}=True" +
                                              $"&{nameof(ProductsViewModel.Title)}=Popular Items"));

            ItemCommand = new Command<ProductViewModel>(OnItemSelected);
            CategoryCommand = new Command<Category>(OnCategorySelected);
            BannerCommand = new Command<Banner>(OnBannerSelected);
        }

        async Task ExecuteLoadPageCommand()
        {
            ProductViewModel pvm;

            IsBusy = true;

            flyWeight.Clear();

            Banners.Clear();
            var banners = await service.GetBannersAsync();
            foreach (var item in banners)
                Banners.Add(item);

            Categories.Clear();
            var categories = await service.GetCategoriesAsync(null);
            foreach (var item in categories)
                Categories.Add(item);

            NewItems.Clear();
            var newItems = await service.GetProductsAsync(onlyNew: true);
            foreach (var item in newItems)
            {
                if (flyWeight.TryGetValue(item.Id, out pvm) == false)
                {
                    pvm = new ProductViewModel(item);
                    flyWeight.Add(item.Id, pvm);
                }
                    
                NewItems.Add(pvm);
            }

            FeaturedItems.Clear();
            var featuredItems = await service.GetProductsAsync(onlyFeatured: true);
            foreach (var item in featuredItems)
            {
                if (flyWeight.TryGetValue(item.Id, out pvm) == false)
                {
                    pvm = new ProductViewModel(item);
                    flyWeight.Add(item.Id, pvm);
                }

                FeaturedItems.Add(pvm);
            }

            PopularItems.Clear();
            var popularItems = await service.GetProductsAsync(onlyPopular: true);
            foreach (var item in popularItems)
            {
                if (flyWeight.TryGetValue(item.Id, out pvm) == false)
                {
                    pvm = new ProductViewModel(item);
                    flyWeight.Add(item.Id, pvm);
                }

                PopularItems.Add(pvm);
            }

            IsBusy = false;
        }

        public void OnAppearing()
        {
            IsBusy = true;
        }

        async void OnBannerSelected(Banner item)
        {
            if (item == null) return;

            await Shell.Current.GoToAsync(item.GoTo);
        }

        async void OnCategorySelected(Category item)
        {
            if (item == null) return;

            await Shell.Current.GoToAsync($"{nameof(ProductsPage)}" +
                                          $"?{nameof(ProductsViewModel.CategoryIds)}={item.Id}" +
                                          $"&{nameof(ProductsViewModel.Title)}={item.Name}");
        }

        async void OnItemSelected(ProductViewModel item)
        {
            if (item == null) return;

            await Shell.Current.GoToAsync($"{nameof(ProductDetailPage)}" +
                                          $"?{nameof(ProductDetailViewModel.ProductId)}={item.Id}");
        }

    }
}
