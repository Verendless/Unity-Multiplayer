using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

namespace Network
{
    public static class AuthenticationHandler
    {
        public static AuthState AuthState { get; private set; } = AuthState.NotAuthenticated;

        public static async Task<AuthState> DoAuth(int maxRetries = 5)
        {
            if (AuthState == AuthState.Authenticated) return AuthState;

            // Check if already authenticating
            if (AuthState == AuthState.Authenticating)
            {
                Debug.LogWarning("Already Authenticating!");
                await Authenticating();
                return AuthState;
            }

            await SingInAnonymouslyAsync(maxRetries);
            return AuthState;
        }

        public static async Task<AuthState> Authenticating()
        {
            while (AuthState == AuthState.Authenticating || AuthState == AuthState.NotAuthenticated)
            {
                await Task.Delay(200);
            }

            return AuthState;
        }

        private static async Task SingInAnonymouslyAsync(int maxTries)
        {
            AuthState = AuthState.Authenticating;
            int retries = 0;
            while (AuthState == AuthState.Authenticating && retries < maxTries)
            {
                // Try to Auth User
                try
                {
                    await AuthenticationService.Instance.SignInAnonymouslyAsync();

                    if (AuthenticationService.Instance.IsSignedIn && AuthenticationService.Instance.IsAuthorized)
                    {
                        AuthState = AuthState.Authenticated;
                        break;
                    }
                }
                // Catch login error
                catch (AuthenticationException authExc)
                {
                    Debug.LogError(authExc.Message);
                    AuthState = AuthState.Error;
                }
                // Catch request error
                catch (RequestFailedException reqExc)
                {
                    Debug.LogError(reqExc.Message);
                    AuthState = AuthState.Error;
                }

                retries++;
                await Task.Delay(1000);
            }

            if (AuthState != AuthState.Authenticated)
            {
                Debug.LogWarning($"Player was not signed in succesfully after {retries} tries");

                // Failed to signed in
                AuthState = AuthState.TimeOut;
            }
        }
    }

    public enum AuthState
    {
        NotAuthenticated,
        Authenticating,
        Authenticated,
        Error,
        TimeOut
    }
}

