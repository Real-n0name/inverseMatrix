using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            dataGridView1.RowCount = 1;
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton1.Checked)
            {

                dataGridView1.RowCount = (int)numericUpDown1.Value;
                dataGridView1.ColumnCount = (int)numericUpDown1.Value;
                for (int i = 0; i < dataGridView1.ColumnCount; i++)
                {
                    dataGridView1.Columns[i].HeaderText = i.ToString();
                }
            }
            else
            {
                dataGridView1.RowCount = (int)numericUpDown1.Value;
                dataGridView1.ColumnCount = (int)numericUpDown1.Value;
                Random rand = new Random();
                for (int j = 0; j < dataGridView1.RowCount; j++)
                {
                    for (int i = 0; i < dataGridView1.ColumnCount; i++)
                    {
                        dataGridView1.Rows[j].Cells[i].Value = rand.Next(-100, 100);
                        dataGridView1.Columns[i].HeaderText = i.ToString();
                    }
                }

            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var myArray1 = new int[dataGridView1.RowCount, dataGridView1.ColumnCount];

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                for (int j = 0; j < dataGridView1.ColumnCount; j++)
                {
                    myArray1[i, j] = Convert.ToInt32(dataGridView1.Rows[i].Cells[j].Value);
                }
            }

            double[][] m = MatrixCreate(dataGridView1.RowCount, dataGridView1.ColumnCount);
            for(int i = 0; i < m.Length; i++)
            {
                for(int j = 0; j < m.Length; j++)
                {
                    m[i][j] = myArray1[i, j];
                }
            }

            int[] perm;
            int toggle;
            double[][] luMatrix = MatrixDecompose(m, out perm, out toggle);
            double[][] inverse = MatrixInverse(m);
            double det = MatrixDeterminant(m);

            for (int i = 0; i < inverse.Length; i++)
            {
                for (int j = 0; j < inverse.Length; j++)
                {
                    Console.WriteLine(inverse[i][j]);
                }
            }

            labelDet.Text = "Определитель матрицы = " + det;

            dataGridView3.RowCount = dataGridView1.RowCount;
            dataGridView3.ColumnCount = dataGridView1.ColumnCount;

            for (int i = 0; i < dataGridView3.RowCount; i++)
            {
                for (int j = 0; j < dataGridView3.ColumnCount; j++)
                {
                    dataGridView3.Rows[i].Cells[j].Value = inverse[i][j];
                }
            }
        }

        private double MatrixDeterminant(double[][] m)
        {
            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(m, out perm, out toggle);
            if (lum == null)
                throw new Exception("Ошибка");
            double result = toggle;
            for (int i = 0; i < lum.Length; ++i)
                result *= lum[i][i];
            return result;
        }

        private double[][] MatrixInverse(double[][] m)
        {
            int n = m.Length;
            double[][] result = MatrixDuplicate(m);

            int[] perm;
            int toggle;
            double[][] lum = MatrixDecompose(m, out perm, out toggle);
            if (lum == null)
                throw new Exception("Ошибка");

            double[] b = new double[n];
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    if (i == perm[j])
                        b[j] = 1.0;
                    else
                        b[j] = 0.0;
                }

                double[] x = HelperSolve(lum, b);

                for (int j = 0; j < n; ++j)
                    result[j][i] = x[j];
            }
            return result;
        }

        private double[] HelperSolve(double[][] luMatrix, double[] b)
        {
            int n = luMatrix.Length;
            double[] x = new double[n];
            b.CopyTo(x, 0);

            for (int i = 1; i < n; ++i)
            {
                double sum = x[i];
                for (int j = 0; j < i; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum;
            }

            x[n - 1] /= luMatrix[n - 1][n - 1];
            for (int i = n - 2; i >= 0; --i)
            {
                double sum = x[i];
                for (int j = i + 1; j < n; ++j)
                    sum -= luMatrix[i][j] * x[j];
                x[i] = sum / luMatrix[i][i];
            }

            return x;
        }

        private double[][] MatrixDecompose(double[][] m, out int[] perm, out int toggle)
        {
            int rows = m.Length;
            int cols = m[0].Length;
            if (rows != cols)
                throw new Exception("Attempt to MatrixDecompose a non-square mattrix");

            int n = rows;

            double[][] result = MatrixDuplicate(m);

            perm = new int[n];
            for (int i = 0; i < n; ++i) { perm[i] = i; }

            toggle = 1;

            for (int j = 0; j < n - 1; ++j)
            {
                double colMax = Math.Abs(result[j][j]);
                int pRow = j;
                for (int i = j + 1; i < n; ++i)
                {
                    if (result[i][j] > colMax)
                    {
                        colMax = result[i][j];
                        pRow = i;
                    }
                }

                if (pRow != j)
                {
                    double[] rowPtr = result[pRow];
                    result[pRow] = result[j];
                    result[j] = rowPtr;

                    int tmp = perm[pRow];
                    perm[pRow] = perm[j];
                    perm[j] = tmp;

                    toggle = -toggle;
                }

                if (Math.Abs(result[j][j]) < 1.0E-20)
                    return null;

                for (int i = j + 1; i < n; ++i)
                {
                    result[i][j] /= result[j][j];
                    for (int k = j + 1; k < n; ++k)
                    {
                        result[i][k] -= result[i][j] * result[j][k];
                    }
                }
            }

            return result;
        }

        private double[][] MatrixDuplicate(double[][] m)
        {
            double[][] result = MatrixCreate(m.Length, m[0].Length);
            for (int i = 0; i < m.Length; ++i)
                for (int j = 0; j < m[i].Length; ++j)
                    result[i][j] = m[i][j];
            return result;
        }

        private double[][] MatrixCreate(int rowCount, int columnCount)
        {
            double[][] result = new double[rowCount][];
            for (int i = 0; i < rowCount; ++i)
                result[i] = new double[columnCount];

            return result;
        }
    }
}
