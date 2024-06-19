namespace ImageTrackingApi.Tracking.Pose25
{
    public static class Constants
    {

        public static readonly Dictionary<string, int> BodyParts = new Dictionary<string, int>()
        {
            { "Nose", 0 },
            { "Neck", 1 },
            { "RShoulder", 2 },
            { "RElbow", 3 },
            { "RWrist", 4 },
            {"LShoulder",5},
            { "LElbow", 6 },
            { "LWrist", 7 },
            { "MidHip", 8 },
            { "RHip", 9 },
            { "RKnee", 10 },
            {"RAnkle",11},
            { "LHip", 12 },
            { "LKnee", 13 },
            { "LAnkle", 14 },
            { "REye", 15 },
            { "LEye", 16 },
            {"REar",17},
            { "LEar", 18 },
            { "LBigToe", 19 },
            { "LSmallToe", 20 },
            { "LHeel", 21 },
            { "RBigToe", 22 },
            {"RSmallToe",23},
            { "RHeel", 24 },
            { "Background", 25 }
        };

        public static readonly int[,] PointPairs = new int[,]
        {
                            {1, 0}, {1, 2}, {1, 5},
                            {2, 3}, {3, 4}, {5, 6},
                            {6, 7}, {0, 15}, {15, 17},
                            {0, 16}, {16, 18}, {1, 8},
                            {8, 9}, {9, 10}, {10, 11},
                            {11, 22}, {22, 23}, {11, 24},
                            {8, 12}, {12, 13}, {13, 14},
                            {14, 19}, {19, 20}, {14, 21}
        };
    }
}
