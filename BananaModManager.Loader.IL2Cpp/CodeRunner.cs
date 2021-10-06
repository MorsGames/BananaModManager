using System;
using System.Linq;
using System.Reflection;
using Il2CppSystem.Collections.Generic;
using UnityEngine;

namespace BananaModManager.Loader.IL2Cpp
{
    public class CodeRunner : MonoBehaviour
    {
        public CodeRunner(IntPtr value) : base(value) { }

        internal static void Create()
        {
            var obj = new GameObject("BananaModManagerCodeRunner");
            DontDestroyOnLoad(obj);

            var runner = new CodeRunner(obj.AddComponent(UnhollowerRuntimeLib.Il2CppType.Of<CodeRunner>()).Pointer);

            foreach (var type in Loader.Mods.SelectMany(mod => mod.GetAssembly().GetTypes()))
            {
                type.GetMethod("OnModStart")?.Invoke(null, null);

                var update = type.GetMethod("OnModUpdate");
                if (update != null) runner.UpdateMethods.Add(update);

                var fixedUpdate = type.GetMethod("OnModFixedUpdate");
                if (fixedUpdate != null) runner.FixedUpdateMethods.Add(fixedUpdate);

                var lateUpdate = type.GetMethod("OnModLateUpdate");
                if (lateUpdate != null) runner.LateUpdateMethods.Add(lateUpdate);

                var gui = type.GetMethod("OnModGUI");
                if (gui != null) runner.GUIMethods.Add(gui);
            }
        }

        public List<MethodInfo> FixedUpdateMethods { get; set; } = new List<MethodInfo>();
        public List<MethodInfo> GUIMethods { get; set; } = new List<MethodInfo>();
        public List<MethodInfo> LateUpdateMethods { get; set; } = new List<MethodInfo>();
        public List<MethodInfo> UpdateMethods { get; set; } = new List<MethodInfo>();

        private void Update()
        {
            foreach (var method in UpdateMethods) method.Invoke(null, null);
        }

        private void FixedUpdate()
        {
            foreach (var method in FixedUpdateMethods) method.Invoke(null, null);
        }

        private void LateUpdate()
        {
            foreach (var method in LateUpdateMethods) method.Invoke(null, null);
        }

        private void OnGUI()
        {
            foreach (var method in GUIMethods) method.Invoke(null, null);
        }
    }
}