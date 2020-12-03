namespace CoreXF.Abstractions.Base
{
    public interface IExtensionWithViews : IExtension
    {
        /// <summary>
        /// Views assembly name, usually assembly-name.Views.dll
        /// </summary>
        public string Views { get; }
    }
}