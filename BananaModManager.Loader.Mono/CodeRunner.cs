using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace BananaModManager.Loader.Mono;

public class CodeRunner : MonoBehaviour
{
    public List<MethodInfo> UpdateMethods { get; set; } = new List<MethodInfo>();
    public List<MethodInfo> FixedUpdateMethods { get; set; } = new List<MethodInfo>();
    public List<MethodInfo> LateUpdateMethods { get; set; } = new List<MethodInfo>();
    public List<MethodInfo> GUIMethods { get; set; } = new List<MethodInfo>();


    private void Update()
    {
        foreach (var method in UpdateMethods)
        {
            method.Invoke(null, null);
        }
    }

    private void FixedUpdate()
    {
        foreach (var method in FixedUpdateMethods)
        {
            method.Invoke(null, null);
        }
    }

    private void LateUpdate()
    {
        foreach (var method in LateUpdateMethods)
        {
            method.Invoke(null, null);
        }
    }

    private void OnGUI()
    {
        foreach (var method in GUIMethods)
        {
            method.Invoke(null, null);
        }
    }
}
