using Fusion;
using StarterAssets;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkThirdPersonController : NetworkBehaviour
{
    private ThirdPersonController _controller;

    public override void Spawned()
    {
        _controller = GetComponent<ThirdPersonController>();
        // Disable input if not our player
        if (!HasInputAuthority)
        {
            // Disable local-only components for remote players
            var playerInput = GetComponent<UnityEngine.InputSystem.PlayerInput>();
            if (playerInput) Destroy(playerInput);

            var starterInputs = GetComponent<StarterAssetsInputs>();
            if (starterInputs) Destroy(starterInputs);
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out PlayerInputData inputData))
        {
            // Pass network input to your ThirdPersonController
            _controller.SetInput(inputData);
        }
    }
}

public struct PlayerInputData : INetworkInput
{
    public Vector2 Move;
    public Vector2 Look;
    public NetworkBool Jump;
    public NetworkBool Sprint;
    public NetworkBool Talk;
    public NetworkBool AnalogMovement;
}

