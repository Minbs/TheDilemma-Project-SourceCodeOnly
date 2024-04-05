using Sirenix.OdinInspector;
using UnityEngine;

public class TestScripts : MonoBehaviour
{
#if UNITY_EDITOR
    [Button]
    public void PrintSomething()
    {
        Debug.Log("야호");
    }

    [Button("야호 출력")]
    public void PrintSomething_2()
    {
        Debug.Log("야호");
    }
#endif
}
