using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

// A behaviour that is attached to a playable
public class StageAnimScript : PlayableBehaviour
{

    public GameObject m_StageController;

    public bool changeArt = false;
    public int enterArt;
    public bool changeFlow = false;
    public bool activeArt;
    public bool changeSpeed = false;
    public float moveSpeed;
    public float rotateSpeed;

    // Called when the owning graph starts playing
    public override void OnGraphStart(Playable playable)
    {

    }

    // Called when the owning graph stops playing
    public override void OnGraphStop(Playable playable)
    {
        
    }

    // Called when the state of the playable is set to Play
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        if(changeArt)
            ObjectManager.instance.SwitchToArt(enterArt);
        if(changeFlow){
            if(activeArt)
                ObjectManager.instance.StartFlow();
            else
            {
                ObjectManager.instance.StopFlow();
            }
        }
        if(changeSpeed){
            ObjectManager.instance.MoveSpeed = moveSpeed;
            ObjectManager.instance.RotateSpeed = rotateSpeed;
        }

    }

    // Called when the state of the playable is set to Paused
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        
    }

    // Called each frame while the state is set to Play
    public override void PrepareFrame(Playable playable, FrameData info)
    {

    }
}
