using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentForm
{
    public class CashPayment : IPaymentMethod
    {
        public string ProcessPayment(double amount)
        {
            return $"Paid {amount:C} using Cash";
        }
    }
}
