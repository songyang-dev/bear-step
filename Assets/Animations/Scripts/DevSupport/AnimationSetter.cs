using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System;

public class AnimationSetter : Editor
{
    private static AnimatorController orbMessage;

    [MenuItem("Prefabs/Link Animations")]
    static void SetAllAnimations()
    {
        SetUp();

        SetOrbMessage(orbMessage);

        //CleanUp();
    }

    private static void SetOrbMessage(AnimatorController orbMessage)
    {
        var states = orbMessage.layers[0].stateMachine.states;
        foreach (var item in states)
        {
            switch(item.state.name)
            {
                case "Inactive":
                    // remove all scripts
                    foreach (var script in item.state.behaviours)
                    {
                        DestroyImmediate(script, true);
                    }

                    // add the inactive state script
                    item.state.AddStateMachineBehaviour<OrbMessageInactiveState>();
                    break;

                case "OrbMessageAppear":
                    item.state.motion = AssetDatabase.LoadAssetAtPath<Motion>("Assets/Animations/OrbMessageAppear.anim");
                    break;

                case "OrbMessageFade":
                    item.state.motion = AssetDatabase.LoadAssetAtPath<Motion>("Assets/Animations/OrbMessageFade.anim");
                    break;

                // ignore other states
                default:
                    break;
            }
        }
    }

    private static void SetUp()
    {
        orbMessage = AssetDatabase.LoadAssetAtPath<AnimatorController>("Assets/Animations/Orb message.controller");
    }
 
}
