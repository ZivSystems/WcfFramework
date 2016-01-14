namespace Ziv.ServiceModel.Operations
{
    /// <summary>
    /// To be used as the return type of operations that have no return value.
    /// </summary>
    public sealed class VoidResult
    {
        private VoidResult() { }

        public static VoidResult Instance
        {
            get
            {
                return new VoidResult();
            }
        }
    }
}
