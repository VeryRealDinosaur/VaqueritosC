using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Interactivity;
using AvaloniaGif;
using System.Timers;
using Avalonia.Media;
using Avalonia.Threading;


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
            Barra = this.FindControl<GifImage>("Barra");
            Shoot = this.FindControl<GifImage>("Shoot");
            Bola = this.FindControl<GifImage>("Bola");
            Start = this.FindControl<Button>("Start");
            Wanted = this.FindControl<Image>("Wanted");
            All = this.FindControl<Grid>("All");
            Revolver = this.FindControl<GifImage>("Revolver");
            Lose = this.FindControl<GifImage>("Lose");
            Win = this.FindControl<GifImage>("Win");
            MiniBala = this.FindControl<GifImage>("MiniBala");
            MiniShoot = this.FindControl<GifImage>("MiniShoot");

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
        
        private int debugAction;
        int turn;
        
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
        
        void MiniBalaAnimation()
        {
            Timer timer = new Timer();
            MiniBala.Opacity = 1;
            MiniBala.Start();
            timer.Start();
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => MiniBala.Stop();
        }
        
        void ShootAnimation()
        {
            Timer timer = new Timer();
            Shoot.Opacity = 1;
            Shoot.Start();
            timer.Start();
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                Shoot.Stop();
                Shoot.Opacity = 0;
            };
        }
        
        void MiniShootAnimation()
        {
            Timer timer = new Timer();
            MiniShoot.Opacity = 1;
            MiniShoot.Start();
            timer.Start();
            timer.Interval = 500;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                MiniShoot.Stop();
                MiniShoot.Opacity = 0;
            };
        }
        
        void BolaAnimation()
        {
            Bola.Start();
            Wanted.ZIndex = 0;
            Start.ZIndex = 0;
            Timer timer = new Timer();
            Bola.Opacity = 1;
            timer.Start();
            timer.Interval = 4000;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => Bola.Stop();
        }
        
        void RevolverAnimation()
        {
            Timer timer = new Timer();
            Revolver.Start();
            Revolver.InvalidateArrange();
            timer.Interval = 1000;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) => 
            {
                // Marshal the call back to the UI thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    Revolver.Stop();
                    Revolver.InvalidateArrange();
                });
            };
            timer.Start();
        }
        
        void Back()
        {
            Timer timer = new Timer();
            timer.Interval = 4000;
            timer.AutoReset = false;
            timer.Elapsed += (sender, e) =>
            {
                // Marshal the call back to the UI thread
                Dispatcher.UIThread.InvokeAsync(() =>
                {
                    All.ZIndex = 4;
                    All.InvalidateArrange();
                });
            };
            timer.Start();
        }
        
        

        bool Acciones(int PlayerAction)
        {
            if (!hasBotWon || !hasPlayerWon)
            {
                Comprobacion();
                switch (PlayerAction)
                {
                    case 1: // Recargar
                        Comprobacion();
                        if (canPlayerRecharge)
                        {
                            Barra.Stop();
                            playerProtected = false;
                            BalaAnimation();
                            playerMunition++;
                            canPlayerRecharge = false;
                            Barra.Start();
                            return true;
                        }
                        else
                        {
                            return false;
                        }

                    case 2: // Escudo
                        Barra.Stop();
                        playerProtected = true;
                        Barra.Start();
                        return true;

                    case 3: // Disparar
                        Comprobacion();
                        if (canPlayerShoot)
                        {
                            Barra.Stop();
                            playerProtected = false;
                            ShootAnimation();
                            hasPlayerShot = true;
                            playerMunition--;
                            Barra.Start();
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                }
            }

            return false;
        }

        void AccionesBot(int botAction)
        {
            if (!hasBotWon || !hasPlayerWon)
            {
                Comprobacion();
                switch (botAction)
                {
                    case 0: // Recargar
                        Comprobacion();
                        if (canBotRecharge)
                        {
                            botProtected = false;
                            if (hasPlayerShot)
                            {
                                hasPlayerWon = true;
                            }
                            botMunition++;
                            MiniBalaAnimation();
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
                            if (!playerProtected)
                            {
                                hasBotWon = true;
                            }
                            MiniShootAnimation();
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

        void UpdateRevolver()
        {
            switch (playerMunition)
            {
                case 0:
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T0.gif");
                    break;
                case 1: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T1.gif");
                    break;
                case 2: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T2.gif");
                    break;
                case 3: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T3.gif");
                    break;
                case 4: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T4.gif");
                    break;
                case 5: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T5.gif");
                    break;
                case 6: 
                    Revolver.SourceUri = new Uri("avares://Vaqueritos/Assets/T6.gif");
                    break;
            }
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
                AccionesBot(debugAction);
                turn++;
                UpdateRevolver();
                RevolverAnimation();
                if (hasBotWon)
                {
                    Barra.Stop();
                    Lose.Start();
                    Lose.ZIndex = 5;
                    balas.InvalidateVisual();
                
                }
                else if (hasPlayerWon)
                {
                    Barra.Stop();
                    Win.Start();
                    Win.ZIndex = 5;
                    balas.InvalidateVisual();
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
                AccionesBot(debugAction);
                turn++;
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
                AccionesBot(debugAction);
                turn++;
                UpdateRevolver();
                RevolverAnimation();
                if (hasBotWon)
                {
                    Barra.Stop();
                    Lose.Start();
                    Lose.ZIndex = 5;
                    balas.InvalidateVisual();
                
                }
                else if (hasPlayerWon)
                {
                    Barra.Stop();
                    Win.Start();
                    Win.ZIndex = 5;
                    balas.InvalidateVisual();
                }
            }
            else
            {
                Disparar.BorderBrush = Brushes.Red;
            }
                
        }

        private void Start_OnClick(object? sender, RoutedEventArgs e)
        {
            Back();
            BolaAnimation();
        }
    }
}