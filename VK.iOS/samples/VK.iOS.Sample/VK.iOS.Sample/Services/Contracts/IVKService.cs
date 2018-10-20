using System;
using VK.iOS.Sample.Models;

namespace VK.iOS.Sample.Services.Contracts
{
    public interface IVKService
    {
        void Login(Action<VKUserModel, string> onLoginComplete);

        void Logout();
    }
}
