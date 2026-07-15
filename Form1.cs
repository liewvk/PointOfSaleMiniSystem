using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;

namespace PointOfSaleMiniSystem
{
    public partial class Form1 : Form
    {
        private DataTable cartTable = new DataTable();

        private decimal currentSubtotal = 0;
        private decimal currentGrandTotal = 0;
        private decimal currentPayment = 0;
        private decimal currentChange = 0;

        private decimal GetProductPrice(string productName)
        {
            if (productName == "Notebook")
            {
                return 5.50m;
            }
            else if (productName == "Pen")
            {
                return 1.20m;
            }
            else if (productName == "USB Drive")
            {
                return 25.00m;
            }
            else if (productName == "Wireless Mouse")
            {
                return 35.50m;
            }
            else if (productName == "Keyboard")
            {
                return 45.00m;
            }
            else if (productName == "Headphones")
            {
                return 60.00m;
            }
            else
            {
                return 0;
            }
        }

        private void UpdateTotals()
        {
            currentSubtotal = 0;

            foreach (DataRow row in cartTable.Rows)
            {
                currentSubtotal += Convert.ToDecimal(row["Subtotal"]);
            }

            lblSubtotal.Text = $"RM {currentSubtotal:0.00}";

            decimal discount = 0;

            if (decimal.TryParse(txtDiscount.Text.Trim(), out decimal discountValue))
            {
                discount = discountValue;
            }

            currentGrandTotal = currentSubtotal - discount;

            if (currentGrandTotal < 0)
            {
                currentGrandTotal = 0;
            }

            lblGrandTotal.Text = $"RM {currentGrandTotal:0.00}";
        }
      
private void ClearProductInput()
        {
            cmbProduct.SelectedIndex = -1;
            txtUnitPrice.Clear();
            txtQuantity.Clear();

            cmbProduct.Focus();
        }

        public Form1()
        {
            InitializeComponent();
        }
        private void ClearSale()
        {
            cartTable.Rows.Clear();

            ClearProductInput();

            txtDiscount.Text = "0";
            txtPayment.Clear();

            currentSubtotal = 0;
            currentGrandTotal = 0;
            currentPayment = 0;
            currentChange = 0;

            lblSubtotal.Text = "RM 0.00";
            lblGrandTotal.Text = "RM 0.00";
            lblChange.Text = "RM 0.00";

            rtbReceipt.Clear();

            cmbProduct.Focus();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            cmbProduct.Items.Add("Notebook");
            cmbProduct.Items.Add("Pen");
            cmbProduct.Items.Add("USB Drive");
            cmbProduct.Items.Add("Wireless Mouse");
            cmbProduct.Items.Add("Keyboard");
            cmbProduct.Items.Add("Headphones");

            cmbProduct.SelectedIndex = -1;

            cartTable.Columns.Add("Product", typeof(string));
            cartTable.Columns.Add("Unit Price", typeof(decimal));
            cartTable.Columns.Add("Quantity", typeof(int));
            cartTable.Columns.Add("Subtotal", typeof(decimal));

            dgvCart.DataSource = cartTable;

            dgvCart.Columns["Unit Price"].DefaultCellStyle.Format = "0.00";
            dgvCart.Columns["Subtotal"].DefaultCellStyle.Format = "0.00";

            txtUnitPrice.Clear();
            txtQuantity.Clear();
            txtDiscount.Text = "0";
            txtPayment.Clear();

            UpdateTotals();

        }

        private void cmbProduct_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex < 0)
            {
                txtUnitPrice.Clear();
                return;
            }

            string productName = cmbProduct.SelectedItem.ToString();
            decimal price = GetProductPrice(productName);

