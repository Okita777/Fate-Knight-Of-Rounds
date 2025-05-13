namespace AsiActionEngine.RunTime
{
    [System.Serializable]
    public class CheckOnHit : IInterruptCondition
    {
        public int InterruptType => -(int)EInterruptTypeInternal.EIT_CheckOnHit;
        public bool CheckInterrupt(Unit unit, ActionStatePart actionStatePart)
        {
            //始终返回True，无任何逻辑，这就是它最终面膜面貌，并不是未完成的状态，也最好别动
            return true;
        }

        public IInterruptCondition Clone()
        {
            return this;
        }
    }
}