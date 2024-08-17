using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentForm
{
    public interface IPaymentMethod
    {
        string ProcessPayment(double amount);
    }
}
