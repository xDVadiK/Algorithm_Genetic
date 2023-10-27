using System;
using System.Collections.Generic;
using System.Linq;

namespace algorithmGenetic
{
    internal class GeneticReal
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

        public GeneticReal(int generations, int populationSize, int chromosomeLength, double crossoverRate, double mutationRate, double[] bounds, Form1 form)
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
            Chromosome<double> bestChromosome = population[0];

            for (int generation = 0; generation < generations; generation++)
            {
                var newPopulation = new List<Chromosome<double>>();

                for (int i = 0; i < populationSize; i++)
                {
                    Chromosome<double> firstParent = SelectParent(population);
                    Chromosome<double> lastParent = SelectParent(population);

                    if (random.NextDouble() < crossoverRate)
                    {
                        Chromosome<double> child = CrossoverBinary(firstParent, lastParent);

                        if (random.NextDouble() < mutationRate)
                            child = MutateBinary(child);

                        newPopulation.Add(child);

                        if (form.FitnessFunction(child.X, child.Y, child.Z) < form.FitnessFunction(bestChromosome.X, bestChromosome.Y, bestChromosome.Z))
                            bestChromosome = child;
                    }
                    else
                    {
                        newPopulation.Add(random.NextDouble() <= 0.5 ? firstParent : lastParent);
                    }
                }

                population = newPopulation;
            }
            return bestChromosome;
        }

        // Creating an initial population
        private List<Chromosome<double>> InitializePopulation()
        {
            return Enumerable.Range(0, populationSize)
                .Select(_ => GenerateChromosome())
                .ToList();
        }

        // Creating a chromosome
        private Chromosome<double> GenerateChromosome()
        {
            double x = xmin + random.NextDouble() * (xmax - xmin);
            double y = ymin + random.NextDouble() * (ymax - ymin);
            double z = zmin + random.NextDouble() * (zmax - zmin);
            return new Chromosome<double>(x, y, z);
        }

        // Choosing a parent
        private Chromosome<double> SelectParent(List<Chromosome<double>> population)
        {
            double totalFitness = population.Sum(chromosome => 1.0 / form.FitnessFunction(chromosome.X, chromosome.Y, chromosome.Z));
            double value = random.NextDouble() * totalFitness;
            double sum = 0;

            foreach (var chromosome in population)
            {
                sum += 1.0 / form.FitnessFunction(chromosome.X, chromosome.Y, chromosome.Z);
                if (sum >= value)
                    return chromosome;
            }

            return population.Last();
        }

        // Crossing
        private Chromosome<double> CrossoverBinary(Chromosome<double> firstParent, Chromosome<double> secondParent)
        {
            double alpha = random.NextDouble();
            double x = CheckBound(alpha * firstParent.X + (1 - alpha) * secondParent.X, xmin, xmax);
            double y = CheckBound(alpha * firstParent.Y + (1 - alpha) * secondParent.Y, ymin, ymax);
            double z = CheckBound(alpha * firstParent.Z + (1 - alpha) * secondParent.Z, zmin, zmax);

            return new Chromosome<double>(x, y, z);
        }

        // Checking for being in a given interval
        private double CheckBound(double value, double min, double max)
        {
            if (value > max)
            {
                value = max;
            }
            if (value < min)
            {
                value = min;
            }
            return value;
        }

        // Mutation
        private Chromosome<double> MutateBinary(Chromosome<double> chromosome)
        {
            double mutation = random.NextDouble() * 0.1;
            chromosome.X = CheckBound(chromosome.X + mutation, xmin, xmax);
            chromosome.Y = CheckBound(chromosome.Y + mutation, ymin, ymax);
            chromosome.Z = CheckBound(chromosome.Z + mutation, zmin, zmax);

            return chromosome;
        }
    }
}