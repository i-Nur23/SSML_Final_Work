using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;

namespace SML_Iskhakov_4211_7

{
    delegate string SetToStr(string currentLabel);
    public partial class Form2 : Form
    {
        int numberOfVars = 0;
        List<int> funcResults;
        HashSet<int> epsSet;
        Form1 mainForm;
        int k;
        string filepath = "Results.txt";

        DataGridView table = new DataGridView();
        TextBox SKNF = new TextBox();
        Label set = new Label();
        Label isSaveSet = new Label();

        public Form2()
        {
            InitializeComponent();
        }

        private bool isSaved()
        {
            if (numberOfVars == 1)
            {
                for (int i = 0; i < funcResults.Count; i++)
                {
                    if (epsSet.Contains(Convert.ToInt32(table[0,i].Value)) && !epsSet.Contains(Convert.ToInt32(table[1, i].Value)))
                    {
                        return false;
                    }
                }
                return true;
            }

            else
            {
                for (int i = 0; i < funcResults.Count; i++)
                {
                    if (epsSet.Contains(Convert.ToInt32(table[0, i].Value)) && epsSet.Contains(Convert.ToInt32(table[1, i].Value)) && !epsSet.Contains(Convert.ToInt32(table[2, i].Value)))
                    {
                        return false;
                    }
                }
                return true;
            }
        }

        string AddNewDis(string form, int ix, int iy,int res)
        {
            if (res == 0)
            {
                return String.Concat(form, $"(~J{ix}(X) v ~J{iy}(Y))");
            }
            else
            {
                return String.Concat(form, $"({res} v ~J{ix}(X) v ~J{iy}(Y))");
            }
        }

        string AddNewDis(string form, int ix, int res)
        {
            if (res == 0)
            {
                return String.Concat(form, $"(~J{ix}(X))");
            }
            else
            {
                return String.Concat(form, $"({res} v ~J{ix}(X))");
            }
        }

        private void CreateTable()
        {

            table.Location = new Point(label3.Location.X, label3.Location.Y + 50);
            table.RowHeadersVisible = false;

            DataGridViewTextBoxColumn columnF = new DataGridViewTextBoxColumn();
            columnF.Name = "result";
            columnF.HeaderText = "Result";

            File.AppendAllText(filepath, "Таблица значений\n");

            if (numberOfVars == 1)
            {
                File.AppendAllText(filepath, "X/Y\tResult\n");
                
                DataGridViewTextBoxColumn columnXY = new DataGridViewTextBoxColumn();
                columnXY.Name = "xy";
                columnXY.HeaderText = "X/Y";

                table.Columns.AddRange(columnXY, columnF);
                table.Columns[0].Width = 60;
                table.Columns[1].Width = 60;
                table.Size = new Size(140, table.Rows[0].Height * (k+2));

                for (int i = 0; i < k; i++)
                {
                    table.Rows.Add(new DataGridViewRow());
                }

                int countRows = 0;
                while (countRows < funcResults.Count)
                {
                    for (int i = 0; i < k; i++)
                    {
                        table[0, countRows].Value = i; File.AppendAllText(filepath, i.ToString() + "\t");
                        table[1, countRows].Value = funcResults[countRows]; File.AppendAllText(filepath, funcResults[countRows].ToString() +"\n");
                        countRows++;
                    }
                }
            }

            else
            {
                File.AppendAllText(filepath, "X\tY\tResult\n");

                DataGridViewTextBoxColumn columnX = new DataGridViewTextBoxColumn();
                columnX.Name = "x";
                columnX.HeaderText = "X";

                DataGridViewTextBoxColumn columnY = new DataGridViewTextBoxColumn();
                columnY.Name = "y";
                columnY.HeaderText = "Y";

                table.Columns.AddRange(columnX, columnY, columnF);
                table.Columns[0].Width = 60;
                table.Columns[1].Width = 60;
                table.Columns[2].Width = 60;
                table.Size = new Size(200, table.Rows[0].Height * (k + 2));

                for (int i = 0; i < Math.Pow(k,2); i++)
                {
                    table.Rows.Add(new DataGridViewRow());
                }

                int countRows = 0;
                while (countRows < funcResults.Count)
                {
                    for (int i = 0; i < k; i++)
                    {
                        for (int j = 0; j < k; j++)
                        {
                            table[0, countRows].Value = i; File.AppendAllText(filepath, i.ToString() + "\t");
                            table[1, countRows].Value = j; File.AppendAllText(filepath, j.ToString() + "\t");
                            table[2, countRows].Value = funcResults[countRows]; File.AppendAllText(filepath, funcResults[countRows].ToString() + "\n");
                            countRows++;
                        }
                    }
                }
            }
        }
        private void TB_SKNF(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        void CreateSKNF()
        {
            string form = "";
            int lastDis = 0;

            File.AppendAllText(filepath, "\nАналог СКНФ:\n");

            for (int i = 0; i < funcResults.Count; i++)
            {
                if (funcResults[i] != k-1)
                {
                    lastDis = i;
                }
            }

            for (int i = 0; i < funcResults.Count; i++)
            {
                if (funcResults[i] != k-1)
                {
                    if (numberOfVars == 2)
                    {
                        form = AddNewDis(form, (int)table[0,i].Value, (int)table[1,i].Value, funcResults[i]);
                    }
                    else
                    {
                        form = AddNewDis(form, (int)table[0, i].Value, funcResults[i]);
                    }

                    if (i < lastDis)
                    {
                        form += " & ";
                    }
                }
            }

            File.AppendAllText(filepath, form+"\n");

            SKNF.Text = form;
            SKNF.Location  = new Point(label5.Location.X, label5.Location.Y + 25);
            SKNF.Width = 600;
            SKNF.ForeColor = Color.Black;
            this.SKNF.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TB_SKNF);
        }

