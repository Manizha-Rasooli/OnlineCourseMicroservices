using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using FreeCourse.Web.Services.Interfaces;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Implementation
{
    /// <summary>
    /// This class contains all the request to IdentityServer
    /// </summary>
    public class IdentityService : IIdentityService
    {
        #region Private Variables
        private readonly HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ClientSettings _clientSettings;
        private readonly ServiceApiSettings _serviceApiSettings;

        #endregion
        public IdentityService(HttpClient client, IHttpContextAccessor httpContextAccessor, IOptions<ClientSettings> clientSettings,
            IOptions<ServiceApiSettings> serviceApiSettings)
        {
            _httpClient = client;
            _httpContextAccessor = httpContextAccessor;
            _clientSettings = clientSettings.Value;
            _serviceApiSettings = serviceApiSettings.Value;
        }

        /// <summary>
        /// This Method Update Cookie and Get AccessToken By RefreshToken
        /// </summary>
        /// <returns></returns>
        public async Task<TokenResponse> GetAccessTokenByRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });

            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            RefreshTokenRequest refreshTokenRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                RefreshToken = refreshToken,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestRefreshTokenAsync(refreshTokenRequest);
            if(token.IsError)
            {
                return null;
            }

            var authenticationTokens = new List<AuthenticationToken>() {
            new AuthenticationToken{Name= OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
            new AuthenticationToken{Name= OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
            new AuthenticationToken{Name= OpenIdConnectParameterNames.ExpiresIn,
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            };

            var authenticationResult = await _httpContextAccessor.HttpContext.AuthenticateAsync();
            var properties = authenticationResult.Properties;
            properties.StoreTokens(authenticationTokens);

            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                authenticationResult.Principal, properties);

            return token;
        }

        /// <summary>
        /// This Method clear Refresh Token of Identity server from memory When the User is logout
        /// </summary>
        /// <returns></returns>
        public async Task RevokeRefreshToken()
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if (disco.IsError)
            {
                throw disco.Exception;
            }

            var refreshToken = await _httpContextAccessor.HttpContext.GetTokenAsync(OpenIdConnectParameterNames.RefreshToken);
            TokenRevocationRequest tokenRevocationRequest = new()
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                Address = disco.RevocationEndpoint,
                Token = refreshToken,
                TokenTypeHint = "refresh_token"
            };

            await _httpClient.RevokeTokenAsync(tokenRevocationRequest);

        }

        /// <summary>
        /// Cookie Based Authentication(This Method create AccessToken and RefreshToken. And keep it in the cookie)
        /// </summary>
        /// <param name="signInInput"></param>
        /// <returns></returns>
        public async Task<Response<bool>> SignIn(SignInInput signInInput)
        {
            var disco = await _httpClient.GetDiscoveryDocumentAsync(new DiscoveryDocumentRequest
            {
                Address = _serviceApiSettings.IdentityBaseUri,
                Policy = new DiscoveryPolicy { RequireHttps = false }
            });
            if(disco.IsError)
            {
                throw disco.Exception;
            }

            var passwordTokenRequest = new PasswordTokenRequest
            {
                ClientId = _clientSettings.WebClientForUser.ClientId,
                ClientSecret = _clientSettings.WebClientForUser.ClientSecret,
                UserName = signInInput.Email,
                Password = signInInput.Password,
                Address = disco.TokenEndpoint
            };

            var token = await _httpClient.RequestPasswordTokenAsync(passwordTokenRequest);

            if(token.IsError)
            {
                var reponseContent = await token.HttpResponse.Content.ReadAsStringAsync();
                var errorDto = JsonSerializer.Deserialize<ErrorDto>(reponseContent, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                return Response<bool>.Fail(errorDto.Errors, 400);
            }

            var userInfoRequest = new UserInfoRequest
            {
                Token = token.AccessToken,
                Address = disco.UserInfoEndpoint
            };

            var userInfo = await _httpClient.GetUserInfoAsync(userInfoRequest);
            if(userInfo.IsError)
            {
                throw userInfo.Exception;
            }

            #region Creating Cookie
            ClaimsIdentity claimsIdentity = new ClaimsIdentity(userInfo.Claims, CookieAuthenticationDefaults.AuthenticationScheme, "name", "role");

            ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            var authenticationProperties = new AuthenticationProperties();
            authenticationProperties.StoreTokens(new List<AuthenticationToken>() {
            new AuthenticationToken{Name= OpenIdConnectParameterNames.AccessToken, Value = token.AccessToken},
            new AuthenticationToken{Name= OpenIdConnectParameterNames.RefreshToken, Value = token.RefreshToken},
            new AuthenticationToken{Name= OpenIdConnectParameterNames.ExpiresIn, 
                Value = DateTime.Now.AddSeconds(token.ExpiresIn).ToString("o",CultureInfo.InvariantCulture)}
            });

            authenticationProperties.IsPersistent = signInInput.IsRemember;
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal, authenticationProperties);

            return Response<bool>.Success(200);
            #endregion



        }
    }
}
