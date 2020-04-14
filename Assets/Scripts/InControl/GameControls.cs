using UnityEngine;
using InControl;

public class GameControls : MonoBehaviour
{
    public static GamePlayActions gamePlayActions;

    void Start()
    {
        gamePlayActions = new GamePlayActions();
        BindDefaultControls();
    }

    void BindDefaultControls()
    {
        gamePlayActions.playerInteract.AddDefaultBinding(Key.R);
        gamePlayActions.playerInteract.AddDefaultBinding(InputControlType.Action3);

        gamePlayActions.playerDodge.AddDefaultBinding(Key.Space);
        gamePlayActions.playerDodge.AddDefaultBinding(InputControlType.Action1);

        gamePlayActions.playerSprint.AddDefaultBinding(Key.LeftShift);
        gamePlayActions.playerSprint.AddDefaultBinding(InputControlType.LeftStickButton);

        gamePlayActions.playerSwapWeapon.AddDefaultBinding(Key.LeftAlt);
        gamePlayActions.playerSwapWeapon.AddDefaultBinding(InputControlType.Action4);

        gamePlayActions.playerLockOn.AddDefaultBinding(Key.Tab);
        gamePlayActions.playerLockOn.AddDefaultBinding(InputControlType.RightStickButton);

        gamePlayActions.playerSwitchLockOnTargetLeft.AddDefaultBinding(Key.C);
        gamePlayActions.playerSwitchLockOnTargetLeft.AddDefaultBinding(InputControlType.RightStickLeft);

        gamePlayActions.playerSwitchLockOnTargetRight.AddDefaultBinding(Key.F);
        gamePlayActions.playerSwitchLockOnTargetRight.AddDefaultBinding(InputControlType.RightStickRight);

        gamePlayActions.playerUp.AddDefaultBinding(Key.W);
        gamePlayActions.playerUp.AddDefaultBinding(InputControlType.LeftStickUp);
        gamePlayActions.playerDown.AddDefaultBinding(Key.S);
        gamePlayActions.playerDown.AddDefaultBinding(InputControlType.LeftStickDown);
        gamePlayActions.playerLeft.AddDefaultBinding(Key.A);
        gamePlayActions.playerLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
        gamePlayActions.playerRight.AddDefaultBinding(Key.D);
        gamePlayActions.playerRight.AddDefaultBinding(InputControlType.LeftStickRight);

        gamePlayActions.playerLookUp.AddDefaultBinding(InputControlType.RightStickUp);
        gamePlayActions.playerLookDown.AddDefaultBinding(InputControlType.RightStickDown);
        gamePlayActions.playerLookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
        gamePlayActions.playerLookRight.AddDefaultBinding(InputControlType.RightStickRight);

        gamePlayActions.playerLeftAttack.AddDefaultBinding(Mouse.LeftButton);
        gamePlayActions.playerLeftAttack.AddDefaultBinding(InputControlType.LeftTrigger);

        gamePlayActions.playerRightAttack.AddDefaultBinding(Mouse.RightButton);
        gamePlayActions.playerRightAttack.AddDefaultBinding(InputControlType.RightTrigger);

        gamePlayActions.playerLeftSpecialAttack.AddDefaultBinding(Key.Q);
        gamePlayActions.playerLeftSpecialAttack.AddDefaultBinding(InputControlType.LeftBumper);

        gamePlayActions.playerRightSpecialAttack.AddDefaultBinding(Key.E);
        gamePlayActions.playerRightSpecialAttack.AddDefaultBinding(InputControlType.RightBumper);

        // UI Actions
        gamePlayActions.playerInventory.AddDefaultBinding(Key.I);
        gamePlayActions.playerInventory.AddDefaultBinding(InputControlType.Action2);

        gamePlayActions.menuPause.AddDefaultBinding(Key.Escape);
        gamePlayActions.menuPause.AddDefaultBinding(InputControlType.Pause);

        gamePlayActions.menuSelect.AddDefaultBinding(Mouse.LeftButton);
        gamePlayActions.menuSelect.AddDefaultBinding(InputControlType.Action1);

        gamePlayActions.menuContext.AddDefaultBinding(Mouse.RightButton);
        gamePlayActions.menuContext.AddDefaultBinding(InputControlType.Action3);

        gamePlayActions.menuUp.AddDefaultBinding(Key.UpArrow);
        gamePlayActions.menuUp.AddDefaultBinding(InputControlType.DPadUp);

        gamePlayActions.menuDown.AddDefaultBinding(Key.DownArrow);
        gamePlayActions.menuDown.AddDefaultBinding(InputControlType.DPadDown);

        gamePlayActions.menuLeft.AddDefaultBinding(Key.LeftArrow);
        gamePlayActions.menuLeft.AddDefaultBinding(InputControlType.DPadLeft);

        gamePlayActions.menuRight.AddDefaultBinding(Key.RightArrow);
        gamePlayActions.menuRight.AddDefaultBinding(InputControlType.DPadRight);
    }
}
