using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Interactivity;
using AvaloniaGif;
using System.Timers;


namespace Vaqueritos
{
    public partial class Game : Window
    {
        public Game()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
            Recargar = this.FindControl<Button>("Recargar");
            Bala = this.FindControl<GifImage>("Bala");
            balas = this.FindControl<Label>("balas");
        }

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
            timer.Interval = 500; // 1 second
            timer.AutoReset = false; // Set to false to stop the timer after one tick
            timer.Elapsed += (sender, e) => Bala.Stop(); // Attach an event handle
        }

        void Acciones(int PlayerAction)
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
                        }
                        else
                        {
                            balas.Content = "error";
                        }

                        break;

                    case 2: // Escudo
                        playerProtected = true;
                        break;

                    case 3: // Disparar
                        Comprobacion();
                        if (canPlayerShoot)
                        {
                            playerProtected = false;
                            hasPlayerShot = true;
                            playerMunition--;
                            balas.Content = $"{playerMunition}";
                        }

                        break;
                }
            }
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
                    case 1: // Recargar
                        Comprobacion();
                        if (canPlayerRecharge)
                        {
                            botProtected = false;
                            botMunition++;
                            canBotRecharge = false;
                        }
                        else
                        {
                            balas.Content = "error";
                        }

                        break;

                    case 2: // Escudo
                        botProtected = true;
                        break;

                    case 3: // Disparar
                        Comprobacion();
                        if (canBotShoot)
                        {
                            botProtected = false;
                            hasBotShot = true;
                            botMunition--;
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
            Acciones(1);
            AccionesBot(1);
        }

        private void Escudo_OnClick(object? sender, RoutedEventArgs e)
        {
            Acciones(2);
            AccionesBot(1);
            Comprobacion();
        }

        private void Disparar_OnClick(object? sender, RoutedEventArgs e)
        {
            Acciones(3);
            AccionesBot(1);
            Comprobacion();
        }
    }
}