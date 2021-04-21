using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Plivo;

namespace _2fa
{
	public class TwofaClass
	{
		public static string AuthId
		{
			get;
			set;
		}
		public static string AuthToken
		{
			get;
			set;
		}
		public static string AppNumber
		{
			get;
			set;
		}
		public static string PhloId
		{
			get;
			set;
		}
		public static PlivoApi Client
		{
			get;
			set;
		}

        public static IConfiguration _configuration;
		public TwofaClass(IConfiguration configuration)
		{
			_configuration = configuration;
			AuthId = _configuration.GetValue<string>("Credentials:AuthId");
			AuthToken = _configuration.GetValue<string>("Credentials:AuthToken");
			AppNumber = _configuration.GetValue<string>("AppNumber");
			PhloId = _configuration.GetValue<string>("PhloId");
			Client = new PlivoApi(AuthId, AuthToken);
		}
		/// <summary>
		/// Send Verification Code via SMS.
		/// </summary>
		/// <returns>The Code.</returns>
		/// <param name="DstNumber">Destinationk number.</param>
		/// <param name="Message">Message Text.</param>
		/// <param name="Code">OTP code.</param>
		public int SendVerificationCodeSms(String DstNumber, String Message)
		{
			Random r = new Random();
			var code = r.Next(999999);
            var response = Client.Message.Create(
                src: AppNumber,
                dst: new List<String> { DstNumber },
                text: Message.Replace("__code__", code.ToString()));
            return code;
		}

		/// <summary>
		/// Send Verification Code via Call.
		/// </summary>
		/// <returns>The Code.</returns>
		/// <param name="DstNumber">Destinationk number.</param>
		/// <param name="AnswerUrl">Generates SSML along with OTP code</param>

		public int SendVerificationCodeCall(String DstNumber)
		{
			Random r = new Random();
			var code = r.Next(999999);
            var response = Client.Call.Create(
                to:new List<String>{DstNumber},
                    from:AppNumber,
                    answerMethod:"POST",
                    answerUrl:"https://twofa-answerurl.herokuapp.com/answer_url/"+code);
            return code;
		}

		/// <summary>
		/// Send Verification Code via PHLO.
		/// </summary>
		/// <returns>The Code.</returns>
		/// <param name="DstNumber">Destinationk number.</param>
		/// <param name="AppNumber">SourceNumber.</param>
		/// <param name="Code">OTP code.</param>
		/// <param name="Mode">To trigger a Call or SMS via PHLO.</param>

		public int SendVerificationCodePhlo(String DstNumber, String mode)
		{
			Random r = new Random();
			var code = r.Next(999999);
            var phloClient = new PhloApi(AuthId, AuthToken);
            var phloID = PhloId;
            var phlo = phloClient.Phlo.Get(phloID); 
            var data = new Dictionary<string, object>
            {
                { "from", AppNumber },
                { "to", DstNumber },
				{ "mode", mode },
				{ "otp", code },

            };  
			phlo.Run(data);
            return code;
		}
	}
}