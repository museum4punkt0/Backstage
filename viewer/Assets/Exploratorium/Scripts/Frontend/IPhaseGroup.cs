namespace Exploratorium.Frontend
{
    public interface IPhaseGroup
    {
        public void RecenterViews();
        public void Select(IPhased phasedPresenter);
        public void Deselect(IPhased phasedPresenter);
        public void UnRegister(IPhased phasedPresenter);
        public void Register(IPhased phasedPresenter);
    }
}