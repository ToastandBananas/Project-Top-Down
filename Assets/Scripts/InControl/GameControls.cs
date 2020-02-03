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

        gamePlayActions.playerUp.AddDefaultBinding(Key.W);
        gamePlayActions.playerUp.AddDefaultBinding(InputControlType.LeftStickUp);

        gamePlayActions.playerDown.AddDefaultBinding(Key.S);
        gamePlayActions.playerDown.AddDefaultBinding(InputControlType.LeftStickDown);

        gamePlayActions.playerLeft.AddDefaultBinding(Key.A);
        gamePlayActions.playerLeft.AddDefaultBinding(InputControlType.LeftStickLeft);

        gamePlayActions.playerRight.AddDefaultBinding(Key.D);
        gamePlayActions.playerRight.AddDefaultBinding(InputControlType.LeftStickRight);

        gamePlayActions.playerLeftAttack.AddDefaultBinding(Mouse.LeftButton);
        gamePlayActions.playerLeftAttack.AddDefaultBinding(InputControlType.LeftTrigger);

        gamePlayActions.playerRightAttack.AddDefaultBinding(Mouse.RightButton);
        gamePlayActions.playerRightAttack.AddDefaultBinding(InputControlType.RightTrigger);

        gamePlayActions.playerLeftSpecialAttack.AddDefaultBinding(Key.Q);
        gamePlayActions.playerLeftSpecialAttack.AddDefaultBinding(InputControlType.LeftBumper);

        gamePlayActions.playerRightSpecialAttack.AddDefaultBinding(Key.E);
        gamePlayActions.playerRightSpecialAttack.AddDefaultBinding(InputControlType.RightBumper);
    }
}
