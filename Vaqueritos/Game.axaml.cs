using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Interactivity;
using AvaloniaGif;
using System.Timers;
using Avalonia.Media;


namespace Vaqueritos
{
    public partial class Game : Window
    {
        double[][][] randomModel = MainMenu.RandomModelSingleton.Instance.GetRandomModel();
        
        public Game()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Recargar = this.FindControl<Button>("Recargar");
            Escudo = this.FindControl<Button>("Escudo");
            Disparar = this.FindControl<Button>("Disparar");
            Bala = this.FindControl<GifImage>("Bala");
            balas = this.FindControl<Label>("balas");
        }
        
        static int MakeRandomSelection(double[][][] randomModel, int turn)
        {
            Random rand = new Random();
            double[] probabilities = randomModel[turn].SelectMany(x => x).ToArray(); // Flatten the 2D array
            double randomValue = rand.NextDouble();
            double cumulativeProbability = 0;

            // Make the random selection
            for (int action = 0; action < probabilities.Length; action++)
            {
                cumulativeProbability += probabilities[action];
                if (randomValue <= cumulativeProbability)
                {
                    return action;
                }
            }

            return -1;
        }

        int debugAction;
        int turn;
        int Reponse;

        int playerMunition = 0;
        int botMunition = 0;

        bool playerProtected;
        bool botProtected;

        bool canBotShoot;
        bool canPlayerShoot;

        bool canPlayerRecharge;
        bool canBotRecharge;

        bool hasPlayerShot;
        bool hasBotShot;

        bool hasPlayerWon;
        bool hasBotWon;

        void BalaAnimation()
        {
            Timer timer = new Timer();
            Bala.Opacity = 1;
            Bala.Start();
            timer.Start();
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => Bala.Stop();
        }

        bool Acciones(int PlayerAction)
        {
            if (!playerProtected && hasBotShot)
            {
                hasBotWon = true;
            }
            else if (!botProtected && hasPlayerShot)
            {
                hasPlayerWon = true;
            }
            else if (!hasBotWon || !hasPlayerWon)
            {
                Comprobacion();
                switch (PlayerAction)
                {
                    case 1: // Recargar
                        Comprobacion();
                        if (canPlayerRecharge)
                        {
                            playerProtected = false;
                            BalaAnimation();
                            playerMunition++;
                            balas.Content = $"{playerMunition}";
                            canPlayerRecharge = false;
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                        break;

                    case 2: // Escudo
                        playerProtected = true;
                        return true;
                        break;

                    case 3: // Disparar
                        Comprobacion();
                        if (canPlayerShoot)
                        {
                            playerProtected = false;
                            hasPlayerShot = true;
                            playerMunition--;
                            balas.Content = $"{playerMunition}";
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                        break;
                }
            }

            return false;
        }

        void AccionesBot(int BotAction)
        {
            if (!botProtected && hasBotShot)
            {
                hasBotWon = true;
            }
            else if (!playerProtected && hasPlayerShot)
            {
                hasPlayerWon = true;
            }
            else if (!hasBotWon || !hasPlayerWon)
            {
                Comprobacion();
                switch (BotAction)
                {
                    case 0: // Recargar
                        Comprobacion();
                        if (canBotRecharge)
                        {
                            botProtected = false;
                            botMunition++;
                            canBotRecharge = false;
                        }
                        else
                        {
                            AccionesBot(MakeRandomSelection(randomModel,turn));
                        }
                        break;

                    case 1: // Escudo
                        botProtected = true;
                        break;

                    case 2: // Disparar
                        Comprobacion();
                        if (canBotShoot)
                        {
                            botProtected = false;
                            hasBotShot = true;
                            botMunition--;
                        }
                        else
                        {
                            AccionesBot(MakeRandomSelection(randomModel,turn));
                        }
                        break;
                }
            }
        }

        void Comprobacion()
    {
            

            //Can Shoot Player
            canPlayerShoot = playerMunition > 0;

            //Can Shoot Bot
            canBotShoot = botMunition > 0;

            canPlayerRecharge = playerMunition < 6;
            canBotRecharge = botMunition < 6;
        
    }

        private void Recargar_OnClick(object? sender, RoutedEventArgs e)
        {
            Disparar.BorderBrush = default;
            Recargar.BorderBrush = default;
            Escudo.BorderBrush = default;
            if (Acciones(1))
            {
                turn++;
                debugAction = (MakeRandomSelection(randomModel, turn));
                balas.Content = debugAction;
                AccionesBot(debugAction);
                turn++;
                Comprobacion();
                if (hasPlayerWon)
                {
                    balas.Content = "has ganado";
                }
                else if (hasBotWon)
                {
                    balas.Content = "Bot ha ganado";
                }
            }
            else Recargar.BorderBrush = Brushes.Red;
            
        }

        private void Escudo_OnClick(object? sender, RoutedEventArgs e)
        {
            Disparar.BorderBrush = default;
            Recargar.BorderBrush = default;
            Escudo.BorderBrush = default;
            if (Acciones(2))
            {
                turn++;
                debugAction = (MakeRandomSelection(randomModel, turn));
                balas.Content = debugAction;
                AccionesBot(debugAction);
                turn++;
                if (hasPlayerWon)
                {
                    balas.Content = "has ganado";
                }
                else if (hasBotWon)
                {
                    balas.Content = "Bot ha ganado";
                }
            }
            else
            {
                Escudo.BorderBrush = Brushes.Red;
            }
            
        }

        private void Disparar_OnClick(object? sender, RoutedEventArgs e)
        {
            Disparar.BorderBrush = default;
            Recargar.BorderBrush = default;
            Escudo.BorderBrush = default;
            if (Acciones(3))
            {
                turn++;
                debugAction = (MakeRandomSelection(randomModel, turn));
                balas.Content = debugAction;
                AccionesBot(debugAction);
                turn++;
                if (hasPlayerWon)
                {
                    balas.Content = "has ganado";
                }
                else if (hasBotWon)
                {
                    balas.Content = "Bot ha ganado";
                }
            }
            else
            {
                Disparar.BorderBrush = Brushes.Red;
            }
                
        }
    }
}