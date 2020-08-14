using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EasySystem.Models
{
    public class GetAPI
    {
        //private readonly IOptions<GetAPI> appSettings;
        //static IConfiguration _configuration;
        //static string apiKey = "";
        //public GetAPI(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    apiKey = _configuration.GetValue<string>("GetAPI:MyAPI");
        //}

        public string MyAPI { get; set; }
    }
}
