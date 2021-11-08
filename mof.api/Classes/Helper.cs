using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mof.ServiceModels;
using Microsoft.IdentityModel;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using IdentityModel.Client;
using Microsoft.Extensions.Localization;
using mof.ServiceModels.Common;
using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using Newtonsoft.Json;
using System.IdentityModel.Tokens.Jwt;

namespace mof.api
{
    public class Helper
    {
        private  IConfiguration _cfg;
        public Helper(IConfiguration cfg)
        {
            _cfg = cfg;
        }
        public static ServiceModels.Common.ReturnMessage ThrowException(Exception ex, IStringLocalizer<MessageLocalizer> msglocalizer)
        {
            var ret = new ServiceModels.Common.ReturnMessage(msglocalizer);
            ret.IsCompleted = false;
            ret.AddError(ex);
            ret.AppException = ex;
            //try
            //{
            //    // Create the source, if it does not already exist.
            //    if (!EventLog.SourceExists("PDMO-API"))
            //    {
            //        //An event log source should not be created and immediately used.
            //        //There is a latency time to enable the source, it should be created
            //        //prior to executing the application that uses the source.
            //        //Execute this sample a second time to use the new source.
            //        EventLog.CreateEventSource("MySource", "MyNewLog");
            //        // The source is created.  Exit the application to allow it to be registered.
                    
            //    }

            //    // Create an EventLog instance and assign its source.
            //    EventLog myLog = new EventLog();
            //    myLog.Source = "PDMO-API";

            //    // Write an informational entry to the event log.    
            //    myLog.WriteEntry(JsonConvert.SerializeObject(ex), EventLogEntryType.Error);
            //    ret.Message.Insert(0, new MessageData { Code = "SYSTEMLOG", Message = "Success", MessageType = "Information" });
            //} catch (Exception e)
            //{
            //    ret.Message.Insert(0, new MessageData { Code = "SYSTEMLOG", Message = "Cannot write system log", EX = e, MessageType = "Error" });
            //}
            return ret;
        }
        public  async Task<ServiceModels.IdentityServerConfig> GetIDSConfig() {
            var client = new HttpClient();
            var url = _cfg.GetSection("IdentityServerUrl").Value;
            //var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            var disco = await client.GetDiscoveryDocumentAsync(url);
            var ret = new ServiceModels.IdentityServerConfig();
            ret.Client = "js";
            ret.ClientSecret = "secret";
            ret.disc = disco;
            
            return ret;
        }
        public  async Task<TokenResponse> GetIDSToken(string user, string password)
        {
            var cfg = await GetIDSConfig();
            var client = new HttpClient();
            var url = _cfg.GetSection("IdentityServerUrl").Value;
            url = url + "/connect/token";
            var tokenResponse = await client.RequestPasswordTokenAsync(new PasswordTokenRequest
            {
                Address = url, //cfg.disc.TokenEndpoint,
                ClientId = "js",
                ClientSecret = "secret",

                UserName = user,
                Password = password,
                Scope = "api1"
            });


            //var tok = await client.RequestClientCredentialsTokenAsync( new ClientCredentialsTokenRequest
            //{
            //    Address = cfg.disc.TokenEndpoint,
            //    ClientId = "js",
            //    ClientSecret = "secret",
            //    Scope = "api1"
            //});
            //var revoke = client.RevokeTokenAsync(new TokenRevocationRequest
            //{
            //    Address =  cfg.disc.RevocationEndpoint,

            //});
            //var intro = await client.IntrospectTokenAsync(new TokenIntrospectionRequest
            //{
            //    Address = cfg.disc.IntrospectionEndpoint,
            //    Token = tokenResponse.AccessToken,
            //    ClientId = "js",
            //    ClientSecret = "K7gNU3sdo+OL0wNhqoVWhr3g6s1xYv72ol/pe/Unols=",

            //});
            //var tokenHandler = new JwtSecurityTokenHandler();
            //var securityToken = tokenHandler.ReadToken(tokenResponse.AccessToken) as JwtSecurityToken;

            //var stringClaimValue = securityToken.Claims.First(claim => claim.Type == claimType).Value;
            // return stringClaimValue;
            return tokenResponse;
        }
    }
}
