using Zenject;

namespace Foundation
{
    public abstract class AbstractService<T> : MonoInstaller
        where T : class
    {
        ObserverHandleManager observers;
        protected ObserverHandleManager Observers { get {
                if (observers == null)
                    observers = new ObserverHandleManager();
                return observers;
            } }

        protected void Observe<O>(IObserverList<O> observable)
            where O : class
        {
            Observers.Observe(observable, this as O);
        }

        protected void Observe<O>(ref ObserverHandle handle, IObserverList<O> observable)
            where O : class
        {
            Observers.Observe(ref handle, observable, this as O);
        }

        protected void Unobserve(ObserverHandle handle)
        {
            Observers.Unobserve(handle);
        }

        protected virtual void OnEnable()
        {
        }

        protected virtual void OnDisable()
        {
            if (observers != null)
                observers.Clear();
        }

        public override void InstallBindings()
        {
            Container.Bind<T>().FromInstance(this as T);
        }
    }

    public abstract class AbstractService<T1, T2> : AbstractService<T1>
        where T1 : class
        where T2 : class
    {
        public override void InstallBindings()
        {
            base.InstallBindings();
            Container.Bind<T2>().FromInstance(this as T2);
        }
    }
}
