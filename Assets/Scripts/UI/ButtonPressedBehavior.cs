using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPressedBehavior : StateMachineBehaviour
{
    public static Dictionary<string, System.Action> buttonFunctionTable;


    private void Awake()
    {
        buttonFunctionTable = new Dictionary<string, System.Action>();
    }

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        UIInput.Instance.DisableAllUIInput();
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        buttonFunctionTable[animator.gameObject.name].Invoke();
    }


}
