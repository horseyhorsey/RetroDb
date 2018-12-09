using RetroDb.Data;

namespace RetroDb.Engine.Import
{
    public class FrontEndBuilder
    {
        /// <summary>
        /// Gets a Frontend from name.
        /// </summary>
        /// <param name="feName">Name of the Frontend eg:Hyperspin</param>
        /// <param name="fePath">Path to frontend</param>
        /// <returns></returns>
        public IFrontEnd GetFrontEnd(string feName, string fePath)
        {
            switch (feName.ToLower())
            {
                case "hyperspin":
                    return new Hyperspin(fePath);
                default:
                    return null;
            }
        }
    }
}
