using BillingNextQuotation.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace BillingNextQuotation.Services
{
    public class AuthMessageSender:IMessageSender
    {
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(IOptions<ISMSOptions> optionsAccessor, ILogger<AuthMessageSender> logger)
        {
            _logger = logger;
            Options = optionsAccessor.Value;
        }
        public ISMSOptions Options { get; }

        public async Task<bool> SendSmsAsync(string number, string message)
        {
            try
            {
                string url = String.Format("http://api.msg91.com/api/sendhttp.php?route=4&sender={0}&mobiles={1}&authkey={2}&message={3}&country=91", Options.SMSAccountFrom, number, Options.SMSAccountIdentification, message);
                HttpClient httpClient = new HttpClient();
                var result = httpClient.GetAsync(url).Result;
                _logger.LogInformation("Message Sent: Response Code- " +result);
                return true;
            }
            catch(Exception e)
            {
                _logger.LogInformation("Error sending SMS: "+e);
                return false;
            }
        }
    }
}
