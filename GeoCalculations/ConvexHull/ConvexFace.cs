﻿namespace GeoCalculations.ConvexHull
{
    public abstract class ConvexFace<TVertex, TFace>
        where TVertex : IVertex
        where TFace : ConvexFace<TVertex, TFace>
    {
        public TFace[] Adjacency { get; set; }
        public TVertex[] Vertices { get; set; }
        public double[] Normal { get; set; }
    }

    public class DefaultConvexFace<TVertex> : ConvexFace<TVertex, DefaultConvexFace<TVertex>>
        where TVertex : IVertex
    {

    }
}
