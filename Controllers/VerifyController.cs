using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using Microsoft.Extensions.Configuration;

namespace _2fa.Controllers
{
	public class VerifyController : Controller
	{
		private readonly IConfiguration _configuration;

		public VerifyController(IConfiguration configuration)
		{
			_configuration = configuration;
		}

		TwofaClass helper = new TwofaClass(Startup.Configuration);
		//
		// POST: /verify/{number}
		[Route("/verify/{number}")]
		public String Number(String number)
		{
			
			ConnectionMultiplexer redis = ConnectionMultiplexer.Connect(_configuration.GetValue<string>("RedisHost"));
			IDatabase conn = redis.GetDatabase();
			String DstNumber = number;
			String value = _configuration.GetValue<string>("PhloId");
			if (value == null)
			{
				int code = helper.SendVerificationCodeSms(DstNumber, "Your verification code is __code__. Code will expire in 1 minute.");
				var key = string.Format("number:{0}:code", number);
				conn.StringSet(key, code, TimeSpan.FromSeconds(60));
			}
			else
			{
				int code = helper.SendVerificationCodePhlo(DstNumber,"sms");
				var key = string.Format("number:{0}:code", number);
				conn.StringSet(key, code, TimeSpan.FromSeconds(60));
			}
			
			Verification verification = new Verification();
			verification.status = "success";
			verification.message = "verification initiated";
			string output = JsonConvert.SerializeObject(verification);
			return output;
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