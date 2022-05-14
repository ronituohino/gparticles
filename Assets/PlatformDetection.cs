using UnityEngine;

public class PlatformDetection : MonoBehaviour
{
    public bool onPhone = false;

    // Start is called before the first frame update
    void Start()
    {
        if(Application.platform == RuntimePlatform.Android)
        {
            onPhone = true;
        } 
    }

#if UNITY_EDITOR
    // Update is called once per frame
    void Update()
    {
        if (UnityEditor.EditorApplication.isRemoteConnected)
        {
            onPhone = true;
        } else
        {
            onPhone = false;
        }
    }
#endif
}
