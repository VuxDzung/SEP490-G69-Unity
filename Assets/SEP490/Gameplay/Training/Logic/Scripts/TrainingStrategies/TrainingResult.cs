namespace SEP490G69.Training
{
    using System.Collections.Generic;

    public class TrainingResult
    {
        public bool IsSuccess;
        public List<StatChange> Changes = new();
    }
    public class StatChange
    {
        public EStatusType StatusType;
        public float Before;
        public float After;
        public float Delta;

        public StatChange(EStatusType type, float before, float delta)
        {
            StatusType = type;
            Before = before;
            After = before + delta;
            Delta = delta;
        }
    }
}