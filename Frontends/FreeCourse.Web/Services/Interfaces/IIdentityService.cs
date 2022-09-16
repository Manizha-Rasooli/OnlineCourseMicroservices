using FreeCourse.Shared.Dtos;
using FreeCourse.Web.Models;
using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreeCourse.Web.Services.Interfaces
{
    /// <summary>
    /// This interface contains all the request to IdentityServer
    /// </summary>
    public interface IIdentityService
    {
        /// <summary>
        /// Cookie Based Authentication(This Method create AccessToken and RefreshToken. And keep it in the cookie)
        /// </summary>
        /// <param name="signInInput"></param>
        /// <returns></returns>
        Task<Response<bool>> SignIn(SignInInput signInInput);

        /// <summary>
        /// This Method Update Cookie and Get AccessToken By RefreshToken
        /// </summary>
        /// <returns></returns>
        Task<TokenResponse> GetAccessTokenByRefreshToken();

        /// <summary>
        /// This Method clear Refresh Token of Identity server from memory When the User is logout
        /// </summary>
        /// <returns></returns>
        Task RevokeRefreshToken();
    }
}
