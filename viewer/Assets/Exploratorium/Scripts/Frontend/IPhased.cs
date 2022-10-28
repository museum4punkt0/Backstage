namespace Exploratorium.Frontend
{
    public interface IPhased : IOpenable
    {
        IPhaseGroup PhaseGroup { get; }
        bool IsValid { get; }
        void SetPhaseGroup(IPhaseGroup phaseGroup);
        void SelectWithoutNotify();
        void DeSelectWithoutNotify();
    }
}