using InControl;

public class GamePlayActions : PlayerActionSet
{
    public PlayerAction playerInventory;
    public PlayerAction playerInteract;
    public PlayerAction playerDodge;
    public PlayerAction playerSprint;
    public PlayerAction playerLockOn;
    public PlayerAction playerSwitchLockOnTargetLeft, playerSwitchLockOnTargetRight;
    public PlayerOneAxisAction playerSwitchLockOnTargetAxis;
    public PlayerAction playerUp, playerDown, playerLeft, playerRight;
    public PlayerTwoAxisAction playerMovementAxis;
    public PlayerAction playerLeftAttack, playerRightAttack;
    public PlayerAction playerLeftSpecialAttack, playerRightSpecialAttack;

    public GamePlayActions()
    {
        playerInventory = CreatePlayerAction("PlayerInventory");
        playerInteract = CreatePlayerAction("PlayerInteract");
        playerDodge = CreatePlayerAction("PlayerDodge");
        playerSprint = CreatePlayerAction("PlayerSprint");

        playerLockOn = CreatePlayerAction("PlayerLockOn");
        playerSwitchLockOnTargetLeft = CreatePlayerAction("PlayerSwitchLockOnTargetLeft");
        playerSwitchLockOnTargetRight = CreatePlayerAction("PlayerSwitchLockOnTargetRight");
        playerSwitchLockOnTargetAxis = CreateOneAxisPlayerAction(playerSwitchLockOnTargetLeft, playerSwitchLockOnTargetRight);

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
