using InControl;

public class GamePlayActions : PlayerActionSet
{
    public PlayerAction playerInteract;
    public PlayerAction playerDodge;
    public PlayerAction playerSprint;
    public PlayerAction playerUp, playerDown, playerLeft, playerRight;
    public PlayerTwoAxisAction playerMovementAxis;
    public PlayerAction playerLeftAttack, playerRightAttack;
    public PlayerAction playerLeftSpecialAttack, playerRightSpecialAttack;

    public GamePlayActions()
    {
        playerInteract = CreatePlayerAction("PlayerInteract");
        playerDodge = CreatePlayerAction("PlayerDodge");
        playerSprint = CreatePlayerAction("PlayerSprint");
        playerUp = CreatePlayerAction("PlayerUp");
        playerDown = CreatePlayerAction("PlayerDown");
        playerLeft = CreatePlayerAction("PlayerLeft");
        playerRight = CreatePlayerAction("PlayerRight");
        playerMovementAxis = CreateTwoAxisPlayerAction(playerLeft, playerRight, playerDown, playerUp);
        playerLeftAttack = CreatePlayerAction("PlayerLeftAttack");
        playerRightAttack = CreatePlayerAction("PlayerRightAttack");
        playerLeftSpecialAttack = CreatePlayerAction("PlayerLeftSpecialAttack");
        playerRightSpecialAttack = CreatePlayerAction("PlayerRightSpecialAttack");
    }
}
