using UnityEngine;

namespace ElympicsLobbyPackage
{
    internal class WebGLFunctionalities
    {
        public void Init()
        {
#if UNITY_WEBGL_API
            WebGLInput.captureAllKeyboardInput = false;
#endif
        }
    }
}
