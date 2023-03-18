public class GoSmoke : GAction {
    public override bool PrePerform() {

        return true;
    }

    public override bool PostPerform() {

        beliefs.RemoveState("needSmoke");
        return true;
    }
}
