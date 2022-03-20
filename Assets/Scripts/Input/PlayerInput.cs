using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


[CreateAssetMenu(menuName = "Player Input")]
public class PlayerInput : ScriptableObject, InputActions.IGamePlayerActions,InputActions.IPauseMenuActions,InputActions.IGameOverScreenActions
{
    InputActions inputActions;
    //输入事件
    public event UnityAction<Vector2> onMove = delegate { };
    public event UnityAction onStopMove = delegate { };
    public event UnityAction onFire = delegate { };
    public event UnityAction onStopFire = delegate { };
    public event UnityAction onDodge = delegate { };
    public event UnityAction onOverdrive = delegate { };
    public event UnityAction onPause = delegate { };
    public event UnityAction onUnPause = delegate { };
    public event UnityAction onLaunchMissile = delegate { };
    public event UnityAction onGameOver = delegate { };

    void OnEnable()
    {
        inputActions = new InputActions();

        inputActions.GamePlayer.SetCallbacks(this);       //注册输入表
        inputActions.PauseMenu.SetCallbacks(this);
        inputActions.GameOverScreen.SetCallbacks(this);
    }

    void OnDisable()
    {
        DisableAllInputs();
    }

    //其他类禁用动作表，如过程动画，UI交互、人物交互
    public void DisableAllInputs() => inputActions.Disable();

    //其他的类需要时启用这个动作表
    public void EnableGameplayerInput() => SwitchActionMap(inputActions.GamePlayer, false);

    public void EnablePauseMenuInput() => SwitchActionMap(inputActions.PauseMenu, true);

    public void EnableGameOverScreenInput() => SwitchActionMap(inputActions.GameOverScreen, false);

    //设置系统输出的更新时间
    public void SwitchToDynamicUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInDynamicUpdate;
    public void SwitchToFixedUpdateMode() => InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsInFixedUpdate;
    void SwitchActionMap(InputActionMap actionMap,bool isUIIntput)
    {
        inputActions.Disable();
        actionMap.Enable();

        if (isUIIntput)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onMove.Invoke(context.ReadValue<Vector2>());
        }
        if (context.phase == InputActionPhase.Canceled)
        {
            onStopMove.Invoke();
        }
    }

    public void OnFire(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            onFire.Invoke();
        }
        if (context.canceled)     //两种写法
        {
            onStopFire.Invoke();
        }
    }

    public void OnDodge(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onDodge.Invoke();
        }
    }

    public void OnOverdrive(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onOverdrive.Invoke();
        }
    }

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onPause.Invoke();
        }
    }

    public void OnUnPause(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onUnPause.Invoke();
        }
    }

    public void OnLaunchMissile(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onLaunchMissile.Invoke();
        }
    }

    public void OnConfirmGameOver(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            onGameOver.Invoke();
        }
    }
}
