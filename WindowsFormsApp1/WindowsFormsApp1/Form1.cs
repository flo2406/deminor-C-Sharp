using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        private Dictionary<Button,(int,int)> grid_button;
        private int[,] grid;
        private int[,] complete_grid;
        private List<Button> to_decolor;

        // complete_grid possibility
        // 0 => not click
        // 1 => click
        // 2 => flag









        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            grid_button = new Dictionary<Button, (int, int)>();
            to_decolor = new List<Button>();

            for(int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 30; j++)
                {
                    Button button = new Button();
                    button.Location = new Point(j * 28 - 1, i * 28 - 1);
                    button.Size = new Size(30, 30);
                    button.Visible = true;
                    this.Controls.Add(button);
                    button.MouseDown += Click_button;
                    button.MouseUp += Release_button;
                    grid_button.Add(button, (i, j));
                }
            }
            create_grid();
        }

        private void create_grid()
        {
            Random random = new Random();
            grid = new int[20,30];
            complete_grid = new int[20,30];

            for(int i = 0; i < 20; i++)
            {
                for(int j = 0; j < 30; j++)
                {
                    complete_grid[i, j] = 0;
                    int val = random.Next(0, 100);
                    if (val < 25)
                        grid[i, j] = 1;
                    else
                        grid[i, j] = 0;
                }
            }
        }








        private bool is_bomb(int i, int j)
        {
            if(i < 0 || i >= 20)
            {
                return false;
            }

            if(j < 0 || j >= 30)
            {
                return false;
            }

            return grid[i,j] == 1;
        }

        private int number_neighbours(int i, int j)
        {
            int sum = 0;

            if (is_bomb(i - 1, j - 1))
                sum++;

            if (is_bomb(i, j - 1))
                sum++;

            if (is_bomb(i + 1, j - 1))
                sum++;

            if (is_bomb(i - 1, j))
                sum++;

            if (is_bomb(i + 1, j))
                sum++;

            if (is_bomb(i - 1, j + 1))
                sum++;

            if (is_bomb(i, j + 1))
                sum++;

            if (is_bomb(i + 1, j + 1))
                sum++;

            return sum;
        }


        private bool is_flag(int i, int j)
        {
            if (i < 0 || i >= 20)
            {
                return false;
            }

            if (j < 0 || j >= 30)
            {
                return false;
            }

            return complete_grid[i, j] == 2;
        }

        private int flag_neighbors(int i, int j)
        {
            int sum = 0;

            if (is_flag(i - 1, j - 1))
                sum++;

            if (is_flag(i, j - 1))
                sum++;

            if (is_flag(i + 1, j - 1))
                sum++;

            if (is_flag(i - 1, j))
                sum++;

            if (is_flag(i + 1, j))
                sum++;

            if (is_flag(i - 1, j + 1))
                sum++;

            if (is_flag(i, j + 1))
                sum++;

            if (is_flag(i + 1, j + 1))
                sum++;

            return sum;
        }








        private void color_neighbours(int i, int j)
        {
            for (int a = -1 + i; a <= 1 + i; a++)
            {
                for (int b = -1 + j; b <= 1 + j; b++)
                {
                    if (a >= 0 && a < 20 && b >= 0 && b < 30 && complete_grid[a, b] == 0)
                    {
                        Button neigh = grid_button.FirstOrDefault(x => x.Value == (a, b)).Key;
                        neigh.BackColor = Color.Azure;
                        to_decolor.Add(neigh);
                    }
                }
            }
        }



        private int discover_case(Button my_button, int i, int j, bool first)
        {
            int mines = number_neighbours(i, j);
            int flags = flag_neighbors(i, j);

            if (complete_grid[i, j] == 0 && grid[i, j] == 0)
            {
                complete_grid[i, j] = 1;

                if (mines != 0)
                {
                    my_button.Text = mines.ToString();
                    my_button.TextAlign = ContentAlignment.MiddleCenter;
                }
                my_button.BackColor = Color.Violet;
            }

            if (complete_grid[i, j] == 1)
            {
                if (first)
                    color_neighbours(i, j);
                if (mines == flags)
                {
                    for (int a = i - 1; a <= i + 1; a++)
                    {
                        for (int b = j - 1; b <= j + 1; b++)
                        {
                            if ((a == i && b == j) || !(a >= 0 && a < 20 && b >= 0 && b < 30))
                                continue;

                            if (complete_grid[a, b] == 0)
                            {
                                Button button = grid_button.FirstOrDefault(x => x.Value == (a, b)).Key;
                                int val = discover_case(button, a, b, false);
                                if (val == 1)
                                    return 1;
                            }
                        }
                    }
                }

                return 0;
            }

            MessageBox.Show("boom");
            return 1;
        }


        private void put_flag(Button my_button, int i, int j)
        {
            if (my_button.BackColor != Color.Red)
            {
                complete_grid[i, j] = 2;
                my_button.BackColor = Color.Red;
                my_button.FlatAppearance.BorderColor = Color.Red;
            }
            else
            {
                complete_grid[i, j] = 0;
                my_button.BackColor = SystemColors.Control;
                my_button.UseVisualStyleBackColor = true;
            }
        }










        private void Release_button(object sender, EventArgs e)
        {
            foreach (Button button in to_decolor)
            {
                if (button.BackColor != Color.Violet)
                {
                    button.BackColor = SystemColors.Control;
                    button.UseVisualStyleBackColor = true;
                }
            }
            to_decolor.Clear();
        }





        private void Click_button(object sender, EventArgs e)
        {
            Button my_button = sender as Button;
            (int, int) pos = grid_button[my_button];
            int i = pos.Item1;
            int j = pos.Item2;

            if ((e as MouseEventArgs).Button == MouseButtons.Left && complete_grid[i,j] != 2)
            {
                discover_case(my_button, i, j, true);
            }
            if ((e as MouseEventArgs).Button == MouseButtons.Right && complete_grid[i,j] != 1)
            {
                put_flag(my_button, i, j);
            }
        }
    }
}
