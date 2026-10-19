namespace _Project.Scripts.Core.Input
{
    public interface ICoreInputReader
    {
        float MoveX { get; }
        bool AttackHeld { get; }

        event System.Action JumpPressed;
        event System.Action DashPressed;
        event System.Action ReloadPressed;
        event System.Action SwitchWeaponPressed;
        event System.Action InteractPressed;
        event System.Action PausePressed;
    }
}