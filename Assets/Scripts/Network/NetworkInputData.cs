using Fusion;
using UnityEngine;

public struct NetworkInputData : INetworkInput
{
    public NetworkButtons jumpButton;
    public float horizontalMovement;
    public Vector2 direction;
}