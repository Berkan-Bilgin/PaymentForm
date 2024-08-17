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
                            string paymentMethodName = reader["PaymentMethodName"].ToString(); // Kolon adı
                            string paymentMethodValue = reader["PaymentMethodValue"].ToString();
                            int paymentMethodId = Convert.ToInt32(reader["PaymentMethodId"]);
                            list.Add(new PaymentMethod { PaymentMethodId = paymentMethodId, PaymentMethodName = paymentMethodName, PaymentMethodValue = paymentMethodValue }); // Veriyi listeye ekle
                        }
                    }
                }
            }
            comboBoxPaymentMethod.Items.AddRange(list.ToArray());
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void label2_Click(object sender, EventArgs e)
        {

        }

        private void btnPay_Click(object sender, EventArgs e)
        {

            PaymentMethod selectedPaymentMethod = comboBoxPaymentMethod.SelectedItem as PaymentMethod;
            string className = selectedPaymentMethod.PaymentMethodValue;
            string displayName = selectedPaymentMethod.PaymentMethodName;

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
                MessageBox.Show("Hata oluştu: " + results.Errors[0].ErrorText);
                return;
            }

            // Derlenen sınıfın örneğini oluşturma
            Type type = results.CompiledAssembly.GetType(className);
            object instance = Activator.CreateInstance(type);

            if (!double.TryParse(textBoxAmount.Text, out double amount))
            {
                MessageBox.Show("Lütfen geçerli bir miktar girin.");
                return;
            }

            // Metodu çağırma
            var method = type.GetMethod(methodName);
            string resultMessage = (string)method.Invoke(instance, new object[] { amount });

            // Sonucu ekrana yazdırma
            lblResult.Text = resultMessage;



        }
    }
}
