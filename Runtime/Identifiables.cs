using UnityEngine;

namespace Acciaio
{
    public interface IIdentifiable
    {
        public Id Id { get; }
    }
    
    public abstract class IdentifiableObject : ScriptableObject, IIdentifiable
    {
        public abstract Id Id { get; } 
        
        protected IdentifiableObject() { }
		
        public override bool Equals(object other) 
            => other is IdentifiableObject ido && Equals(ido);

        public bool Equals(IdentifiableObject identifiable) 
            => ReferenceEquals(identifiable, this) || Id == identifiable.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
	
    public abstract class IdentifiableObject<TId> : IdentifiableObject where TId : Id
    {
        [SerializeField]
        protected TId ID;
        
        protected IdentifiableObject() { }

        public override Id Id => ID;
    }
    
    public abstract class IdentifiableBehaviour : MonoBehaviour, IIdentifiable
    {
        public abstract Id Id { get; } 
        
        protected IdentifiableBehaviour() { }
		
        public override bool Equals(object other) 
            => other is IdentifiableBehaviour ido && Equals(ido);

        public bool Equals(IdentifiableBehaviour identifiable) 
            => ReferenceEquals(identifiable, this) || Id == identifiable.Id;

        public override int GetHashCode() => Id.GetHashCode();
    }
	
    public abstract class IdentifiableBehaviour<TId> : IdentifiableBehaviour where TId : Id
    {
        [SerializeField]
        protected TId ID;

        public override Id Id => ID;
        
        protected IdentifiableBehaviour() { }
    }
}