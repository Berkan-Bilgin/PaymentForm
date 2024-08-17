using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentForm
{
    public class PaymentMethod
    {
        public int PaymentMethodId { get; set; }
        public string PaymentMethodName { get; set; }
        public string PaymentMethodValue { get; set; }

        public override string ToString()
        {
            return PaymentMethodName;
        }
    }
}
