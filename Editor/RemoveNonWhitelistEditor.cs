using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Amazon.CognitoIdentity.Model;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using VRCSDK2;

[CustomEditor(typeof(RemoveNonWhitelist))]
public class RemoveNonWhitelistEditor : Editor
{
    private string labelText =
        "Removes non-whitelisted VRChat components";
    RemoveNonWhitelist obj;

    public void OnEnable()
    {
        obj = (RemoveNonWhitelist)target;

    }
    public override void OnInspectorGUI()
    {
        GUILayout.Label(labelText);
        if (!GUILayout.Button("Remove components."))
            return;
        DoEverything();
    }

    private void Cleanup()
    {
        // Remove this script from the avatar so that VRC is happy.
        DestroyImmediate(obj.gameObject.GetComponent<RemoveNonWhitelist>());
    }

    private void DoEverything()
    {
        try
        {

            var illegalComponents = VRCSDK2.AvatarValidation.FindIllegalComponents(obj.name, obj.gameObject).ToList();
            var thisComponent = illegalComponents.FirstOrDefault(x => x.GetType().FullName.Contains("RemoveNonWhitelist"));
            illegalComponents.Remove(thisComponent);
            var illegalComponentsNames = illegalComponents.Select(x => x.GetType().FullName).ToList();
            if (illegalComponents.Count <= 0)
            {
                Cleanup();
                return;
            }
            var result = EditorUtility.DisplayDialog("Remove components",
                "The following illegal components were found;\n"+ String.Join("\n", illegalComponentsNames.ToArray()) +" \nRemove them?", "Yes", "Cancel");
            if (result)
            {
                foreach (var component in illegalComponents)
                {
                    DestroyImmediate(component);
                }
                Cleanup();
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex);
        }
    }
}
