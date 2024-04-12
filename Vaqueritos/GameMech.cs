using Avalonia.Data;

namespace Vaqueritos;

public class GameMech(int opcion)
{
    private int PlayerAction;
    private int BotAction;
    private int Reponse;

    int playerMunition = 0;
    int botMunition = 0;

    bool playerProtected;
    bool botProtected;

    private bool canBotShoot;
    private bool canPlayerShoot;

    private bool canPlayerRecharge;
    private bool canBotRecharge;

    bool hasPlayerShot;
    bool hasBotShot;

    bool hasPlayerWon;
    bool hasBotWon;

    public void Acciones()
    {
        if (!hasBotWon || !hasPlayerWon)
        {
            switch (PlayerAction)
            {
                //Recargar
                case 1:
                    if (canPlayerRecharge)
                    {
                        playerMunition++;
                    }

                    break;

                //Escudo
                case 2:
                    playerProtected = true;
                    break;

                //Disparar
                case 3:
                    if (canPlayerShoot)
                    {
                        hasPlayerShot = true;
                        playerMunition--;
                    }

                    break;
            }
        }
    }
    
    void AccionesBot()
    {
        if (!hasBotWon || !hasPlayerWon)
        {
            switch (BotAction)
            {
                //Recargar
                case 1:
                    if (canBotRecharge)
                    {
                        botMunition++;
                    }

                    break;

                //Escudo
                case 2:
                    botProtected = true;
                    break;

                //Disparar
                case 3:
                    if (canBotShoot)
                    {
                        hasBotShot = true;
                        botMunition--;
                    }

                    break;
            }
        }
    }

    void Comprobacion()
    {
        //Win Case Bot
        if (!playerProtected && hasBotShot)
        {
            hasBotWon = true;
        }

        //Win Case Player
        if (!botProtected && hasPlayerShot)
        {
            hasPlayerWon = true;
        }

        //Can Shoot Player
        if (playerMunition > 0)
        {
            canPlayerShoot = true;
        }

        //Can Shoot Bot
        if (botMunition > 0)
        {
            canBotShoot = true;
        }

        //Can Player Recharge
        if (playerMunition < 6)
        {
            canPlayerShoot = true;
        }

        //Can Player Recharge
        if (botMunition < 6)
        {
            canBotShoot = true;
        }
    }

    void Reset()
    {
        playerProtected = false;
        botProtected = false;
        canBotShoot = false;
        canPlayerShoot = false;
        canPlayerRecharge = false;
        canBotRecharge = false;
    }
}



    

