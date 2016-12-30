namespace Cadoscopia.SketchServices
{
    public abstract class SketchObject
    {
        #region Properties

        /// <summary>
        /// Id of this object.
        /// </summary>
        /// <remarks>
        /// Each object has a different id. Ids start at zero.
        /// </remarks>
        public int Id { get; internal set; } = Sketch.INVALID_ID;

        #endregion
    }
}