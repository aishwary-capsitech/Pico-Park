using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons jumpButton;
    // public NetworkButtons leftButtons;
    // public NetworkButtons rightButtons;
    public float horizontalMovement;
    public Vector2 direction;
}