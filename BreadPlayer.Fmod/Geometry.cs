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
        public Result Release               ()
        {
            Result result = FMOD_Geometry_Release(GetRaw());
            if (result == Result.Ok)
            {
                RawPtr = IntPtr.Zero;
            }
            return result;
        }

        // Polygon manipulation.
        public Result AddPolygon            (float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector[] vertices, out int polygonindex)
        {
            return FMOD_Geometry_AddPolygon(RawPtr, directocclusion, reverbocclusion, doublesided, numvertices, vertices, out polygonindex);
        }
        public Result GetNumPolygons        (out int numpolygons)
        {
            return FMOD_Geometry_GetNumPolygons(RawPtr, out numpolygons);
        }
        public Result GetMaxPolygons        (out int maxpolygons, out int maxvertices)
        {
            return FMOD_Geometry_GetMaxPolygons(RawPtr, out maxpolygons, out maxvertices);
        }
        public Result GetPolygonNumVertices (int index, out int numvertices)
        {
            return FMOD_Geometry_GetPolygonNumVertices(RawPtr, index, out numvertices);
        }
        public Result SetPolygonVertex      (int index, int vertexindex, ref Vector vertex)
        {
            return FMOD_Geometry_SetPolygonVertex(RawPtr, index, vertexindex, ref vertex);
        }
        public Result GetPolygonVertex      (int index, int vertexindex, out Vector vertex)
        {
            return FMOD_Geometry_GetPolygonVertex(RawPtr, index, vertexindex, out vertex);
        }
        public Result SetPolygonAttributes  (int index, float directocclusion, float reverbocclusion, bool doublesided)
        {
            return FMOD_Geometry_SetPolygonAttributes(RawPtr, index, directocclusion, reverbocclusion, doublesided);
        }
        public Result GetPolygonAttributes  (int index, out float directocclusion, out float reverbocclusion, out bool doublesided)
        {
            return FMOD_Geometry_GetPolygonAttributes(RawPtr, index, out directocclusion, out reverbocclusion, out doublesided);
        }

        // Object manipulation.
        public Result SetActive             (bool active)
        {
            return FMOD_Geometry_SetActive(RawPtr, active);
        }
        public Result GetActive             (out bool active)
        {
            return FMOD_Geometry_GetActive(RawPtr, out active);
        }
        public Result SetRotation           (ref Vector forward, ref Vector up)
        {
            return FMOD_Geometry_SetRotation(RawPtr, ref forward, ref up);
        }
        public Result GetRotation           (out Vector forward, out Vector up)
        {
            return FMOD_Geometry_GetRotation(RawPtr, out forward, out up);
        }
        public Result SetPosition           (ref Vector position)
        {
            return FMOD_Geometry_SetPosition(RawPtr, ref position);
        }
        public Result GetPosition           (out Vector position)
        {
            return FMOD_Geometry_GetPosition(RawPtr, out position);
        }
        public Result SetScale              (ref Vector scale)
        {
            return FMOD_Geometry_SetScale(RawPtr, ref scale);
        }
        public Result GetScale              (out Vector scale)
        {
            return FMOD_Geometry_GetScale(RawPtr, out scale);
        }
        public Result Save                  (IntPtr data, out int datasize)
        {
            return FMOD_Geometry_Save(RawPtr, data, out datasize);
        }

        // Userdata set/get.
        public Result SetUserData               (IntPtr userdata)
        {
            return FMOD_Geometry_SetUserData(RawPtr, userdata);
        }
        public Result GetUserData               (out IntPtr userdata)
        {
            return FMOD_Geometry_GetUserData(RawPtr, out userdata);
        }

        #region importfunctions
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_Release              (IntPtr geometry);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_AddPolygon           (IntPtr geometry, float directocclusion, float reverbocclusion, bool doublesided, int numvertices, Vector[] vertices, out int polygonindex);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetNumPolygons       (IntPtr geometry, out int numpolygons);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetMaxPolygons       (IntPtr geometry, out int maxpolygons, out int maxvertices);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetPolygonNumVertices(IntPtr geometry, int index, out int numvertices);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetPolygonVertex     (IntPtr geometry, int index, int vertexindex, ref Vector vertex);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetPolygonVertex     (IntPtr geometry, int index, int vertexindex, out Vector vertex);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetPolygonAttributes (IntPtr geometry, int index, float directocclusion, float reverbocclusion, bool doublesided);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetPolygonAttributes (IntPtr geometry, int index, out float directocclusion, out float reverbocclusion, out bool doublesided);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetActive            (IntPtr geometry, bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetActive            (IntPtr geometry, out bool active);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetRotation          (IntPtr geometry, ref Vector forward, ref Vector up);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetRotation          (IntPtr geometry, out Vector forward, out Vector up);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetPosition          (IntPtr geometry, ref Vector position);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetPosition          (IntPtr geometry, out Vector position);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetScale             (IntPtr geometry, ref Vector scale);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_GetScale             (IntPtr geometry, out Vector scale);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_Save                 (IntPtr geometry, IntPtr data, out int datasize);
        [DllImport(FmodVersion.Dll)]
        private static extern Result FMOD_Geometry_SetUserData          (IntPtr geometry, IntPtr userdata);
        [DllImport(FmodVersion.Dll)]
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