            txtUnitPrice.Text = price.ToString("0.00");
            txtQuantity.Focus();

        }

        private void btnAddToCart_Click(object sender, EventArgs e)
        {
            if (cmbProduct.SelectedIndex < 0)
            {
                MessageBox.Show("Please select a product.",
                                "Missing Product",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                cmbProduct.Focus();
                return;
            }

            if (!int.TryParse(txtQuantity.Text.Trim(), out int quantity))
            {
                MessageBox.Show("Please enter a valid quantity.",
                                "Invalid Quantity",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtQuantity.Focus();
                return;
            }

            if (quantity <= 0)
            {
                MessageBox.Show("Quantity must be greater than zero.",
                                "Invalid Quantity",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtQuantity.Focus();
                return;
            }

            string productName = cmbProduct.SelectedItem.ToString();
            decimal unitPrice = GetProductPrice(productName);
            decimal subtotal = unitPrice * quantity;

            cartTable.Rows.Add(productName, unitPrice, quantity, subtotal);

            UpdateTotals();
            ClearProductInput();

            MessageBox.Show("Item added to cart.",
                            "Cart Updated",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

        }

        private void btnRemoveItem_Click(object sender, EventArgs e)
        {
            if (dgvCart.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to remove.",
                                "No Item Selected",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to remove the selected item?",
                                                  "Confirm Remove",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                int rowIndex = dgvCart.SelectedRows[0].Index;

                cartTable.Rows.RemoveAt(rowIndex);

                UpdateTotals();

                MessageBox.Show("Item removed from cart.",
                                "Cart Updated",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
            }

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("The cart is empty.",
                                "No Items",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }

            if (!decimal.TryParse(txtDiscount.Text.Trim(), out decimal discount))
            {
                MessageBox.Show("Please enter a valid discount amount.",
                                "Invalid Discount",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtDiscount.Focus();
                return;
            }

            if (discount < 0)
            {
                MessageBox.Show("Discount cannot be negative.",
                                "Invalid Discount",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtDiscount.Focus();
                return;
            }

            if (discount > currentSubtotal)
            {
                MessageBox.Show("Discount cannot be greater than subtotal.",
                                "Invalid Discount",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtDiscount.Focus();
                return;
            }

            if (!decimal.TryParse(txtPayment.Text.Trim(), out currentPayment))
            {
                MessageBox.Show("Please enter a valid payment amount.",
                                "Invalid Payment",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPayment.Focus();
                return;
            }

            if (currentPayment < 0)
            {
                MessageBox.Show("Payment cannot be negative.",
                                "Invalid Payment",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPayment.Focus();
                return;
            }

            currentGrandTotal = currentSubtotal - discount;

            if (currentPayment < currentGrandTotal)
            {
                MessageBox.Show("Payment is not enough.",
                                "Insufficient Payment",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPayment.Focus();
                return;
            }

            currentChange = currentPayment - currentGrandTotal;

            lblSubtotal.Text = $"$ {currentSubtotal:0.00}";
            lblGrandTotal.Text = $"$ {currentGrandTotal:0.00}";
            lblChange.Text = $"$ {currentChange:0.00}";

            MessageBox.Show("Payment calculated successfully.",
                            "Calculation Complete",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information);

        }

        private void btnGenerateReceipt_Click(object sender, EventArgs e)
        {
            if (cartTable.Rows.Count == 0)
            {
                MessageBox.Show("The cart is empty. Please add items before generating a receipt.",
                                "No Items",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                return;
            }

            if (currentPayment < currentGrandTotal)
            {
                MessageBox.Show("Please calculate payment before generating the receipt.",
                                "Payment Required",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);

                txtPayment.Focus();
                return;
            }

            decimal discount = 0;
            decimal.TryParse(txtDiscount.Text.Trim(), out discount);

            rtbReceipt.Clear();

            rtbReceipt.AppendText("POINT-OF-SALE RECEIPT\n");
            rtbReceipt.AppendText("--------------------------------\n");
            rtbReceipt.AppendText($"Date: {DateTime.Now}\n");
            rtbReceipt.AppendText("--------------------------------\n");

            foreach (DataRow row in cartTable.Rows)
            {
                string product = row["Product"].ToString();
                decimal unitPrice = Convert.ToDecimal(row["Unit Price"]);
                int quantity = Convert.ToInt32(row["Quantity"]);
                decimal subtotal = Convert.ToDecimal(row["Subtotal"]);

                rtbReceipt.AppendText($"{product}\n");
                rtbReceipt.AppendText($"  {quantity} x $ {unitPrice:0.00} = $ {subtotal:0.00}\n");
            }

            rtbReceipt.AppendText("--------------------------------\n");
            rtbReceipt.AppendText($"Subtotal    : $ {currentSubtotal:0.00}\n");
            rtbReceipt.AppendText($"Discount    : $ {discount:0.00}\n");
            rtbReceipt.AppendText($"Grand Total : $ {currentGrandTotal:0.00}\n");
            rtbReceipt.AppendText($"Payment     : $ {currentPayment:0.00}\n");
            rtbReceipt.AppendText($"Change      : $ {currentChange:0.00}\n");
            rtbReceipt.AppendText("--------------------------------\n");
            rtbReceipt.AppendText("Thank you for your purchase!\n");

        }

        private void btnClearSale_Click(object sender, EventArgs e)
        {

            if (cartTable.Rows.Count == 0 && rtbReceipt.Text == "")
            {
                ClearSale();
                return;
            }

            DialogResult result = MessageBox.Show("Are you sure you want to clear the current sale?",
                                                  "Confirm Clear Sale",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                ClearSale();
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {

            DialogResult result = MessageBox.Show("Are you sure you want to exit?",
                                                  "Confirm Exit",
                                                  MessageBoxButtons.YesNo,
                                                  MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }

        }
    }
}
