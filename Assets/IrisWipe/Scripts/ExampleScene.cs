using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExampleScene : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            IrisWipeController.Instance.WipeIn(0.75f, 0.6f, 0.5f, IrisWipeController.Instance.target);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            IrisWipeController.Instance.MoveFromAtoB(0.75f, IrisWipeController.Instance.target, 0.35f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            IrisWipeController.Instance.MoveFromAtoB(0.75f, IrisWipeController.Instance.target, .85f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            IrisWipeController.Instance.WipeOut(0.75f);
        }
    }
}
