using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine.SceneManagement;

namespace Network
{
    public class ClientGameManager
    {
        public const string MenuSceneName = "Menu";

        public async Task<bool> InitAsync()
        {
            // Authenticate Player
            await UnityServices.InitializeAsync();
            AuthState authState = await AuthenticationHandler.DoAuth();

            if (authState == AuthState.Authenticated)
            {
                return true;
            }
            return false;
        }

        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }
    }

}
