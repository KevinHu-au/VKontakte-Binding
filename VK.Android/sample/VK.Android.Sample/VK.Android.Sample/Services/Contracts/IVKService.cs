using System;
using VK.Android.Sample.Models;

namespace VK.Android.Sample.Services.Contracts
{
    public interface IVKService
    {
        void Login(Action<VKUserModel, string> onLoginComplete);

        void Logout();
    }
}
