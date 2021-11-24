namespace Injecterize
{
    /// <summary>
    /// Holds Options when Using Containerize 
    /// </summary>
    public class InjecterizeOptions
    {
        /// <summary>
        /// If no Interface is provided as part of the Attribute , look at the first Interface found on the instances and use that when you
        /// register the service
        ///
        /// If the Service already exists with that Interface , just register the type
        /// </summary>
        public bool TryRegisterWithFirstInterface { get; set; }
    }
}