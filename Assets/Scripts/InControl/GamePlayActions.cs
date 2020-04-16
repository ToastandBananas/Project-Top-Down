using InControl;

public class GamePlayActions : PlayerActionSet
{
    public PlayerAction playerInteract;
    public PlayerAction playerDodge;
    public PlayerAction playerSprint;
    public PlayerAction playerSwapWeapon;
    public PlayerAction playerLockOn;
    public PlayerAction playerSwitchLockOnTargetLeft, playerSwitchLockOnTargetRight;
    public PlayerOneAxisAction playerSwitchLockOnTargetAxis;
    public PlayerAction playerUp, playerDown, playerLeft, playerRight;
    public PlayerTwoAxisAction playerMovementAxis;
    public PlayerAction playerLookUp, playerLookDown, playerLookLeft, playerLookRight;
    public PlayerTwoAxisAction playerLookAxis;
    public PlayerAction playerLeftAttack, playerRightAttack;
    public PlayerAction playerLeftSpecialAttack, playerRightSpecialAttack;

    // UI Actions
    public PlayerAction playerInventory;
    public PlayerAction menuPause, menuSelect, menuContext, menuDropItem;
    public PlayerAction menuLeft, menuRight, menuUp, menuDown;
    public PlayerAction menuContainerTakeAll, menuContainerTakeGold;

    public GamePlayActions()
    {
        playerInteract = CreatePlayerAction("PlayerInteract");
        playerDodge = CreatePlayerAction("PlayerDodge");
        playerSprint = CreatePlayerAction("PlayerSprint");

        playerSwapWeapon = CreatePlayerAction("PlayerSwapWeapon");

        playerLockOn = CreatePlayerAction("PlayerLockOn");
        playerSwitchLockOnTargetLeft = CreatePlayerAction("PlayerSwitchLockOnTargetLeft");
        playerSwitchLockOnTargetRight = CreatePlayerAction("PlayerSwitchLockOnTargetRight");
        playerSwitchLockOnTargetAxis = CreateOneAxisPlayerAction(playerSwitchLockOnTargetLeft, playerSwitchLockOnTargetRight);

        playerUp = CreatePlayerAction("PlayerUp");
        playerDown = CreatePlayerAction("PlayerDown");
        playerLeft = CreatePlayerAction("PlayerLeft");
        playerRight = CreatePlayerAction("PlayerRight");
        playerMovementAxis = CreateTwoAxisPlayerAction(playerLeft, playerRight, playerDown, playerUp);

        playerLookUp = CreatePlayerAction("PlayerLookUp");
        playerLookDown = CreatePlayerAction("PlayerLookDown");
        playerLookLeft = CreatePlayerAction("PlayerLookLeft");
        playerLookRight = CreatePlayerAction("PlayerLookRight");
        playerLookAxis = CreateTwoAxisPlayerAction(playerLookLeft, playerLookRight, playerLookDown, playerLookUp);

        playerLeftAttack = CreatePlayerAction("PlayerLeftAttack");
        playerRightAttack = CreatePlayerAction("PlayerRightAttack");
        playerLeftSpecialAttack = CreatePlayerAction("PlayerLeftSpecialAttack");
        playerRightSpecialAttack = CreatePlayerAction("PlayerRightSpecialAttack");

        // UI Actions
        playerInventory = CreatePlayerAction("PlayerInventory");
        menuPause = CreatePlayerAction("MenuPause");
        menuSelect = CreatePlayerAction("MenuSelect");
        menuContext = CreatePlayerAction("MenuContext");
        menuDropItem = CreatePlayerAction("MenuDropItem");
        menuLeft = CreatePlayerAction("MenuLeft");
        menuRight = CreatePlayerAction("MenuRight");
        menuUp = CreatePlayerAction("MenuUp");
        menuDown = CreatePlayerAction("MenuDown");
        menuContainerTakeAll = CreatePlayerAction("MenuContainerTakeAll");
        menuContainerTakeGold = CreatePlayerAction("MenuContainerTakeGold");
    }
}
