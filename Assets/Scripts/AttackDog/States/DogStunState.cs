using UnityEngine;
public class DogStunState : State
{
    private DogStateMachine dogContext;
    private float curTime;
    public DogStunState(DogStateMachine currentContext) : base(currentContext)
    {
        dogContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        dogContext.Anim.Play("Idle");
        dogContext.AppliedMovementX = 0f;
        dogContext.AppliedMovementY = 0f;
        curTime = 0f;
    }
    public override void UpdateState()
    {
        curTime += Time.deltaTime;
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        dogContext.IsStunned = false;
    }

    public override void CheckSwitchStates()
    {
        if (curTime > dogContext.StunTime)
        {
            if (dogContext.InRange() && dogContext.OnGround)
            {
                SwitchState(new DogPounceState(dogContext));
            } else if (!dogContext.InRange() && dogContext.OnGround)
            {
                SwitchState(new DogWalkState(dogContext));
            }
        } 
    }
}
