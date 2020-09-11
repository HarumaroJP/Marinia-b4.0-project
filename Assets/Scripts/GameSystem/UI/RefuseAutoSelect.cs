using UnityEngine.UI;

public class RefuseAutoSelect : InputField
{
    protected override void LateUpdate()
    {
        base.LateUpdate();

        MoveTextEnd(false);
    }
}
