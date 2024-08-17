using PaymentForm;
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PaymentForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FillComboBox();
        }

        private void FillComboBox()
        {
            string connectionString = "Server=localhost; Database=RadoreDB;Trusted_Connection=True; TrustServerCertificate=True";
            string query = "SELECT * FROM PaymentMethods";
            List<PaymentMethod> list = new List<PaymentMethod>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string paymentMethodName = reader["PaymentMethodName"].ToString();
                            string paymentMethodValue = reader["PaymentMethodValue"].ToString();
                            int paymentMethodId = Convert.ToInt32(reader["PaymentMethodId"]);
                            list.Add(new PaymentMethod { PaymentMethodId = paymentMethodId, PaymentMethodName = paymentMethodName, PaymentMethodValue = paymentMethodValue });
                        }
                    }
                }
            }
            comboBoxPaymentMethod.Items.AddRange(list.ToArray());
        }

        private void btnPay_Click(object sender, EventArgs e)
        {
            PaymentMethod selectedPaymentMethod = comboBoxPaymentMethod.SelectedItem as PaymentMethod;
            if (selectedPaymentMethod == null)
            {
                MessageBox.Show("Lütfen bir ödeme yöntemi seçin.");
                return;
            }

            if (!double.TryParse(textBoxAmount.Text, out double amount))
            {
                MessageBox.Show("Lütfen geçerli bir miktar girin.");
                return;
            }

            try
            {
                Type paymentType = GeneratePaymentType(selectedPaymentMethod);
                object instance = CreatePaymentInstance(paymentType);

                string resultMessage = InvokePaymentMethod(instance, paymentType, amount);
                lblResult.Text = resultMessage;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Hata oluştu: " + ex.Message);
            }
        }

        private Type GeneratePaymentType(PaymentMethod selectedPaymentMethod)
        {
            string className = selectedPaymentMethod.PaymentMethodValue;
            string interfaceName = "IPaymentMethod";
            string methodName = "ProcessPayment";

            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CompilerParameters parameters = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true
            };

            parameters.ReferencedAssemblies.Add("System.dll");
            parameters.ReferencedAssemblies.Add("System.Core.dll");
            parameters.ReferencedAssemblies.Add(typeof(IPaymentMethod).Assembly.Location);

            string code = $@"
                using PaymentForm;
                public class {className} : {interfaceName}
                {{
                    public string {methodName}(double amount)
                    {{
                       return ""Paid "" + amount.ToString(""C"") + "" using {className}"";
                    }}
                }}";

            CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
            if (results.Errors.Count > 0)
            {
                throw new Exception("Hata oluştu: " + results.Errors[0].ErrorText);
            }

            return results.CompiledAssembly.GetType(className);
        }

        private object CreatePaymentInstance(Type paymentType)
        {
            return Activator.CreateInstance(paymentType);
        }

        //private string InvokePaymentMethod(object instance, Type paymentType, double amount)
        //{
        //    string methodName = "ProcessPayment";
        //    var method = paymentType.GetMethod(methodName);
        //    return (string)method.Invoke(instance, new object[] { amount });
        //}

        private string InvokePaymentMethod(object instance, Type paymentType, double amount)
        {
            dynamic dynamicInstance = instance;
            return dynamicInstance.ProcessPayment(amount);
        }


        private void label2_Click(object sender, EventArgs e)
        {
        }

        private void label1_Click(object sender, EventArgs e)
        {
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
        }
    }
}