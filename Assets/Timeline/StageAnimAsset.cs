using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[System.Serializable]
public class StageAnimAsset : PlayableAsset
{

    public ExposedReference<GameObject> m_StageController;
    public bool changeArt = false;
    public int enterArt;
    public bool changeFlow = false;
    public bool activeArt;
    public bool changeSpeed = false;
    [Header("預設為0.5")]
    public float moveSpeed = 0.5f;
    [Header("預設為0.5")]
    public float rotateSpeed = 0.5f;

    // Factory method that generates a playable based on this asset
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        //Get access to the Playable Behaviour script
        StageAnimScript playableBehaviour = new StageAnimScript();
        //Resolve the exposed reference on the Scene GameObject by returning the table used by the graph
        playableBehaviour.m_StageController = m_StageController.Resolve(graph.GetResolver());
        playableBehaviour.changeArt = changeArt;
        playableBehaviour.changeFlow = changeFlow;
        playableBehaviour.enterArt = enterArt;
        playableBehaviour.activeArt = activeArt;
        playableBehaviour.changeSpeed = changeSpeed;
        playableBehaviour.moveSpeed = moveSpeed;
        playableBehaviour.rotateSpeed = rotateSpeed;

        //Create a custom playable from this script using the Player Behaviour script
        return ScriptPlayable<StageAnimScript>.Create(graph, playableBehaviour);
        //return Playable.Create(graph);
    }
}
