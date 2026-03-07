using UnityEngine;
public class DogStartState : State
{
    private DogStateMachine dogContext;
    private float curTime;
    public DogStartState(DogStateMachine currentContext) : base(currentContext)
    {
        dogContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        dogContext.Anim.Play("Idle");
        dogContext.AppliedMovementX = 0f;
        dogContext.AppliedMovementY = 0f;
    }
    public override void UpdateState()
    {
        CheckSwitchStates();
    }
    public override void ExitState()
    {
        dogContext.AppliedMovementX = 0f;
        dogContext.AppliedMovementY = 0f;
    }

    public override void CheckSwitchStates()
    {
        Debug.Log(dogContext);
        if (dogContext.InRange() && dogContext.OnGround)
        {
            SwitchState(new DogPounceState(dogContext));
        } else if (!dogContext.InRange() && dogContext.OnGround)
        {
            SwitchState(new DogWalkState(dogContext));
        }
    }
}
