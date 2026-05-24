using System;

public class StageStepBus
{
    public static event Action<int> step;

    private static int step_count = 0;

    public static void publishStep()
    {
        step_count += 1;
        step?.Invoke(step_count);
    }

    public static void reset()
    {
        step_count = 0;
    }
}