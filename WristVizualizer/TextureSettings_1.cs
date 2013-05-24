using System;
using System.Collections.Generic;
using System.Text;

namespace WristVizualizer
{
    class TextureSettings
    {
        public static float[][] BoneColors = {
            new float[] { 1.0f, 0.0f, 0.0f},
            new float[] { 1.0f, 0.0f, 0.0f},
            new float[] { 0.1f, 0.0f, 1.0f},
            new float[] { 0.1f, 1.0f, 0.0f},
            new float[] { 0.9f, 0.5f, 0.0f},
            new float[] { 0.5f, 0.5f, 0.0f},
            new float[] { 0.1f, 0.5f, 0.5f},
            new float[] { 0.5f, 0.0f, 0.5f},
            new float[] { 0.0f, 0.5f, 0.9f},
            new float[] { 0.1f, 0.1f, 0.8f},
            new float[] { 0.7f, 0.3f, 0.3f},
            new float[] { 0.7f, 0.1f, 0.1f},
            new float[] { 0.3f, 0.7f, 0.3f},
            new float[] { 0.3f, 0.3f, 0.7f},
            new float[] { 0.5f, 0.4f, 0.8f},
            new float[] { 0.1f, 1.0f, 0.3f},
        };

        public static string[] ShortBNames = { "rad", "uln", "sca", "lun", "trq", "pis", "tpd", "tpm", "cap", "ham", "mc1", "mc2", "mc3", "mc4", "mc5" };
        public static string[] LongBNames = { "radius", "ulna", "scaphoid", "lunate", "triquetrum", "pisiform", "trapezoid", "trapezium", "capitate", "hamate", "metacarpal1", "metacarpal2", "metacarpal3", "metacarpal4", "metacarpal5" };
        public static string[] TransformBNames = { "radius", "ulna", "scaphoid", "lunate", "triquetrum", "pisiform", "trapezoid", "trapezium", "capitate", "hamate", "metacarpal1", "metacarpal2", "metacarpal3", "metacarpal4", "metacarpal5" };
    }
}
