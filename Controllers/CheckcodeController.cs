using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace _2fa.Controllers
{
	public class CheckcodeController : Controller
	{
		private readonly IConfiguration _configuration;

		public CheckcodeController(IConfiguration configuration)
		{
			_configuration = configuration;
		}
		//
		// POST: /checkcode/{number}/{code}
		[Route("/checkcode/{number}/{code}")]
		public string Index(string number, string code)
		{
			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_configuration.GetValue<string>("RedisHost"));
			IDatabase conn = redis.GetDatabase();

			string key = $"number:{number}:code";
			var compare_code = (string)conn.StringGet(key);

			
				if (compare_code == code)
				{
					conn.KeyDelete(key);
					Verification verification = new Verification();
					verification.status = "success";
                    verification.message = "Number verified";
					string output = JsonConvert.SerializeObject(verification);
					return output;
				}
				else if(compare_code != code)
				{
					Verification verification = new Verification();
					verification.status = "failure";
					verification.message = "Number verified";
					string output = JsonConvert.SerializeObject(verification);
					return output;
				}
			
				else
				{
					Verification verification = new Verification();
					verification.status = "failure";
					verification.message = "number not found!";
					string output = JsonConvert.SerializeObject(verification);
					return output;
				}
		}

		private class Verification
		{
			public string status
			{
				get;
				internal set;
			}
			public string message
			{
				get;
				internal set;
			}
		}
	}
}