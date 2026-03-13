using UnityEngine;
public class DogWalkState : State
{
    private DogStateMachine dogContext;
    public DogWalkState(DogStateMachine currentContext) : base(currentContext)
    {
        dogContext = currentContext;
        isBaseState = true;
    }
    public override void EnterState()
    {
        dogContext.AppliedMovementY = 0;
        dogContext.Anim.Play("Walk");
        
    }
    public override void UpdateState()
    {
        Vector3 target = new Vector3(dogContext.Player.gameObject.transform.position.x, dogContext.RB.gameObject.transform.position.y, 0f);
        Vector3 currentPos = new Vector3(dogContext.RB.gameObject.transform.position.x, dogContext.RB.gameObject.transform.position.y, 0f);
        Vector3 direction = (target - currentPos).normalized;
        dogContext.AppliedMovementX = direction.x * dogContext.MoveSpeed;
        
        CheckSwitchStates();
    }
    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (dogContext.IsStunned)
        {   
            SwitchState(new DogStunState(dogContext));
        }
        if (dogContext.InRange() && !dogContext.InAttack)
        {
            SwitchState(new DogPounceState(dogContext));
        }
    }
}
