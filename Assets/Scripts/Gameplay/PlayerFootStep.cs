using FMOD.Studio;
using FMODUnity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFootStep : SimpleAudio
{
    [SerializeField] LayerMask soundLayer;
    Player player;
    [SerializeField]GroundMatType matType;
    [SerializeField] EventReference footStep;

    private void Awake()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        var hit = Physics2D.Raycast(Player.Instance.rayCaster.position, Vector2.down, player.groundCheckDist, soundLayer);

        if (hit.collider != null)
        {
            var script = hit.collider.GetComponent<FootStepSoundChanger>();
            if (script)
            {
                matType = script.groundMat;
            }
            else
            {
                matType = GroundMatType.Grass;
            }
        }
    }

    public void AE_PlayFootStep()
    {
        EventInstance instance = RuntimeManager.CreateInstance(footStep);
        RuntimeManager.AttachInstanceToGameObject(instance, transform);
        instance.setParameterByName("Material", (float)matType);
        instance.start();
        instance.release();
    }
}

public enum GroundMatType { Grass, Wood }
