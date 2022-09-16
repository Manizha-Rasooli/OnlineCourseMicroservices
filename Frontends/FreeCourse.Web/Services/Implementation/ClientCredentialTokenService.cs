using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.AspNetCore.AccessTokenManagement;
using IdentityModel.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Implementation
{
    public class ClientCredentialTokenService : IClientCredentialTokenService
    {
        #region Private Variables
        private readonly ServiceApiSettings _serviceApiSettings;
        private readonly ClientSettings _clientSettings;
        private readonly IClientAccessTokenCache _clientAccessTokenCache;
        private readonly HttpClient _httpClient;
        private readonly ClientAccessTokenParameters clientAccessTokenParameters;
        private readonly CancellationToken cancellationToken;

        public ClientCredentialTokenService(IOptions<ServiceApiSettings> serviceApiSettings, IOptions<ClientSettings> clientSettings,
            IClientAccessTokenCache clientAccessTokenCache, HttpClient httpClient, ClientAccessTokenParameters clientAccessTokenParameters = null, CancellationToken cancellationToken = default)
        {
            _serviceApiSettings = serviceApiSettings.Value;
            _clientSettings = clientSettings.Value;
            _clientAccessTokenCache = clientAccessTokenCache;
            _httpClient = httpClient;
            this.clientAccessTokenParameters = clientAccessTokenParameters;
            this.cancellationToken = cancellationToken;
        }

        #endregion


        public async Task<string> GetToken()
        {
            var currentToken = await _clientAccessTokenCache.GetAsync("WebClientToken", clientAccessTokenParameters, cancellationToken);

            if (currentToken != null)
            {
                return currentToken.AccessToken;
            }

            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var clientCredentialTokenRequest = new ClientCredentialsTokenRequest
            {
                ClientId = _clientSettings.WebClient.ClientId,
                ClientSecret = _clientSettings.WebClient.ClientSecret,
                Address = disco.TokenEndpoint
            };

            var newToken = await _httpClient.RequestClientCredentialsTokenAsync(clientCredentialTokenRequest);

            if (newToken.IsError)
            {
                throw newToken.Exception;
            }

            await _clientAccessTokenCache.SetAsync("WebClientToken", newToken.AccessToken, newToken.ExpiresIn, clientAccessTokenParameters, cancellationToken);

            return newToken.AccessToken;
        }
    }
}
