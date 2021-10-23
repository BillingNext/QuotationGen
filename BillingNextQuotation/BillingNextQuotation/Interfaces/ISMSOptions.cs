using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BillingNextQuotation.Interfaces
{  
    //It is kind of interface but breaks dotnet core if we keep it interface, so chnaging it to class, but shall stay in interfcaes folder due to it's nature
    public class ISMSOptions
    {
        public string SMSAccountIdentification { get; set; }

        public string SMSAccountFrom { get; set; }
    }
}