        void CreateSet()
        {
            File.AppendAllText(filepath, "\nСохраняет ли множество?: ");
            
            set.Location = new Point(label6.Location.X + label6.Width + 15, label6.Location.Y - 5);
            set.Font = new Font("Robotic", 10, FontStyle.Regular);
            set.TextAlign = ContentAlignment.MiddleLeft;
            set.Text += "{";
            int i = 0;
            foreach (var item in epsSet)
            {
                set.Text += item.ToString();

                if (i < epsSet.Count - 1)
                {
                    set.Text += ", ";
                }
                else
                {
                    set.Text += "}";
                }

                i++;
            }

            File.AppendAllText(filepath, set.Text.ToString()+"\n");

            isSaveSet.Location = new Point(label6.Location.X, label6.Location.Y + 15);
            isSaveSet.Font = new Font("Robotic", 10, FontStyle.Regular);

            if (isSaved())
            {
                isSaveSet.Text = "Да";
            }
            else
            {
                isSaveSet.Text = "Нет";
            }

            File.AppendAllText (filepath, isSaveSet.Text.ToString()+"\n");

        }

        public Form2(Form1 form1 ,int n, List<int> list, HashSet<int> set)
        {
            InitializeComponent();
            mainForm = form1;
            numberOfVars = n;
            funcResults = list;
            epsSet = set;

            if (numberOfVars == 1)
            {
                k = list.Count;
            }
            else
            {
                k = Convert.ToInt32(Math.Sqrt(list.Count));
            }

            CreateTable();
            this.Controls.Add(table);

            CreateSKNF();
            this.Controls.Add(SKNF);

            CreateSet();
            this.Controls.Add(this.set);
            this.Controls.Add(isSaveSet);


        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            mainForm.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mainForm.Show();
            this.Dispose();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            mainForm.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Process proc = Process.Start("notepad.exe", filepath);
            proc.WaitForExit();
            proc.Close();
        }
    }
}
