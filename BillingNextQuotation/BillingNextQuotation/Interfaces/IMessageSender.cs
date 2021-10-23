using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Interfaces
{
    public interface IMessageSender
    {
        Task<bool> SendSmsAsync(string number, string message);
    }
}
