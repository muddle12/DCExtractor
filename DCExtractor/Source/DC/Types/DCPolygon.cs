namespace DC.Types
{
    /// <summary>
    /// Class   :   "Polygon"
    /// 
    /// Purpose :   for lack of a better term, a contiguous chunk of triangles that correspond to a section of the mesh.
    /// 
    /// The polygon data starts at the beginning of the index data chunk, functioning as a header for the following index data.
    /// Once the polygon header is parsed, the remaining data is triangle strips until strip count is reached, at which point
    /// another polygon starts. This continues until all index data is read.
    /// </summary>
    public class Polygon
    {
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Data Members.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public int unk1;            //Either 0 or 0xBB000C      I don't know what this is for.          
        public int Type;            //Always 0x10               It's probably corresponding to the length of the header.
        public int StripCount;      //                          Varies depending on the number of strips.
        public int Unused2;         //Always 0                  Appears to be unused.

        public TriangleStrip[] Strips;


        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// Functions.
        ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Clones the polygon.
        /// </summary>
        /// <returns>A copy of the polygon.</returns>
        public Polygon Clone()
        {
            Polygon tPoly = new Polygon();
            tPoly.unk1 = unk1;
            tPoly.Type = Type;
            tPoly.StripCount = StripCount;
            tPoly.Unused2 = Unused2;
            tPoly.Strips = new TriangleStrip[Strips.Length];
            for (int strip = 0; strip < Strips.Length; strip++)
                tPoly.Strips[strip] = Strips[strip].Clone();

            return tPoly;
        }
    }
}
