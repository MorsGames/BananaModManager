using Il2CppSystem;
using Il2CppSystem.Collections.Generic;
using UnhollowerBaseLib;
using UnityEngine;
using Object = Il2CppSystem.Object;

namespace BananaModManager.Loader.IL2Cpp
{
    public class CodeRunner : MonoBehaviour
    {
        public CodeRunner(System.IntPtr value) : base(value) { }

        private void Update()
        {
            Loader.InvokeUpdate();
        }

        private void FixedUpdate()
        {
            Loader.InvokeFixedUpdate();
        }

        private void LateUpdate()
        {
            Loader.InvokeLateUpdate();
        }

        private void OnGUI()
        {
            Loader.InvokeGUI();
        }
    }
}