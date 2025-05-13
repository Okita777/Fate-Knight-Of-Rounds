namespace AsiActionEngine.RunTime
{
    public interface IInterruptCondition
    {
        int InterruptType { get; }
        string GetInterruptDescription() { return "";}
        bool CheckInterrupt(Unit unit, ActionStatePart actionStatePart);
        IInterruptCondition Clone();
        
        void EditorDraw(CharacterConfig characterConfig, ActionMachineTime _actionTime) {}
    }
}