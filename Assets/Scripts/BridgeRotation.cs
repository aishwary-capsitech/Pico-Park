using UnityEngine;
using System.Collections.Generic;
using Fusion;

public class BridgeRotation : NetworkBehaviour
{
    [SerializeField] private float requiredMass = 5f;
    [SerializeField] private float rotationSpeed = 45f;
    
    [Networked] private NetworkBool isRotating { get; set; }
    [Networked] private NetworkBool isHorizontal { get; set; }
    
    private Quaternion targetRotation;
    private Quaternion initialRotation;
    private HashSet<Rigidbody2D> players = new HashSet<Rigidbody2D>();
    
    public override void Spawned()
    {
        initialRotation = transform.rotation;
        targetRotation = transform.rotation * Quaternion.Euler(0, 0, -90f);
    }
    
    public override void FixedUpdateNetwork()
    {
        if (isRotating && !isHorizontal)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Runner.DeltaTime);
            
            if (Quaternion.Angle(transform.rotation, targetRotation) < 0.1f)
            {
                isHorizontal = true;
                isRotating = false;
            }
        }
    }
    
    void OnCollisionEnter2D(Collision2D col)
    {
        if (!HasStateAuthority) return;
        
        Rigidbody2D rb = col.rigidbody;
        if (rb != null) players.Add(rb);
        CheckMass();
    }
    
    void OnCollisionExit2D(Collision2D col)
    {
        if (!HasStateAuthority) return;
        
        Rigidbody2D rb = col.rigidbody;
        if (rb != null) players.Remove(rb);
    }
    
    void CheckMass()
    {
        if (isHorizontal) return;
        
        float totalMass = 0f;
        players.RemoveWhere(rb => rb == null);
        
        foreach (var rb in players)
            totalMass += rb.mass;
        
        if (totalMass >= requiredMass)
            isRotating = true;
    }
}