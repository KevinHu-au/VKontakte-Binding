using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VK.iOS.Sample.Models;
using VK.iOS.Sample.Services.Contracts;

namespace VK.iOS.Sample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        readonly IVKService _vkService;
        readonly IPageDialogService _dialogService;

        public DelegateCommand VKLoginCommand { get; set; }
        public DelegateCommand VKLogoutCommand { get; set; }

        bool _isLogedIn;
        public bool IsLogedIn
        {
            get { return _isLogedIn; }
            set { SetProperty(ref _isLogedIn, value); }
        }

        VKUserModel _vkUser;
        public VKUserModel VKUser
        {
            get { return _vkUser; }
            set { SetProperty(ref _vkUser, value); }
        }

        public MainPageViewModel(INavigationService navigationService,
                                 IVKService service,
                                 IPageDialogService dialogService)
            : base(navigationService)
        {
            _vkService = service;
            _dialogService = dialogService;

            IsLogedIn = false;
            VKLoginCommand = new DelegateCommand(VKLogin);
            VKLogoutCommand = new DelegateCommand(VKLogout);
            Title = "Main Page";
        }

        void VKLogout()
        {
            _vkService.Logout();
            IsLogedIn = false;
        }

        void VKLogin()
        {
            _vkService.Login(OnLoginComplete);
        }

        void OnLoginComplete(VKUserModel vkUser, string message)
        {
            if (vkUser != null)
            {
                VKUser = vkUser;
                IsLogedIn = true;
            }
            else
            {
                IsLogedIn = false;
                _dialogService.DisplayAlertAsync("Error", message, "Ok");
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);
            if (parameters.ContainsKey("title"))
                Title = (string)parameters["title"] + " and Prism";
        }
    }
}
