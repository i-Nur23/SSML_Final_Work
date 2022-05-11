using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SML_Iskhakov_4211_7
{
    public partial class Form1 : Form
    {
        private static List<char> permittedSymbolsForFunction = new List<char>() { '(', ')', '^', (char)Keys.Back, '+', 'x', 'y' };
        private static HashSet<string> forbiddenCombs = new HashSet<string>() {"++","+)","(+","=^","^^","^=","^(","xy","yx"};
        public Form1()
        {
            InitializeComponent();
        }

        private int Pow3(int x)
        {
            return (x * x * x) % TrackBar.Value;
            
        }

        private int Sum(int x, int y)
        {
            return (x + y) % TrackBar.Value;
        }

        private int FindValue(int value, List <char> list, int key, SortedDictionary<int, int> dict) // если функция одной переменной
        {
            int result = 0;

            int i = key + 1;
            while (i < dict[key])
            {
                if (list[i] == '(')
                {
                    if (dict[i] != list.Count - 1 && list[dict[i] + 1] == '^')
                    {
                        result += Pow3(FindValue(value, list, i, dict));
                        i = dict[i] + 3;
                    }
                    else
                    {
                        result += FindValue(value, list, i, dict) % TrackBar.Value;
                        i = dict[i];
                    }
                }

                else if (list[i] == 'y' || list[i] == 'x')
                {
                    if (i + 1 != list.Count && list[i + 1] == '^')
                    {
                        result += Pow3(value);
                        i += 3;
                    }
                    else
                    {
                        result += value % TrackBar.Value;
                        i++;
                    }
                }

                else if (Char.IsDigit(list[i]))
                {
                    if (i + 1 != list.Count && list[i + 1] == '^')
                    {
                        result += Pow3((int)(list[i] - '0'));
                        i += 3;
                    }
                    else
                    {
                        result += (int)(list[i] - '0') % TrackBar.Value;
                        i++;
                    }
                }

                else
                {
                    i++;
                }
            }
            return result % TrackBar.Value;
        }

        private int FindValue(int valuex, int valuey, List<char> list, int key, SortedDictionary<int,int> dict) // если функция двух переменных
        {
            int result = 0;

            int i = key + 1;
            while (i < dict[key])
            {
                if (list[i] == '(')
                {
                    if (dict[i] != list.Count - 1 && list[dict[i] + 1] == '^')
                    {
                        result += Pow3(FindValue(valuex, valuey, list, i, dict));
                        i = dict[i] + 3;
                    }
                    else
                    {
                        result += FindValue(valuex, valuey, list, i, dict);
                        i = dict[i];
                    }
                }

                else if (list[i] == 'x')
                {
                    if (i + 1 != list.Count && list[i+1] == '^')
                    {
                        result += Pow3(valuex);
                        i += 3;
                    }
                    else
                    {
                        result += valuex;
                        i++;
                    }
                }
                else if (list[i] == 'y') 
                {
                    if (i + 1 != list.Count && list[i + 1] == '^')
                    {
                        result += Pow3(valuey);
                        i += 3;
                    }
                    else
                    {
                        result += valuey;
                        i ++;
                    }
                }

                else if (Char.IsDigit(list[i]))
                {
                    if (i + 1 != list.Count && list[i + 1] == '^')
                    {
                        result += Pow3((int)(list[i] - '0'));
                        i += 3;
                    }
                    else
                    {
                        result += (int)(list[i] - '0') % TrackBar.Value;
                        i++;
                    }
                }

                else
                {
                    i++;
                }
            }
            return result % TrackBar.Value;
        }

        private void ReadFunction(List<int> list, ref bool toContinue)
        {
            var funcCharsList = new List<char>();
            funcCharsList = TB_function.Text.ToList();
            
            for (int i = 0; i < funcCharsList.Count - 1; i++)
            {
                if (forbiddenCombs.Contains(funcCharsList[i].ToString() + funcCharsList[i + 1].ToString()))
                {
                    toContinue = false;
                    return;
                }
            }

            if (funcCharsList.Count == 0)
            {
                toContinue = false;
                return;
            }

            int count1 = 0;
            int count2 = 0;

            for (int i = 0; i < funcCharsList.Count; i++) // подсчёт скобок 
            {
                if (funcCharsList[i] == '(') { count1++; }
                if (funcCharsList[i] == ')') { count2++; }
            }

            if (count1 != count2) // ошибка если кол-во открывающихся скобок не совпадает с кол-вом закрывающихся
            {
                toContinue = false;
                return;
            }

            if (RB_1_var.Checked && funcCharsList.Contains('x') && funcCharsList.Contains('y')) // ошибка если функция объявлена функцией одной переменной, но в формуле 2
            {
                toContinue = false;
                return;
            }

            var StaplesDict = new SortedDictionary<int, int>();
            StaplesDict.Add(-1, funcCharsList.Count);
            var KeysList = StaplesDict.Keys.ToList();

            int counter = 0, currentFree = 0;
            int limit = funcCharsList.Count;
            for (int i = 0; i < limit; i++) // создание словаря (начло скобки; конец скобки)
            {
                if (funcCharsList[i] == '(')
                {
                    counter++;
                    currentFree = i;
                    StaplesDict.Add(currentFree, -1);
                }
                else if (funcCharsList[i] == ')')
                {
                    StaplesDict[currentFree] = i;
                    KeysList = StaplesDict.Keys.ToList();
                    if (currentFree != KeysList[0])
                    {
                        currentFree = KeysList[KeysList.IndexOf(currentFree) - 1];
                    }
                }
            }

            if (StaplesDict.Values.ToList().Contains(-1)) // возвращает ошибку если скобки разложены в неправильном порядке ")("
            {
                toContinue = false;
                return;
            }

            if (RB_2_Var.Checked)
            {
                for (int i = 0; i < TrackBar.Value; i++)
                {
                    for (int j = 0; j < TrackBar.Value; j++)
                    {
                        int result = FindValue(i, j, funcCharsList, -1, StaplesDict);
                        list.Add(result);
                    }
                }
            }

            else
            {
                for (int i = 0; i < TrackBar.Value; i++)
                {
                    int result = FindValue(i, funcCharsList, -1, StaplesDict);
                    list.Add(result);
                }
            }

        }

        private void ProccessSet(HashSet<int> set, ref bool toContinue)
        {
            set.Clear();

            var str = TB_set.ToString();

            if (TB_set.Text.ToString() == String.Empty)
            {
                toContinue = false;
                return;
            }

            var maxKey = Convert.ToInt32(TrackBar.Value.ToString());

            for (int i = maxKey; i < 10; i++)
            {
                if (str.Contains(Convert.ToChar(i)))
                {
                    toContinue = false;
                };
            }

            var strArray = str.Split(' ').ToList();
            strArray.RemoveAt(0);
            strArray.RemoveAt(0);

            foreach (var item in strArray)
            {
                if (Convert.ToInt32(item) >= maxKey)
                {
                    toContinue = false;
                }
                set.Add(Convert.ToInt32(item));
            }
        }

        private void TB_function_KeyPress(object sender, KeyPressEventArgs e)
        {
            var symbol = e.KeyChar;
            if (!(permittedSymbolsForFunction.Contains(symbol) || Char.IsDigit(symbol)))
            {
                e.Handled = true;
            }
        }

        private void TB_set_KeyPress(object sender, KeyPressEventArgs e)
        {
            var symbol = e.KeyChar;
            if (!(Char.IsDigit(symbol) || symbol == (char)Keys.Space || symbol == (char)Keys.Back))
            {
                e.Handled = true;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bool toContinue = true;

            var EpsSet = new HashSet<int>();
            var FuncResults = new List<int>();

            ProccessSet(EpsSet, ref toContinue);

            ReadFunction(FuncResults, ref toContinue);

            if (!toContinue)
            {
                MessageBox.Show("Проверьте корректность данных");
            }

            else
            {
                Form2 form2;
                if (RB_2_Var.Checked)
                {
                    form2 = new Form2(this, 2, FuncResults, EpsSet);
                }
                else
                {
                    form2 = new Form2(this, 1, FuncResults, EpsSet);
                }
                this.Hide();
                RB_1_var.Checked = true;
                TrackBar.Value = 3;
                TB_function.Clear();
                TB_set.Clear();
                form2.Show();
            }
        }
    }
}
