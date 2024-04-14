using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Vaqueritos;

public partial class MainMenu : Window
{
    public MainMenu()
    {
        InitializeComponent();
    }

    public class RandomModelSingleton
{
    private static RandomModelSingleton instance;
    private double[][][] randomModel;

    private RandomModelSingleton()
    {
        randomModel = BuildRandomModel();
    }

    public static RandomModelSingleton Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new RandomModelSingleton();
            }
            return instance;
        }
    }

    private double[][][] BuildRandomModel()
    {
        int numGames = 100;
        int numTurns = 100; // Ensure this is the minimum number of turns you want per game
        double[][][] randomModel = new double[numTurns][][]; // First dimension is now 'numTurns'

        // Initialize random model
        for (int turn = 0; turn < numTurns; turn++)
        {
            randomModel[turn] = new double[3][]; // Three actions: Recargar, Escudo, Disparar
            for (int action = 0; action < 3; action++)
            {
                randomModel[turn][action] = new double[3]; // Probability of each action at each turn
            }
        }

        // Define a jagged array to store game patterns
        List<int>[] gamePatterns = new List<int>[numGames];

        // Initialize each game's pattern
        for (int j = 0; j < numGames; j++)
        {
            gamePatterns[j] = new List<int>();
            Random rand = new Random();
            for (int i = 0; i < numTurns; i++)
            {
                int action = rand.Next(1, 4); // Generating a random action: 1 (Recargar), 2 (Escudo), or 3 (Disparar)
                gamePatterns[j].Add(action);
            }
        }

        // Count occurrences of each action at each turn position
        for (int turn = 0; turn < numTurns; turn++)
        {
            int[] actionOccurrences = new int[3]; // Count of each action
            for (int game = 0; game < numGames; game++)
            {
                int action = gamePatterns[game][turn] - 1; // Adjust for 0-based index
                actionOccurrences[action]++;
            }

            // Calculate probabilities
            for (int action = 0; action < 3; action++)
            {
                for (int otherAction = 0; otherAction < 3; otherAction++)
                {
                    randomModel[turn][action][otherAction] = (double)actionOccurrences[otherAction] / numGames;
                }
            }
        }

        return randomModel;
    }

    public double[][][] GetRandomModel()
    {
        return randomModel;
    }
}

    
    private void Button_OnClick(object? sender, RoutedEventArgs e)
    {
        var secondWindow = new Game();
        secondWindow.Show();
    }

    private void EntrenarButton_OnClick(object? sender, RoutedEventArgs e)
    {
        var thirdWindow = new Training();
        thirdWindow.Show();
    }

    private void Salir_OnClick(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }
}