using System;
using System.Windows.Forms;

namespace algorithmGenetic
{
    public partial class Form1 : Form
    {
        private int dimension;
        private int fuctionIndex;

        private int generations;
        private int populationSize;
        private int chromosomeLength;
        private double crossoverRate;
        private double mutationRate;

        private double[] bounds = new double[6];

        private double xmin;
        private double xmax;
        private double ymin;
        private double ymax;
        private double zmin;
        private double zmax;

        public Form1()
        {
            InitializeComponent();
            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            textBox1.Text = "100";
            textBox2.Text = "100";
            textBox3.Text = "10";
            textBox4.Text = "0,7";
            textBox5.Text = "0,01";
            textBox6.Text = "-5";
            textBox7.Text = "5";
            textBox8.Text = "-5";
            textBox9.Text = "5";
            textBox10.Text = "-5";
            textBox11.Text = "5";
        }

        public double FitnessFunction(double x, double y, double z)
        {
            double result = x * x;
            switch (fuctionIndex)
            {
                case 0:
                    result = (y - x * x) * (y - x * x) + 100 * (1 - x) * (1 - x);
                    return result;
                case 1:
                    result = (x - 2) * (x - 2) * (x - 2) * (x - 2) + (x - 2 * y) * (x - 2 * y);
                    return result;
                case 2:
                    result = (x - 1) * (x - 1) + (y - 3) * (y - 3) + 4 * (z + 5) * (z + 5);
                    return result;
                default:
                    return result;
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            fuctionIndex = comboBox1.SelectedIndex;
            switch (fuctionIndex)
            {
                case 0:
                    dimension = 2;

                    label9.Text = "Bound X: ";
                    textBox6.Visible = true;
                    textBox7.Visible = true;
                    label10.Text = "Bound Y: ";
                    textBox8.Visible = true;
                    textBox9.Visible = true;
                    label11.Visible = false;
                    textBox10.Visible = false;
                    textBox11.Visible = false;

                    label2.Text = "f(x,y) = (y - x^2)^2 + 100(1 - x)^2\n"; 
                    break;
                case 1:
                    dimension = 2;

                    label9.Text = "Bound X: ";
                    textBox6.Visible = true;
                    textBox7.Visible = true;
                    label10.Text = "Bound Y: ";
                    textBox8.Visible = true;
                    textBox9.Visible = true;
                    label11.Visible = false;
                    textBox10.Visible = false;
                    textBox11.Visible = false;

                    label2.Text = "f(x,y) = (x - 2)^4 + (x - 2y)^2\n";
                    break;
                case 2:
                    dimension = 3;

                    label9.Text = "Bound X: ";
                    textBox6.Visible = true;
                    textBox7.Visible = true;
                    label10.Text = "Bound Y: ";
                    textBox8.Visible = true;
                    textBox9.Visible = true;
                    label11.Text = "Bound Z: ";
                    label11.Visible = true;
                    textBox10.Visible = true;
                    textBox11.Visible = true;

                    label2.Text = "f(x,y,z) = (x - 1)^2 + (y - 3)^2 + 4(z + 5)^2\n";
                    break;
                default:
                    dimension = 1;

                    label9.Text = "Bound X: ";
                    textBox6.Visible = true;
                    textBox7.Visible = true;
                    label10.Visible = false;
                    textBox8.Visible = false;
                    textBox9.Visible = false;
                    label11.Visible = false;
                    textBox10.Visible = false;
                    textBox11.Visible = false;

                    label2.Text = "f(x) = x * x";
                    break;
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                generations = int.Parse(textBox1.Text);
                if(generations < 0)
                {
                    throw new Exception();
                }
                populationSize = int.Parse(textBox2.Text);
                if(populationSize < 0)
                {
                    throw new Exception();
                }
                chromosomeLength = int.Parse(textBox3.Text);
                if (chromosomeLength < 0) 
                {
                    throw new Exception();
                }
                crossoverRate = double.Parse(textBox4.Text);
                if(crossoverRate < 0 || crossoverRate > 1)
                {
                    throw new Exception();
                }
                mutationRate = double.Parse(textBox5.Text);
                if (mutationRate < 0 || mutationRate > 1)
                {
                    throw new Exception();
                }
                xmin = double.Parse(textBox6.Text);
                xmax = double.Parse(textBox7.Text);
                if(xmin > xmax)
                {
                    throw new Exception();
                }
                ymin = double.Parse(textBox8.Text);
                ymax = double.Parse(textBox9.Text);
                if (ymin > ymax)
                {
                    throw new Exception();
                }
                zmin = double.Parse(textBox10.Text);
                zmax = double.Parse(textBox11.Text);
                if (zmin > zmax)
                {
                    throw new Exception();
                }
                bounds[0] = xmin; 
                bounds[1] = xmax; 
                bounds[2] = ymin; 
                bounds[3] = ymax; 
                bounds[4] = zmin; 
                bounds[5] = zmax;
            } 
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception");
            }
            
            Chromosome<double> bestChromosome;
            if (comboBox2.SelectedIndex == 0)
            {
                GeneticBinary genetic = new GeneticBinary(generations, populationSize, chromosomeLength, crossoverRate, mutationRate, bounds, this);
                bestChromosome = genetic.FindMinimum();
            } 
            else
            {
                GeneticReal genetic = new GeneticReal(generations, populationSize, chromosomeLength, crossoverRate, mutationRate, bounds, this);
                bestChromosome = genetic.FindMinimum();
            }

            switch (dimension)
            {
                case 1:
                    label3.Text = "x = " + bestChromosome.X;
                    break;
                case 2:
                    label3.Text = "x = " + bestChromosome.X + "\ny = " + bestChromosome.Y;
                    break;
                case 3:
                    label3.Text = "x = " + bestChromosome.X + "\ny = " + bestChromosome.Y + "\nz = " + bestChromosome.Z + "\n";
                    break;
                default:
                    label3.Text = "Exeption";
                    break;
            }
        }
    }
}