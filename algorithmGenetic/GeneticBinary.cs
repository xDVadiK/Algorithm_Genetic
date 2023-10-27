using System;
using System.Collections.Generic;
using System.Linq;

namespace algorithmGenetic
{
    internal class GeneticBinary
    {
        private Form1 form;
        private Random random = new Random();

        private int generations;
        private int populationSize;
        private int chromosomeLength;
        private double crossoverRate;
        private double mutationRate;
        
        private double xmin;
        private double xmax;
        private double ymin;
        private double ymax;
        private double zmin;
        private double zmax;

        public GeneticBinary(int generations, int populationSize, int chromosomeLength, double crossoverRate, double mutationRate, double[] bounds, Form1 form)
        {
            this.generations = generations;
            this.populationSize = populationSize;
            this.chromosomeLength = chromosomeLength;
            this.crossoverRate = crossoverRate;
            this.mutationRate = mutationRate;
            this.form = form;
            xmin = bounds[0]; 
            xmax = bounds[1]; 
            ymin = bounds[2]; 
            ymax = bounds[3];
            zmin = bounds[4]; 
            zmax = bounds[5];
        }

        // Calculation of the minimum point of a given function
        public Chromosome<double> FindMinimum()
        {
            var population = InitializePopulation();
            Chromosome<string> bestChromosome = population[0];

            for (int generation = 0; generation < generations; generation++)
            {
                var newPopulation = new List<Chromosome<string>>();

                for (int i = 0; i < populationSize; i++)
                {
                    Chromosome<string> firstParent = SelectParent(population);
                    Chromosome<string> secondParent = SelectParent(population);

                    if (random.NextDouble() < crossoverRate)
                    {
                        Chromosome<string> child = Crossover(firstParent, secondParent);

                        if (random.NextDouble() < mutationRate)
                            child = Mutate(child);

                        newPopulation.Add(child);

                        if (FitnessFunction(child) < FitnessFunction(bestChromosome))
                            bestChromosome = child;
                    }
                    else
                    {
                        newPopulation.Add(random.NextDouble() <= 0.5 ? firstParent : secondParent);
                    }
                }

                population = newPopulation;
            }

            double x = BinaryStringToDouble(bestChromosome.X, xmin, xmax);
            double y = BinaryStringToDouble(bestChromosome.Y, ymin, ymax);
            double z = BinaryStringToDouble(bestChromosome.Z, zmin, zmax);

            Chromosome<double> chromosome = new Chromosome<double>(x, y, z);

            return chromosome;
        }

        // Creating an initial population
        private List<Chromosome<string>> InitializePopulation()
        {
            return Enumerable.Range(0, populationSize)
                .Select(_ => GenerateChromosome())
                .ToList();
        }

        // Creating a chromosome
        private Chromosome<string> GenerateChromosome()
        {
            string x = BinaryEncoding(xmin, xmax);
            string y = BinaryEncoding(ymin, ymax);
            string z = BinaryEncoding(zmin, zmax);
            return new Chromosome<string>(x, y, z);
        }

        // Encoding the value
        private string BinaryEncoding(double min, double max)
        {
            double value = min + random.NextDouble() * (max - min);
            return DoubleToBinaryString(value, min, max);
        }

        // Converting a value to a bit string
        private string DoubleToBinaryString(double value, double min, double max)
        {
            double normalizedValue = (value - min) / (max - min);
            double maxPossibleValue = Math.Pow(2, chromosomeLength) - 1;
            int intValue = (int)(normalizedValue * maxPossibleValue);
            return Convert.ToString(intValue, 2).PadLeft(chromosomeLength, '0');
        }

        // Choosing a parent
        private Chromosome<string> SelectParent(List<Chromosome<string>> population)
        {
            double totalFitness = population.Sum(chromosome => 1.0 / FitnessFunction(chromosome));
            double value = random.NextDouble() * totalFitness;
            double sum = 0;

            foreach (var chromosome in population)
            {
                sum += 1.0 / FitnessFunction(chromosome);
                if (sum >= value)
                    return chromosome;
            }

            return population.Last();
        }

        // Вычисление значения функции
        public double FitnessFunction(Chromosome<string> chromosome)
        {
            double x = BinaryStringToDouble(chromosome.X, xmin, xmax);
            double y = BinaryStringToDouble(chromosome.Y, ymin, ymax);
            double z = BinaryStringToDouble(chromosome.Z, zmin, zmax);
            return form.FitnessFunction(x, y, z);
        }

        // Преобразование битовой строки в значение
        public double BinaryStringToDouble(string binaryString, double min, double max)
        {
            double maxPossibleValue = Math.Pow(2, chromosomeLength) - 1;
            int intValue = Convert.ToInt32(binaryString, 2);
            double normalizedValue = intValue / maxPossibleValue;
            return min + normalizedValue * (max - min);
        }

        // Crossing
        private Chromosome<string> Crossover(Chromosome<string> firstParent, Chromosome<string> secondParent)
        {
            int crossoverPoint = random.Next(1, chromosomeLength);

            Chromosome<string> newChromosome = new Chromosome<string>(
                firstParent.X.Substring(0, crossoverPoint) + secondParent.X.Substring(crossoverPoint),
                firstParent.Y.Substring(0, crossoverPoint) + secondParent.Y.Substring(crossoverPoint),
                firstParent.Z.Substring(0, crossoverPoint) + secondParent.Z.Substring(crossoverPoint)
                );

            newChromosome.X = CheckBound(newChromosome.X, xmin, xmax);
            newChromosome.Y = CheckBound(newChromosome.Y, ymin, ymax);
            newChromosome.Z = CheckBound(newChromosome.Z, zmin, zmax);

            return newChromosome;
        }

        // Checking for being in a given interval
        private string CheckBound(string value, double min, double max)
        {
            if (BinaryStringToDouble(value, min, max) > max)
            {
                value = DoubleToBinaryString(max, min, max);
            }
            if (BinaryStringToDouble(value, min, max) < min)
            {
                value = DoubleToBinaryString(min, min, max);
            }
            return value;
        }

        // Mutation
        private Chromosome<string> Mutate(Chromosome<string> chromosome) 
        {
            int mutationPoint = random.Next(chromosomeLength);

            chromosome.X = chromosome.X.Substring(0, mutationPoint) + (chromosome.X.ElementAt(mutationPoint) == '0' ? '1' : '0') + chromosome.X.Substring(mutationPoint + 1);
            chromosome.Y = chromosome.Y.Substring(0, mutationPoint) + (chromosome.Y.ElementAt(mutationPoint) == '0' ? '1' : '0') + chromosome.Y.Substring(mutationPoint + 1);
            chromosome.Z = chromosome.Z.Substring(0, mutationPoint) + (chromosome.Z.ElementAt(mutationPoint) == '0' ? '1' : '0') + chromosome.Z.Substring(mutationPoint + 1);

            chromosome.X = CheckBound(chromosome.X, xmin, xmax);
            chromosome.Y = CheckBound(chromosome.Y, ymin, ymax);
            chromosome.Z = CheckBound(chromosome.Z, zmin, zmax);

            return chromosome;
        }
    }
}