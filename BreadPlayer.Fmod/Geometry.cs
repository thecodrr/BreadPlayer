/* ========================================================================================== */
/*                                                                                            */
/* BreadPlayer.Fmod Studio - C# Wrapper . Copyright (c), Firelight Technologies Pty, Ltd. 2004-2016.      */
/*                                                                                            */
/* ========================================================================================== */

using BreadPlayer.Fmod.Enums;
using BreadPlayer.Fmod.Structs;
using System;
using System.Runtime.InteropServices;

namespace BreadPlayer.Fmod
{
    /*
            'Geometry' API
        */
    public class Geometry : HandleBase
    {
        public Result release               ()
        {
            Result result = FMOD_Geometry_Release(getRaw());
            if (result == Result.OK)
            {
                rawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Polygon manipulation.
        public Result addPolygon            (float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector[] vertices, out int polygonindex)
        {
            return FMOD_Geometry_AddPolygon(rawPtr, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
        }
        public Result getNumPolygons        (out int numpolygons)
        {
            return FMOD_Geometry_GetNumPolygons(rawPtr, out numpolygons);
        }
        public Result getMaxPolygons        (out int maxpolygons, out int maxvertices)
        {
            return FMOD_Geometry_GetMaxPolygons(rawPtr, out maxpolygons, out maxvertices);
        }
        public Result getPolygonNumVertices (int index, out int numvertices)
        {
            return FMOD_Geometry_GetPolygonNumVertices(rawPtr, index, out numvertices);
        }
        public Result setPolygonVertex      (int index, int vertexindex, ref Vector vertex)
        {
            return FMOD_Geometry_SetPolygonVertex(rawPtr, index, vertexindex, ref vertex);
        }
        public Result getPolygonVertex      (int index, int vertexindex, out Vector vertex)
        {
            return FMOD_Geometry_GetPolygonVertex(rawPtr, index, vertexindex, out vertex);
        }
        public Result setPolygonAttributes  (int index, float directocclusion, float reverbocclusion, bool doublesided)
        {
            return FMOD_Geometry_SetPolygonAttributes(rawPtr, index, directocclusion, reverbocclusion, doublesided);
        }
        public Result getPolygonAttributes  (int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
        {
            return FMOD_Geometry_GetPolygonAttributes(rawPtr, index, out directocclusion, out reverbocclusion, out doublesided);
        }

        // Object manipulation.
        public Result setActive             (bool active)
        {
            return FMOD_Geometry_SetActive(rawPtr, active);
        }
        public Result getActive             (out bool active)
        {
            return FMOD_Geometry_GetActive(rawPtr, out active);
        }
        public Result setRotation           (ref Vector forward, ref Vector up)
        {
            return FMOD_Geometry_SetRotation(rawPtr, ref forward, ref up);
        }
        public Result getRotation           (out Vector forward, out Vector up)
        {
            return FMOD_Geometry_GetRotation(rawPtr, out forward, out up);
        }
        public Result setPosition           (ref Vector position)
        {
            return FMOD_Geometry_SetPosition(rawPtr, ref position);
        }
        public Result getPosition           (out Vector position)
        {
            return FMOD_Geometry_GetPosition(rawPtr, out position);
        }
        public Result setScale              (ref Vector scale)
        {
            return FMOD_Geometry_SetScale(rawPtr, ref scale);
        }
        public Result getScale              (out Vector scale)
        {
            return FMOD_Geometry_GetScale(rawPtr, out scale);
        }
        public Result save                  (IntPtr data, out int datasize)
        {
            return FMOD_Geometry_Save(rawPtr, data, out datasize);
        }

        // Userdata set/get.
        public Result setUserData               (IntPtr userdata)
        {
            return FMOD_Geometry_SetUserData(rawPtr, userdata);
        }
        public Result getUserData               (out IntPtr userdata)
        {
            return FMOD_Geometry_GetUserData(rawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_Release              (IntPtr geometry);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_AddPolygon           (IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector[] vertices, out int polygonindex);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetNumPolygons       (IntPtr geometry, out int numpolygons);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetMaxPolygons       (IntPtr geometry, out int maxpolygons, out int maxvertices);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetPolygonVertex     (IntPtr geometry, int index, int vertexindex, ref Vector vertex);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetPolygonVertex     (IntPtr geometry, int index, int vertexindex, out Vector vertex);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetPolygonAttributes (IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetPolygonAttributes (IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetActive            (IntPtr geometry, bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetActive            (IntPtr geometry, out bool active);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetRotation          (IntPtr geometry, ref Vector forward, ref Vector up);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetRotation          (IntPtr geometry, out Vector forward, out Vector up);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetPosition          (IntPtr geometry, ref Vector position);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetPosition          (IntPtr geometry, out Vector position);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetScale             (IntPtr geometry, ref Vector scale);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetScale             (IntPtr geometry, out Vector scale);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_Save                 (IntPtr geometry, IntPtr data, out int datasize);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_SetUserData          (IntPtr geometry, IntPtr userdata);
        [DllImport(FMODVersion.DLL)]
        private static extern Result FMOD_Geometry_GetUserData          (IntPtr geometry, out IntPtr userdata);
        #endregion

        #region wrapperinternal

        public Geometry(IntPtr raw)
            : base(raw)
        {
        }

        #endregion
    }
}
